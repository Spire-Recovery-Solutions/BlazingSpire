# Playwright E2E Testing Reference

## Project Setup

The E2E test project is at `test/BlazingSpire.Tests.E2E/`. It targets `net10.0` and uses xUnit with `Microsoft.Playwright` and `Deque.AxeCore.Playwright`.

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Playwright" />
  <PackageReference Include="Deque.AxeCore.Playwright" />
  <PackageReference Include="xunit" />
  <PackageReference Include="xunit.runner.visualstudio" PrivateAssets="all" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" />
</ItemGroup>
```

### Browser Installation

After building, install browsers via the generated PowerShell script:

```bash
pwsh test/BlazingSpire.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps chromium
```

`--with-deps` installs OS-level dependencies (libgbm, libnss3, etc.) needed on CI runners. Only install `chromium` to keep CI fast -- expand to `firefox` and `webkit` when cross-browser coverage matters.

### xUnit Integration

Playwright ships `Microsoft.Playwright.Xunit` with a base class hierarchy:

```
PlaywrightTest → BrowserTest → ContextTest → PageTest
```

`PageTest` provides `Page`, `Context`, `Browser`, and `Playwright` properties plus `Expect()` assertion helpers. Each test class gets a fresh `IBrowserContext` (isolated cookies, storage, service workers).

```csharp
public class DialogE2ETests : PageTest
{
    [Fact]
    public async Task Dialog_Opens_On_Trigger_Click()
    {
        await Page.GotoAsync("https://localhost:5001/components/dialog");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
    }
}
```

---

## Launching a Blazor WASM App

E2E tests need the demo app running. Two strategies:

### Strategy 1: xUnit Class Fixture (recommended)

Start `dotnet run` in a fixture, share across all tests in a collection.

```csharp
public class BlazorAppFixture : IAsyncLifetime
{
    private Process? _process;
    public string BaseUrl => "https://localhost:5001";

    public async Task InitializeAsync()
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project src/BlazingSpire.Demo/BlazingSpire.Demo.csproj --urls https://localhost:5001",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            }
        };
        _process.Start();

        // Poll until the server responds
        using var http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        });
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await http.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode) return;
            }
            catch { /* not ready yet */ }
            await Task.Delay(500);
        }
        throw new TimeoutException("Blazor app did not start within 30 seconds");
    }

    public Task DisposeAsync()
    {
        _process?.Kill(entireProcessTree: true);
        _process?.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("BlazorApp")]
public class BlazorAppCollection : ICollectionFixture<BlazorAppFixture>;
```

### Strategy 2: External Process (CI)

In CI, start the app in a prior step and pass the URL via environment variable:

```yaml
- run: dotnet run --project src/BlazingSpire.Demo -c Release --urls https://localhost:5001 &
- run: dotnet test BlazingSpire.Tests.E2E
  env:
    APP_URL: https://localhost:5001
```

---

## Waiting for WASM Boot

BlazingSpire uses the **skeleton-outside-app** pattern (see `08-performance.md`). The boot sequence in `index.html`:

1. `#skeleton` div is visible with skeleton UI
2. `#app` div has `style="display:none"`
3. `Blazor.start()` downloads and boots the WASM runtime
4. On boot complete: `#skeleton` is removed, `#app` display is set to `''`

The correct wait strategy targets `#app` becoming visible:

```csharp
public abstract class BlazingSpireE2EBase : PageTest
{
    protected string BaseUrl =>
        Environment.GetEnvironmentVariable("APP_URL") ?? "https://localhost:5001";

    protected async Task NavigateAndWaitForBlazor(string path = "/")
    {
        await Page.GotoAsync($"{BaseUrl}{path}");

        // Wait for WASM boot: #app becomes visible after Blazor.start() resolves
        await Page.Locator("#app").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000, // WASM boot can take 10-20s on slow CI
        });

        // Skeleton should be gone
        await Expect(Page.Locator("#skeleton")).ToHaveCountAsync(0);
    }
}
```

**Why 30 seconds?** WASM boot on CI runners (2-core GitHub Actions) takes 10-20 seconds. First run is cold (no HTTP cache). Subsequent navigations within the same `Page` are instant because the runtime is already loaded.

---

## Page Object Pattern

```csharp
public class DemoPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public DemoPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    // Navigation
    public Task GotoComponentAsync(string component)
        => _page.GotoAsync($"{_baseUrl}/components/{component}");

    // Common locators
    public ILocator ThemeToggle => _page.GetByRole(AriaRole.Button, new() { Name = "Toggle theme" });
    public ILocator MainHeading => _page.GetByRole(AriaRole.Heading, new() { Level = 1 });
    public ILocator NavLink(string text) => _page.GetByRole(AriaRole.Link, new() { Name = text });

    // Wait for Blazor boot
    public async Task WaitForBlazorAsync()
    {
        await _page.Locator("#app").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });
    }
}
```

---

## Component Testing Patterns

### Dialog -- Open, Focus Trap, Escape to Close, Click-Outside

```csharp
[Collection("BlazorApp")]
public class DialogE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Dialog_Traps_Focus()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" });
        await trigger.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        // First focusable element inside dialog should have focus
        var closeButton = dialog.GetByRole(AriaRole.Button, new() { Name = "Close" });
        await Expect(closeButton).ToBeFocusedAsync();

        // Tab cycles within dialog (focus trap)
        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Tab");
        // Focus should still be inside the dialog
        var focused = Page.Locator(":focus");
        await Expect(dialog.Locator(":focus")).ToHaveCountAsync(1);
    }

    [Fact]
    public async Task Dialog_Closes_On_Escape()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Dialog_Closes_On_Click_Outside()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        // Click the overlay backdrop (outside the dialog content)
        await Page.Locator("[data-dialog-overlay]").ClickAsync(new() { Position = new() { X = 5, Y = 5 } });
        await Expect(dialog).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Dialog_Restores_Focus_On_Close()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" });
        await trigger.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();

        // Focus returns to trigger
        await Expect(trigger).ToBeFocusedAsync();
    }
}
```

### Select -- Open Dropdown, Keyboard Navigation, Selection

```csharp
[Fact]
public async Task Select_Keyboard_Navigation()
{
    await NavigateAndWaitForBlazor("/components/select");

    var trigger = Page.GetByRole(AriaRole.Combobox);
    await trigger.ClickAsync();

    var listbox = Page.GetByRole(AriaRole.Listbox);
    await Expect(listbox).ToBeVisibleAsync();

    // Arrow down highlights next option
    await Page.Keyboard.PressAsync("ArrowDown");
    await Page.Keyboard.PressAsync("ArrowDown");

    // Enter selects the highlighted option
    await Page.Keyboard.PressAsync("Enter");
    await Expect(listbox).ToBeHiddenAsync();

    // Trigger text updates to reflect selection
    await Expect(trigger).Not.ToHaveTextAsync("Select an option");
}

[Fact]
public async Task Select_Type_Ahead()
{
    await NavigateAndWaitForBlazor("/components/select");

    var trigger = Page.GetByRole(AriaRole.Combobox);
    await trigger.ClickAsync();

    // Type to filter -- typing "dar" should highlight "Dark" theme option
    await Page.Keyboard.TypeAsync("dar", new() { Delay = 50 });

    var highlighted = Page.GetByRole(AriaRole.Option).Filter(new() { HasText = "Dark" });
    await Expect(highlighted).ToHaveAttributeAsync("data-highlighted", "");
}
```

### Tabs -- Tab Switching, Keyboard Navigation

```csharp
[Fact]
public async Task Tabs_Arrow_Key_Navigation()
{
    await NavigateAndWaitForBlazor("/components/tabs");

    var tablist = Page.GetByRole(AriaRole.Tablist);
    var firstTab = tablist.GetByRole(AriaRole.Tab).First;
    await firstTab.ClickAsync();
    await Expect(firstTab).ToHaveAttributeAsync("aria-selected", "true");

    // ArrowRight moves to next tab (roving tabindex)
    await Page.Keyboard.PressAsync("ArrowRight");
    var secondTab = tablist.GetByRole(AriaRole.Tab).Nth(1);
    await Expect(secondTab).ToBeFocusedAsync();
    await Expect(secondTab).ToHaveAttributeAsync("aria-selected", "true");

    // ArrowLeft wraps back
    await Page.Keyboard.PressAsync("ArrowLeft");
    await Expect(firstTab).ToBeFocusedAsync();
}
```

### Accordion -- Expand/Collapse

```csharp
[Fact]
public async Task Accordion_Expand_Collapse()
{
    await NavigateAndWaitForBlazor("/components/accordion");

    var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Is it accessible?" });
    await Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");

    await trigger.ClickAsync();
    await Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");

    // Panel content is visible
    var panel = Page.GetByRole(AriaRole.Region).Filter(new() { Has = Page.GetByText("Yes.") });
    await Expect(panel).ToBeVisibleAsync();

    // Collapse
    await trigger.ClickAsync();
    await Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
}
```

---

## Keyboard Testing

bUnit can simulate keyboard events (`KeyboardEventArgs`) and verify that `FocusAsync()` was called, but it **cannot** verify:

- Which element actually has DOM focus (`document.activeElement`)
- Tab order (real browser tabindex resolution)
- Focus trap cycling (requires real Tab key processing)
- Type-ahead search (requires real keypress timing)
- Screen reader announcements (requires real ARIA live region processing)

Playwright tests real browser behavior. The key APIs:

```csharp
// Assert which element is focused
await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Close" })).ToBeFocusedAsync();

// Press keys
await Page.Keyboard.PressAsync("Tab");
await Page.Keyboard.PressAsync("Shift+Tab");
await Page.Keyboard.PressAsync("ArrowDown");
await Page.Keyboard.PressAsync("Escape");
await Page.Keyboard.PressAsync("Enter");
await Page.Keyboard.PressAsync("Home");
await Page.Keyboard.PressAsync("End");

// Type text (for type-ahead, with realistic delay)
await Page.Keyboard.TypeAsync("search text", new() { Delay = 50 });

// Check tabindex
await Expect(Page.Locator("[role=tab]").First).ToHaveAttributeAsync("tabindex", "0");
await Expect(Page.Locator("[role=tab]").Nth(1)).ToHaveAttributeAsync("tabindex", "-1");
```

**Rule of thumb:** bUnit tests ARIA attributes and event handler logic. Playwright tests that those attributes produce correct browser behavior.

---

## Accessibility Testing with AxeCore

`Deque.AxeCore.Playwright` runs the axe-core engine inside the browser to detect WCAG violations. It catches approximately 57% of accessibility issues automatically.

```csharp
using Deque.AxeCore.Playwright;

[Collection("BlazorApp")]
public class WcagComplianceTests : BlazingSpireE2EBase
{
    [Theory]
    [InlineData("/")]
    [InlineData("/components/dialog")]
    [InlineData("/components/select")]
    [InlineData("/components/tabs")]
    [InlineData("/components/accordion")]
    public async Task Page_Has_No_Axe_Violations(string path)
    {
        await NavigateAndWaitForBlazor(path);

        var results = await Page.RunAxe(new RunOptions
        {
            RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = ["wcag2a", "wcag2aa", "wcag21aa"],
            },
        });

        Assert.Empty(results.Violations);
    }

    [Fact]
    public async Task Dialog_Open_State_Is_Accessible()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        // Open the dialog first
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Dialog" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        // Scan only the dialog, not the inert page behind it
        var results = await Page.Locator("[role=dialog]").RunAxe();
        Assert.Empty(results.Violations);
    }
}
```

### Common Axe Rules That Catch Real Bugs

| Rule | What It Catches |
|------|----------------|
| `aria-required-attr` | Missing `aria-expanded` on accordion triggers |
| `aria-valid-attr-value` | `aria-selected="True"` instead of `"true"` (Blazor renders C# bool casing) |
| `color-contrast` | OKLCH colors that don't meet 4.5:1 ratio |
| `focus-order-semantics` | Focusable elements without roles |
| `keyboard-access` | Interactive elements that can't receive focus |

### Excluding Known Issues

```csharp
var results = await Page.RunAxe(new RunOptions
{
    Rules = new Dictionary<string, RuleOptions>
    {
        // Prism.js injects elements that fail color contrast -- not our code
        ["color-contrast"] = new() { Enabled = true },
    },
    Exclude = [["pre code"]], // Exclude syntax highlighting blocks
});
```

---

## Visual Regression

Playwright screenshots enable pixel-level comparison across changes.

```csharp
[Collection("BlazorApp")]
public class ScreenshotTests : BlazingSpireE2EBase
{
    [Theory]
    [InlineData("dialog", "Open Dialog")]
    [InlineData("select", "Select an option")]
    public async Task Component_Matches_Snapshot(string component, string triggerText)
    {
        await NavigateAndWaitForBlazor($"/components/{component}");

        // Open the component to its interactive state
        await Page.GetByRole(AriaRole.Button, new() { Name = triggerText }).ClickAsync();

        // Wait for animations to settle
        await Page.WaitForTimeoutAsync(300);

        await Expect(Page).ToHaveScreenshotAsync(new()
        {
            FullPage = false,
            MaxDiffPixelRatio = 0.01f, // 1% tolerance for anti-aliasing
        });
    }

    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    public async Task Theme_Snapshot(string theme)
    {
        await NavigateAndWaitForBlazor("/");

        if (theme == "dark")
        {
            await Page.EvaluateAsync("document.documentElement.classList.add('dark')");
        }
        else
        {
            await Page.EvaluateAsync("document.documentElement.classList.remove('dark')");
        }

        await Expect(Page).ToHaveScreenshotAsync($"home-{theme}.png");
    }
}
```

### Platform-Specific Snapshots

Font rendering differs across OS. Playwright stores snapshots per platform automatically when using `ToHaveScreenshotAsync`. The snapshot directory structure:

```
test/BlazingSpire.Tests.E2E/
  ScreenshotTests.Component_Matches_Snapshot_dialog/
    Component-Matches-Snapshot-dialog-1-chromium-linux.png
    Component-Matches-Snapshot-dialog-1-chromium-darwin.png
```

**CI rule:** Generate baseline snapshots on the CI runner OS (Linux). Never commit macOS-generated snapshots as the baseline -- font rendering differences will cause false failures.

```bash
# Update snapshots on CI
dotnet test BlazingSpire.Tests.E2E -- Playwright.UpdateSnapshots=true
```

---

## Performance Measurement

Playwright can measure timings via the browser's Performance API. The targets from `13-performance-targets.md`:

| Component | Target | Fail Threshold |
|-----------|--------|----------------|
| Select 200 items: open-to-rendered | < 30 ms | > 60 ms |
| Dialog time-to-interactive | < 50 ms | > 100 ms |
| Accordion expand single | < 16 ms | > 32 ms |

### Measuring Component Render Timing

```csharp
[Fact]
public async Task Dialog_Opens_Within_Budget()
{
    await NavigateAndWaitForBlazor("/components/dialog");

    // Inject performance marks around the interaction
    var timing = await Page.EvaluateAsync<double>("""
        await new Promise(resolve => {
            const trigger = document.querySelector('[data-dialog-trigger]');
            performance.mark('dialog-open-start');
            trigger.click();

            // Wait for the dialog to be visible and focusable
            const observer = new MutationObserver(() => {
                const dialog = document.querySelector('[role="dialog"]');
                if (dialog && dialog.offsetParent !== null) {
                    performance.mark('dialog-open-end');
                    performance.measure('dialog-open', 'dialog-open-start', 'dialog-open-end');
                    const measure = performance.getEntriesByName('dialog-open')[0];
                    observer.disconnect();
                    resolve(measure.duration);
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        });
    """);

    Assert.True(timing < 100, $"Dialog time-to-interactive was {timing:F1}ms (threshold: 100ms)");
}
```

### Measuring LCP

```csharp
[Fact]
public async Task LCP_Under_Target()
{
    var page = await Context.NewPageAsync();

    var lcp = await page.EvaluateAsync<double>($$"""
        await new Promise(resolve => {
            new PerformanceObserver(list => {
                const entries = list.getEntries();
                const last = entries[entries.length - 1];
                resolve(last.startTime);
            }).observe({ type: 'largest-contentful-paint', buffered: true });

            // Navigate after observer is set up
            window.location.href = '{{BaseUrl}}';
        });
    """);

    // With skeleton-outside-app, LCP should be the skeleton (~0.5s)
    Assert.True(lcp < 1500, $"LCP was {lcp:F0}ms (target: < 1500ms)");
}
```

---

## CI Integration

### GitHub Actions Workflow

```yaml
e2e-tests:
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'

    - name: Build E2E tests
      run: dotnet build test/BlazingSpire.Tests.E2E

    - name: Install Playwright browsers
      run: pwsh test/BlazingSpire.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps chromium

    - name: Start demo app
      run: dotnet run --project src/BlazingSpire.Demo -c Release --urls https://localhost:5001 &

    - name: Wait for app ready
      run: |
        for i in $(seq 1 30); do
          curl -ks https://localhost:5001 && exit 0
          sleep 1
        done
        exit 1

    - name: Run E2E tests
      run: dotnet test test/BlazingSpire.Tests.E2E --logger "trx;LogFileName=e2e-results.trx"
      env:
        APP_URL: https://localhost:5001

    - name: Upload test results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: e2e-results
        path: |
          test/BlazingSpire.Tests.E2E/TestResults/
          test/BlazingSpire.Tests.E2E/bin/Debug/net10.0/playwright-traces/
```

### Trace Collection on Failure

Configure Playwright to capture traces on failure for debugging:

```csharp
public override BrowserNewContextOptions ContextOptions()
{
    return new()
    {
        Locale = "en-US",
        ColorScheme = ColorScheme.Light,
        RecordVideoDir = TestOk ? null : "videos/",
    };
}

public override async Task InitializeAsync()
{
    await base.InitializeAsync();
    await Context.Tracing.StartAsync(new()
    {
        Screenshots = true,
        Snapshots = true,
        Sources = true,
    });
}

public override async Task DisposeAsync()
{
    if (!TestOk)
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = $"playwright-traces/{GetType().Name}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip",
        });
    }
    await base.DisposeAsync();
}
```

View traces locally: `pwsh playwright.ps1 show-trace trace.zip`

---

## Anti-Patterns

### Blazor WASM-Specific Flakiness

| Trap | Why It's Flaky | Fix |
|------|----------------|-----|
| **Short timeouts on first navigation** | WASM boot downloads ~1.4 MB brotli; CI runners are slow | Use 30s timeout for first `WaitForAsync`, 5s for subsequent |
| **`WaitForLoadStateAsync("networkidle")`** | Blazor keeps a WebSocket (Server) or does periodic fetches; never truly idle | Wait for a specific DOM state (`#app` visible) instead |
| **Asserting immediately after click** | Blazor renders async; DOM update may not be synchronous | Use `Expect().ToBeVisibleAsync()` which auto-retries |
| **Hardcoded `WaitForTimeoutAsync`** | Hides real timing bugs, fails on slow CI | Use `WaitForSelectorAsync` or Playwright's built-in auto-waiting |
| **Testing during WASM boot** | Components render with default state then re-render with data | Always `WaitForBlazor()` before interacting |
| **Parallel tests sharing state** | Blazor WASM is single-threaded; multiple tabs compete for runtime | Each test gets its own `BrowserContext`, but share one app instance |
| **Checking `innerText` of Blazor loading screen** | The `#blazor-error-ui` div is always in DOM (hidden) | Check `display` style or visibility, not existence |

### General Playwright Anti-Patterns

- **`Page.QuerySelectorAsync` instead of `Page.Locator`** -- `QuerySelector` returns a snapshot; `Locator` auto-retries and is less flaky
- **CSS selectors for component state** -- prefer `GetByRole`, `GetByText`, `GetByTestId`
- **Screenshot tests without animation settling** -- add `WaitForTimeoutAsync(300)` or disable CSS transitions in test context
- **Not cleaning up `Process` in fixture** -- leaked `dotnet run` processes accumulate on CI; always `Kill(entireProcessTree: true)`
