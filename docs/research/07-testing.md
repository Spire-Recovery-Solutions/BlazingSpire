# Testing Strategy

## Test Pyramid

```
┌───────────────────────┐
│   E2E (Playwright)     │  ~50 tests — real browser, real JS interop
│   Visual Regression    │  Playwright screenshots per component/state
│   Accessibility        │  Deque.AxeCore.Playwright (WCAG 2.1 AA)
├───────────────────────┤
│   Unit (bUnit)         │  ~500+ tests — rendering, ARIA, events, state
│   Keyboard Navigation  │  bUnit event simulation + JS mock verification
│   Snapshot Tests       │  Markup comparison with semantic diffing
├───────────────────────┤
│   Performance          │  Benchmark.Blazor (render cycles, allocations)
│   Trim Compatibility   │  Publish with trimming, run integration tests
└───────────────────────┘
```

## Tools

| Tool | Purpose |
|------|---------|
| **bUnit** 2.7.x | Component unit tests — rendering, events, ARIA assertions, JS interop mocking |
| **Playwright .NET** 1.58+ | E2E tests, visual regression screenshots, real JS interop |
| **Deque.AxeCore.Playwright** 4.11+ | Automated WCAG compliance scanning (~57% of a11y issues) |
| **Benchmark.Blazor** | Component render performance benchmarking |
| **BlazingStory** | Storybook equivalent — component isolation and documentation |
| **Coverlet** + **ReportGenerator** | Code coverage |

## bUnit Capabilities

- Simulate keyboard events with `Key.ArrowDown`, `Key.Enter`, `Key.Escape`, modifiers
- Assert ARIA attributes via AngleSharp DOM queries
- Mock JS interop modules (collocated `.razor.js`)
- Test across render modes via `SetAssignedRenderMode()` and `SetRendererInfo()`
- Semantic HTML comparison (ignores whitespace, attribute order)
- Verify `FocusAsync()` calls with `VerifyFocusAsyncInvoke()`
- **Cannot** test actual DOM focus state (`document.activeElement`) — use Playwright

## Blazor-Specific Flakiness Sources

| Trap | Why It's Flaky | Fix |
|------|----------------|-----|
| **JS interop strict mode** | bUnit defaults to strict — unmocked JS call throws | `JSInterop.Mode = JSRuntimeMode.Loose` in test base class |
| **Missing `cut.InvokeAsync()`** | State changes need Blazor's sync context | Always use `cut.InvokeAsync(() => ...)` for programmatic state changes |
| **Render timing** | `OnAfterRenderAsync` doesn't fire synchronously | Use `cut.WaitForState(() => condition)` with timeout |
| **Shared static state** | Static dictionaries, singletons, cached CSS | Each test gets its own `BunitContext` |
| **Markup comparison fragility** | Exact HTML including whitespace/attr order | Use `MarkupMatches()` + `diff:ignore` for dynamic attrs |

## Test Base Class

```csharp
public abstract class BlazingSpireTestBase : BunitContext
{
    protected override void OnInitialized()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/BlazingSpire.Primitives/js/focus.js");
        JSInterop.SetupModule("./_content/BlazingSpire.Primitives/js/positioning.js");
        Services.AddBlazingSpireServices();
    }

    protected static void AssertAriaExpanded(IElement el, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), el.GetAttribute("aria-expanded"));

    protected static void AssertRole(IElement el, string role)
        => Assert.Equal(role, el.GetAttribute("role"));
}
```

## Anti-Patterns to Avoid

- **No assertions** — a test that renders without asserting proves nothing
- **Duplicate test methods per variant** — use `[Theory]`/`[InlineData]` for data-driven tests
- **Mocking types you own** — only mock external boundaries (JS interop, HTTP)
- **`Thread.Sleep` for synchronization** — use `cut.WaitForState()` or Playwright's auto-waiting
- **Testing implementation details** — assert rendered output and ARIA, not internal state

## Test Organization

```
src/
  BlazingSpire.Primitives/
test/
  BlazingSpire.Primitives.Tests/          # bUnit
    Components/
      DialogPrimitiveTests.cs
      FocusTrapTests.cs
      AccordionTests.cs
    Shared/
      BlazingSpireTestBase.cs
      AccessibilityAssertions.cs
  BlazingSpire.Tests.E2E/                 # Playwright
    Accessibility/WcagComplianceTests.cs
    Visual/ScreenshotTests.cs
    Components/DialogE2ETests.cs
  BlazingSpire.Tests.Performance/         # BenchmarkDotNet
    Rendering/ButtonBenchmarks.cs
```

## CI Pipeline

```yaml
jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - run: dotnet test BlazingSpire.Primitives.Tests --collect:"XPlat Code Coverage"
  e2e-tests:
    runs-on: ubuntu-latest
    steps:
      - run: pwsh bin/Debug/net10.0/playwright.ps1 install --with-deps chromium
      - run: dotnet test BlazingSpire.Tests.E2E
  performance:
    runs-on: ubuntu-latest
    steps:
      - run: dotnet run -c Release --project BlazingSpire.Tests.Performance
```
