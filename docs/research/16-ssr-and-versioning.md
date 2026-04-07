# SSR Graceful Degradation & Primitives Versioning

## Part 1: SSR Graceful Degradation

### 1. SSR Fallback Matrix

When a component renders in Static SSR mode (no WebSocket, no WebAssembly, no JS), the following behaviors apply:

| Component | SSR Fallback | Rendered HTML | Notes |
|-----------|-------------|---------------|-------|
| **Dialog** | Hidden; renders nothing visible | `<dialog>` element present but closed, trigger button visible | Trigger button is clickable but non-functional without JS. Content is in the DOM for SEO but not visible. |
| **Select** | Native `<select>` | `<select>` with `<option>` children | Fully functional form control. Server-side `EditForm` submission works. |
| **Combobox** | Native `<input>` + `<datalist>` | `<input list="id">` + `<datalist id="id"><option>` | Browser-native autocomplete. No custom filtering, but usable. |
| **Dropdown Menu** | Hidden; trigger visible | `<button>` trigger renders, menu content present but `hidden` | Similar to Dialog. Menu items are in DOM for SEO. |
| **Tabs** | Default tab panel visible only | All `<div role="tabpanel">` rendered; non-default panels get `hidden` attribute | Tab navigation non-functional. Default panel content is accessible. |
| **Accordion** | All panels expanded | All `<details><summary>` with `open` attribute | Uses native `<details>` element. Fully functional without JS. |
| **Tooltip** | `title` attribute | Host element gets `title="..."` attribute | Browser-native tooltip on hover. No positioning or custom styling. |
| **Toast** | Not rendered | Nothing | Service-based, requires interactivity to trigger. Server actions can queue toasts that appear after hydration. |
| **Popover** | Hidden; trigger visible | `<button>` trigger renders, popover content present but `hidden` | Same pattern as Dropdown Menu. |
| **Sheet** | Hidden; trigger visible | Trigger renders, sheet content present but `hidden` | Slide-out panels require JS for open/close. |

#### SSR Fallback Rendering Examples

**Dialog - SSR output:**
```razor
@* Dialog.razor - SSR renders the trigger and a hidden <dialog> *@
<button type="button"
        class="@TriggerClass"
        @attributes="TriggerAttributes">
    @Trigger
</button>
<dialog @attributes="DialogAttributes"
        aria-labelledby="@TitleId"
        aria-describedby="@DescriptionId"
        class="@ContentClass">
    @ChildContent
</dialog>
```

The `<dialog>` element is in the DOM but not open. Crawlers can index the content. After hydration, the primitive attaches click handlers, focus trap, scroll lock, and escape-to-close.

**Select - SSR native fallback:**
```razor
@* SelectPrimitive.razor *@
@if (!IsInteractive)
{
    <select name="@Name"
            id="@Id"
            class="@NativeSelectClass"
            disabled="@Disabled"
            @attributes="AdditionalAttributes">
        @if (Placeholder is not null)
        {
            <option value="" disabled selected>@Placeholder</option>
        }
        @foreach (var item in Items)
        {
            <option value="@item.Value"
                    selected="@(item.Value == Value)">
                @item.Label
            </option>
        }
    </select>
}
else
{
    @* Full custom select with Floating UI positioning, keyboard nav, etc. *@
    <div role="combobox" aria-expanded="@IsOpen" ...>
        @* ... interactive implementation ... *@
    </div>
}
```

**Combobox - SSR native fallback:**
```razor
@if (!IsInteractive)
{
    <input type="text"
           list="@DatalistId"
           name="@Name"
           value="@Value"
           placeholder="@Placeholder"
           class="@InputClass"
           @attributes="AdditionalAttributes" />
    <datalist id="@DatalistId">
        @foreach (var item in Items)
        {
            <option value="@item.Value">@item.Label</option>
        }
    </datalist>
}
else
{
    @* Full interactive combobox with filtering, highlighting, keyboard nav *@
}
```

**Accordion - SSR with native `<details>`:**
```razor
@foreach (var item in Items)
{
    @if (!IsInteractive)
    {
        <details open="@(item.DefaultOpen || ExpandAllInSSR)"
                 class="@ItemClass">
            <summary class="@TriggerClass">@item.Header</summary>
            <div class="@ContentClass">@item.Content</div>
        </details>
    }
    else
    {
        @* Controlled accordion with animated height transitions *@
    }
}
```

**Tabs - SSR default panel only:**
```razor
<div role="tablist" class="@TabListClass">
    @foreach (var tab in Tabs)
    {
        <button role="tab"
                aria-selected="@(tab.Value == DefaultValue)"
                class="@GetTabClass(tab)"
                @* No click handler in SSR - that's fine *@>
            @tab.Label
        </button>
    }
</div>
@foreach (var tab in Tabs)
{
    <div role="tabpanel"
         id="@tab.PanelId"
         @(tab.Value != DefaultValue ? "hidden" : null)
         class="@PanelClass">
        @tab.Content
    </div>
}
```

#### Detecting Render Mode in Components

The `IsInteractive` check is fundamental to the SSR fallback pattern. Primitives expose this via a base class:

```csharp
// BlazingSpirePrimitive.cs
public abstract class BlazingSpirePrimitive : ComponentBase, IAsyncDisposable
{
    [Inject] private IComponentRenderModeResolver? RenderModeResolver { get; set; }

    /// <summary>
    /// True when running in an interactive render mode (Server or WebAssembly).
    /// False during static SSR or prerendering before hydration completes.
    /// </summary>
    protected bool IsInteractive => RendererInfo.IsInteractive;

    /// <summary>
    /// True after OnAfterRenderAsync(firstRender: true) has completed and
    /// JS interop modules are initialized.
    /// </summary>
    protected bool IsJsReady { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeJsAsync();
            IsJsReady = true;
            StateHasChanged();
        }
    }

    protected virtual ValueTask InitializeJsAsync() => ValueTask.CompletedTask;

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
```

### 2. Progressive Enhancement Pattern

#### Phase 1: SSR Prerender

The server renders valid, semantic HTML. This is what crawlers and users-with-JS-disabled see:

```
Server renders HTML
        │
        ▼
┌─────────────────────────────┐
│  Static HTML + ARIA attrs   │
│  Native fallbacks active    │
│  No event handlers          │
│  No JS modules loaded       │
│  Layout dimensions stable   │
└─────────────────────────────┘
```

#### Phase 2: Hydration (interactive mode activates)

```
Blazor runtime loads
        │
        ▼
┌─────────────────────────────┐
│  RendererInfo.IsInteractive │
│  = true                     │
│  Component re-renders       │
│  Native fallbacks replaced  │
│  C# event handlers attach   │
└─────────────────────────────┘
        │
        ▼
OnAfterRenderAsync(firstRender: true)
        │
        ▼
┌─────────────────────────────┐
│  JS modules loaded          │
│  Focus trap initialized     │
│  Floating UI connected      │
│  Click-outside listeners    │
│  Keyboard nav active        │
│  IsJsReady = true           │
└─────────────────────────────┘
```

#### Avoiding Layout Shift During Upgrade

Layout shift during hydration is the primary UX risk. Strategies:

1. **Reserve dimensions in SSR markup.** The native `<select>` and the custom select trigger should occupy the same space. Use identical height, padding, and border classes on both:

```razor
@* Both paths use the same dimensional classes *@
@{
    const string sharedDimensions = "h-9 w-full rounded-md border px-3 py-2 text-sm";
}

@if (!IsInteractive)
{
    <select class="@sharedDimensions @NativeSelectExtra" ...>
        @* native options *@
    </select>
}
else
{
    <button role="combobox" class="@sharedDimensions @CustomTriggerExtra" ...>
        @* custom trigger content *@
    </button>
}
```

2. **No visibility toggling on hydration.** Components that are visible in SSR stay visible. Components that are hidden in SSR stay hidden. No flash.

3. **CSS `content-visibility: auto`** on heavy off-screen content (accordion panels, tab panels) to defer rendering cost without affecting layout.

4. **Skeleton matching.** If using `<Skeleton>` placeholders during SSR, ensure they match the final component dimensions exactly.

#### `@starting-style` and Hydration Animations

**Problem:** When a `<dialog>` element has `@starting-style` CSS animations, will the entry animation play when the component hydrates (even though it was already in the DOM)?

**Answer:** No, and here is why. The `@starting-style` rule applies when an element first enters the rendered state (e.g., `display: none` to `display: block`, or when the `open` attribute is added to `<dialog>`). During hydration, the `<dialog>` element was already in the DOM in its closed state. It does not transition to open. The animation only plays when the user explicitly opens the dialog after hydration.

However, there is an edge case: if a component re-renders and swaps from the native fallback to the custom markup (e.g., `<select>` to `<div role="combobox">`), the new elements are inserted fresh. To prevent unwanted entry animations on these fresh elements:

```css
/* Suppress entry animations during hydration swap */
[data-hydrating] [data-blazingspire-animate] {
    animation: none !important;
    transition: none !important;
}
```

```js
// Remove the hydration guard after the swap settles
export function markHydrationComplete(rootElement) {
    rootElement.removeAttribute('data-hydrating');
}
```

The root layout sets `data-hydrating` during SSR. The first `OnAfterRenderAsync` in the app root calls `markHydrationComplete` after a `requestAnimationFrame`, allowing all component swaps to complete before animations are re-enabled.

### 3. SSR-Only Components (No JS Required)

These components render pure HTML + Tailwind CSS classes. They work identically in all render modes and never load JS modules:

| Component | Renders As | Why No JS |
|-----------|-----------|-----------|
| **Button** | `<button>` or `<a>` | Pure styling + ARIA attributes |
| **Card** | `<div>` with semantic sections | Layout container only |
| **Badge** | `<span>` | Text with background color |
| **Avatar** | `<img>` + fallback `<span>` | CSS handles fallback via `onerror` on `<img>` or conditional rendering |
| **Skeleton** | `<div>` with pulse animation | CSS `@keyframes` only |
| **Separator** | `<hr>` or `<div role="separator">` | Pure CSS border/line |
| **Typography** | `<h1>`-`<h6>`, `<p>`, `<span>` | Semantic HTML elements |
| **Alert** | `<div role="alert">` | Static content display |
| **Table** | `<table>` with semantic elements | Standard HTML table |
| **Label** | `<label>` | Form association via `for` |
| **Input** | `<input>` | Native form control |
| **Textarea** | `<textarea>` | Native form control |
| **Checkbox** | `<input type="checkbox">` | Native form control (custom styling via CSS) |
| **Radio Group** | `<fieldset>` + `<input type="radio">` | Native form controls |
| **Progress** | `<progress>` or `<div role="progressbar">` | CSS width percentage |
| **Aspect Ratio** | `<div>` with `aspect-ratio` CSS | Pure CSS |

These components should inherit from a simpler base that does not import JS interop infrastructure:

```csharp
public abstract class BlazingSpireStaticComponent : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

### 4. SEO Considerations

#### Components That Affect Indexable Content

| Concern | Components | Strategy |
|---------|-----------|----------|
| **Hidden content** | Dialog, Sheet, Dropdown Menu, Popover | Content is in the DOM but hidden. Google indexes hidden content but may deprioritize it. For critical content, avoid putting it only inside overlays. |
| **Tab content** | Tabs | Only the default panel is visible in SSR. If all tab content matters for SEO, render all panels in SSR with `hidden` attribute (Google still indexes `hidden` content). |
| **Accordion content** | Accordion | All panels expanded in SSR using native `<details open>`. Content is fully visible to crawlers. |
| **Dynamic content** | Combobox, Select | Option lists are in the DOM via `<datalist>` / `<option>`. Searchable by crawlers. |
| **Toast messages** | Toast | Not in initial HTML. Never indexable. Do not put important content in toasts. |

#### Semantic HTML Requirements

Primitives must render semantically correct HTML to maximize crawler comprehension:

```razor
@* Dialog: proper heading hierarchy *@
<dialog aria-labelledby="@TitleId" aria-describedby="@DescriptionId">
    <h2 id="@TitleId">@Title</h2>
    <p id="@DescriptionId">@Description</p>
    @ChildContent
</dialog>

@* Tabs: proper list semantics for tab list *@
<div role="tablist" aria-label="@AriaLabel">
    <button role="tab" aria-selected="true" aria-controls="panel-1">Tab 1</button>
</div>
<div role="tabpanel" id="panel-1" aria-labelledby="tab-1">
    @* Content *@
</div>

@* Navigation in dropdown: use <nav> with <ul>/<li> *@
<nav aria-label="@AriaLabel">
    <ul role="menu">
        <li role="menuitem"><a href="/page">Link</a></li>
    </ul>
</nav>
```

#### Structured Data Compatibility

Components should not interfere with JSON-LD or microdata. Styled components must allow arbitrary `data-*` and `itemprop` attributes to pass through via `AdditionalAttributes`:

```razor
<Card AdditionalAttributes="@(new Dictionary<string, object>
{
    ["itemscope"] = true,
    ["itemtype"] = "https://schema.org/Product"
})">
    <CardTitle itemprop="name">Product Name</CardTitle>
</Card>
```

---

## Part 2: Primitives Versioning & Compatibility

### 5. API Stability Policy

The `BlazingSpire.Primitives` NuGet package follows [Semantic Versioning 2.0](https://semver.org/). The API surface is partitioned into three stability tiers:

| Tier | What's Included | Versioning Contract |
|------|----------------|-------------------|
| **Stable (public API)** | `[Parameter]` properties, `[CascadingParameter]` types, `EventCallback` parameters, public context types (`DialogContext`, `SelectContext<T>`, etc.), public service interfaces (`IPositioningService`, `IPortalService`) | Semver. Breaking changes only in major versions. |
| **Unstable (internal rendering)** | HTML element structure, CSS class names, DOM attribute names, number of rendered elements | Can change in minor versions. Styled components may need updates. |
| **Internal (not public contract)** | JS interop module APIs (function names, parameter shapes), internal C# types marked `[EditorBrowsable(Never)]`, `internal`/`private` members | Can change in any release. |

```csharp
// Stable API - part of the public contract
public class DialogPrimitive : BlazingSpirePrimitive
{
    /// <summary>Controls whether the dialog is open. Stable API.</summary>
    [Parameter] public bool IsOpen { get; set; }

    /// <summary>Callback when open state changes. Stable API.</summary>
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>Whether to trap focus inside the dialog. Stable API.</summary>
    [Parameter] public bool TrapFocus { get; set; } = true;

    /// <summary>Whether clicking outside closes the dialog. Stable API.</summary>
    [Parameter] public bool CloseOnOutsideClick { get; set; } = true;

    /// <summary>Whether pressing Escape closes the dialog. Stable API.</summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;
}

// Stable API - context type passed to styled components
public sealed record DialogContext(
    bool IsOpen,
    string TitleId,
    string DescriptionId,
    EventCallback RequestClose
);

// Internal - not part of public contract
[EditorBrowsable(EditorBrowsableState.Never)]
public static class DialogJsInterop
{
    internal const string ModulePath = "./_content/BlazingSpire.Primitives/dialog.js";
}
```

### 6. Breaking Change Taxonomy

Every change to the `Stable` tier is classified:

| Change Type | Breaking? | Example | Migration |
|------------|-----------|---------|-----------|
| Parameter added with default value | No | `[Parameter] public string Size { get; set; } = "md";` | None needed |
| Parameter added without default (required) | **Yes** | `[Parameter, EditorRequired] public string Label { get; set; }` | Must supply value |
| Parameter removed | **Yes** | Removing `IsDisabled` | Replace with alternative |
| Parameter type changed | **Yes** | `bool IsOpen` to `DialogState State` | Update call sites |
| Parameter renamed | **Yes** | `IsOpen` to `Open` | Rename in markup |
| Context type: member added | **Potentially** | Adding `bool IsFullScreen` to `DialogContext` | Non-breaking for most usage patterns. Breaking if user code uses positional construction or exhaustive pattern matching. |
| Context type: member removed | **Yes** | Removing `string DescriptionId` | Update consuming code |
| Context type: member type changed | **Yes** | `string TitleId` to `ElementReference TitleRef` | Update consuming code |
| Event signature changed | **Yes** | `EventCallback<bool>` to `EventCallback<DialogCloseReason>` | Update handler signature |
| Behavior change (same API) | **Case-by-case** | Dialog now auto-focuses first input | Document in release notes; provide opt-out parameter |
| Default value changed | **Case-by-case** | `CloseOnEscape` default `true` to `false` | Document; may break user expectations |

**Context types use `record` (not positional) to reduce breakage:**

```csharp
// GOOD: named properties - adding a member is non-breaking for property access
public sealed record DialogContext
{
    public required bool IsOpen { get; init; }
    public required string TitleId { get; init; }
    public required string DescriptionId { get; init; }
    public required EventCallback RequestClose { get; init; }
}

// BAD: positional record - adding a parameter breaks all construction sites
// public sealed record DialogContext(bool IsOpen, string TitleId, ...);
```

### 7. Compatibility Adapter Pattern

When a breaking change is necessary, both old and new APIs coexist for one full major version:

```csharp
public class DialogPrimitive : BlazingSpirePrimitive
{
    // New API (v2.0+)
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    // Deprecated API (removed in v3.0)
    [Parameter]
    [Obsolete("Use 'Open' instead. Will be removed in BlazingSpire.Primitives v3.0.")]
    public bool IsOpen
    {
        get => Open;
        set => Open = value;
    }

    [Parameter]
    [Obsolete("Use 'OpenChanged' instead. Will be removed in BlazingSpire.Primitives v3.0.")]
    public EventCallback<bool> IsOpenChanged
    {
        get => OpenChanged;
        set => OpenChanged = value;
    }
}
```

#### `blazingspire compat-check` CLI Command

The CLI includes a command to scan local (copy-pasted) components for deprecated API usage:

```
$ dotnet blazingspire compat-check

BlazingSpire Compatibility Check
================================

Scanning 23 components in ./Components/ui/...

  WARNING  Dialog.razor:14
           Parameter 'IsOpen' is deprecated since Primitives v2.0.
           Use 'Open' instead. Will be removed in v3.0.
           Auto-fix available: run with --fix

  WARNING  Dialog.razor:15
           Parameter 'IsOpenChanged' is deprecated since Primitives v2.0.
           Use 'OpenChanged' instead. Will be removed in v3.0.
           Auto-fix available: run with --fix

  WARNING  Select.razor:8
           Context member 'SelectedItem' is deprecated since Primitives v2.0.
           Use 'Value' instead. Will be removed in v3.0.

Found 3 deprecation warnings in 2 files.
Run 'dotnet blazingspire compat-check --fix' to auto-fix where possible.
```

Implementation approach: the CLI scans `.razor` files for known deprecated parameter names and context member accesses using simple regex/text matching and a deprecation registry shipped alongside each Primitives version:

```json
// deprecations.json (embedded resource in BlazingSpire.Primitives)
{
    "version": "2.0.0",
    "deprecations": [
        {
            "component": "DialogPrimitive",
            "member": "IsOpen",
            "replacement": "Open",
            "removedIn": "3.0.0",
            "autoFixable": true,
            "pattern": "IsOpen=",
            "fixPattern": "Open="
        }
    ]
}
```

### 8. Styled Component / Primitive Version Matrix

Since styled components are copy-pasted into the user's project, they are decoupled from the Primitives NuGet version. This creates a version skew risk.

#### Version Declaration

Each styled component carries a metadata comment declaring its target Primitives version:

```razor
@* Components/ui/Dialog.razor *@
@* blazingspire-primitives: ^1.0.0 *@
@* blazingspire-cli: 1.2.0 *@

@using BlazingSpire.Primitives.Dialog

<DialogPrimitive Open="@IsOpen" OpenChanged="@IsOpenChanged" ...>
    @* styled implementation *@
</DialogPrimitive>
```

The `^1.0.0` syntax means "compatible with Primitives 1.x". This metadata is used by the CLI.

#### Version Mismatch Detection

The CLI checks installed Primitives version against component metadata on `dotnet build` (via an MSBuild target) and on explicit check:

```
$ dotnet blazingspire version-check

BlazingSpire Version Matrix
============================

Primitives NuGet: 2.1.0 (installed)

  WARN  Dialog.razor targets Primitives ^1.0.0
        You have Primitives v2.1.0 installed.
        Breaking changes may affect this component.
        Run 'dotnet blazingspire update dialog' to get the v2.x version.

  OK    Button.razor targets Primitives ^2.0.0 (compatible)
  OK    Select.razor targets Primitives ^2.0.0 (compatible)
  OK    Tabs.razor targets Primitives ^2.0.0 (compatible)

3 of 4 components compatible. 1 component needs update.
```

#### MSBuild Integration (Optional Warning)

```xml
<!-- In BlazingSpire.Primitives.targets (ships with NuGet package) -->
<Target Name="BlazingSpireCompatCheck"
        AfterTargets="Build"
        Condition="'$(BlazingSpireSkipCompatCheck)' != 'true'">
  <Exec Command="dotnet blazingspire version-check --quiet --msbuild"
        IgnoreExitCode="true"
        ConsoleToMSBuild="true"
        Condition="Exists('$(ToolCommandPath)')">
    <Output TaskParameter="ConsoleOutput" ItemName="BlazingSpireWarnings" />
  </Exec>
</Target>
```

### 9. The `@starting-style` Unmount Problem

#### Problem Statement

CSS exit animations (fade-out, zoom-out, slide-out) defined via `@starting-style` and `transition` require the element to remain in the DOM while the animation plays. But Blazor removes elements from the DOM immediately when the component state changes (e.g., `IsOpen = false` triggers a re-render that removes the `<dialog>` content).

The result: exit animations never play because the element is gone before the transition can run.

#### Options Analysis

| Approach | Pros | Cons |
|----------|------|------|
| **(a) `transitionend` JS event** | Precise timing. Element removed exactly when animation completes. | Requires JS interop for every animated close. Risk of stuck elements if event never fires. |
| **(b) Keep mounted, `visibility: hidden`** | No timing issues. Element stays in DOM permanently. | Permanently increased DOM size. Accessibility concern (hidden elements still in a11y tree unless `aria-hidden`). Focus trap complexity. |
| **(c) Fixed timeout matching animation duration** | Simple. No JS event listener needed. | Fragile. Timeout must exactly match CSS duration. `prefers-reduced-motion` changes duration. |

#### Recommended Approach: (a) `transitionend` with Safety Timeout

Use `transitionend` as the primary signal, with a safety timeout as fallback:

```csharp
// DialogPrimitive.razor.cs
public async Task RequestClose()
{
    if (!IsJsReady)
    {
        // No JS means no animations - close immediately
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(false);
        return;
    }

    // Signal CSS to begin exit animation by setting a data attribute
    _isClosing = true;
    StateHasChanged();

    // Wait for the exit animation to complete (JS side)
    await JsModule!.InvokeVoidAsync("awaitExitAnimation", _dialogRef, _animationTimeoutMs);

    // Now actually close
    _isClosing = false;
    IsOpen = false;
    await IsOpenChanged.InvokeAsync(false);
}
```

```js
// dialog.razor.js
export function awaitExitAnimation(element, timeoutMs) {
    return new Promise((resolve) => {
        if (!element) { resolve(); return; }

        const cleanup = () => {
            clearTimeout(fallback);
            element.removeEventListener('transitionend', onEnd);
            resolve();
        };

        const onEnd = (e) => {
            // Only react to transitions on the element itself, not children
            if (e.target === element) {
                cleanup();
            }
        };

        element.addEventListener('transitionend', onEnd);

        // Safety fallback: if transitionend never fires (e.g., display:none
        // was set, or no transition is defined), resolve after timeout.
        const fallback = setTimeout(cleanup, timeoutMs ?? 300);
    });
}
```

```css
/* The CSS side: two-state animation via data attribute */
.dialog-overlay[data-state="open"] {
    opacity: 1;
    transition: opacity 200ms ease-out;
}

.dialog-overlay[data-state="closing"] {
    opacity: 0;
    transition: opacity 150ms ease-in;
}

.dialog-content[data-state="open"] {
    opacity: 1;
    transform: scale(1);
    transition: opacity 200ms ease-out, transform 200ms ease-out;
    @starting-style {
        opacity: 0;
        transform: scale(0.95);
    }
}

.dialog-content[data-state="closing"] {
    opacity: 0;
    transform: scale(0.95);
    transition: opacity 150ms ease-in, transform 150ms ease-in;
}
```

```razor
@* DialogPrimitive.razor - the data-state attribute drives CSS transitions *@
@if (IsOpen || _isClosing)
{
    <div class="dialog-overlay"
         data-state="@(_isClosing ? "closing" : "open")"
         @onclick="HandleOverlayClick">
    </div>
    <dialog @ref="_dialogRef"
            class="dialog-content"
            data-state="@(_isClosing ? "closing" : "open")"
            open
            aria-labelledby="@TitleId"
            aria-describedby="@DescriptionId">
        @ChildContent
    </dialog>
}
```

Key points:
- The `_isClosing` state keeps elements in the DOM during the exit animation.
- `data-state="closing"` triggers the CSS exit transition.
- JS listens for `transitionend` to signal completion back to C#.
- A safety timeout (default 300ms) ensures cleanup even if the event never fires.
- When `prefers-reduced-motion` is active, the CSS duration is shorter (or zero), and the `transitionend` fires sooner (or the timeout resolves quickly).

### 10. Portal Consolidation

#### Two Mechanisms in Play

From [04-js-interop-layer.md](04-js-interop-layer.md), there are two portal approaches:

1. **C# Portal Service** - `PortalService` with `ConcurrentDictionary<string, RenderFragment>`. A `<PortalHost>` in the layout renders fragments by key. No DOM manipulation. Pure Blazor rendering.

2. **JS DOM Reparenting** - `portal.js` physically moves elements to a `position: fixed` container at `<body>` level. Needed to escape CSS stacking contexts (`overflow: hidden`, `z-index` containment).

#### Recommendation: JS DOM Reparenting as Primary

**Use JS DOM reparenting as the single primary portal mechanism.** Here is the rationale:

| Factor | C# PortalService | JS DOM Reparenting |
|--------|------------------|-------------------|
| Escapes `overflow: hidden` | No (rendered in original tree position) | **Yes** (moved to body-level container) |
| Escapes `z-index` stacking | No | **Yes** |
| Works in Static SSR | **Yes** (renders in layout) | No (requires JS) |
| RenderFragment boundary | Same render mode only | Same render mode only |
| Component state preserved | Yes (same component tree) | Yes (DOM move preserves state) |
| Debugging | Easier (Blazor component tree) | Harder (DOM doesn't match component tree) |

The C# PortalService has a fatal flaw: it cannot escape CSS stacking contexts, which is the entire reason portals exist. Dialogs, popovers, dropdowns, and tooltips all need to render above everything else, and CSS `overflow: hidden` on any ancestor will clip them.

#### Portal Architecture

```
┌─ <body> ──────────────────────────────────┐
│                                           │
│  ┌─ #app ───────────────────────────────┐ │
│  │ <MainLayout>                         │ │
│  │   <div style="overflow: hidden">     │ │
│  │     <Dialog>                         │ │
│  │       <div data-portal-source>       │ │
│  │         <!-- content moved out -->   │ │
│  │       </div>                         │ │
│  │     </Dialog>                        │ │
│  │   </div>                             │ │
│  └──────────────────────────────────────┘ │
│                                           │
│  ┌─ #blazingspire-portals (fixed) ──────┐ │
│  │  <div data-portal-content="dialog-1">│ │
│  │    <!-- dialog content lives here --> │ │
│  │  </div>                              │ │
│  └──────────────────────────────────────┘ │
└───────────────────────────────────────────┘
```

#### Implementation

```js
// portal.js
const PORTAL_CONTAINER_ID = 'blazingspire-portals';

function getOrCreateContainer() {
    let container = document.getElementById(PORTAL_CONTAINER_ID);
    if (!container) {
        container = document.createElement('div');
        container.id = PORTAL_CONTAINER_ID;
        container.style.position = 'fixed';
        container.style.inset = '0';
        container.style.pointerEvents = 'none';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
    }
    return container;
}

export function mountPortal(sourceElement, portalId) {
    const container = getOrCreateContainer();
    const wrapper = document.createElement('div');
    wrapper.setAttribute('data-portal-content', portalId);
    wrapper.style.pointerEvents = 'auto';

    // Move all children from source to portal wrapper
    while (sourceElement.firstChild) {
        wrapper.appendChild(sourceElement.firstChild);
    }
    container.appendChild(wrapper);

    return {
        dispose() {
            // Move children back before removing wrapper
            while (wrapper.firstChild) {
                sourceElement.appendChild(wrapper.firstChild);
            }
            wrapper.remove();
        }
    };
}
```

```csharp
// Portal.razor - thin C# wrapper
@implements IAsyncDisposable

<div @ref="_sourceRef" data-portal-source="@PortalId" style="display:contents">
    @if (!IsInteractive)
    {
        @* In SSR, render content in-place (no JS available for reparenting) *@
        @ChildContent
    }
</div>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string PortalId { get; set; } = Guid.NewGuid().ToString("N")[..8];

    private ElementReference _sourceRef;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _portalHandle;

    private bool IsInteractive => RendererInfo.IsInteractive;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazingSpire.Primitives/portal.js");
            _portalHandle = await _jsModule.InvokeAsync<IJSObjectReference>(
                "mountPortal", _sourceRef, PortalId);
            StateHasChanged(); // Re-render to show content (now in portal)
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_portalHandle is not null)
            await _portalHandle.InvokeVoidAsync("dispose");
        if (_jsModule is not null)
            await _jsModule.DisposeAsync();
    }
}
```

#### When C# PortalService Is Still Useful

Keep the C# `PortalService` for one specific case: **cross-render-mode content injection**. When a static SSR layout needs to provide a "slot" that an interactive island can render into (e.g., a toast container in the layout), the C# PortalService works because both the host and the content are in the same Blazor component tree, just at different nesting levels.

```
JS DOM Reparenting (primary):
  - Dialog, Sheet, Dropdown Menu, Popover, Tooltip, Select dropdown
  - Any floating/overlay element that must escape stacking contexts

C# PortalService (secondary, specific case):
  - Toast container in root layout populated by interactive islands
  - Cross-component-tree content projection within the same render mode
```

Both mechanisms use the `data-portal-content` attribute so that click-outside detection (from `interaction.js`) correctly identifies portal content as "inside" the component that spawned it.
