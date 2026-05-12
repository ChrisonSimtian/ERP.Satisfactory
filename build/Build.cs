using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable AllUnderscoreLocalParameterName

/// <summary>
/// ERP.Satisfactory build pipeline.
///
/// Local usage:
///   ./build.ps1               (default target — Compile)
///   ./build.sh Test           (runs Clean → Restore → Compile → Test)
///   ./build.cmd Format        (verifies dotnet format passes; excludes vendor/)
///
/// CI invokes the same entry points (see .github/workflows/ci.yml). Logic
/// lives here in C# rather than YAML so it runs identically locally.
/// </summary>
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build — Debug locally, Release on CI.")]
    readonly Configuration Configuration = IsServerBuild ? Configuration.Release : Configuration.Debug;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution = null!;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";

    Target Clean => _ => _
        .Description("Clears the artifacts directory.")
        .Before(Restore)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Description("Restores NuGet packages for the solution.")
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .Description("Builds the solution.")
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Format => _ => _
        .Description("Verifies dotnet format passes (read-only; CI gate). Vendor submodule excluded.")
        .Executes(() =>
        {
            DotNet($"format \"{Solution.Path}\" --verify-no-changes --exclude vendor/");
        });

    Target Test => _ => _
        .Description("Runs all xUnit test projects, emits TRX results to artifacts/test-results/.")
        .DependsOn(Compile)
        .Executes(() =>
        {
            TestResultsDirectory.CreateOrCleanDirectory();
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetResultsDirectory(TestResultsDirectory)
                .AddLoggers("trx;LogFilePrefix=test")
                .AddLoggers("console;verbosity=normal"));
            Log.Information("TRX results written to {Dir}", TestResultsDirectory);
        });
}
