using ERP.Application;
using ERP.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SatisfactorySaveNet;
using SatisfactorySaveNet.Abstracts.Model;

const string DefaultSaveDir = @"C:\Users\ChrisSimon\AppData\Local\FactoryGame\Saved\SaveGames\76561198103946376";

var savePath = args.Length > 0 ? args[0] : FindLatestSave(DefaultSaveDir);
if (savePath is null)
{
    Console.Error.WriteLine($"No .sav files found under {DefaultSaveDir}");
    return 1;
}

Console.WriteLine($"Loading: {savePath}");
Console.WriteLine();

// === Path 1: raw library output (sanity check that v1.2 still parses) ===
RawDiagnostic(savePath);

// === Path 2: live IFactoryStateProvider end-to-end through DI ===
Console.WriteLine();
Console.WriteLine("=== IFactoryStateProvider (adapter under DI) ===");

var services = new ServiceCollection();
services.AddLogging(b => b.AddSimpleConsole(o => { o.SingleLine = true; o.TimestampFormat = "HH:mm:ss "; }));
services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(
    new Dictionary<string, string?> { ["FactoryState:Satisfactory:SavePath"] = savePath }
).Build());
services.AddErpInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());

using var sp = services.BuildServiceProvider();
var provider = sp.GetRequiredService<IFactoryStateProvider>();

var status = provider.GetStatus();
Console.WriteLine($"  IsLoaded:       {status.IsLoaded}");
Console.WriteLine($"  Source:         {status.Source}");
Console.WriteLine($"  Session:        {status.SessionName}");
Console.WriteLine($"  Save version:   {status.SaveVersion}  (build {status.BuildVersion})");
Console.WriteLine($"  Save UTC:       {status.SaveDateTimeUtc:O}");
Console.WriteLine();
Console.WriteLine($"  Miners:         {status.MinerCount}");
Console.WriteLine($"  Buildings:      {status.BuildingCount}");
Console.WriteLine($"  Belts:          {status.BeltCount}");
Console.WriteLine($"  Generators:     {status.GeneratorCount}");
Console.WriteLine($"  Resource nodes: {status.ResourceNodeCount}");

var s = provider.Current;
Console.WriteLine();
Console.WriteLine("=== Miners by tier ===");
foreach (var g in s.Miners.GroupBy(m => m.Tier).OrderBy(g => g.Key))
    Console.WriteLine($"  {g.Key}: {g.Count()}");

Console.WriteLine();
Console.WriteLine("=== Buildings by type ===");
foreach (var g in s.Buildings.GroupBy(b => b.Building.Value).OrderByDescending(g => g.Count()))
    Console.WriteLine($"  {g.Count(),5:N0}  {g.Key}");

Console.WriteLine();
Console.WriteLine("=== Belts by tier ===");
foreach (var g in s.Belts.GroupBy(b => b.Tier).OrderBy(g => g.Key))
    Console.WriteLine($"  {g.Key}: {g.Count()}");

Console.WriteLine();
Console.WriteLine("=== Generators by kind ===");
foreach (var g in s.Generators.GroupBy(g => g.Kind).OrderBy(g => g.Key))
    Console.WriteLine($"  {g.Key}: {g.Count()}");

return 0;

static string? FindLatestSave(string dir)
{
    if (!Directory.Exists(dir)) return null;
    return new DirectoryInfo(dir)
        .GetFiles("*.sav")
        .OrderByDescending(f => f.LastWriteTime)
        .FirstOrDefault()?.FullName;
}

static void RawDiagnostic(string savePath)
{
    using var headerStream = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    using var headerReader = new BinaryReader(headerStream);
    var headerOnly = HeaderSerializer.Instance.Deserialize(headerReader);
    Console.WriteLine("=== Header (raw library) ===");
    Console.WriteLine($"  HeaderVersion={headerOnly.HeaderVersion}  SaveVersion={headerOnly.SaveVersion}  Build={headerOnly.BuildVersion}");
    Console.WriteLine($"  Session={headerOnly.SessionName}  Played={TimeSpan.FromSeconds(headerOnly.PlayedSeconds)}  SaveUtc={headerOnly.SaveDateTimeUtc:O}");
}
