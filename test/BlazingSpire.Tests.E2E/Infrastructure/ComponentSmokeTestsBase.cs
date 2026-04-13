using BlazingSpire.Tests.E2E;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for metadata-driven smoke tests.
/// Concrete class is emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class ComponentSmokeTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected ComponentSmokeTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    protected static System.Collections.Generic.IEnumerable<object[]> AllComponentsData() =>
        ComponentMetadata.TopLevel.Select(c => new object[] { c.Name });

    protected async Task RunSmokeAsync(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        await Expect(driver.Preview).ToBeVisibleAsync();

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
