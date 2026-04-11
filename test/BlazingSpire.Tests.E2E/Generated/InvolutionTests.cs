using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Metadata-driven parameter involution and liveness tests.
///
/// For every enum and bool parameter on every top-level component, asserts:
///   snap(A) == snap(A_again)   — setting the same value twice is deterministic (involution)
///   snap(A) != snap(B)         — the parameter actually affects the output (liveness)
///
/// Snapshot = Preview innerHTML with non-deterministic attributes stripped
/// (generated IDs, ARIA reference attributes, Floating UI nonces).
///
/// Sharded into 8 classes matching the ParameterPermutationTests pattern so xUnit
/// can run them in parallel across the 10 available threads.
/// </summary>
public abstract class InvolutionTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected InvolutionTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    protected const int ShardCount = 8;

    // Components whose floating/overlay content renders in a portal OUTSIDE the
    // preview pane — setting Side, Align, IsOpen, DefaultIsOpen won't change the
    // preview innerHTML, making liveness assertions false-positives.
    private static readonly HashSet<string> LivenessSkipBaseTiers =
        new(StringComparer.Ordinal) { "OverlayBase", "PopoverBase" };

    // Params that are purely behavioral and produce no visible HTML change.
    private static readonly HashSet<string> LivenessSkipParamNames =
        new(StringComparer.Ordinal) { "Immediate" };

    // Component+param combos that are structurally real but visually indistinguishable
    // in the playground default state, or whose effect renders outside the preview pane.
    private static readonly HashSet<(string, string)> LivenessSkipCombos =
        new()
        {
            // ToggleGroup.Type: visually identical with a single-item group
            ("ToggleGroup", "Type"),
            // CommandDialog.IsOpen: dialog content renders in a portal outside the preview pane
            ("CommandDialog", "IsOpen"),
        };

    private static bool ShouldSkipLiveness(string componentName, string paramName)
    {
        var meta = ComponentMetadata.All.FirstOrDefault(c => c.Name == componentName);
        if (meta is not null && LivenessSkipBaseTiers.Contains(meta.BaseTier))
            return true;
        if (LivenessSkipParamNames.Contains(paramName))
            return true;
        if (LivenessSkipCombos.Contains((componentName, paramName)))
            return true;
        return false;
    }

    protected static IEnumerable<object?[]> EnumShard(int shard) =>
        ComponentMetadata.EnumPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object?[]
            {
                t.x.Component.Name,
                t.x.Parameter.Name,
                t.x.Value,
                // valueB: first enum value that differs from valueA (null if only one value)
                t.x.Parameter.EnumValues!.FirstOrDefault(v => v != t.x.Value),
            });

    protected static IEnumerable<object[]> BoolShard(int shard) =>
        ComponentMetadata.BoolPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object[] { t.x.Component.Name, t.x.Parameter.Name, t.x.Value });

    /// <summary>
    /// Set enum param to A, snapshot, set to A again, snapshot — must match (determinism).
    /// If a second value B exists, set to B — snapshot must differ from A (liveness).
    /// </summary>
    protected async Task RunEnumAsync(string component, string param, string valueA, string? valueB)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetEnumParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA1 = await driver.InvolutionSnapshotAsync(driver.Preview);

        // Re-set the same value — must produce identical normalized HTML.
        await driver.SetEnumParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA2 = await driver.InvolutionSnapshotAsync(driver.Preview);

        Assert.Equal(snapA1, snapA2);

        // Set a different value — must produce different normalized HTML.
        // Skip liveness for params that render outside the preview pane (overlays,
        // popovers) or that are purely behavioral (no visible HTML effect).
        if (valueB is not null && !ShouldSkipLiveness(component, param))
        {
            await driver.SetEnumParam(param, valueB);
            await Page.WaitForTimeoutAsync(200);
            var snapB = await driver.InvolutionSnapshotAsync(driver.Preview);
            Assert.NotEqual(snapA1, snapB);
        }

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    /// <summary>
    /// Set bool param to A, snapshot, set to A again — must match (determinism).
    /// Then set to !A — snapshot must differ (liveness).
    /// </summary>
    protected async Task RunBoolAsync(string component, string param, bool valueA)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetBoolParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA1 = await driver.InvolutionSnapshotAsync(driver.Preview);

        // Re-set the same value — must produce identical normalized HTML.
        await driver.SetBoolParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA2 = await driver.InvolutionSnapshotAsync(driver.Preview);

        Assert.Equal(snapA1, snapA2);

        // Flip the value — must produce different output.
        // Skip liveness for params that render outside the preview pane (overlays,
        // popovers) or that are purely behavioral (no visible HTML effect).
        if (!ShouldSkipLiveness(component, param))
        {
            await driver.SetBoolParam(param, !valueA);
            await Page.WaitForTimeoutAsync(200);
            var snapB = await driver.InvolutionSnapshotAsync(driver.Preview);
            Assert.NotEqual(snapA1, snapB);
        }

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}

public class InvolutionTestsShard0 : InvolutionTestsBase
{
    public InvolutionTestsShard0(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(0);
    public static IEnumerable<object[]> Bools() => BoolShard(0);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard1 : InvolutionTestsBase
{
    public InvolutionTestsShard1(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(1);
    public static IEnumerable<object[]> Bools() => BoolShard(1);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard2 : InvolutionTestsBase
{
    public InvolutionTestsShard2(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(2);
    public static IEnumerable<object[]> Bools() => BoolShard(2);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard3 : InvolutionTestsBase
{
    public InvolutionTestsShard3(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(3);
    public static IEnumerable<object[]> Bools() => BoolShard(3);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard4 : InvolutionTestsBase
{
    public InvolutionTestsShard4(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(4);
    public static IEnumerable<object[]> Bools() => BoolShard(4);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard5 : InvolutionTestsBase
{
    public InvolutionTestsShard5(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(5);
    public static IEnumerable<object[]> Bools() => BoolShard(5);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard6 : InvolutionTestsBase
{
    public InvolutionTestsShard6(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(6);
    public static IEnumerable<object[]> Bools() => BoolShard(6);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}

public class InvolutionTestsShard7 : InvolutionTestsBase
{
    public InvolutionTestsShard7(PlaywrightBrowserFixture f) : base(f) { }

    public static IEnumerable<object?[]> Enums() => EnumShard(7);
    public static IEnumerable<object[]> Bools() => BoolShard(7);

    [Theory, MemberData(nameof(Enums))]
    public Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);

    [Theory, MemberData(nameof(Bools))]
    public Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);
}
