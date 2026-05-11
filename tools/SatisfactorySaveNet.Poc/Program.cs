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

// Read header first so we can diagnose version mismatches before the body parse blows up.
using (var headerStream = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
using (var headerReader = new BinaryReader(headerStream))
{
    var headerOnly = HeaderSerializer.Instance.Deserialize(headerReader);
    Console.WriteLine();
    Console.WriteLine("=== Header (read independently) ===");
    DumpProps(headerOnly);
}

var stopwatch = System.Diagnostics.Stopwatch.StartNew();
SatisfactorySave save;
try
{
    save = SaveFileSerializer.Instance.Deserialize(savePath);
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine($"!!! Full deserialize FAILED after {stopwatch.ElapsedMilliseconds} ms: {ex.GetType().Name}: {ex.Message}");
    Console.Error.WriteLine($"    at {ex.StackTrace?.Split(Environment.NewLine).FirstOrDefault()?.Trim()}");
    return 3;
}
stopwatch.Stop();
Console.WriteLine($"Parsed in {stopwatch.ElapsedMilliseconds} ms");

if (save.Body is not BodyV8 body)
{
    Console.Error.WriteLine($"Body is not V8 (got {save.Body?.GetType().Name ?? "null"}). v1.2 saves should be V8 — bailing.");
    return 2;
}

var allObjects = body.Levels.SelectMany(l => l.Objects).ToList();
Console.WriteLine();
Console.WriteLine($"Levels: {body.Levels.Count}, Total objects: {allObjects.Count:N0}");

var typeCounts = allObjects
    .GroupBy(o => ShortName(o.TypePath))
    .Select(g => new { Name = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count)
    .ToList();

Console.WriteLine();
Console.WriteLine("=== Top 20 class names ===");
foreach (var t in typeCounts.Take(20))
    Console.WriteLine($"  {t.Count,6:N0}  {t.Name}");

string[] interestingPatterns = ["MinerMk", "Smelter", "Constructor", "Assembler", "Foundry", "OilRefinery", "Generator", "ResourceNode", "ConveyorBelt"];

foreach (var pattern in interestingPatterns)
{
    var matches = typeCounts
        .Where(t => t.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
        .ToList();
    if (matches.Count == 0) continue;

    Console.WriteLine();
    Console.WriteLine($"=== {pattern} ===");
    foreach (var m in matches)
        Console.WriteLine($"  {m.Count,5:N0}  {m.Name}");
}

Console.WriteLine();
Console.WriteLine("=== Miner positions ===");
var miners = allObjects.OfType<ActorObject>()
    .Where(a => a.TypePath.Contains("MinerMk", StringComparison.OrdinalIgnoreCase))
    .ToList();

foreach (var m in miners)
{
    var name = ShortName(m.TypePath);
    Console.WriteLine($"  {name,-30}  pos=({m.Position.X,8:F0}, {m.Position.Y,8:F0}, {m.Position.Z,7:F0})  ref={m.ObjectReference.PathName}");
}

Console.WriteLine();
Console.WriteLine($"Total miners: {miners.Count}");

return 0;

static string? FindLatestSave(string dir)
{
    if (!Directory.Exists(dir)) return null;
    return new DirectoryInfo(dir)
        .GetFiles("*.sav")
        .OrderByDescending(f => f.LastWriteTime)
        .FirstOrDefault()?.FullName;
}

static string ShortName(string typePath)
{
    if (string.IsNullOrEmpty(typePath)) return "(empty)";
    var lastDot = typePath.LastIndexOf('.');
    return lastDot < 0 ? typePath : typePath[(lastDot + 1)..];
}

static void DumpProps(object obj)
{
    foreach (var p in obj.GetType().GetProperties())
    {
        object? value;
        try { value = p.GetValue(obj); }
        catch { continue; }

        if (value is null) continue;
        if (value is System.Collections.IEnumerable enumerable and not string)
        {
            var count = 0;
            foreach (var _ in enumerable) count++;
            Console.WriteLine($"  {p.Name}: <{p.PropertyType.Name}, {count} items>");
        }
        else
        {
            var str = value.ToString();
            if (str is { Length: > 100 }) str = str[..100] + "...";
            Console.WriteLine($"  {p.Name}: {str}");
        }
    }
}
