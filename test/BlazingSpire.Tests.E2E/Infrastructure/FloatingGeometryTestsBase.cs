using BlazingSpire.Tests.E2E;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for geometric floating invariant tests on PopoverBase-derived components.
/// Concrete class is emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class FloatingGeometryTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected FloatingGeometryTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    private const int PixelTolerance = 4;

    protected static System.Collections.Generic.IEnumerable<object[]> ClickTriggeredPopoverComponentsData() =>
        ComponentMetadata.PopoverComponents
            .Where(c => c.Name is not ("ContextMenu" or "HoverCard" or "Tooltip"))
            .Select(c => new object[] { c.Name });

    protected async Task RunGeometryAsync(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        // ── Bottom side ────────────────────────────────────────────────────────
        await driver.SetEnumParam("Side", "Bottom");
        await Page.WaitForTimeoutAsync(150);

        var triggerAll = driver.Preview.GetByText("Trigger", new() { Exact = true });
        if (await triggerAll.CountAsync() == 0) return;

        var trigger = triggerAll.First;
        await trigger.ClickAsync();

        // Wait for floating UI's final positioning pass (see comment in Top-side section).
        await Page.WaitForFunctionAsync(
            "() => { const el = document.querySelector('[data-side]'); return el && el.style.visibility !== 'hidden'; }",
            null,
            new() { Timeout = 5_000 });

        var contentBox = await driver.FloatingContent.BoundingBoxAsync();
        var triggerBox = await trigger.BoundingBoxAsync();

        if (contentBox is null || triggerBox is null) return;

        var actualSide = await driver.FloatingContent.GetAttributeAsync("data-side") ?? "bottom";
        if (actualSide == "bottom")
            Assert.True(
                contentBox.Y >= triggerBox.Y + triggerBox.Height - PixelTolerance,
                $"[{componentName}] data-side=bottom: content top ({contentBox.Y:F1}) " +
                $"should be >= trigger bottom ({triggerBox.Y + triggerBox.Height:F1}) - {PixelTolerance}px");
        else if (actualSide == "top")
            Assert.True(
                contentBox.Y <= triggerBox.Y + PixelTolerance,
                $"[{componentName}] data-side=top (flipped): content top ({contentBox.Y:F1}) " +
                $"should be <= trigger top ({triggerBox.Y:F1}) + {PixelTolerance}px");

        // ── Close and switch to Top side ──────────────────────────────────────
        await driver.CloseFloatingAsync();

        // Verify the popover actually closed. If escape doesn't work for this component
        // (e.g. PopoverBase which doesn't use focusTrap), fall back to a click-outside.
        var closedOk = false;
        try
        {
            await Page.WaitForFunctionAsync(
                "() => document.querySelector('[data-side]') === null",
                null,
                new() { Timeout = 1_000 });
            closedOk = true;
        }
        catch { /* Escape didn't close it — try click-outside */ }

        if (!closedOk)
        {
            // Click in the top-left of the page (nav area) to trigger HandleInteractOutside
            await Page.Mouse.ClickAsync(5, 5);
            await Page.WaitForTimeoutAsync(200);
        }

        await driver.SetEnumParam("Side", "Top");
        await Page.WaitForTimeoutAsync(150);

        await trigger.ClickAsync();

        // Wait until floating UI has completed its final positioning pass.
        // autoUpdate can fire multiple times (first pass sets data-side, flip middleware may
        // reposition and update data-side in a second pass). We wait for the element to become
        // visible (floating.js sets visibility='' only after the final .then callback), which
        // guarantees data-side and bounding box are both from the stable state.
        await Page.WaitForFunctionAsync(
            "() => { const el = document.querySelector('[data-side]'); return el && el.style.visibility !== 'hidden'; }",
            null,
            new() { Timeout = 5_000 });

        var contentBox2 = await driver.FloatingContent.BoundingBoxAsync();
        var triggerBox2 = await trigger.BoundingBoxAsync();

        if (contentBox2 is null || triggerBox2 is null) return;

        var actualSide2 = await driver.FloatingContent.GetAttributeAsync("data-side") ?? "top";

        // Diagnostic: all direct children of the preview div (including position:fixed ones)
        var diagInfo = await Page.EvaluateAsync<string>("""
            () => {
                const preview = document.querySelector('[data-playground-preview]');
                if (!preview) return 'no-preview';
                const children = Array.from(preview.children).map(c => {
                    const r = c.getBoundingClientRect();
                    const pos = getComputedStyle(c).position;
                    return `${c.tagName}(${c.className.substring(0,20)}) pos=${pos} y=${r.y.toFixed(1)} h=${r.height.toFixed(1)} side=${c.dataset.side||''} state=${c.dataset.state||''}`;
                });
                const floats = document.querySelectorAll('[data-side]');
                const floatStyles = Array.from(floats).map(f => `style-top=${f.style.top}`);
                return `children=[${children.join(' | ')}] floatStyles=[${floatStyles.join('|')}]`;
            }
            """);

        if (actualSide2 == "top")
            Assert.True(
                contentBox2.Y <= triggerBox2.Y + PixelTolerance,
                $"[{componentName}] data-side=top: content top ({contentBox2.Y:F1}) " +
                $"should be <= trigger top ({triggerBox2.Y:F1}) + {PixelTolerance}px. " +
                $"[data-side] elements: {diagInfo}");
        else if (actualSide2 == "bottom")
            Assert.True(
                contentBox2.Y >= triggerBox2.Y + triggerBox2.Height - PixelTolerance,
                $"[{componentName}] data-side=bottom (flipped): content top ({contentBox2.Y:F1}) " +
                $"should be >= trigger bottom ({triggerBox2.Y + triggerBox2.Height:F1}) - {PixelTolerance}px. " +
                $"[data-side] elements: {diagInfo}");

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
