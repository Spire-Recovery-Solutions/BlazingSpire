# Performance & Optimization

## Source Generators

| Generator | Purpose | Impact |
|-----------|---------|--------|
| **SetParametersAsync optimization** | Replace reflection-based parameter assignment with switch-based | 3-6x faster parameter binding |
| **CSS variant pre-computation** | Generate `FrozenDictionary` of variant→class mappings at compile time | Zero runtime string concatenation |
| **DI registration** | Generate explicit `AddBlazingSpireServices()` without reflection | Trim/AOT safe |
| **Tailwind class validation** (novel) | Validate Tailwind utility classes at compile time | Catch typos before runtime |

**Key constraint:** Source generators cannot see `.razor` file output (Razor compiler is itself a source generator). Use code-behind `.razor.cs` files with custom attributes.

**Reference packages:**
- **BlazorDelta** — 3-4x faster params, `[UpdatesCss]`, `[OnParameterChanged]`, zero-alloc splatting
- **Excubo.Generators.Blazor** — SetParametersAsync optimization, `@key` loop warnings

---

## IL Trimming & AOT

### Configuration

```xml
<!-- In Directory.Build.targets (NOT .props — TFM not available in .props) -->
<PropertyGroup Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">
  <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

`IsAotCompatible` enables BOTH trim and AOT analyzers. `IsTrimmable` alone misses AOT-specific warnings (IL3050).

### Rules

- Zero reflection in hot paths — use source generators instead
- `System.Text.Json` source generators mandatory for all serialization
- Never `[UnconditionalSuppressMessage]` or `#pragma warning disable` for IL warnings
- Test trimmed output in CI: `dotnet publish -c Release` + integration tests
- WASM AOT: `<WasmStripILAfterAOT>true</WasmStripILAfterAOT>` reduces bundle 20-50%

### Specific Traps

**JS Interop is trim-safe by default.** `IJSRuntime.InvokeAsync<T>()` uses string method names, not reflection.

**`DotNetObjectReference<T>`** — `[JSInvokable]` methods survive automatically. Only need `[DynamicallyAccessedMembers]` if using reflection to discover invokable methods.

**`DialogService.Show<TComponent>()`** — the AOT danger zone:
```csharp
// Annotate to preserve the component type for DynamicComponent
public Task Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>()
```

**`RenderFragment` and `EventCallback`** are delegates — trim-safe, no annotations needed.

---

## Rendering Performance Patterns

| Technique | When | Impact |
|-----------|------|--------|
| `ShouldRender()` override | Primitives with frequent param updates but rare visual change | Eliminates unnecessary render tree diffs |
| `@key` on collection items | Any `@foreach` loop | O(n) diff instead of O(n²) |
| `CascadingValue IsFixed="true"` | Theme/config providers | Eliminates subscription tracking overhead |
| `IHandleEvent` | Prevent auto `StateHasChanged` after event handlers | Reduces re-renders in high-frequency events |
| `SetParametersAsync` source gen | Grid cells, chart points, repeated leaf components | 3-6x faster parameter binding |
| Cached delegates | Event handlers in loops | Eliminates per-render lambda allocation |
| `Virtualize<T>` | Large lists | Only renders visible viewport |

---

## Allocation Anti-Patterns

### Hot Path: Cn() / TailwindMerge

```csharp
// BAD: allocates params array + string.Join every render
public static string Cn(params string?[] inputs) =>
    TwMerge.Merge(string.Join(" ", inputs.Where(s => !string.IsNullOrWhiteSpace(s))));

// BETTER: use ReadOnlySpan (C# 13), cache result in OnParametersSet
public static string Cn(params ReadOnlySpan<string?> inputs) { ... }
```

- Cache the final merged class string — only recompute when parameters change
- Pre-compute variant dictionaries as `static readonly FrozenDictionary<TEnum, string>`
- TailwindMerge.NET has internal LRU cache — ensure it's effective by using stable input strings

### JS Interop

```csharp
// BAD: allocates per-render, leaks GC handles
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    _dotNetRef = DotNetObjectReference.Create(this); // LEAK
}

// GOOD: create once, dispose once
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/...");
    }
}
public async ValueTask DisposeAsync() { _dotNetRef?.Dispose(); ... }
```

### Events

- `EventCallback.Factory.Create()` allocates — prefer `[Parameter] public EventCallback OnClick { get; set; }` directly
- Event handlers in `@foreach` loops: use pre-bound delegates, not inline lambdas
- Avoid LINQ inside render logic — use `foreach` with conditions

---

## Bundle Size

| Metric | Value |
|--------|-------|
| MudBlazor CSS | ~500KB |
| Bootstrap 5.3 CSS | ~228KB |
| Tailwind (typical) | 10-50KB |
| blazor.web.js (.NET 10) | ~43KB (76% reduction from .NET 9) |
| Floating UI | ~3KB |
| BlazingSpire custom JS | ~300 lines (~5KB minified) |

---

## Validated WASM Boot Optimizations (2026-04-07)

Results from 19 Lighthouse iterations on the demo app, measuring each change in isolation.

### Lighthouse v13 Scoring Weights (Desktop)

| Metric | Weight | Notes |
|--------|--------|-------|
| FCP | 10% | |
| Speed Index | 10% | |
| **LCP** | **25%** | Main lever for WASM apps |
| **TBT** | **30%** | |
| **CLS** | **25%** | |
| TTI | 0% | Diagnostic only — NOT in the score since Lighthouse v10 |

### AOT vs Interpreter Tradeoff

AOT is **counterproductive** for small/medium apps. The native WASM binary dominates download time:

| Config | Brotli payload | LCP | TTI | Lighthouse Perf |
|--------|---------------|-----|-----|-----------------|
| AOT + IL strip + partial trim | 3,419 KB | 4.3s | 4.3s | 75 |
| AOT + full trim + all flags | 3,308 KB | 4.2s | 4.2s | 75 |
| **No AOT + Jiterpreter + full trim** | **1,472 KB** | **2.1s** | **2.1s** | **90** |

**Rule of thumb:** Use AOT only when runtime computation is the bottleneck (data grids, charts, crypto). For UI-heavy apps, the interpreter + Jiterpreter provides acceptable runtime speed with half the download.

### Skeleton-Outside-App Pattern

The breakthrough optimization: put the pre-rendered skeleton in a **sibling div outside `#app`**, not inside it.

**Why it works:** Per the W3C LCP spec, a new LCP candidate is emitted whenever a larger element renders. When the skeleton is *inside* `#app`, Blazor replaces it on boot and the new (larger) content becomes LCP at 4.2s. When the skeleton is *outside* `#app`, Blazor renders into a hidden div — the skeleton stays as LCP at 0.5s.

```html
<!-- Skeleton OUTSIDE #app — Blazor cannot replace it -->
<div id="skeleton">
    <nav>...</nav>
    <main><h1>BlazingSpire</h1><p>Description...</p></main>
</div>

<!-- Blazor renders here, hidden until ready -->
<div id="app" style="display:none"></div>

<script src="_framework/blazor.webassembly.js" autostart="false"></script>
<script>
    Blazor.start().then(function () {
        document.getElementById('skeleton').remove();
        document.getElementById('app').style.display = '';
    });
</script>
```

| With skeleton inside #app | With skeleton outside #app |
|--------------------------|---------------------------|
| LCP = 2.1s, Perf = 90 | LCP = 0.5s, Perf = 100 |

### MSBuild Properties — Validated Minimal Config

These properties produce a 1.35 MB brotli payload with Lighthouse 100/100/100/100:

```xml
<PropertyGroup>
  <!-- No AOT — interpreter + Jiterpreter is faster boot for UI apps -->
  <RunAOTCompilation>false</RunAOTCompilation>
  <BlazorWebAssemblyJiterpreter>true</BlazorWebAssemblyJiterpreter>

  <!-- Globalization stripping -->
  <InvariantGlobalization>true</InvariantGlobalization>
  <InvariantTimezone>true</InvariantTimezone>
  <BlazorEnableTimeZoneSupport>false</BlazorEnableTimeZoneSupport>
  <BlazorWebAssemblyPreserveCollationData>false</BlazorWebAssemblyPreserveCollationData>

  <!-- Trimming -->
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>full</TrimMode>
  <!-- Requires TrimmerRoots.xml to preserve component constructors -->

  <!-- SIMD not needed for UI -->
  <WasmEnableSIMD>false</WasmEnableSIMD>

  <!-- Strip unused runtime features -->
  <EventSourceSupport>false</EventSourceSupport>
  <HttpActivityPropagationSupport>false</HttpActivityPropagationSupport>
  <DebuggerSupport>false</DebuggerSupport>
  <MetadataUpdaterSupport>false</MetadataUpdaterSupport>
  <UseSystemResourceKeys>true</UseSystemResourceKeys>
</PropertyGroup>

<ItemGroup>
  <TrimmerRootDescriptor Include="TrimmerRoots.xml" />
</ItemGroup>
```

### Trimming Gotchas

| Property | Effect | Safe? |
|----------|--------|-------|
| `TrimMode=full` | Trims all assemblies aggressively | Breaks Blazor components without TrimmerRoots.xml |
| `JsonSerializerIsReflectionEnabledByDefault=false` | Disables STJ reflection | **Breaks JS interop** — Blazor marshalling needs it |
| `WasmEnableThreads=true` | Multi-threading | **Breaks Blazor** — renderer assumes single-threaded |

### What We Can't Trim (Framework Floor)

The remaining 32 assemblies at 1.35 MB brotli are all structurally required:

| Assembly | Brotli | Why |
|----------|--------|-----|
| System.Private.CoreLib | 434 KB | .NET runtime core |
| dotnet.native | 404 KB | Mono WASM runtime |
| System.Text.Json | 121 KB | JS interop marshalling |
| Microsoft.AspNetCore.Components | 89 KB | Blazor renderer |
| Everything else | ~300 KB | DI, logging, config, collections |

### .NET 10 WASM Features (defaults)

| Feature | Property | Default | Notes |
|---------|----------|---------|-------|
| WASM SIMD | `WasmEnableSIMD` | true | We disable for size |
| Exception Handling | `WasmEnableExceptionHandling` | true | 20-30% improvement in try/catch |
| Jiterpreter | `BlazorWebAssemblyJiterpreter` | true | 30-50% runtime improvement |
| WebCIL | `WasmEnableWebcil` | true | .wasm packaging |
| Compression | `CompressionEnabled` | true | Brotli + gzip |
| Threading | `WasmEnableThreads` | false | DO NOT enable for Blazor |

### Unused _Imports.razor Namespaces Pull Dependencies

Default Blazor template includes `System.Net.Http`, `System.Net.Http.Json`, `Microsoft.AspNetCore.Components.Forms`, `Virtualization` in `_Imports.razor`. Removing unused imports helps the trimmer eliminate more code.

### FocusOnNavigate Causes h1 Selection Flash

`<FocusOnNavigate RouteData="routeData" Selector="h1" />` in `App.razor` calls `focus()` on the h1 after navigation. Fix: `h1:focus { outline: none; }` in CSS. Keeps accessibility (screen readers) while hiding the visual artifact.

### JS Interop: Named Functions, Not eval()

`JS.InvokeAsync("eval", "...")` is slower (browser parses each call) and blocks CSP `unsafe-eval`. Use named functions in a `.js` file:

```javascript
// wwwroot/js/theme.js
window.blazingSpire = {
    getTheme: () => document.documentElement.classList.contains('dark'),
    setTheme: (isDark) => { /* ... */ }
};
```

### Blazor Loading Progress Bar

Blazor exposes `--blazor-load-percentage` CSS custom property during boot:

```html
<div style="position:fixed;top:0;left:0;height:3px;z-index:9999;
            background:oklch(0.42 0.18 25);
            width:calc(var(--blazor-load-percentage, 0) * 1%);
            transition:width 0.3s ease"></div>
```
