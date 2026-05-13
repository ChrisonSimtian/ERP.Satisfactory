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
}
