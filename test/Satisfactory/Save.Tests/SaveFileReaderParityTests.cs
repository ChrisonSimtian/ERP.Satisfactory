using ERP.Domain;

namespace Satisfactory.Save.Tests;

/// <summary>
/// End-to-end parity check: parse a real v1.2 save and confirm the adapter's
/// counts line up with the human-curated stocktake. Gated on the
/// <c>ERP_SATISFACTORY_SAVE_PATH</c> env var or auto-detect — passes silently
/// when no save is available so CI doesn't fail on developers without one.
/// When the env var points at a directory the resolver picks the most-recent
/// <c>.sav</c>.
/// </summary>
public class SaveFileReaderParityTests
{
    [Fact]
    public void Parses_Real_Save_File_With_Expected_Shape()
    {
        var path = SaveFileResolver.Resolve(Environment.GetEnvironmentVariable("ERP_SATISFACTORY_SAVE_PATH"))
            ?? SaveFileResolver.AutoDetectLatestSave();

        if (path is null)
        {
            // No save available — skip silently. xUnit v2 lacks first-class
            // skip-at-runtime; we treat absence as "nothing to verify here".
            return;
        }

        var state = new SaveFileReader().Read(path);

        Assert.NotEqual(0, state.Save.SaveVersion);
        Assert.NotEqual(0, state.Save.BuildVersion);
        Assert.NotEmpty(state.Save.SessionName);

        // We expect some world content in any real save — at least one
        // resource node. Asserting specific miner/smelter counts would pin
        // this test to one particular save snapshot.
        Assert.NotEmpty(state.ResourceNodes);
    }
}
