namespace Satisfactory.Save;

/// <summary>
/// Resolves a user-supplied save-file path. Accepts either a specific
/// <c>.sav</c> file or a SaveGames directory; in the directory case picks the
/// most recently written <c>.sav</c>. Returns <c>null</c> if nothing usable.
/// </summary>
public static class SaveFileResolver
{
    public static string? Resolve(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (File.Exists(path)) return path;

        if (Directory.Exists(path))
        {
            return new DirectoryInfo(path)
                .EnumerateFiles("*.sav", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault()?.FullName;
        }

        return null;
    }

    /// <summary>
    /// Best-effort auto-detect of the user's local SaveGames root. Returns the
    /// directory containing the user's most-recently-played save folder, or
    /// <c>null</c> if the standard FactoryGame Saved/SaveGames path doesn't exist.
    /// </summary>
    public static string? AutoDetectLatestSave()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localAppData)) return null;

        var saveGames = Path.Combine(localAppData, "FactoryGame", "Saved", "SaveGames");
        if (!Directory.Exists(saveGames)) return null;

        // Each SteamID is a subdirectory; pick the one with the most-recent .sav.
        var latest = new DirectoryInfo(saveGames)
            .EnumerateDirectories()
            .SelectMany(d => d.EnumerateFiles("*.sav", SearchOption.TopDirectoryOnly))
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .FirstOrDefault();
        return latest?.FullName;
    }
}
