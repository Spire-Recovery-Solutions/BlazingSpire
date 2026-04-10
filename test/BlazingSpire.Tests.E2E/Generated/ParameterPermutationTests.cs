using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Metadata-driven parameter permutation tests.
/// For each component × parameter × value combination, navigate to the playground,
/// set the parameter, and verify no errors occur. This catches type coercion bugs,
/// missing variant mappings, and any render crashes that happen only on parameter
/// change — the exact class of bugs bUnit tests miss.
///
/// Sharded into 8 classes so xUnit can run them in parallel: each shard is its own
/// test class = its own collection. Combined with ComponentSmokeTests and
/// CompositeInteractionTests that gives 10 parallel collections, matching
/// <c>maxParallelThreads</c> in <c>xunit.runner.json</c>.
/// </summary>
public abstract class ParameterPermutationTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected ParameterPermutationTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    protected const int ShardCount = 8;

    protected static IEnumerable<object[]> EnumShard(int shard) =>
        ComponentMetadata.EnumPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object[] { t.x.Component.Name, t.x.Parameter.Name, t.x.Value });

    protected static IEnumerable<object[]> BoolShard(int shard) =>
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

public class ParameterPermutationTestsShard0 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard0(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(0);
    public static IEnumerable<object[]> Bools() => BoolShard(0);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard1 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard1(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(1);
    public static IEnumerable<object[]> Bools() => BoolShard(1);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard2 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard2(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(2);
    public static IEnumerable<object[]> Bools() => BoolShard(2);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard3 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard3(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(3);
    public static IEnumerable<object[]> Bools() => BoolShard(3);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard4 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard4(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(4);
    public static IEnumerable<object[]> Bools() => BoolShard(4);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard5 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard5(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(5);
    public static IEnumerable<object[]> Bools() => BoolShard(5);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard6 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard6(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(6);
    public static IEnumerable<object[]> Bools() => BoolShard(6);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}

public class ParameterPermutationTestsShard7 : ParameterPermutationTestsBase
{
    public ParameterPermutationTestsShard7(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object[]> Enums() => EnumShard(7);
    public static IEnumerable<object[]> Bools() => BoolShard(7);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);
}
