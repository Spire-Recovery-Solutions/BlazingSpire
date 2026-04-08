using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// E2E tests for the BlazingSpire demo home page (/).
/// Covers page load, hero section, and all component showcase sections.
/// Requires the demo app running at APP_URL (default: https://localhost:5001).
/// </summary>
public class DemoPageTests : BlazingSpireE2EBase
{
    // ── Boot / Page structure ─────────────────────────────────────────────────

    [Fact]
    public async Task Home_Page_Has_Correct_Title()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page).ToHaveTitleAsync("BlazingSpire");
    }

    [Fact]
    public async Task Hero_Heading_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 1, Name = "BlazingSpire" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task Hero_Description_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByText("A Blazor component framework inspired by shadcn/ui"))
            .ToBeVisibleAsync();
    }

    // ── Hero buttons ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Hero_GetStarted_Button_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Get Started" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task Hero_GitHub_Button_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" }))
            .ToBeVisibleAsync();
    }

    // ── Section headings ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("Button")]
    [InlineData("Badge")]
    [InlineData("Card")]
    public async Task Section_Heading_Is_Visible(string sectionName)
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = sectionName }))
            .ToBeVisibleAsync();
    }

    // ── Button section ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Default")]
    [InlineData("Secondary")]
    [InlineData("Destructive")]
    [InlineData("Outline")]
    [InlineData("Ghost")]
    [InlineData("Link")]
    public async Task Button_Variant_Is_Visible(string variantLabel)
    {
        await NavigateAndWaitForBlazor();

        // Scope to Button section so "Outline" / "Default" etc. don't match Badge section
        var buttonSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Button" }) });

        await Expect(buttonSection.GetByRole(AriaRole.Button, new() { Name = variantLabel }).First)
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task Button_Disabled_State_Is_Rendered()
    {
        await NavigateAndWaitForBlazor();

        var buttonSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Button" }) });

        await Expect(buttonSection.Locator("button[disabled]").First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Button_Loading_Spinner_Is_Rendered()
    {
        await NavigateAndWaitForBlazor();

        var buttonSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Button" }) });

        // Loading buttons render an SVG spinner with animate-spin class
        await Expect(buttonSection.Locator("svg.animate-spin").First).ToBeVisibleAsync();
    }

    // ── Badge section ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Default")]
    [InlineData("Secondary")]
    [InlineData("Destructive")]
    [InlineData("Outline")]
    public async Task Badge_Variant_Is_Visible(string variantLabel)
    {
        await NavigateAndWaitForBlazor();

        var badgeSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Badge" }) });

        await Expect(badgeSection.GetByText(variantLabel).First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Badge_Tag_List_Is_Rendered()
    {
        await NavigateAndWaitForBlazor();

        var badgeSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Badge" }) });

        // Tag list badges from the demo
        await Expect(badgeSection.GetByText("Blazor")).ToBeVisibleAsync();
        await Expect(badgeSection.GetByText(".NET 10")).ToBeVisibleAsync();
        await Expect(badgeSection.GetByText("Tailwind v4")).ToBeVisibleAsync();
    }

    // ── Card section ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Create project")]
    [InlineData("Components")]
    [InlineData("Documentation")]
    public async Task Card_Title_Is_Visible(string cardTitle)
    {
        await NavigateAndWaitForBlazor();

        var cardSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Card" }) });

        await Expect(cardSection.GetByText(cardTitle).First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Card_Stats_Are_Rendered()
    {
        await NavigateAndWaitForBlazor();

        var cardSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Card" }) });

        await Expect(cardSection.GetByText("Total Downloads")).ToBeVisibleAsync();
        await Expect(cardSection.GetByText("Active Users")).ToBeVisibleAsync();
        await Expect(cardSection.GetByText("Lighthouse Score")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Card_Notifications_Section_Is_Rendered()
    {
        await NavigateAndWaitForBlazor();

        var cardSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Card" }) });

        await Expect(cardSection.GetByText("You have 3 unread messages.")).ToBeVisibleAsync();
        await Expect(cardSection.GetByRole(AriaRole.Button, new() { Name = "Mark all as read" }))
            .ToBeVisibleAsync();
    }

    // ── Dark mode ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Dark_Mode_Can_Be_Applied_Via_JavaScript()
    {
        await NavigateAndWaitForBlazor();
        await Page.EvaluateAsync("document.documentElement.classList.add('dark')");
        await Expect(Page.Locator("html.dark")).ToHaveCountAsync(1);
    }

    [Fact]
    public async Task Light_Mode_Is_Default_Without_LocalStorage()
    {
        await NavigateAndWaitForBlazor();

        // Remove any stored theme preference, then reload to test the default init path
        await Page.EvaluateAsync("localStorage.removeItem('theme')");
        await Page.ReloadAsync();
        await Page.Locator("#app").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        var hasDark = await Page.EvaluateAsync<bool>(
            "document.documentElement.classList.contains('dark')");

        Assert.False(hasDark);
    }
}
