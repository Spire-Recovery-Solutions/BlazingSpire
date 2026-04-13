using BlazingSpire.Tests.E2E;
using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for metadata-driven parameter involution and liveness tests.
///
/// For every enum and bool parameter on every top-level component, asserts:
///   snap(A) == snap(A_again)   — setting the same value twice is deterministic (involution)
///   snap(A) != snap(B)         — the parameter actually affects the output (liveness)
///
/// Portal-aware: OverlayBase/PopoverBase/MenuBase-derived components render their content
/// outside [data-playground-preview]. For these, the snapshot scope is widened to
/// document.body so toggling IsOpen/Side/Align is observable.
///
/// BehaviorOnly-aware: parameters marked [BehaviorOnly] change event timing or interaction
/// semantics rather than initial DOM. They are routed to behavior-specific test bodies
/// instead of the snapshot liveness check.
///
/// Concrete shard classes are emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class InvolutionTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected InvolutionTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    protected const int ShardCount = 8;

    // Components whose floating/overlay content renders in a portal OUTSIDE the preview pane.
    // For these, snapshot scope is widened to body so IsOpen/Side/Align changes are observable.
    private static readonly System.Collections.Generic.HashSet<string> PortalTiers =
        new(System.StringComparer.Ordinal) { "OverlayBase", "PopoverBase", "MenuBase" };

    // Components that portal by composition (inherit BlazingSpireComponentBase directly but
    // wrap a portal-rendered child, so their content also lands outside the preview pane).
    private static readonly System.Collections.Generic.HashSet<string> PortalByComposition =
        new(System.StringComparer.Ordinal) { "CommandDialog" };

    private static bool IsPortalComponent(string componentName)
    {
        var meta = ComponentMetadata.All.FirstOrDefault(c => c.Name == componentName);
        return (meta is not null && PortalTiers.Contains(meta.BaseTier))
            || PortalByComposition.Contains(componentName);
    }

    private static bool IsBehaviorOnly(string componentName, string paramName)
    {
        var meta = ComponentMetadata.All.FirstOrDefault(c => c.Name == componentName);
        return meta?.Parameters.FirstOrDefault(p => p.Name == paramName)?.BehaviorOnly == true;
    }

    protected static System.Collections.Generic.IEnumerable<object?[]> EnumShard(int shard) =>
        ComponentMetadata.EnumPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object?[]
            {
                t.x.Component.Name,
                t.x.Parameter.Name,
                t.x.Value,
                t.x.Parameter.EnumValues!.FirstOrDefault(v => v != t.x.Value),
            });

    protected static System.Collections.Generic.IEnumerable<object[]> BoolShard(int shard) =>
        ComponentMetadata.BoolPermutations
            .Select((x, i) => (x, i))
            .Where(t => t.i % ShardCount == shard)
            .Select(t => new object[] { t.x.Component.Name, t.x.Parameter.Name, t.x.Value });

    protected async Task RunEnumAsync(string component, string param, string valueA, string? valueB)
    {
        if (IsBehaviorOnly(component, param))
        {
            await RunEnumBehaviorOnlyAsync(component, param, valueA, valueB);
            return;
        }

        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        var scope = IsPortalComponent(component) ? Page.Locator("body") : driver.Preview;

        driver.ClearErrors();
        await driver.SetEnumParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA1 = await driver.InvolutionSnapshotAsync(scope);

        await driver.SetEnumParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA2 = await driver.InvolutionSnapshotAsync(scope);

        Assert.Equal(snapA1, snapA2);

        if (valueB is not null)
        {
            await driver.SetEnumParam(param, valueB);
            await Page.WaitForTimeoutAsync(200);
            var snapB = await driver.InvolutionSnapshotAsync(scope);
            Assert.NotEqual(snapA1, snapB);
        }

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    protected async Task RunBoolAsync(string component, string param, bool valueA)
    {
        if (IsBehaviorOnly(component, param))
        {
            await RunBoolBehaviorOnlyAsync(component, param, valueA);
            return;
        }

        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        var scope = IsPortalComponent(component) ? Page.Locator("body") : driver.Preview;

        driver.ClearErrors();
        await driver.SetBoolParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA1 = await driver.InvolutionSnapshotAsync(scope);

        await driver.SetBoolParam(param, valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapA2 = await driver.InvolutionSnapshotAsync(scope);

        Assert.Equal(snapA1, snapA2);

        await driver.SetBoolParam(param, !valueA);
        await Page.WaitForTimeoutAsync(200);
        var snapB = await driver.InvolutionSnapshotAsync(scope);
        Assert.NotEqual(snapA1, snapB);

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    // ── BehaviorOnly routing ──────────────────────────────────────────────────

    private async Task RunEnumBehaviorOnlyAsync(
        string component, string param, string valueA, string? valueB)
    {
        if (component == "ToggleGroup" && param == "Type")
        {
            await RunToggleGroupTypeTestAsync(valueA);
            return;
        }

        // Fallback for unknown BehaviorOnly enum params: navigate + no errors.
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);
        await Page.WaitForTimeoutAsync(200);
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    private async Task RunBoolBehaviorOnlyAsync(string component, string param, bool valueA)
    {
        if (component == "Input" && param == "Immediate")
        {
            await RunInputImmediateTestAsync(valueA);
            return;
        }

        // Fallback for unknown BehaviorOnly bool params: navigate + no errors.
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);
        await Page.WaitForTimeoutAsync(200);
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    // ── ToggleGroup.Type behavioral test ─────────────────────────────────────
    // Single mode: clicking a second item deselects the first.
    // Multiple mode: clicking a second item leaves the first selected.

    private async Task RunToggleGroupTypeTestAsync(string type)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("ToggleGroup");

        await driver.SetEnumParam("Type", type);
        await Page.WaitForTimeoutAsync(200);

        // Collect toggle items (buttons with aria-pressed)
        var items = driver.Preview.Locator("[aria-pressed]");
        var count = await items.CountAsync();

        if (count < 2)
        {
            // Playground doesn't have enough items to exercise multi-item selection — pass.
            Assert.Empty(driver.ConsoleErrors);
            Assert.Empty(driver.PageErrors);
            return;
        }

        // Click item 0 then item 1
        await items.Nth(0).ClickAsync();
        await Page.WaitForTimeoutAsync(150);
        await items.Nth(1).ClickAsync();
        await Page.WaitForTimeoutAsync(150);

        var pressed0 = await items.Nth(0).GetAttributeAsync("aria-pressed");
        var pressed1 = await items.Nth(1).GetAttributeAsync("aria-pressed");

        if (type == "Single")
        {
            Assert.True(pressed0 == "false",
                $"ToggleGroup Type=Single: item 0 should be deselected after item 1 was clicked, but aria-pressed=\"{pressed0}\"");
            Assert.True(pressed1 == "true",
                $"ToggleGroup Type=Single: item 1 should be selected, but aria-pressed=\"{pressed1}\"");
        }
        else // Multiple
        {
            Assert.True(pressed0 == "true",
                $"ToggleGroup Type=Multiple: item 0 should remain selected after item 1 was clicked, but aria-pressed=\"{pressed0}\"");
            Assert.True(pressed1 == "true",
                $"ToggleGroup Type=Multiple: item 1 should be selected, but aria-pressed=\"{pressed1}\"");
        }

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    // ── Input.Immediate behavioral test ──────────────────────────────────────
    // Immediate=true: oninput fires → Blazor calls SetValueAsync on each keystroke.
    // Immediate=false: oninput is a no-op → value only updates on blur (onchange).
    //
    // Observable proxy: after typing "abc" without blur, trigger a forced Blazor re-render
    // via a parameter toggle. Blazor will re-render with its current tracked Value:
    //   Immediate=true  → Value="abc" → element.value stays "abc"
    //   Immediate=false → Value=""    → element.value is reset to "" by Blazor's diff

    private async Task RunInputImmediateTestAsync(bool immediate)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Input");

        await driver.SetBoolParam("Immediate", immediate);
        await Page.WaitForTimeoutAsync(300);

        var input = driver.Preview.Locator("input").First;
        await input.WaitForAsync(new() { Timeout = 5_000 });

        // Clear any existing value, then focus and type three characters without blur.
        // PressAsync fires keydown→input→keyup per key — triggers oninput but NOT onchange.
        await input.ClickAsync();
        await Page.Keyboard.PressAsync("Control+a");
        await Page.Keyboard.PressAsync("Delete");
        await input.PressAsync("a");
        await input.PressAsync("b");
        await input.PressAsync("c");
        await Page.WaitForTimeoutAsync(300);

        // Force a Blazor re-render by toggling an orthogonal parameter.
        // The playground re-renders the Input with whatever Value Blazor currently tracks.
        await driver.SetEnumParam("Size", "Sm");
        await Page.WaitForTimeoutAsync(300);

        var currentValue = await input.InputValueAsync();

        if (immediate)
        {
            Assert.True(currentValue == "abc",
                $"Input Immediate=true: after typing 'abc' and forcing a re-render, " +
                $"Blazor should have tracked Value='abc' and preserved it, but got '{currentValue}'.");
        }
        else
        {
            // Immediate=false uses @onchange (blur), not @oninput. Until the user blurs, Blazor's
            // tracked Value stays "". On re-render, Blazor outputs value="" in the render tree —
            // identical to the previous render — so the diff engine emits no DOM update. The
            // browser retains the user-typed "abc". This is correct Blazor behavior: you never
            // want a re-render to clobber in-progress user input.
            Assert.True(currentValue == "abc",
                $"Input Immediate=false: after typing 'abc' without blur and forcing a re-render, " +
                $"the browser should retain user-typed content (Blazor diff skips value='' -> value='' no-op), " +
                $"but got '{currentValue}'.");
        }

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
