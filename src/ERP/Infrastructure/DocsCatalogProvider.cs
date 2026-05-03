using ERP.Application;
using ERP.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Satisfactory.Catalog;

namespace ERP.Infrastructure;

/// <summary>
/// <see cref="ICatalogProvider"/> backed by the user's installed <c>Docs.json</c>.
/// Resolves the path on construction (env var → user-saved → appsettings → Steam
/// auto-detect), parses the file, and exposes the result. If no valid path is
/// available the catalogue starts empty and the user is directed to the Settings
/// page to configure one.
/// </summary>
public sealed class DocsCatalogProvider : ICatalogProvider
{
    public const string EnvironmentVariable = "ERP_SATISFACTORY_DOCS_PATH";

    private readonly UserCatalogueConfig _userConfig;
    private readonly CatalogueOptions _options;
    private readonly ILogger<DocsCatalogProvider> _logger;
    private LoadedState _state;

    public DocsCatalogProvider(
        IOptions<CatalogueOptions> options,
        UserCatalogueConfig userConfig,
        ILogger<DocsCatalogProvider> logger)
    {
        _options = options.Value;
        _userConfig = userConfig;
        _logger = logger;
        _state = LoadAtStartup();
    }

    public bool IsLoaded => _state.IsLoaded;
    public string? Source => _state.Source;
    public IReadOnlyList<Item> Items => _state.Items;
    public IReadOnlyList<Building> Buildings => _state.Buildings;
    public IReadOnlyList<Recipe> Recipes => _state.Recipes;

    public Item? FindItem(ItemId id) => _state.ItemsById.GetValueOrDefault(id);
    public Building? FindBuilding(BuildingId id) => _state.BuildingsById.GetValueOrDefault(id);
    public Recipe? FindRecipe(RecipeId id) => _state.RecipesById.GetValueOrDefault(id);

    public Recipe? FindDefaultProducerOf(ItemId item)
    {
        if (!_state.ProducersByItem.TryGetValue(item, out var producers)) return null;

        // Prefer recipes that don't *also* consume the requested item. Several
        // game recipes are net producers via a feedback loop (e.g. Encased
        // Uranium Cell consumes 40 Sulfuric Acid and outputs 10 — net +10/run).
        // Picking those for backwards expansion sends demand exploding because
        // we'd have to produce more of the item just to satisfy the recipe's
        // own input. Filter them out unless they're the only option.
        var noFeedback = producers
            .Where(r => r.Inputs.All(i => i.Item != item))
            .ToList();

        return noFeedback.FirstOrDefault(r => !r.IsAlternate)
            ?? noFeedback.FirstOrDefault()
            ?? producers.FirstOrDefault(r => !r.IsAlternate)
            ?? producers.FirstOrDefault();
    }

    public IReadOnlyList<Recipe> FindAllProducersOf(ItemId item) =>
        _state.ProducersByItem.TryGetValue(item, out var producers) ? producers : [];

    public CatalogueStatus GetStatus() => BuildStatus(_state);

    public CatalogueStatus LoadFromPath(string docsJsonPath)
    {
        var resolved = CatalogueFileResolver.Resolve(docsJsonPath);
        if (resolved is null)
            throw new FileNotFoundException(
                $"Could not resolve a catalogue file from '{docsJsonPath}'. Point at the Docs directory or a specific *.json file inside it.",
                docsJsonPath);

        var newState = LoadedState.FromDocsJson(resolved);
        Interlocked.Exchange(ref _state, newState);
        _userConfig.SetDocsPath(docsJsonPath); // remember what the user gave us, not the resolved file
        _logger.LogInformation(
            "Catalogue loaded from {Path}: {Items} items, {Buildings} buildings, {Recipes} recipes ({Alternates} alternates).",
            resolved, newState.Items.Count, newState.Buildings.Count, newState.Recipes.Count,
            newState.Recipes.Count(r => r.IsAlternate));
        return BuildStatus(newState);
    }

    private LoadedState LoadAtStartup()
    {
        var path = ResolvePath();
        if (path is not null)
        {
            try
            {
                var loaded = LoadedState.FromDocsJson(path);
                _logger.LogInformation(
                    "Catalogue loaded from {Path}: {Items} items, {Buildings} buildings, {Recipes} recipes.",
                    path, loaded.Items.Count, loaded.Buildings.Count, loaded.Recipes.Count);
                return loaded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Docs.json at {Path}; catalogue will be empty until reconfigured.", path);
            }
        }
        else
        {
            _logger.LogWarning("Docs.json path not configured; catalogue is empty. Set ERP_SATISFACTORY_DOCS_PATH or use the Settings page to configure.");
        }
        return LoadedState.Empty();
    }

    private string? ResolvePath()
    {
        var env = CatalogueFileResolver.Resolve(Environment.GetEnvironmentVariable(EnvironmentVariable));
        if (env is not null) return env;

        var user = CatalogueFileResolver.Resolve(_userConfig.GetDocsPath());
        if (user is not null) return user;

        var configured = CatalogueFileResolver.Resolve(_options.DocsPath);
        if (configured is not null) return configured;

        return SteamLibraryDetector.FindDocsJson();
    }

    private static CatalogueStatus BuildStatus(LoadedState state) => new(
        IsLoaded: state.IsLoaded,
        Source: state.Source,
        ItemCount: state.Items.Count,
        BuildingCount: state.Buildings.Count,
        RecipeCount: state.Recipes.Count,
        AlternateRecipeCount: state.Recipes.Count(r => r.IsAlternate),
        Warnings: state.Warnings);

    private sealed record LoadedState(
        bool IsLoaded,
        string Source,
        IReadOnlyList<Item> Items,
        IReadOnlyList<Building> Buildings,
        IReadOnlyList<Recipe> Recipes,
        IReadOnlyList<string> Warnings,
        IReadOnlyDictionary<ItemId, Item> ItemsById,
        IReadOnlyDictionary<BuildingId, Building> BuildingsById,
        IReadOnlyDictionary<RecipeId, Recipe> RecipesById,
        IReadOnlyDictionary<ItemId, IReadOnlyList<Recipe>> ProducersByItem)
    {
        public static LoadedState Empty() =>
            Build(false, "(empty)", [], [], [], []);

        public static LoadedState FromDocsJson(string path)
        {
            using var stream = File.OpenRead(path);
            var parsed = DocsJsonParser.Parse(stream);
            return Build(true, path, parsed.Items, parsed.Buildings, parsed.Recipes, parsed.Warnings);
        }

        private static LoadedState Build(
            bool isLoaded,
            string source,
            IReadOnlyList<Item> items,
            IReadOnlyList<Building> buildings,
            IReadOnlyList<Recipe> recipes,
            IReadOnlyList<string> warnings)
        {
            var itemsById = items.GroupBy(i => i.Id).ToDictionary(g => g.Key, g => g.First());
            var buildingsById = buildings.GroupBy(b => b.Id).ToDictionary(g => g.Key, g => g.First());
            var recipesById = recipes.GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.First());
            var producers = recipes
                .SelectMany(r => r.Outputs.Select(o => (Output: o.Item, Recipe: r)))
                .GroupBy(x => x.Output)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Recipe>)g.Select(x => x.Recipe).ToList());
            return new LoadedState(isLoaded, source, items, buildings, recipes, warnings,
                itemsById, buildingsById, recipesById, producers);
        }
    }
}
