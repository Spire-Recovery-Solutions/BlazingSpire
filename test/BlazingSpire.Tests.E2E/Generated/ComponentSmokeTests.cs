using BlazingSpire.Tests.E2E.Infrastructure;
using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Metadata-driven smoke tests — one test per top-level component.
/// Navigates to each playground page and asserts:
///   • No console errors
///   • No unhandled Blazor exceptions
///   • Preview pane renders
///
/// Generated dynamically via [MemberData] from components.json at runtime,
/// so new components are automatically covered without code changes.
/// </summary>
public class ComponentSmokeTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    public ComponentSmokeTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    public static IEnumerable<object[]> AllComponents() =>
        ComponentMetadata.TopLevel.Select(c => new object[] { c.Name });

    [Theory]
    [MemberData(nameof(AllComponents))]
    public async Task Playground_Page_Loads_Without_Errors(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        await Expect(driver.Preview).ToBeVisibleAsync();

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
