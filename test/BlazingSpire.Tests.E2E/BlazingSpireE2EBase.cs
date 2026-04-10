using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Base class for all BlazingSpire E2E tests.
/// Manages Playwright browser/context/page lifecycle via xUnit IAsyncLifetime.
/// Each test class gets a fresh BrowserContext (isolated cookies, storage, etc.).
/// </summary>
public abstract class BlazingSpireE2EBase : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected string BaseUrl =>
        Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:5299";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
        Context = await _browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _browser!.DisposeAsync();
        _playwright!.Dispose();
    }

    /// <summary>
    /// Navigates to the given path and waits for the Blazor WASM runtime to boot.
    /// Uses the skeleton-outside-app pattern: #app becomes visible when Blazor.start() resolves.
    /// 30s timeout — WASM boot on cold CI runners can take 10-20 seconds.
    /// </summary>
    protected async Task NavigateAndWaitForBlazor(string path = "/")
    {
        await Page.GotoAsync($"{BaseUrl}{path}");

        // #app is display:none until Blazor.start() resolves
        await Page.Locator("#app").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // Skeleton div is removed after boot completes
        await Assertions.Expect(Page.Locator("#skeleton")).ToHaveCountAsync(0);
    }

    protected ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
    protected IPageAssertions Expect(IPage page) => Assertions.Expect(page);
}
