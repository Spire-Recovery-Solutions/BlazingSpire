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
