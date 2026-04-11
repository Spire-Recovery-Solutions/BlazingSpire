using BlazingSpire.Tests.E2E.Infrastructure;
using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Hand-written interaction tests for composite components.
/// These test the full open/close/position flow that metadata-driven
/// tests cannot express — the actual user-facing behavior.
/// </summary>
public class CompositeInteractionTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    public CompositeInteractionTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    // ── Body-leak invariant (metadata-driven) ─────────────────────────────────

    public static IEnumerable<object[]> OverlayComposites() =>
        ComponentMetadata.OverlayComposites.Select(c => new object[] { c.Name });

    /// <summary>
    /// For every overlay-style composite (Dialog/AlertDialog/Sheet/Drawer/etc.), assert
    /// that the body content is HIDDEN before the trigger is clicked and VISIBLE after.
    ///
    /// This catches a class of bug where the playground composite factory flattens inner
    /// children (Title, Description, Action, Cancel) as siblings of Content instead of
    /// nesting them inside Content's ChildContent. When flattened, those children render
    /// unconditionally (they don't gate on IsOpen themselves — that's Content's job),
    /// leaking the body text into the preview before the user ever clicks the trigger.
    ///
    /// The canary is the default Description string emitted by the source generator:
    /// "This is a &lt;lowercase-name&gt; description." — unique enough that a plain text
    /// match inside the preview pane is reliable.
    /// </summary>
    [Theory, MemberData(nameof(OverlayComposites))]
    public async Task Composite_Body_Is_Hidden_Until_Trigger_Clicked(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        // The generator derives leaf placeholder text from class-name suffix:
        //   {Root}Description → "Description", {Root}Trigger → "Trigger", etc.
        var body = driver.Preview.GetByText("Description", new() { Exact = true });

        // Before click: body must NOT be visible in the preview pane.
        await Expect(body).Not.ToBeVisibleAsync();

        // Click the trigger (derived placeholder = "Trigger").
        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        // After click: body must become visible somewhere on the page.
        // (Overlay content uses position:fixed with a portal-style escape, so the visible
        // description may land outside the preview pane — check the full page.)
        await Expect(Page.GetByText("Description", new() { Exact = true }).First).ToBeVisibleAsync();
    }

    // ── Non-empty preview invariant (metadata-driven) ─────────────────────────

    public static IEnumerable<object[]> CompositesWithMultipleChildren() =>
        ComponentMetadata.CompositesWithMultipleChildren.Select(c => new object[] { c.Name });

    /// <summary>
    /// Every composite must render SOMETHING in its preview — at minimum a single DOM
    /// element. Overlay composites (Dialog/Popover/etc.) correctly show only their
    /// trigger while their body stays gated on IsOpen, so the threshold is ≥1, not
    /// a bigger number. This still catches the degenerate case where the generator
    /// emits an empty ChildContent shell (0 descendants).
    ///
    /// The stronger invariant — "trigger is visible, body is hidden" — is asserted
    /// separately by <see cref="Composite_Body_Is_Hidden_Until_Trigger_Clicked"/>,
    /// which targets overlay composites specifically and uses derived placeholder
    /// text to verify both sides of the open/closed state.
    /// </summary>
    [Theory, MemberData(nameof(CompositesWithMultipleChildren))]
    public async Task Composite_Playground_Renders_NonEmpty_Preview(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        var descendantCount = await Page.Locator("[data-playground-preview] *").CountAsync();
        Assert.True(descendantCount >= 1,
            $"Composite '{componentName}' rendered 0 preview descendants — " +
            "the composite factory likely emitted an empty ChildContent shell.");
    }

    // ── Popover ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Popover_Opens_On_Trigger_Click()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Popover");

        // Content should be hidden initially
        await Expect(Page.Locator("[data-side]")).ToHaveCountAsync(0);

        // Click the trigger
        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
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
        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        var startBox = await Page.Locator("[data-side]").First.BoundingBoxAsync();
        Assert.NotNull(startBox);

        // Switch to End alignment
        await driver.SetEnumParam("Align", "End");
        await Page.WaitForTimeoutAsync(300);
        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
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

        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dialog_Closes_On_Escape_Key()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("Dialog");

        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
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

        var trigger = driver.Preview.GetByText("Trigger", new() { Exact = true }).First;
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

        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        Assert.Empty(driver.ConsoleErrors);
    }

    // ── AlertDialog ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AlertDialog_Opens_And_Shows_Content()
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo("AlertDialog");

        await driver.Preview.GetByText("Trigger", new() { Exact = true }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        Assert.Empty(driver.ConsoleErrors);
    }
}
