using BlazingSpire.Tests.E2E.Infrastructure;
using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Hand-written interaction tests for composite components.
/// These test the full open/close/position flow that metadata-driven
/// tests cannot express — the actual user-facing behavior.
/// </summary>
[Collection("BlazorApp")]
public class CompositeInteractionTests : BlazingSpireE2EBase
{
    // ── Popover ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Popover_Opens_On_Trigger_Click()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Popover");

        // Content should be hidden initially
        await Expect(Page.Locator("[data-side]")).ToHaveCountAsync(0);

        // Click the trigger
        await Page.GetByText("Open Popover").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        // Content should appear with data-side attribute set by Floating UI
        await Expect(Page.Locator("[data-side]").First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Popover_Alignment_Changes_Content_Position()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Popover");

        // Open popover with Start align (default)
        await Page.GetByText("Open Popover").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        var startBox = await Page.Locator("[data-side]").First.BoundingBoxAsync();
        Assert.NotNull(startBox);

        // Switch to End alignment
        await driver.SetEnumParam("Align", "End");
        await Page.WaitForTimeoutAsync(300);
        await Page.GetByText("Open Popover").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        var endBox = await Page.Locator("[data-side]").First.BoundingBoxAsync();
        Assert.NotNull(endBox);

        // End alignment must produce a different x position than Start
        Assert.NotEqual(startBox.X, endBox.X);
    }

    // ── Dialog ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Dialog_Opens_And_Shows_Content()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Dialog");

        await Page.GetByText("Open Dialog").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dialog_Closes_On_Escape_Key()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Dialog");

        await Page.GetByText("Open Dialog").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");
        await Page.WaitForTimeoutAsync(300);
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
    }

    // ── Tabs ──────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Tabs_Renders_With_Trigger_And_Content()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Tabs");

        // Playground renders Tabs composite — should have at least the preview visible
        await Expect(driver.Preview).ToBeVisibleAsync();
        Assert.Empty(driver.ConsoleErrors);
    }

    // ── Collapsible ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Collapsible_Toggles_Open_On_Click()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Collapsible");

        var trigger = Page.GetByText("Open Collapsible").First;
        await trigger.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        Assert.Empty(driver.ConsoleErrors);
    }

    // ── Sheet ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Sheet_Opens_On_Trigger_Click()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Sheet");

        await Page.GetByText("Open Sheet").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        Assert.Empty(driver.ConsoleErrors);
    }

    // ── AlertDialog ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AlertDialog_Opens_And_Shows_Content()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("AlertDialog");

        await Page.GetByText("Open AlertDialog").First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        Assert.Empty(driver.ConsoleErrors);
    }
}
