using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// E2E tests for the BlazingSpire demo home page (/).
/// Covers page load, hero section, stats, preview, and category cards.
/// Requires the demo app running at APP_URL (default: https://localhost:5001).
/// </summary>
public class DemoPageTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    public DemoPageTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

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
    public async Task Hero_BrowseComponents_Link_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Browse Components" }))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task Hero_GitHub_Link_Is_Visible()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.Locator("a[href*='github.com'][target='_blank']").First)
            .ToBeVisibleAsync();
    }

    // ── Stats row ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Components")]
    [InlineData("Tests")]
    public async Task Stats_Card_Is_Visible(string statLabel)
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByText(statLabel).First).ToBeVisibleAsync();
    }

    // ── Section headings ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("See it in action")]
    [InlineData("Browse by category")]
    public async Task Section_Heading_Is_Visible(string sectionName)
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = sectionName }))
            .ToBeVisibleAsync();
    }

    // ── Preview form ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Preview_Card_Shows_Create_Account_Form()
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByText("Create account")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Enter your details to get started.")).ToBeVisibleAsync();
    }

    // ── Category cards ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Layout")]
    [InlineData("Forms")]
    [InlineData("Data Display")]
    [InlineData("Feedback")]
    [InlineData("Navigation")]
    public async Task Category_Card_Is_Visible(string categoryName)
    {
        await NavigateAndWaitForBlazor();
        await Expect(Page.GetByText(categoryName).First).ToBeVisibleAsync();
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
