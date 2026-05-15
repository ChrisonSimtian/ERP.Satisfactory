using ERP.Application;
using ERP.Application.Commands.IngestSave;
using ERP.Application.Queries.PlanProduction;
using ERP.Domain;
using ERP.Infrastructure;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Satisfactory.Save;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ICatalogProvider).Assembly);
});

builder.Services.AddErpInfrastructure(builder.Configuration);

// ---- Plan persistence (EF Core, ADR-0018) ----------------------------------
// SQLite by default, Postgres opt-in via `Persistence:Provider=postgres`.
// Connection string lives in `ConnectionStrings:Plans`.
builder.Services.AddErpPersistence(builder.Configuration);

// Registered as a singleton so endpoints + tests can swap in a fake clock.
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

// In Development, apply pending plan-storage migrations on startup so the
// SQLite default Just Works on a fresh checkout. Production / hosted deploys
// should run `dotnet ef database update` (or equivalent) out-of-band to keep
// schema changes explicit.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PlanDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "API service is running. See /catalog/items, /plan, /factory/state.");

app.MapGet("/catalog/items", (ICatalogProvider catalog) =>
    catalog.Items
        .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
        .Select(i => new ItemDto(i.Id.Value, i.Name)));

app.MapGet("/catalog/recipes", (ICatalogProvider catalog) =>
{
    // Per-minute amounts mirror what /plan returns and what the planner UI displays —
    // raw per-cycle counts on the wire would force every consumer to multiply by
    // 60/duration. Recipes with zero duration would be a parser bug, but guard anyway.
    AmountDto ToPerMinute(ItemAmount a, TimeSpan duration) =>
        new(a.Item.Value,
            catalog.FindItem(a.Item)?.Name ?? a.Item.Value,
            duration.TotalSeconds > 0
                ? Math.Round(a.Quantity * 60m / (decimal)duration.TotalSeconds, 4)
                : a.Quantity);

    return catalog.Recipes
        .OrderBy(r => r.IsAlternate)
        .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
        .Select(r =>
        {
            var building = catalog.FindBuilding(r.Building);
            return new RecipeView(
                r.Id.Value,
                r.Name,
                r.Building.Value,
                building?.Name ?? r.Building.Value,
                building?.BasePowerMw ?? 0,
                r.IsAlternate,
                r.Duration.TotalSeconds,
                r.Inputs.Select(i => ToPerMinute(i, r.Duration)).ToList(),
                r.Outputs.Select(o => ToPerMinute(o, r.Duration)).ToList());
        });
});

app.MapGet("/catalogue/status", (ICatalogProvider catalog) => catalog.GetStatus());

app.MapPost("/catalogue/configure", (ConfigureCatalogueRequest request, ICatalogProvider catalog) =>
{
    if (string.IsNullOrWhiteSpace(request.DocsPath))
        return Results.BadRequest(new { error = "DocsPath is required." });

    try
    {
        var status = catalog.LoadFromPath(request.DocsPath);
        return Results.Ok(status);
    }
    catch (FileNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Failed to load catalogue", detail: ex.Message, statusCode: 422);
    }
});

app.MapGet("/factory/state", (IFactoryStateProvider provider, ICatalogProvider catalog) =>
    FactoryStateView.From(provider, catalog));

app.MapGet("/factory/state.geojson", (IFactoryStateProvider provider, ICatalogProvider catalog, Satisfactory.Save.KnownFlora flora) =>
    Results.Json(FactoryStateGeoJson.From(provider, catalog, flora), contentType: "application/geo+json"));

app.MapGet("/factory/saves", () =>
    SaveFileResolver.EnumerateDetectedSaves()
        .Select(f => new DetectedSaveView(f.FullName, f.Name, f.LastWriteTimeUtc, f.Length))
        .ToList());

app.MapPost("/factory/ingest", async (
    IngestSaveRequest request, IMessageBus bus, IFactoryStateProvider provider,
    ICatalogProvider catalog, ILoggerFactory loggerFactory) =>
{
    if (string.IsNullOrWhiteSpace(request.SavePath))
        return Results.BadRequest(new { error = "SavePath is required." });

    try
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await bus.InvokeAsync<FactoryStateStatus>(new IngestSaveCommand(request.SavePath));
        sw.Stop();
        loggerFactory.CreateLogger("FactoryIngestEndpoint")
            .LogInformation("Ingested save in {Elapsed}ms", sw.ElapsedMilliseconds);
        return Results.Ok(FactoryStateView.From(provider, catalog));
    }
    catch (FileNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Failed to parse save", detail: ex.Message, statusCode: 422);
    }
});

// ----- Manual node overrides (#42 Option B) ---------------------------------
// User-curated resource + purity for individual BP_ResourceNode_C actors.
// Persisted to %LOCALAPPDATA%\ERP.Satisfactory\manual-node-overrides.json.
// Body identifies the node by `reference` (the in-save PathName, surfaced in
// GeoJSON as the resource-node feature's `kind`). The server resolves the
// node's position from current state, persists at that position (so the
// override survives across saves of the same world), and refreshes parsed
// state so callers see the change immediately.

app.MapPut("/factory/node-override", (
    NodeOverrideRequest request,
    Satisfactory.Save.ManualNodeOverrides overrides,
    IFactoryStateProvider provider) =>
{
    if (string.IsNullOrWhiteSpace(request.Reference))
        return Results.BadRequest(new { error = "Reference is required." });
    if (string.IsNullOrWhiteSpace(request.Resource))
        return Results.BadRequest(new { error = "Resource is required (e.g. Desc_OreIron_C)." });
    if (!Enum.TryParse<ERP.Domain.NodePurity>(request.Purity, ignoreCase: true, out var purity))
        return Results.BadRequest(new { error = $"Unknown purity '{request.Purity}'. Use Impure, Normal, or Pure." });

    var node = provider.Current.ResourceNodes
        .FirstOrDefault(n => string.Equals(n.Reference, request.Reference, StringComparison.Ordinal));
    if (node is null)
        return Results.NotFound(new { error = $"No resource node with reference '{request.Reference}'." });

    overrides.Upsert(node.Position, request.Resource, purity);
    provider.Refresh();
    return Results.NoContent();
});

app.MapDelete("/factory/node-override", (
    string reference,
    Satisfactory.Save.ManualNodeOverrides overrides,
    IFactoryStateProvider provider) =>
{
    if (string.IsNullOrWhiteSpace(reference))
        return Results.BadRequest(new { error = "reference query parameter is required." });

    var node = provider.Current.ResourceNodes
        .FirstOrDefault(n => string.Equals(n.Reference, reference, StringComparison.Ordinal));
    if (node is null)
        return Results.NotFound(new { error = $"No resource node with reference '{reference}'." });

    var removed = overrides.Delete(node.Position);
    if (removed) provider.Refresh();
    return Results.NoContent();
});

app.MapPost("/plan", async (PlanRequest request, IMessageBus bus, ICatalogProvider catalog, ILoggerFactory loggerFactory) =>
{
    if (!catalog.IsLoaded || catalog.Recipes.Count == 0)
    {
        return Results.Problem(
            title: "Catalogue not loaded",
            detail: "Configure the Docs.json path via POST /catalogue/configure before planning.",
            statusCode: 409);
    }

    var query = new PlanProductionQuery(
        Targets: request.Targets.Select(t => new ProductionTarget(new ItemId(t.ItemId), t.ItemsPerMinute)).ToList(),
        Available: request.Available.Select(a => new ResourceAvailability(new ItemId(a.ItemId), a.ItemsPerMinute)).ToList());

    var logger = loggerFactory.CreateLogger("PlannerEndpoint");
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var plan = await bus.InvokeAsync<ProductionPlan>(query);
    sw.Stop();
    logger.LogInformation(
        "Planner: {Targets} target(s) → {Steps} step(s), {Missing} missing input(s) in {Elapsed}ms",
        query.Targets.Count, plan.Steps.Count, plan.MissingInputs.Count, sw.ElapsedMilliseconds);

    return Results.Ok(PlanDto.From(plan, catalog));
});

// ----- Saved plans & share links (#80) --------------------------------------
// Minimal plan persistence surface — we save the inputs of a planner session
// so it can be shared via a token-protected URL. The planner UI invokes
// POST /plans to capture the current sources/sinks; /plans/{id}/share mints a
// share token; /plans/shared/{token} is the public read-only endpoint that
// the share URL resolves to.

app.MapPost("/plans", async (
    SavePlanRequest request,
    IPlanRepository plans,
    TimeProvider clock,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Name is required." });

    var now = clock.GetUtcNow().UtcDateTime;
    var plan = new SavedPlan(
        id: Guid.NewGuid(),
        name: request.Name,
        targets: request.Targets.Select(t => new ProductionTarget(new ItemId(t.ItemId), t.ItemsPerMinute)).ToList(),
        available: request.Available.Select(a => new ResourceAvailability(new ItemId(a.ItemId), a.ItemsPerMinute)).ToList(),
        createdUtc: now,
        updatedUtc: now);

    await plans.AddAsync(plan, ct);
    await plans.SaveChangesAsync(ct);

    return Results.Created($"/plans/{plan.Id}", SavedPlanView.From(plan));
});

app.MapGet("/plans/{id:guid}", async (Guid id, IPlanRepository plans, CancellationToken ct) =>
{
    var plan = await plans.GetAsync(id, ct);
    return plan is null
        ? Results.NotFound()
        : Results.Ok(SavedPlanView.From(plan));
});

app.MapPost("/plans/{id:guid}/share", async (
    Guid id,
    IPlanRepository plans,
    IPlanShareRepository shares,
    TimeProvider clock,
    HttpRequest http,
    CancellationToken ct) =>
{
    var plan = await plans.GetAsync(id, ct);
    if (plan is null) return Results.NotFound();

    var token = ShareTokenGenerator.NewToken();
    var entity = new PlanShareToken(token, plan.Id, clock.GetUtcNow().UtcDateTime);
    await shares.AddAsync(entity, ct);
    await shares.SaveChangesAsync(ct);

    // Build an absolute URL so the planner UI can present a one-click copy.
    // Trusts the incoming Host/Scheme — fine for an OSS local-first app, and
    // any hosted deployment terminates TLS in front of this layer anyway.
    var baseUrl = $"{http.Scheme}://{http.Host}";
    return Results.Ok(new ShareTokenView(token, $"{baseUrl}/plans/shared/{token}", entity.CreatedUtc, entity.ExpiresUtc));
});

app.MapDelete("/plans/{id:guid}/share/{token}", async (
    Guid id,
    string token,
    IPlanShareRepository shares,
    TimeProvider clock,
    CancellationToken ct) =>
{
    var entity = await shares.GetAsync(token, ct);
    if (entity is null || entity.PlanId != id) return Results.NotFound();

    entity.Revoke(clock.GetUtcNow().UtcDateTime);
    await shares.SaveChangesAsync(ct);
    return Results.NoContent();
});

app.MapGet("/plans/shared/{token}", async (
    string token,
    IPlanRepository plans,
    IPlanShareRepository shares,
    TimeProvider clock,
    CancellationToken ct) =>
{
    var entity = await shares.GetAsync(token, ct);
    if (entity is null || !entity.IsActive(clock.GetUtcNow().UtcDateTime))
        return Results.NotFound();

    var plan = await plans.GetAsync(entity.PlanId, ct);
    return plan is null
        ? Results.NotFound()
        : Results.Ok(SavedPlanView.From(plan));
});

app.MapDefaultEndpoints();

app.Run();

/// <summary>
/// 16-char URL-safe token. Base64-url over 12 random bytes gives 16
/// alphanumeric (+ '-' / '_') chars and ~96 bits of entropy — far more than
/// enough for an unguessable share link without bloating the URL.
/// </summary>
internal static class ShareTokenGenerator
{
    public static string NewToken()
    {
        Span<byte> bytes = stackalloc byte[12];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

public sealed record ItemDto(string Id, string Name);

public sealed record RecipeView(
    string Id,
    string Name,
    string BuildingId,
    string BuildingName,
    double BuildingPowerMw,
    bool IsAlternate,
    double DurationSeconds,
    IReadOnlyList<AmountDto> InputsPerMinute,
    IReadOnlyList<AmountDto> OutputsPerMinute);

public sealed record ConfigureCatalogueRequest(string DocsPath);

public sealed record IngestSaveRequest(string SavePath);

/// <summary>
/// PUT /factory/node-override body. Purity arrives as a string (Impure /
/// Normal / Pure) — Minimal APIs JSON binding doesn't string-bind enums by
/// default, and we don't want to enable that globally for everything else.
/// </summary>
public sealed record NodeOverrideRequest(string Reference, string Resource, string Purity);

public sealed record DetectedSaveView(string Path, string Name, DateTime LastWriteTimeUtc, long SizeBytes);

public sealed record SaveMetadataView(
    string SessionName,
    int SaveVersion,
    int BuildVersion,
    double PlayedSeconds,
    DateTime SaveDateTimeUtc);

public sealed record CountView(string Key, int Count);

/// <summary>One row in the "buildings by type × recipe" table on /factory/ingest.</summary>
public sealed record BuildingGroupView(
    string Building,
    string? Recipe,
    string? RecipeName,
    int Count);

public sealed record FactoryStateView(
    bool IsLoaded,
    string? Source,
    SaveMetadataView? Save,
    IReadOnlyList<CountView> Miners,
    int MinersBoundToNode,
    IReadOnlyList<BuildingGroupView> Buildings,
    int BuildingsWithRecipe,
    IReadOnlyList<CountView> Belts,
    IReadOnlyList<CountView> Generators,
    int ResourceNodeCount,
    IReadOnlyList<string> Warnings)
{
    public static FactoryStateView From(IFactoryStateProvider provider, ICatalogProvider catalog)
    {
        var state = provider.Current;
        var meta = provider.IsLoaded
            ? new SaveMetadataView(
                state.Save.SessionName,
                state.Save.SaveVersion,
                state.Save.BuildVersion,
                state.Save.PlayedTime.TotalSeconds,
                state.Save.SaveDateTimeUtc)
            : null;

        return new FactoryStateView(
            IsLoaded: provider.IsLoaded,
            Source: provider.Source,
            Save: meta,
            Miners: state.Miners
                .GroupBy(m => m.Tier.ToString())
                .Select(g => new CountView(g.Key, g.Count()))
                .OrderBy(c => c.Key, StringComparer.Ordinal)
                .ToList(),
            MinersBoundToNode: state.Miners.Count(m => !string.IsNullOrEmpty(m.ResourceNodeReference)),
            Buildings: state.Buildings
                .GroupBy(b => (Building: b.Building.Value, Recipe: b.Recipe?.Value))
                .Select(g => new BuildingGroupView(
                    Building: g.Key.Building,
                    Recipe: g.Key.Recipe,
                    RecipeName: g.Key.Recipe is { Length: > 0 } r
                        ? catalog.FindRecipe(new RecipeId(r))?.Name
                        : null,
                    Count: g.Count()))
                .OrderBy(b => b.Building, StringComparer.Ordinal)
                .ThenByDescending(b => b.Count)
                .ToList(),
            BuildingsWithRecipe: state.Buildings.Count(b => b.Recipe is not null),
            Belts: state.Belts
                .GroupBy(b => b.Tier.ToString())
                .Select(g => new CountView(g.Key, g.Count()))
                .OrderBy(c => c.Key, StringComparer.Ordinal)
                .ToList(),
            Generators: state.Generators
                .GroupBy(g => g.Kind.ToString())
                .Select(g => new CountView(g.Key, g.Count()))
                .OrderBy(c => c.Key, StringComparer.Ordinal)
                .ToList(),
            ResourceNodeCount: state.ResourceNodes.Count,
            Warnings: state.Warnings);
    }
}

// ---------------------------------------------------------------------------
// GeoJSON projection for the map page (ADR-0013).
// FeatureCollection with one Feature per parsed entity. Coordinates are raw
// Unreal world X/Y in centimetres — the JS layer (using Leaflet's CRS.Simple)
// handles axis orientation + zoom bounds.
// ---------------------------------------------------------------------------

// GeoJSON geometry — Point uses [x, y]; LineString uses [[x, y], …]. We
// serialise both shapes through `object` so the JSON layout matches the
// GeoJSON spec without needing two parallel `GeoFeature` records.
public sealed record GeoGeometry(string Type, object Coordinates)
{
    public static GeoGeometry Point(Position p) => new("Point", new double[] { p.X, p.Y });

    public static GeoGeometry LineString(IReadOnlyList<Position> polyline)
    {
        var coords = new double[polyline.Count][];
        for (var i = 0; i < polyline.Count; i++)
            coords[i] = [polyline[i].X, polyline[i].Y];
        return new GeoGeometry("LineString", coords);
    }
}

public sealed record GeoFeature(
    string Type,
    GeoGeometry Geometry,
    Dictionary<string, object?> Properties)
{
    public static GeoFeature Make(string category, string kind, Position position, Dictionary<string, object?>? extra = null)
        => new("Feature", GeoGeometry.Point(position), BuildProps(category, kind, position, extra));

    public static GeoFeature MakeLine(string category, string kind, IReadOnlyList<Position> polyline, Position fallback, Dictionary<string, object?>? extra = null)
        => new("Feature", GeoGeometry.LineString(polyline), BuildProps(category, kind, fallback, extra));

    private static Dictionary<string, object?> BuildProps(string category, string kind, Position position, Dictionary<string, object?>? extra)
    {
        var props = new Dictionary<string, object?>
        {
            ["category"] = category,
            ["kind"] = kind,
            ["z"] = position.Z,
        };
        if (extra is not null)
            foreach (var kv in extra) props[kv.Key] = kv.Value;
        return props;
    }
}

public sealed record FactoryStateGeoJson(
    string Type,
    IReadOnlyList<GeoFeature> Features,
    Dictionary<string, object?> Metadata)
{
    public static FactoryStateGeoJson From(IFactoryStateProvider provider, ICatalogProvider catalog)
        => From(provider, catalog, Satisfactory.Save.KnownFlora.Empty);

    public static FactoryStateGeoJson From(
        IFactoryStateProvider provider,
        ICatalogProvider catalog,
        Satisfactory.Save.KnownFlora flora)
    {
        var s = provider.Current;
        var features = new List<GeoFeature>();

        foreach (var n in s.ResourceNodes)
            features.Add(GeoFeature.Make("resource-node", n.Reference, n.Position, new()
            {
                ["nodeKind"] = n.Kind.ToString(),
                ["purity"] = n.Purity.ToString(),
                ["resource"] = n.Resource?.Value,
                ["resourceName"] = n.Resource is { Value: { Length: > 0 } } id
                    ? catalog.FindItem(id)?.Name
                    : null,
            }));

        foreach (var m in s.Miners)
            features.Add(GeoFeature.Make("miner", m.Reference, m.Position, new()
            {
                ["tier"] = m.Tier.ToString(),
                ["resourceNode"] = m.ResourceNodeReference,
            }));

        foreach (var b in s.Buildings)
            features.Add(GeoFeature.Make("building", b.Building.Value, b.Position, new()
            {
                ["recipe"] = b.Recipe?.Value,
                ["recipeName"] = b.Recipe is { Value: { Length: > 0 } } id
                    ? catalog.FindRecipe(id)?.Name
                    : null,
                ["clockSpeed"] = b.ClockSpeed,
            }));

        foreach (var belt in s.Belts)
        {
            var props = new Dictionary<string, object?>
            {
                ["tier"] = belt.Tier.ToString(),
            };
            if (belt.Polyline is { Count: >= 2 } poly)
                features.Add(GeoFeature.MakeLine("belt", belt.Reference, poly, belt.Position, props));
            else
                features.Add(GeoFeature.Make("belt", belt.Reference, belt.Position, props));
        }

        foreach (var g in s.Generators)
            features.Add(GeoFeature.Make("generator", g.Reference, g.Position, new()
            {
                ["genKind"] = g.Kind.ToString(),
            }));

        // Flora layer (#62) — static dataset, not from the save. Each feature
        // carries the species ItemId so the JS layer can pick the right wiki
        // item icon (Desc_Berry_C.png etc.) and surface a friendly name.
        foreach (var f in flora.All)
        {
            features.Add(GeoFeature.Make("flora", f.Species, new Position(f.X, f.Y, f.Z), new()
            {
                ["species"] = f.Species,
                ["speciesName"] = f.DisplayName,
            }));
        }

        var meta = new Dictionary<string, object?>
        {
            ["isLoaded"] = provider.IsLoaded,
            ["source"] = provider.Source,
            ["sessionName"] = provider.IsLoaded ? s.Save.SessionName : null,
            ["featureCount"] = features.Count,
        };

        return new FactoryStateGeoJson("FeatureCollection", features, meta);
    }
}

public sealed record TargetDto(string ItemId, decimal ItemsPerMinute);
public sealed record AvailabilityDto(string ItemId, decimal ItemsPerMinute);
public sealed record PlanRequest(IReadOnlyList<TargetDto> Targets, IReadOnlyList<AvailabilityDto> Available);

/// <summary>Body for POST /plans (saves the current planner inputs under a name).</summary>
public sealed record SavePlanRequest(
    string Name,
    IReadOnlyList<TargetDto> Targets,
    IReadOnlyList<AvailabilityDto> Available);

public sealed record SavedPlanView(
    Guid Id,
    string Name,
    IReadOnlyList<TargetDto> Targets,
    IReadOnlyList<AvailabilityDto> Available,
    DateTime CreatedUtc,
    DateTime UpdatedUtc)
{
    public static SavedPlanView From(SavedPlan plan) => new(
        plan.Id,
        plan.Name,
        plan.Targets.Select(t => new TargetDto(t.Item.Value, t.ItemsPerMinute)).ToList(),
        plan.Available.Select(a => new AvailabilityDto(a.Item.Value, a.ItemsPerMinute)).ToList(),
        plan.CreatedUtc,
        plan.UpdatedUtc);
}

public sealed record ShareTokenView(string Token, string Url, DateTime CreatedUtc, DateTime? ExpiresUtc);

/// <summary>
/// Public Program shim so <c>WebApplicationFactory&lt;Program&gt;</c> in the
/// integration-test project can target this entry point — without it the
/// implicit <c>Program</c> class generated from top-level statements is
/// internal and the factory can't see it.
/// </summary>
public partial class Program { }

public sealed record AmountDto(string ItemId, string ItemName, decimal ItemsPerMinute);
public sealed record StepDto(
    string RecipeId,
    string RecipeName,
    string BuildingId,
    string BuildingName,
    decimal BuildingCount,
    decimal PowerMw,
    IReadOnlyList<AmountDto> Inputs,
    IReadOnlyList<AmountDto> Outputs);

public sealed record PlanDto(
    bool IsFeasible,
    IReadOnlyList<StepDto> Steps,
    decimal TotalPowerMw,
    IReadOnlyList<AmountDto> RawInputsConsumed,
    IReadOnlyList<AmountDto> MissingInputs)
{
    public static PlanDto From(ProductionPlan plan, ICatalogProvider catalog)
    {
        AmountDto ToAmount(ItemAmount a) =>
            new(a.Item.Value, catalog.FindItem(a.Item)?.Name ?? a.Item.Value, Math.Round(a.Quantity, 4));

        var steps = plan.Steps.Select(s => new StepDto(
            s.Recipe.Id.Value,
            s.Recipe.Name,
            s.Recipe.Building.Value,
            catalog.FindBuilding(s.Recipe.Building)?.Name ?? s.Recipe.Building.Value,
            Math.Round(s.BuildingCount, 4),
            Math.Round(s.PowerMw, 4),
            s.InputsPerMinute.Select(ToAmount).ToList(),
            s.OutputsPerMinute.Select(ToAmount).ToList())).ToList();

        return new(
            IsFeasible: plan.IsFeasible,
            Steps: steps,
            TotalPowerMw: Math.Round(steps.Sum(s => s.PowerMw), 4),
            RawInputsConsumed: plan.RawInputsConsumed.Select(ToAmount).ToList(),
            MissingInputs: plan.MissingInputs.Select(ToAmount).ToList());
    }
}
