using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Web.UiTests;

public class SmokeTests(AspireAppFixture fixture) : IClassFixture<AspireAppFixture>
{
    [Fact]
    public async Task Home_page_returns_200_and_renders_known_heading()
    {
        var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var response = await page.GotoAsync(fixture.WebFrontendUrl);

        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);
        await Expect(page.Locator("h1")).ToHaveTextAsync("ERP.Satisfactory");
    }

    [Fact]
    public async Task Planner_page_renders_MudAutocomplete_pickers()
    {
        // Phase 1 proof-of-integration test for MudBlazor adoption: confirms the framework
        // is wired correctly (services registered, CSS/JS loaded, providers mounted) by
        // rendering the page and exercising one MudAutocomplete interactively. Full
        // round-trip (type → filter → select → submit) needs a seeded catalogue in
        // tests — deferred to a later phase.
        var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        var consoleErrors = new List<string>();
        page.Console += (_, msg) => { if (msg.Type == "error") consoleErrors.Add(msg.Text); };

        var response = await page.GotoAsync($"{fixture.WebFrontendUrl.TrimEnd('/')}/planner");

        Assert.NotNull(response);
        Assert.Equal(200, response!.Status);

        // MudAutocomplete renders as a MudBlazor text-field-shaped input with a known class.
        // Two pickers on the page (one for sources, one for sinks) → expect 2 occurrences.
        var pickers = page.Locator(".mud-autocomplete");
        await Expect(pickers).ToHaveCountAsync(2);

        // The Blazor error UI is on the page (id="blazor-error-ui") and CSS-hidden until
        // an unhandled circuit exception unhides it. If MudBlazor's providers weren't in
        // the same interactive render tree as the autocompletes, this would flip visible.
        await Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();

        // Click the first picker — forces MudAutocomplete to request its popover, which
        // requires <MudPopoverProvider /> to be reachable in the interactive render tree.
        // Catches the render-mode boundary class of bug that pure rendering tests miss.
        await pickers.First.ClickAsync();
        await Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();

        Assert.Empty(consoleErrors);
    }
}
