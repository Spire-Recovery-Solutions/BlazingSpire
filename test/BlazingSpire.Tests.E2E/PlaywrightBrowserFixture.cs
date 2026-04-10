using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Class-scoped Playwright fixture: one Playwright instance, one Browser, one
/// Context, and one Page per test class — not per test method. This lets the
/// browser's HTTP cache reuse the WASM bundle across tests in the same class,
/// so WASM only boots once per class instead of once per test.
///
/// Each test class declares <c>IClassFixture&lt;PlaywrightBrowserFixture&gt;</c>
/// and receives the fixture via constructor.
/// </summary>
public class PlaywrightBrowserFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
        Context = await _browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (Context is not null) await Context.DisposeAsync();
        if (_browser is not null) await _browser.DisposeAsync();
        _playwright?.Dispose();
    }
}
