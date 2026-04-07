# Performance Targets & Benchmarking Strategy

## 1. Concrete Performance Targets

### 1.1 Micro Benchmarks

| Benchmark | Target (p50) | Target (p99) | Fail Threshold | Notes |
|-----------|-------------|-------------|----------------|-------|
| `SetParametersAsync` (source-gen, 8 params) | < 150 ns | < 300 ns | > 500 ns | Compared to reflection baseline ~900 ns |
| `Cn()` cached hit | < 50 ns | < 100 ns | > 200 ns | LRU cache lookup only |
| `Cn()` uncached (3 inputs) | < 2 us | < 5 us | > 10 us | Full TailwindMerge parse + merge |
| `Cn()` uncached (8 inputs, long strings) | < 5 us | < 12 us | > 20 us | Worst-case variant composition |
| Variant `FrozenDictionary` lookup | < 15 ns | < 30 ns | > 50 ns | Single enum key lookup |
| `EventCallback.InvokeAsync` dispatch | < 100 ns | < 250 ns | > 500 ns | Delegate invocation, no async work |
| `EventCallback` with `IHandleEvent` | < 120 ns | < 300 ns | > 600 ns | Suppresses auto `StateHasChanged` |

### 1.2 Component Benchmarks

| Benchmark | Target | Fail Threshold | Measurement Method |
|-----------|--------|----------------|-------------------|
| Select with 200 items: open-to-rendered | < 30 ms | > 60 ms | Playwright `performance.mark` around open trigger to last item painted |
| Dialog time-to-interactive | < 50 ms | > 100 ms | Playwright: click trigger to focus-trap active + content painted |
| DataTable 500 rows initial render | < 120 ms | > 250 ms | Playwright: component mount to last row visible |
| DataTable 500 rows re-render (sort change) | < 80 ms | > 180 ms | Playwright: click sort header to render complete |
| Accordion 50 items expand/collapse single | < 16 ms | > 32 ms | Must complete within one animation frame budget |
| Accordion 50 items expand all | < 100 ms | > 200 ms | Playwright: programmatic expand-all to all panels visible |

### 1.3 JS Interop Budgets

| Constraint | Target | Rationale |
|-----------|--------|-----------|
| Max JS interop round-trips per component `OnAfterRenderAsync(firstRender)` | <= 2 | Each round-trip is ~20-100 ms on Server mode |
| Max JS interop round-trips per component lifecycle (mount to stable) | <= 3 | Budget: 1 module import + 1 setup call + 1 optional positioning |
| JS module load time (cold, per module) | < 15 ms | Cached by browser after first load |
| JS module load time (warm, from cache) | < 2 ms | Browser module cache hit |
| Total JS interop time per Dialog first-render (Server) | < 80 ms | Sum of module load + focus trap + scroll lock + positioning |
| Batched interop call overhead vs N individual calls | >= 60% reduction | Validates batching is worth the complexity |

### 1.4 Memory Budgets

| Metric | Target | Fail Threshold | Notes |
|--------|--------|----------------|-------|
| Per-connection memory (Server mode, idle page with 20 components) | < 120 KB | > 200 KB | Measured via `dotnet-counters` after GC |
| Per-connection memory (Server mode, DataTable 500 rows) | < 350 KB | > 600 KB | Includes circuit state + component tree |
| Component instance size: Button | < 256 bytes | > 512 bytes | Measured via `ObjectLayoutInspector` |
| Component instance size: Select (200 items) | < 48 KB | > 80 KB | Includes item state array |
| Component instance size: Dialog (mounted, with focus trap) | < 2 KB | > 4 KB | Excludes child content |
| `DotNetObjectReference` leak detection | 0 leaks per test run | > 0 | Assert dispose count == create count |

### 1.5 Bundle Size Budgets

| Asset | Target | Fail Threshold | Measurement |
|-------|--------|----------------|-------------|
| `BlazingSpire.Primitives.dll` (trimmed, IL stripped) | < 80 KB | > 120 KB | `dotnet publish -c Release` with trimming |
| `BlazingSpire.Primitives.dll` (AOT, WASM stripped) | < 150 KB | > 250 KB | With `WasmStripILAfterAOT` |
| Total WASM payload above Blazor baseline | < 350 KB | > 500 KB | Delta between empty Blazor WASM app and app with all primitives |
| `blazingspire-interop.js` (minified + gzipped) | < 4 KB | > 8 KB | ~300 lines source + Floating UI wrapper |
| Floating UI bundle (ESM, minified) | < 3.5 KB | > 5 KB | Vendor dependency, verify on upgrade |
| Source generator output (typical project, 15 components) | < 15 KB IL | > 30 KB IL | Measured from generated `.cs` files compiled |

### 1.6 CSS Budget

| Metric | Target | Fail Threshold | Notes |
|--------|--------|----------------|-------|
| Tailwind output (minified, gzipped) for full demo site | < 12 KB | > 25 KB | Content-scanned, only used utilities |
| Tailwind output (minified, ungzipped) for full demo site | < 45 KB | > 80 KB | Compare to MudBlazor ~500 KB, Bootstrap ~228 KB |
| Per-component CSS contribution (average) | < 800 bytes ungzipped | > 2 KB | Measured by adding/removing a single component |

---

## 2. WASM Payload Reality Check

### Estimated Assembly Sizes

| Component | Estimated IL Size | Estimated AOT Size | Notes |
|-----------|------------------|--------------------|-------|
| `BlazingSpire.Primitives.dll` | ~60-80 KB | ~120-150 KB | Headless primitives: focus trap, keyboard nav, ARIA, portals, scroll lock, positioning service |
| `TailwindMerge.NET` | ~45-60 KB | ~90-130 KB | Parser, conflict resolver, LRU cache, Tailwind v4 class definitions |
| Source generator output (15 styled components) | ~10-15 KB | ~20-35 KB | `SetParametersAsync` switches, `FrozenDictionary` variant maps, DI registration |
| `System.Collections.Immutable` (for `FrozenDictionary`) | ~0 KB delta | ~0 KB delta | Already in Blazor WASM baseline |
| `Floating UI` (JS, not .NET) | 0 KB .NET | 0 KB .NET | JS-only: ~3 KB minified, loaded as ES module |

### Realistic Total

```
Scenario: WASM app using all BlazingSpire primitives + 15 styled components

Blazor WASM baseline (.NET 10):           ~2.1 MB (trimmed, gzipped ~800 KB)
+ BlazingSpire.Primitives.dll (trimmed):  +60-80 KB
+ TailwindMerge.NET (trimmed):            +45-60 KB
+ Source-gen output:                      +10-15 KB
+ JS interop bundle:                      +4 KB (gzipped)
+ Floating UI ESM:                        +3 KB
─────────────────────────────────────────
Delta above baseline:                     ~122-162 KB IL
                                          ~45-60 KB gzipped transfer

With AOT + WasmStripILAfterAOT:
+ AOT compiled .wasm growth:              ~230-315 KB
- IL stripping savings:                   -115-155 KB
─────────────────────────────────────────
Net AOT delta above baseline:             ~115-160 KB additional .wasm
                                          ~50-70 KB gzipped transfer
```

**Verdict:** The total WASM payload increase stays well under the 350 KB target (ungzipped IL). The gzipped transfer cost is ~50-70 KB -- comparable to adding a single medium npm package. TailwindMerge.NET is the largest .NET dependency and the one most worth monitoring. If it grows beyond 80 KB trimmed, consider a lighter fork that only handles the Tailwind v4 class subset BlazingSpire actually uses.

### Key Risk: TailwindMerge.NET Trim Behavior

TailwindMerge.NET internally uses dictionaries of class group definitions. If these are populated via reflection or dynamic patterns, trimming may either break functionality or fail to remove dead class groups. **Action item:** Run `dotnet publish` with `TrimmerSingleWarn=false` and verify zero trim warnings from TailwindMerge.NET. If warnings appear, file upstream or fork.

---

## 3. Server-Mode Scaling Model

### Connections per GB RAM

```
Base overhead per Blazor Server circuit:
  - SignalR connection:          ~5 KB
  - Renderer + component tree:  ~15-30 KB (depends on tree depth)
  - Circuit state:               ~3-5 KB
  - DI scope:                    ~2-4 KB
  ────────────────────────────
  Subtotal (empty page):         ~25-44 KB

BlazingSpire page with 20 interactive components:
  - Component instances:         ~15-25 KB (primitives are lightweight)
  - DotNetObjectReference handles: ~1-2 KB (max 5-6 active refs)
  - TailwindMerge LRU cache:    ~50-100 KB (shared per-connection via scoped DI)
  - JS module references:        ~1 KB
  ────────────────────────────
  Subtotal per connection:       ~92-171 KB

Conservative estimate (170 KB/connection):
  1 GB usable = ~6,000 connections
  4 GB server  = ~24,000 connections (with ~1 GB for runtime/GC)

Aggressive estimate (100 KB/connection):
  1 GB usable = ~10,000 connections
  4 GB server  = ~40,000 connections
```

**Critical insight:** The TailwindMerge LRU cache is the wildcard. If registered as singleton (shared across connections), the per-connection cost drops significantly. If scoped, each connection carries its own cache. **Recommendation:** Register `TwMerge` as singleton with a thread-safe LRU cache sized to 500 entries. This is safe because merged class strings are deterministic and immutable.

### Interop Calls Per Page Load

| Page Scenario | Interop Calls | SignalR Messages | Notes |
|--------------|---------------|-----------------|-------|
| Static page, no interactive components | 0 | 0 | Pure SSR |
| Simple form (Button, Input, Select) | 2-3 | 4-6 | Module load + Select positioning |
| Complex page (Dialog + DataTable + Select) | 5-7 | 10-14 | Each component: 1 module import + 1 setup |
| DataTable 500 rows with sort + filter | 3-4 | 6-8 | Virtualize handles rendering; few interop calls |
| Worst case: Dialog containing DataTable + nested Select | 7-9 | 14-18 | Stacked overlays, each needs positioning |

### SignalR Message Budget

| Constraint | Budget | Rationale |
|-----------|--------|-----------|
| Messages per page load | < 20 | Keeps initial load snappy on high-latency connections |
| Messages per user interaction (click/key) | < 4 | 1 event + 1 interop call + 1 render diff + 1 ack |
| Render diff payload per interaction | < 2 KB | Blazor's diff protocol is already efficient |
| Maximum sustained messages/sec per connection | < 30 | Prevents server saturation under load |
| Idle connection heartbeat | Default (30s) | Do not increase; reduces server-side timer pressure |

---

## 4. TailwindMerge.NET Deep Dive

### Expected Cost Per Call

```
Operation breakdown for TwMerge.Merge("px-4 py-2 bg-blue-500", "bg-red-500"):

1. Input normalization (join + trim):     ~200 ns
2. Cache key computation (string hash):   ~50 ns
3. LRU cache lookup:                      ~30 ns (hit) / N/A (miss)
4. Class parsing (split + classify):      ~800 ns (miss only)
5. Conflict detection + resolution:       ~400 ns (miss only)
6. Result string construction:            ~300 ns (miss only)
─────────────────────────────────────────
Total (cache hit):                        ~280 ns
Total (cache miss, simple):               ~1,800 ns (~1.8 us)
Total (cache miss, complex 8+ classes):   ~4,000-8,000 ns (~4-8 us)
```

### LRU Cache Behavior

TailwindMerge.NET uses an internal `ConcurrentDictionary`-backed LRU cache. Key characteristics:

- **Default capacity:** 500 entries (configurable)
- **Key:** The raw input string (concatenated arguments)
- **Eviction:** Approximate LRU via timestamp tracking
- **Thread safety:** `ConcurrentDictionary` with lock-free reads

### Hit Rate Analysis: DataTable Scenario

```
DataTable with 500 rows, 8 columns = 4,000 cells

Per cell, Cn() is called to merge:
  - Base cell class: "px-4 py-2 text-sm"
  - Column-specific class: varies by column (8 unique values)
  - Row state class: "bg-muted/50" for alternating rows (2 unique values)

Unique Cn() inputs:
  - 8 columns x 2 row states = 16 unique combinations
  - Header cells: 8 unique combinations
  - Total unique: ~24 entries

Cache behavior on first render:
  - First 24 calls: cache miss (~1.8 us each = ~43 us total)
  - Remaining 3,976 calls: cache hit (~280 ns each = ~1.1 ms total)
  - Total Cn() time: ~1.15 ms

Cache behavior on re-render (sort):
  - All 4,000 calls: cache hit
  - Total Cn() time: ~1.12 ms

Hit rate after first render: 99.4%
Hit rate on subsequent renders: 100%
```

### Hit Rate Analysis: Select with 200 Items

```
Select open with 200 items:

Per item, Cn() merges:
  - Base item class: "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm"
  - Highlighted state: "bg-accent text-accent-foreground" (1 item at a time)
  - Disabled state: "pointer-events-none opacity-50" (assume 5 disabled)

Unique Cn() inputs:
  - Normal item: 1 unique
  - Highlighted item: 1 unique
  - Disabled item: 1 unique
  - Total unique: 3 entries

Cache hit rate: 98.5% on first render, 100% thereafter
Total Cn() time on open: ~0.06 ms (3 misses + 197 hits)
```

### Caching Strategy Recommendations

1. **Register `TwMerge` as singleton.** Class merge results are deterministic -- same inputs always produce same output. No per-user state. This eliminates the largest per-connection memory cost.

2. **Size the LRU cache to 500 entries.** Analysis of a full component library (Button, Select, Dialog, DataTable, Accordion, Tabs, Input, Checkbox, etc.) with all variant combinations yields ~200-300 unique `Cn()` input strings. 500 provides headroom for user customization.

3. **Pre-warm the cache on app startup.** In `Program.cs`, call `Cn()` for the most common variant combinations. This moves cache-miss latency from first user request to application boot.

4. **Cache at the component level too.** Store the result of `Cn()` in a field, recomputed only in `OnParametersSet` when the relevant parameters actually change:

```csharp
private string _rootClass = "";
private ButtonVariant _lastVariant;
private ButtonSize _lastSize;

protected override void OnParametersSet()
{
    if (Variant != _lastVariant || Size != _lastSize)
    {
        _rootClass = Cn(BaseClass, VariantClasses[Variant], SizeClasses[Size], Class);
        _lastVariant = Variant;
        _lastSize = Size;
    }
}
```

5. **Avoid dynamic string interpolation in `Cn()` inputs.** Bad: `Cn($"mt-{spacing}")`. Good: use a lookup dictionary `SpacingClasses[spacing]`. Dynamic strings defeat the cache because every unique spacing value creates a new cache entry.

---

## 5. CascadingValue Re-render Analysis

### The Problem

`CascadingValue` notifies all subscribers when its value changes, triggering `SetParametersAsync` on every subscribing descendant. For compound components (Select, Accordion) where a parent state change (e.g., "which item is selected") must propagate to children, this can cause O(N) re-renders where only O(1) is needed.

### Model: Select with 100 Items

**Approach A: CascadingValue (naive)**

```
SelectRoot cascades: { SelectedValue, IsOpen, HighlightedIndex }

On highlight change (keyboard navigation):
  - CascadingValue updates → all 100 SelectItem children receive SetParametersAsync
  - Each SelectItem runs ShouldRender():
    - 1 item: highlighted state changed → renders (produces diff)
    - 1 item: previously highlighted → renders (produces diff)
    - 98 items: no visual change → ShouldRender returns false (but SetParametersAsync still ran)
  - Total SetParametersAsync calls: 100
  - Total renders: 2
  - Total SetParametersAsync cost: 100 x ~150 ns = ~15 us (with source gen)
  - Total SetParametersAsync cost: 100 x ~900 ns = ~90 us (without source gen)
```

**Approach B: Event Subscription (targeted)**

```
SelectRoot exposes: event Action<int> OnHighlightChanged

On highlight change:
  - Event fires → only the 2 affected items receive notification
  - Each calls StateHasChanged() → SetParametersAsync + render
  - Total SetParametersAsync calls: 2
  - Total renders: 2
  - Total cost: 2 x ~150 ns + 2 x render = ~0.3 us + render time
```

**Approach C: Service Injection (context service)**

```
SelectContext registered as scoped service, injected via DI:
  - Items read context.HighlightedIndex in their render
  - On change, service calls StateHasChanged on affected items via callback registry
  - Total SetParametersAsync calls: 2
  - Total renders: 2
  - But: requires manual subscription management, risk of memory leaks
```

**Comparison: Select with 100 Items**

| Approach | SetParametersAsync Calls | Renders | Overhead per Highlight Change | Complexity |
|----------|-------------------------|---------|------------------------------|------------|
| CascadingValue | 100 | 2 | ~15 us (source gen) | Low |
| CascadingValue (no source gen) | 100 | 2 | ~90 us | Low |
| Event Subscription | 2 | 2 | ~0.3 us | Medium |
| Service Injection | 2 | 2 | ~0.3 us | High |

### Model: Accordion with 20 Items

**On expand/collapse of a single item:**

| Approach | SetParametersAsync Calls | Renders | Overhead |
|----------|-------------------------|---------|----------|
| CascadingValue | 20 | 1-2 | ~3 us (source gen) |
| Event Subscription | 1-2 | 1-2 | ~0.3 us |

### Recommendation

**Use CascadingValue with `IsFixed="false"` for compound components with < 50 children.** The overhead is measurable but negligible at that scale (~15 us with source-generated `SetParametersAsync`). The simplicity benefit outweighs the micro-optimization.

**For components that can have 100+ children (Select, ComboBox, virtualized lists):**

1. Use `CascadingValue` for static context (component ID, disabled state) with `IsFixed="true"` -- zero overhead.
2. Use event callbacks for frequently-changing state (highlighted index, selection).
3. Combine: cascade the stable context object, but mutate a property on it and have items check `ShouldRender` against their own index.

```csharp
// SelectContext.cs -- cascaded with IsFixed="true" (reference never changes)
public sealed class SelectContext
{
    public int HighlightedIndex { get; internal set; }
    public string? SelectedValue { get; internal set; }
    public event Action? StateChanged;
    internal void NotifyStateChanged() => StateChanged?.Invoke();
}

// SelectItem.razor.cs
protected override void OnInitialized()
{
    Context.StateChanged += OnContextChanged;
}

private void OnContextChanged() => InvokeAsync(StateHasChanged);

public void Dispose() => Context.StateChanged -= OnContextChanged;

public override bool ShouldRender()
{
    var shouldRender = _isHighlighted != (Context.HighlightedIndex == _index);
    _isHighlighted = Context.HighlightedIndex == _index;
    return shouldRender;
}
```

This hybrid approach gives `IsFixed="true"` efficiency (no cascading subscription overhead) while providing targeted re-render notifications. Items still call `StateHasChanged` on every context change, but `ShouldRender` short-circuits the render for unaffected items. Net cost: 100 `ShouldRender` checks (~50 ns each = ~5 us) + 2 actual renders.

---

## 6. Benchmark Implementation Plan

### 6.1 BenchmarkDotNet Micro Benchmarks

**Project:** `tests/BlazingSpire.Benchmarks/`

```csharp
// SetParametersAsyncBenchmark.cs
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.AspNetCore.Components;

[MemoryDiagnoser]
[ShortRunJob(RuntimeMoniker.Net100)]
[JsonExporterAttribute.Full]
public class SetParametersAsyncBenchmark
{
    private ButtonComponent _buttonSourceGen = null!;
    private ButtonComponentReflection _buttonReflection = null!;
    private ParameterView _parameterView;

    [GlobalSetup]
    public void Setup()
    {
        _buttonSourceGen = new ButtonComponent();
        _buttonReflection = new ButtonComponentReflection();

        var dict = new Dictionary<string, object?>
        {
            ["Variant"] = ButtonVariant.Destructive,
            ["Size"] = ButtonSize.Lg,
            ["Disabled"] = false,
            ["Class"] = "mt-4",
            ["OnClick"] = EventCallback.Empty,
            ["ChildContent"] = (RenderFragment)(b => b.AddContent(0, "Click")),
            ["AriaLabel"] = "Delete item",
            ["Type"] = "button"
        };
        _parameterView = ParameterView.FromDictionary(dict);
    }

    [Benchmark(Baseline = true)]
    public Task SetParams_Reflection()
        => _buttonReflection.SetParametersAsync(_parameterView);

    [Benchmark]
    public Task SetParams_SourceGen()
        => _buttonSourceGen.SetParametersAsync(_parameterView);
}
```

```csharp
// CnBenchmark.cs
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
[ShortRunJob]
public class CnBenchmark
{
    private TwMerge _twMerge = null!;

    [GlobalSetup]
    public void Setup()
    {
        _twMerge = new TwMerge(); // or resolve from DI
    }

    [Benchmark]
    public string Cn_CacheHit_3Inputs()
    {
        // Pre-warmed in setup
        return Cn("px-4 py-2 bg-blue-500", "bg-red-500", "text-sm");
    }

    [Benchmark]
    public string Cn_CacheMiss_3Inputs()
    {
        // Use unique string each iteration to defeat cache
        return Cn("px-4 py-2 bg-blue-500", $"bg-red-{Random.Shared.Next(10000)}", "text-sm");
    }

    [Benchmark]
    public string Cn_CacheHit_8Inputs()
    {
        return Cn(
            "inline-flex items-center justify-center",
            "whitespace-nowrap rounded-md text-sm font-medium",
            "ring-offset-background transition-colors",
            "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
            "disabled:pointer-events-none disabled:opacity-50",
            "bg-primary text-primary-foreground hover:bg-primary/90",
            "h-10 px-4 py-2",
            "mt-4"
        );
    }

    [Benchmark]
    public string VariantLookup()
    {
        return VariantClasses[ButtonVariant.Destructive];
    }

    private string Cn(params ReadOnlySpan<string?> inputs)
    {
        // Actual implementation under test
        return _twMerge.Merge(string.Join(" ",
            inputs.ToArray().Where(s => !string.IsNullOrWhiteSpace(s))));
    }

    private static readonly FrozenDictionary<ButtonVariant, string> VariantClasses =
        new Dictionary<ButtonVariant, string>
        {
            [ButtonVariant.Default] = "bg-primary text-primary-foreground hover:bg-primary/90",
            [ButtonVariant.Destructive] = "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        }.ToFrozenDictionary();
}
```

```csharp
// EventCallbackBenchmark.cs
[MemoryDiagnoser]
[ShortRunJob]
public class EventCallbackBenchmark
{
    private EventCallback _callback;
    private EventCallback<string> _typedCallback;

    [GlobalSetup]
    public void Setup()
    {
        _callback = EventCallback.Factory.Create(new FakeReceiver(), () => { });
        _typedCallback = EventCallback.Factory.Create<string>(new FakeReceiver(), _ => { });
    }

    [Benchmark]
    public Task Dispatch_Void() => _callback.InvokeAsync();

    [Benchmark]
    public Task Dispatch_Typed() => _typedCallback.InvokeAsync("test");

    private sealed class FakeReceiver : IHandleEvent
    {
        public Task HandleEventAsync(EventCallbackWorkItem item, object? arg)
            => item.InvokeAsync(arg);
    }
}
```

### 6.2 Benchmark.Blazor Component Benchmarks

**Project:** `tests/BlazingSpire.ComponentBenchmarks/`

```csharp
// DataTableRenderBenchmark.cs
using Benchmark.Blazor;

public class DataTableRenderBenchmark : ComponentBenchmarkBase
{
    [Benchmark("DataTable_500Rows_InitialRender")]
    public async Task DataTable500Rows()
    {
        var data = Enumerable.Range(0, 500)
            .Select(i => new SampleRow(i, $"Name {i}", $"Value {i}"))
            .ToList();

        var cut = RenderComponent<DataTable<SampleRow>>(parameters => parameters
            .Add(p => p.Items, data)
            .Add(p => p.Columns, DefaultColumns));

        await cut.WaitForRendered();

        Assert.Equal(500, cut.FindAll("tr").Count - 1); // minus header
    }

    [Benchmark("DataTable_500Rows_Sort")]
    public async Task DataTable500RowsSort()
    {
        // Setup: render table
        var cut = RenderComponent<DataTable<SampleRow>>(/* ... */);
        await cut.WaitForRendered();

        // Measure: click sort header
        StartMeasurement();
        cut.Find("th:first-child").Click();
        await cut.WaitForRendered();
        StopMeasurement();
    }
}
```

```csharp
// SelectOpenBenchmark.cs
public class SelectOpenBenchmark : ComponentBenchmarkBase
{
    [Benchmark("Select_200Items_OpenToRendered")]
    public async Task Select200ItemsOpen()
    {
        var items = Enumerable.Range(0, 200)
            .Select(i => new SelectItem($"item-{i}", $"Option {i}"))
            .ToList();

        var cut = RenderComponent<Select<string>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Placeholder, "Choose..."));

        StartMeasurement();
        cut.Find("[role='combobox']").Click(); // Open the select
        await cut.WaitForRendered();
        StopMeasurement();

        Assert.Equal(200, cut.FindAll("[role='option']").Count);
    }
}
```

### 6.3 Playwright Timing Tests

**Project:** `tests/BlazingSpire.E2E/Benchmarks/`

```typescript
// dialog-performance.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Dialog Performance', () => {
  test('time-to-interactive < 50ms @server', async ({ page }) => {
    await page.goto('/benchmark/dialog');

    // Inject performance markers
    await page.evaluate(() => {
      document.querySelector('[data-testid="open-dialog"]')!
        .addEventListener('click', () => performance.mark('dialog-open-start'), { once: true });
    });

    // Observe focus trap activation as "interactive" signal
    const tti = await page.evaluate(async () => {
      performance.mark('dialog-open-start');
      document.querySelector<HTMLElement>('[data-testid="open-dialog"]')!.click();

      // Wait for focus to land inside dialog
      await new Promise<void>(resolve => {
        const observer = new MutationObserver(() => {
          const dialog = document.querySelector('[role="dialog"]');
          if (dialog && dialog.contains(document.activeElement)) {
            performance.mark('dialog-interactive');
            observer.disconnect();
            resolve();
          }
        });
        observer.observe(document.body, { childList: true, subtree: true, attributes: true });
      });

      const measure = performance.measure('dialog-tti', 'dialog-open-start', 'dialog-interactive');
      return measure.duration;
    });

    console.log(`Dialog TTI: ${tti.toFixed(2)}ms`);
    expect(tti).toBeLessThan(50); // CI fail threshold
  });

  test('DataTable 500 rows initial render < 120ms @wasm', async ({ page }) => {
    await page.goto('/benchmark/datatable-500');

    const renderTime = await page.evaluate(async () => {
      performance.mark('render-start');

      // Trigger render by setting items
      (window as any).__benchmarkTrigger();

      // Wait for last row to appear
      await new Promise<void>(resolve => {
        const check = () => {
          if (document.querySelectorAll('tbody tr').length >= 500) {
            performance.mark('render-complete');
            resolve();
          } else {
            requestAnimationFrame(check);
          }
        };
        requestAnimationFrame(check);
      });

      return performance.measure('datatable-render', 'render-start', 'render-complete').duration;
    });

    console.log(`DataTable 500 rows: ${renderTime.toFixed(2)}ms`);
    expect(renderTime).toBeLessThan(120);
  });

  test('Select 200 items open-to-rendered < 30ms', async ({ page }) => {
    await page.goto('/benchmark/select-200');

    const openTime = await page.evaluate(async () => {
      const trigger = document.querySelector<HTMLElement>('[role="combobox"]')!;
      performance.mark('select-open-start');
      trigger.click();

      await new Promise<void>(resolve => {
        const check = () => {
          const items = document.querySelectorAll('[role="option"]');
          if (items.length >= 200) {
            performance.mark('select-open-complete');
            resolve();
          } else {
            requestAnimationFrame(check);
          }
        };
        requestAnimationFrame(check);
      });

      return performance.measure('select-open', 'select-open-start', 'select-open-complete').duration;
    });

    console.log(`Select 200 items open: ${openTime.toFixed(2)}ms`);
    expect(openTime).toBeLessThan(30);
  });
});
```

### 6.4 CI Integration with Regression Detection

**GitHub Actions workflow:** `.github/workflows/benchmarks.yml`

```yaml
name: Performance Benchmarks

on:
  pull_request:
    paths:
      - 'src/**'
      - 'tests/BlazingSpire.Benchmarks/**'
  push:
    branches: [main]

jobs:
  micro-benchmarks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Run BenchmarkDotNet
        run: |
          dotnet run -c Release \
            --project tests/BlazingSpire.Benchmarks \
            -- --filter '*' \
            --exporters json \
            --artifacts ./benchmark-results

      - name: Check regression
        uses: benchmark-action/github-action-benchmark@v1
        with:
          tool: 'benchmarkdotnet'
          output-file-path: benchmark-results/BenchmarkDotNet.Artifacts/results/*-report-full-compressed.json
          github-token: ${{ secrets.GITHUB_TOKEN }}
          alert-threshold: '130%'       # 30% regression triggers warning
          fail-on-alert: true            # PR fails if any benchmark regresses > 30%
          comment-on-alert: true
          auto-push: ${{ github.ref == 'refs/heads/main' }}
          gh-pages-branch: benchmarks

  bundle-size:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Publish trimmed WASM
        run: |
          dotnet publish src/BlazingSpire.Primitives -c Release \
            -p:PublishTrimmed=true \
            -p:TrimMode=link \
            -o ./publish-trimmed

      - name: Check assembly size
        run: |
          SIZE=$(stat -c%s ./publish-trimmed/BlazingSpire.Primitives.dll 2>/dev/null || stat -f%z ./publish-trimmed/BlazingSpire.Primitives.dll)
          echo "Primitives DLL size: $SIZE bytes"
          if [ "$SIZE" -gt 122880 ]; then  # 120 KB fail threshold
            echo "::error::BlazingSpire.Primitives.dll exceeds 120 KB ($SIZE bytes)"
            exit 1
          fi

      - name: Check JS bundle size
        run: |
          GZIP_SIZE=$(gzip -c src/BlazingSpire.Primitives/wwwroot/blazingspire-interop.min.js | wc -c)
          echo "JS bundle gzipped: $GZIP_SIZE bytes"
          if [ "$GZIP_SIZE" -gt 8192 ]; then  # 8 KB fail threshold
            echo "::error::JS bundle exceeds 8 KB gzipped ($GZIP_SIZE bytes)"
            exit 1
          fi

  e2e-performance:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Install Playwright
        run: npx playwright install --with-deps chromium

      - name: Build and start test server
        run: |
          dotnet build tests/BlazingSpire.E2E.Host -c Release
          dotnet run --project tests/BlazingSpire.E2E.Host -c Release &
          sleep 5  # Wait for server startup

      - name: Run performance tests
        run: |
          npx playwright test tests/BlazingSpire.E2E/Benchmarks/ \
            --reporter=json \
            --output=./playwright-results
        env:
          BASE_URL: http://localhost:5000

      - name: Upload results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-benchmark-results
          path: ./playwright-results/
```

### 6.5 Pass/Fail Threshold Summary

| Category | Metric | Pass | Warn | Fail |
|----------|--------|------|------|------|
| Micro | SetParametersAsync (source gen, 8 params) | < 200 ns | 200-400 ns | > 500 ns |
| Micro | Cn() cached | < 80 ns | 80-150 ns | > 200 ns |
| Micro | Cn() uncached (3 inputs) | < 3 us | 3-8 us | > 10 us |
| Micro | FrozenDictionary lookup | < 20 ns | 20-40 ns | > 50 ns |
| Component | Select 200 items open | < 30 ms | 30-50 ms | > 60 ms |
| Component | Dialog TTI | < 50 ms | 50-80 ms | > 100 ms |
| Component | DataTable 500 rows | < 120 ms | 120-200 ms | > 250 ms |
| Bundle | Primitives DLL (trimmed) | < 80 KB | 80-100 KB | > 120 KB |
| Bundle | JS interop (gzipped) | < 4 KB | 4-6 KB | > 8 KB |
| Bundle | WASM delta (total) | < 250 KB | 250-400 KB | > 500 KB |
| Memory | Per-connection (20 components) | < 120 KB | 120-170 KB | > 200 KB |
| Regression | Any BenchmarkDotNet metric | < 110% of baseline | 110-130% | > 130% |

---

## 7. JS Module Loading Waterfall

### Dialog First-Render on Server Mode (Current Design)

```
Time (ms)  0    20    40    60    80    100   120   140
           |     |     |     |     |     |     |     |
User clicks "Open Dialog"
  ├─ [SignalR] Event sent to server ──────────┐
  │                                            │ ~20-40ms RTT
  ├─ [Server] OnClick handler runs             │
  │   └─ Renders Dialog component              │
  ├─ [SignalR] Render diff sent to client ─────┘
  │                                            
  ├─ [Browser] DOM updated, Dialog visible     
  │                                            
  ├─ [SignalR] OnAfterRenderAsync(true) ───────┐
  │                                            │
  │   ├─ JS Call 1: import('./blazingspire-interop.js')
  │   │   └─ [HTTP] Fetch JS module ──────────── ~15ms (cold) / ~2ms (warm)
  │   │       └─ [HTTP] Fetch floating-ui.js ─── ~10ms (cold, nested import)
  │   │
  │   ├─ JS Call 2: setupFocusTrap(dialogEl)
  │   │   └─ [SignalR RTT] ───────────────────── ~20-40ms
  │   │
  │   ├─ JS Call 3: lockBodyScroll()
  │   │   └─ [SignalR RTT] ───────────────────── ~20-40ms
  │   │
  │   ├─ JS Call 4: setupClickOutside(dialogEl)
  │   │   └─ [SignalR RTT] ───────────────────── ~20-40ms
  │   │
  │   └─ JS Call 5: setupPositioning(trigger, floating, opts)
  │       └─ [SignalR RTT] ───────────────────── ~20-40ms
  │
  └─ Dialog fully interactive
  
  Total (cold, sequential): ~140-215ms  ← TOO SLOW
  Total (warm, sequential): ~100-160ms  ← Still over budget
```

### Problem Analysis

Each `IJSRuntime.InvokeAsync` call in Server mode is a full SignalR round-trip. Five sequential interop calls at 20-40 ms each dominate the Dialog TTI.

### Optimization 1: Single Batched Init Call

Combine all five JS setup operations into one interop call:

```javascript
// blazingspire-interop.js
export function initDialog(dialogEl, triggerEl, options) {
    const focusTrap = setupFocusTrap(dialogEl);
    const scrollLock = lockBodyScroll();
    const clickOutside = setupClickOutside(dialogEl, options.onClose);
    const positioning = options.hasPositioning
        ? setupPositioning(triggerEl, dialogEl, options.positioning)
        : null;

    return {
        _cleanupId: registerCleanup(() => {
            focusTrap.dispose();
            scrollLock.dispose();
            clickOutside.dispose();
            positioning?.dispose();
        })
    };
}
```

```csharp
// Dialog.razor.cs
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        _module = await JS.InvokeAsync<IJSObjectReference>("import",
            "./_content/BlazingSpire.Primitives/blazingspire-interop.js");

        _cleanup = await _module.InvokeAsync<IJSObjectReference>("initDialog",
            _dialogRef, _triggerRef, new { HasPositioning = true, OnClose = _dotNetRef });
    }
}
```

**Result: 2 interop calls instead of 5.**

### Optimization 2: Module Preload

Add a `<link rel="modulepreload">` in the layout so the JS module is fetched while the page loads, before any component needs it:

```html
<!-- In MainLayout.razor or _Host.cshtml -->
<link rel="modulepreload"
      href="_content/BlazingSpire.Primitives/blazingspire-interop.js" />
<link rel="modulepreload"
      href="_content/BlazingSpire.Primitives/floating-ui-dom.esm.min.js" />
```

**Result: Module load drops from ~15 ms to ~0 ms (already in browser cache).**

### Optimization 3: Single Bundled Module

Instead of separate `focus.js`, `positioning.js`, `interaction.js`, etc., ship a single `blazingspire-interop.js` that includes all functionality. Floating UI is vendored inline.

```
Before: 6 separate module files + Floating UI = 7 HTTP requests (worst case)
After:  1 bundled module file = 1 HTTP request
```

**Result: Eliminates nested `import()` waterfalls.**

### Optimized Waterfall

```
Time (ms)  0    20    40    60    80    100
           |     |     |     |     |     |
User clicks "Open Dialog"
  ├─ [SignalR] Event to server ────────────┐
  │                                         │ ~20-40ms
  ├─ [Server] OnClick → render Dialog       │
  ├─ [SignalR] Render diff to client ───────┘
  │                                         
  ├─ [Browser] DOM updated                  
  │                                         
  ├─ [SignalR] OnAfterRenderAsync(true) ────┐
  │                                         │
  │   ├─ JS Call 1: import() ── ~0ms (preloaded)
  │   │
  │   └─ JS Call 2: initDialog(el, trigger, opts)
  │       └─ [SignalR RTT] ──────────────── ~20-40ms
  │       (focus trap + scroll lock + click-outside + positioning all run in one call)
  │
  └─ Dialog fully interactive
  
  Total (with preload): ~60-80ms  ← WITHIN 80ms BUDGET
```

### Optimization 4: Eager Module Import

For components known to be used on a page, import the JS module during `OnInitialized` (before `OnAfterRenderAsync`) using a fire-and-forget pattern:

```csharp
// InteropModuleCache.cs (singleton service)
public sealed class InteropModuleCache(IJSRuntime js) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _module = new(() =>
        js.InvokeAsync<IJSObjectReference>("import",
            "./_content/BlazingSpire.Primitives/blazingspire-interop.js").AsTask());

    public ValueTask<IJSObjectReference> GetModuleAsync()
        => new(_module.Value);

    public async ValueTask DisposeAsync()
    {
        if (_module.IsValueCreated)
        {
            var module = await _module.Value;
            await module.DisposeAsync();
        }
    }
}
```

This ensures the module `import()` starts as early as possible and is shared across all components on the page.

### Full Request Chain Summary

| Step | Before (ms) | After (ms) | Optimization Applied |
|------|-------------|-----------|---------------------|
| SignalR: event to server | 20-40 | 20-40 | (unchanged) |
| Server: render + diff | 5-10 | 5-10 | (unchanged) |
| SignalR: diff to client | 20-40 | 20-40 | (unchanged) |
| JS module load | 15-25 | 0-2 | modulepreload + single bundle |
| Floating UI load | 10-15 | 0 | Vendored into single bundle |
| Focus trap setup | 20-40 | 0 | Batched into initDialog |
| Scroll lock | 20-40 | 0 | Batched into initDialog |
| Click-outside | 20-40 | 0 | Batched into initDialog |
| Positioning | 20-40 | 20-40 | Single batched call |
| **Total** | **140-215** | **60-80** | **55-65% reduction** |

---

## Appendix: Benchmark Project Structure

```
tests/
├── BlazingSpire.Benchmarks/              # BenchmarkDotNet micro benchmarks
│   ├── BlazingSpire.Benchmarks.csproj
│   ├── Program.cs                        # BenchmarkSwitcher entry point
│   ├── SetParametersAsyncBenchmark.cs
│   ├── CnBenchmark.cs
│   ├── EventCallbackBenchmark.cs
│   ├── CascadingValueBenchmark.cs
│   └── MemoryProfileBenchmark.cs
├── BlazingSpire.ComponentBenchmarks/     # Benchmark.Blazor component-level
│   ├── DataTableRenderBenchmark.cs
│   ├── SelectOpenBenchmark.cs
│   ├── AccordionBenchmark.cs
│   └── DialogBenchmark.cs
├── BlazingSpire.E2E/                     # Playwright tests
│   ├── Benchmarks/
│   │   ├── dialog-performance.spec.ts
│   │   ├── datatable-performance.spec.ts
│   │   ├── select-performance.spec.ts
│   │   └── accordion-performance.spec.ts
│   └── playwright.config.ts
└── BlazingSpire.E2E.Host/               # Test host app with benchmark pages
    ├── Pages/Benchmark/
    │   ├── DialogBenchmark.razor
    │   ├── DataTableBenchmark.razor
    │   ├── SelectBenchmark.razor
    │   └── AccordionBenchmark.razor
    └── Program.cs
```
