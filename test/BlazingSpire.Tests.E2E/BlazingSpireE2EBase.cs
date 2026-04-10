using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Base class for all BlazingSpire E2E tests.
/// The Playwright browser + context + page come from a class-scoped
/// <see cref="PlaywrightBrowserFixture"/> so WASM only needs to boot once per class
/// instead of once per test method. Each test class must declare
/// <c>IClassFixture&lt;PlaywrightBrowserFixture&gt;</c> and pass the fixture through
/// its constructor.
/// </summary>
public abstract class BlazingSpireE2EBase
{
    protected IBrowserContext Context { get; }
    protected IPage Page { get; }

    protected string BaseUrl =>
        Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:5299";

    protected BlazingSpireE2EBase(PlaywrightBrowserFixture browserFixture)
    {
        Context = browserFixture.Context;
        Page = browserFixture.Page;
    }

    /// <summary>
    /// Navigates to the given path and waits for the Blazor WASM runtime to boot.
    /// Uses the skeleton-outside-app pattern: #app becomes visible when Blazor.start() resolves.
    /// 90s timeout — WASM boot on cold runners (or 10 parallel browsers contending for the
    /// dev server) can take 60+ seconds.
    /// </summary>
    protected async Task NavigateAndWaitForBlazor(string path = "/")
    {
        await Page.GotoAsync($"{BaseUrl}{path}");

        await Page.Locator("#app").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 90_000,
        });

        // Skeleton div is removed after boot completes
        await Assertions.Expect(Page.Locator("#skeleton")).ToHaveCountAsync(0);
    }

    protected ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
    protected IPageAssertions Expect(IPage page) => Assertions.Expect(page);
}
