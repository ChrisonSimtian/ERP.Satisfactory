using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;

namespace Web.UiTests;

/// <summary>
/// Boots the full Aspire AppHost (apiservice + webfrontend) once per test class and exposes
/// a Playwright browser pointed at the webfrontend's real endpoint.
/// </summary>
public sealed class AspireAppFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = null!;
    public string WebFrontendUrl { get; private set; } = null!;
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Idempotent — Playwright skips the download when the browser is already cached.
        // Belt-and-braces with the NUKE InstallPlaywrightBrowsers target so a fresh
        // local checkout running `dotnet test` directly still works.
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Playwright chromium install failed with exit code {exitCode}.");
        }

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();
        App = await builder.BuildAsync();
        await App.StartAsync();

        await App.ResourceNotifications
            .WaitForResourceHealthyAsync("webfrontend")
            .WaitAsync(TimeSpan.FromMinutes(2));

        WebFrontendUrl = App.GetEndpoint("webfrontend").ToString();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync();
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }
        Playwright?.Dispose();
        if (App is not null)
        {
            await App.DisposeAsync();
        }
    }
}
