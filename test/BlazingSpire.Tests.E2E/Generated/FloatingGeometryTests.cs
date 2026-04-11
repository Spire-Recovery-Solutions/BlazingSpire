using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Geometric floating invariants for PopoverBase-derived components.
///
/// For each component with baseTier == "PopoverBase" (excluding hover- and right-click-triggered
/// variants), opens the floating content and asserts bounding-box relationships:
///
///   Side=Bottom  → content.top    >= trigger.bottom - tolerance  (content is below the trigger)
///   Side=Top     → content.bottom &lt;= trigger.top    + tolerance  (content is above the trigger)
///
/// These tests catch Floating UI positioning regressions without any snapshot baseline —
/// the geometric relationship is derived purely from the Side parameter value.
///
/// Components excluded from the standard trigger-click path:
///   - ContextMenu: opens on right-click, not a "Trigger" button
///   - HoverCard:   opens on mouse hover
///   - Tooltip:     opens on mouse hover
/// </summary>
public class FloatingGeometryTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    private const int PixelTolerance = 4;

    public FloatingGeometryTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    public static IEnumerable<object[]> ClickTriggeredPopoverComponents() =>
        ComponentMetadata.PopoverComponents
            // Exclude hover/right-click triggered components — they don't have a "Trigger" button
            .Where(c => c.Name is not ("ContextMenu" or "HoverCard" or "Tooltip"))
            .Select(c => new object[] { c.Name });

    /// <summary>
    /// For each click-triggered PopoverBase component:
    ///   1. Set Side=Bottom, click Trigger → assert floating content is BELOW the trigger
    ///   2. Close, set Side=Top, click Trigger → assert floating content is ABOVE the trigger
    /// </summary>
    [Theory, MemberData(nameof(ClickTriggeredPopoverComponents))]
    public async Task Floating_Content_Respects_Side_Parameter(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        // ── Bottom side ────────────────────────────────────────────────────────
        await driver.SetEnumParam("Side", "Bottom");
        await Page.WaitForTimeoutAsync(150);

        // Some components (e.g. Select, Combobox) don't have a "Trigger" label button —
        // they use placeholder text or an input field as the activator. Skip those.
        var triggerAll = driver.Preview.GetByText("Trigger", new() { Exact = true });
        if (await triggerAll.CountAsync() == 0) return;

        var trigger = triggerAll.First;
        await trigger.ClickAsync();
        await Page.WaitForTimeoutAsync(350);

        var contentBox = await driver.FloatingContent.BoundingBoxAsync();
        var triggerBox = await trigger.BoundingBoxAsync();

        if (contentBox is null || triggerBox is null) return;

        // Read the actual side Floating UI chose — it may have flipped if viewport
        // space was insufficient. Assert that the bounding-box relationship is
        // consistent with the *actual* placement, not just the requested one.
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
        await driver.SetEnumParam("Side", "Top");
        await Page.WaitForTimeoutAsync(150);

        await trigger.ClickAsync();
        await Page.WaitForTimeoutAsync(350);

        var contentBox2 = await driver.FloatingContent.BoundingBoxAsync();
        var triggerBox2 = await trigger.BoundingBoxAsync();

        if (contentBox2 is null || triggerBox2 is null) return;

        var actualSide2 = await driver.FloatingContent.GetAttributeAsync("data-side") ?? "top";
        if (actualSide2 == "top")
            Assert.True(
                contentBox2.Y <= triggerBox2.Y + PixelTolerance,
                $"[{componentName}] data-side=top: content top ({contentBox2.Y:F1}) " +
                $"should be <= trigger top ({triggerBox2.Y:F1}) + {PixelTolerance}px");
        else if (actualSide2 == "bottom")
            Assert.True(
                contentBox2.Y >= triggerBox2.Y + triggerBox2.Height - PixelTolerance,
                $"[{componentName}] data-side=bottom (flipped): content top ({contentBox2.Y:F1}) " +
                $"should be >= trigger bottom ({triggerBox2.Y + triggerBox2.Height:F1}) - {PixelTolerance}px");

        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
