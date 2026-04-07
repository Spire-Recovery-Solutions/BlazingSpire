# Blazor .NET 10 Limitations & Workarounds

## 1. No Programmatic `preventDefault`

**Problem:** `@onkeydown:preventDefault` is compile-time, not runtime. Cannot inspect key in C# then decide.
**Status:** Closed as "not planned" ([aspnetcore#45949](https://github.com/dotnet/aspnetcore/issues/45949))

**Workarounds (ranked):**

| Pattern | Description | Used By |
|---------|-------------|---------|
| **Custom Event Type** (primary) | `Blazor.registerCustomEventType` — `createEventArgs` runs synchronously in browser, can call `preventDefault()` | Officially recommended |
| **JS KeyInterceptor** (complex components) | Collocated `.razor.js` with native `addEventListener`, calls `preventDefault()` in JS, invokes .NET via `DotNetObjectReference` | MudBlazor |
| **Unconditional prevent** | `@onkeydown:preventDefault="true"` on inputs, manage text state manually | MudBlazor (Autocomplete) |

## 2. No Direct DOM Access

**Problem:** Focus management, scroll locking, click-outside, positioning — all require JS interop.

**Solution:** Minimal JS layer. See [04-js-interop-layer.md](04-js-interop-layer.md).

**What stays in pure C#:** ARIA attributes, component state, event routing, portal service (RenderFragment registry).

## 3. RenderFragment Can't Cross Static→Interactive Boundary

**Problem:** Cannot pass `RenderFragment` from static SSR parent to interactive child.
**Status:** Fix targeted for .NET 11 preview 5 (June 2026) for `RenderFragment`. `RenderFragment<T>` will **never** be supported.
**Tracked:** [aspnetcore#52768](https://github.com/dotnet/aspnetcore/issues/52768)

**Workarounds:**

| Pattern | When to Use |
|---------|-------------|
| **Dual API** | Declarative `<Dialog>` for same-mode; `DialogService.Show<T>()` for cross-boundary |
| **Global interactivity** | `<Routes @rendermode="InteractiveServer" />` — loses SSR benefits |
| **Type-based content** | Accept `Type` + `IDictionary<string, object>` instead of `RenderFragment` |
| **Sections API** | Layout-level slots only; content inherits outlet's render mode |

**How existing libraries handle it:**
- **MudBlazor:** Requires global interactivity. Does NOT support static SSR.
- **Radzen:** Service-based pattern (`DialogService.OpenAsync<TComponent>()`)
- **Fluent UI Blazor:** `DialogService.ShowDialogAsync<TComponent>()`
- **Blazor Blueprint:** Render-mode agnostic components + `ToastService.Show()` for overlays

## 4. Prerendering Double Initialization

**Problem:** `OnInitialized` runs twice. JS interop unavailable on first pass.
**Solution:** Use `[PersistentState]` (.NET 10) for data state. Move all JS interop to `OnAfterRenderAsync(firstRender: true)`. Design components to render valid static HTML before hydration.

## 5. CSS Isolation Incompatible with Tailwind

**Problem:** `.razor.css` scoped styles conflict with Tailwind utility approach.
**Solution:** Don't use CSS isolation. All styling via Tailwind utility classes in `.razor` markup.

## 6. Event Handling Latency (Server Mode)

**Problem:** Every JS interop call crosses SignalR (~20-100ms round-trip).
**Solution:** Batch interop calls. Put conditional logic (like `preventDefault`) in JS. Keep keyboard/scroll handlers in JS, send results to .NET.

## 7. Known .NET 10 Regressions

- Custom elements broken in RC2 ([#64087](https://github.com/dotnet/aspnetcore/issues/64087))
- Hot Reload breaks CSS isolation with RCLs ([#57660](https://github.com/dotnet/aspnetcore/issues/57660))
- `NavigationManager.NavigateTo` always scrolls to top
- Auto render mode never changes mid-page (by design)
- Layout components cannot be interactive in per-page rendering
- Interactive Server requires sticky sessions for load balancing
