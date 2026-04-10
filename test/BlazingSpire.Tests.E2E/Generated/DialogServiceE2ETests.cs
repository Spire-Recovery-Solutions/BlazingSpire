using BlazingSpire.Tests.E2E.Infrastructure;
using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// E2E tests for the IDialogService integration at /components/dialog-service.
/// Covers the MessageBox branch and the custom ShowAsync&lt;TDialog&gt; branch.
/// </summary>
public class DialogServiceE2ETests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    public DialogServiceE2ETests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    // ── MessageBox ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MessageBox_Opens_With_Configured_Title()
    {
        await NavigateAndWaitForBlazor("/components/dialog-service");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Message Box" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        var dialog = Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();
        await Expect(dialog.GetByText("Confirm Action")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task MessageBox_Confirm_Displays_Confirmed_Result()
    {
        await NavigateAndWaitForBlazor("/components/dialog-service");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Message Box" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, proceed" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(Page.GetByText("Result: Confirmed")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task MessageBox_Cancel_Displays_Cancelled_Result()
    {
        await NavigateAndWaitForBlazor("/components/dialog-service");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Message Box" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).First.ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(Page.GetByText("Result: Cancelled")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task MessageBox_Escape_Dismisses_With_Cancelled()
    {
        await NavigateAndWaitForBlazor("/components/dialog-service");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Message Box" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(Page.GetByText("Result: Cancelled")).ToBeVisibleAsync();
    }

    // ── Custom Form Dialog ────────────────────────────────────────────────────

    [Fact]
    public async Task FormDialog_Save_Returns_Typed_Name()
    {
        await NavigateAndWaitForBlazor("/components/dialog-service");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Form Dialog" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        var input = Page.GetByRole(AriaRole.Textbox).First;
        await input.FillAsync("BlazingSpire");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Expect(Page.GetByText("Saved:")).ToBeVisibleAsync();
        await Expect(Page.Locator("strong.text-foreground")).ToBeVisibleAsync();
    }
}
