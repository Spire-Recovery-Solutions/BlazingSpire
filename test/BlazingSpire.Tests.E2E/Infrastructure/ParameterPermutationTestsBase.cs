using BlazingSpire.Tests.E2E;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for metadata-driven parameter permutation tests.
/// For each component × parameter × value combination, navigate to the playground,
/// set the parameter, and verify no errors occur.
///
/// Concrete shard classes are emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class ParameterPermutationTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected ParameterPermutationTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    protected const int ShardCount = 8;

    protected static System.Collections.Generic.IEnumerable<object[]> EnumShard(int shard) =>
        ComponentMetadata.EnumPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object[] { t.x.Component.Name, t.x.Parameter.Name, t.x.Value });

    protected static System.Collections.Generic.IEnumerable<object[]> BoolShard(int shard) =>
        ComponentMetadata.BoolPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object[] { t.x.Component.Name, t.x.Parameter.Name, t.x.Value });

    protected async Task RunEnumAsync(string component, string param, string value)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetEnumParam(param, value);
        await Page.WaitForTimeoutAsync(200);

        await Expect(driver.Preview).ToBeVisibleAsync();
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    protected async Task RunBoolAsync(string component, string param, bool value)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetBoolParam(param, value);
        await Page.WaitForTimeoutAsync(200);

        await Expect(driver.Preview).ToBeVisibleAsync();
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
