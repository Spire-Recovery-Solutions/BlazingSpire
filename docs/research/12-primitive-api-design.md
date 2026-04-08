# Primitive API Design: The 5 Hardest Components

This document defines concrete C# API specifications for the five most architecturally challenging headless primitives in `BlazingSpire.Primitives`. These five drive every major design decision — focus trapping, floating positioning, keyboard navigation, portals, scroll locking, and cross-component state coordination. Every other primitive in the library is a subset of patterns established here.

**Note:** The base component hierarchy (`Components/Shared/`) now provides centralized infrastructure for many of these concerns. `OverlayBase` handles focus trap, click outside, scroll lock, escape key, portal rendering, and JS interop lifecycle. `PopoverBase` adds Floating UI positioning. `MenuBase` adds item registry, roving focus, and keyboard navigation. Individual primitives extend these bases rather than implementing the infrastructure individually. See `docs/research/20-styled-component-patterns.md` for the full hierarchy.

**Target:** .NET 10, Blazor United (SSR + Interactive Server + InteractiveAuto + WASM)

---

## Table of Contents

1. [CascadingValue Performance Analysis](#cascadingvalue-performance-analysis)
2. [The AsChild Pattern](#the-aschild-pattern)
3. [Dialog](#1-dialog)
4. [Select](#2-select)
5. [Combobox](#3-combobox)
6. [Menu / Dropdown](#4-menu--dropdown)
7. [Tabs](#5-tabs)
8. [Consumer Usage Examples](#consumer-usage-examples)

---

## CascadingValue Performance Analysis

### The Problem

A `Select` with 100+ `SelectItem` children, each consuming a `CascadingValue<SelectContext>`, triggers a re-render of **every child** whenever the context object changes — even if the change (e.g., highlighted index moving from 42 to 43) is irrelevant to 98 of those children.

Blazor's `CascadingValue` uses reference equality by default. If the context is a mutable class that is replaced on every state change (new object), all subscribers re-render. If it is the same object instance, `CascadingValue` skips notification — but then children never learn about changes.

### Approach Comparison

| Approach | Re-renders | Complexity | SSR Compatible | Trim/AOT Safe |
|----------|-----------|------------|----------------|---------------|
| **CascadingValue (replace object)** | O(n) on every state change | Low | Yes | Yes |
| **CascadingValue (same ref) + manual notify** | None automatic; must call `StateHasChanged` selectively | Medium | Yes | Yes |
| **CascadingValue\<TRoot\> (parent ref)** | Only the parent re-renders; children pull state imperatively | Medium | Yes | Yes |
| **Scoped DI service** | None automatic; subscription-based | High | Partial (no DI in static SSR) | Yes |
| **Event subscription (IObservable / Action)** | Only subscribers whose predicate matches | Medium-High | Yes (C# events work in SSR) | Yes |

### Recommended: Tiered Strategy

**Tier 1 — Simple primitives (Dialog, Tabs):** Use `CascadingValue<TContext>` with `IsFixed="false"`. Child count is small (Dialog has ~5 parts, Tabs rarely exceed 20). The O(n) re-render cost is negligible.

**Tier 2 — Collection primitives (Select, Menu, Combobox):** Use `CascadingValue<TRoot>` where the cascaded value is the **root component reference itself**, set once with `IsFixed="true"`. Children read state directly from the parent reference. State changes call `StateHasChanged()` only on the root, which re-renders its own render tree. Individual items override `ShouldRender()` to skip unless their own identity-relevant state changed (e.g., `IsHighlighted` or `IsSelected` changed for *this specific item*).

```csharp
// Tier 2 pattern: parent reference cascade
public partial class SelectRoot : ComponentBase, IAsyncDisposable
{
    // Cascaded as IsFixed="true" — one-time subscription, zero overhead
    // Children access state via this.Parent.HighlightedIndex, etc.
}

public partial class SelectItem : ComponentBase
{
    [CascadingParameter] public SelectRoot Root { get; set; } = default!;

    private bool _wasHighlighted;
    private bool _wasSelected;

    protected override bool ShouldRender()
    {
        var isHighlighted = Root.HighlightedIndex == Index;
        var isSelected = Root.SelectedValue?.Equals(Value) == true;
        if (isHighlighted == _wasHighlighted && isSelected == _wasSelected)
            return false;
        _wasHighlighted = isHighlighted;
        _wasSelected = isSelected;
        return true;
    }
}
```

**Why not scoped DI?** Services registered in DI are not available during static SSR rendering when the component is not inside an interactive render mode boundary. Since our primitives must render valid HTML in static SSR (even if non-interactive), DI-based state sharing would require a separate SSR code path. `CascadingValue` works in all render modes.

**Why not `IHandleEvent` suppression?** `IHandleEvent` prevents the automatic `StateHasChanged()` call after event handlers. This is useful for high-frequency events (mousemove, scroll) but dangerous as a general strategy — forgetting to manually call `StateHasChanged()` leads to stale UI. We reserve `IHandleEvent` for specific hot-path scenarios (e.g., the roving focus highlight in Menu/Select) rather than making it a structural pattern.

---

## The AsChild Pattern

Radix UI's `asChild` lets a primitive render its behavior (ARIA attributes, event handlers, refs) onto a consumer-supplied element instead of emitting its own wrapper element. In Blazor, we implement this with a `RenderFragment<TAttributes>` pattern.

### Design

```csharp
// Every primitive part that renders an element supports AsChild
public partial class DialogTrigger : ComponentBase
{
    /// <summary>
    /// When true, the component does not render its own element.
    /// Instead, it passes attributes to the child via AsChildContent.
    /// </summary>
    [Parameter] public bool AsChild { get; set; }

    /// <summary>
    /// Content rendered when AsChild is false (default). Standard RenderFragment.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Content rendered when AsChild is true. Receives a dictionary of HTML attributes
    /// (ARIA, event handlers, refs) that MUST be splatted onto the root element.
    /// </summary>
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
}
```

### Consumer Usage

```razor
@* Default: DialogTrigger renders its own <button> *@
<DialogTrigger>Open Dialog</DialogTrigger>

@* AsChild: attributes splatted onto consumer's element *@
<DialogTrigger AsChild="true">
    <AsChildContent Context="attrs">
        <MyStyledButton @attributes="attrs">Open Dialog</MyStyledButton>
    </AsChildContent>
</DialogTrigger>
```

### Implementation Detail

Each primitive part builds an attribute dictionary containing its required ARIA attributes, event handlers, and `@ref` callback. When `AsChild` is false, the component renders its own element with those attributes. When `AsChild` is true, it passes the dictionary to `AsChildContent` and renders nothing else.

```csharp
protected override void BuildRenderTree(RenderTreeBuilder builder)
{
    var attrs = BuildAttributes(); // ARIA, events, ref

    if (AsChild && AsChildContent is not null)
    {
        builder.AddContent(0, AsChildContent(attrs));
    }
    else
    {
        builder.OpenElement(0, "button");
        foreach (var (key, value) in attrs)
            builder.AddAttribute(1, key, value);
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
```

---

## 1. Dialog

### Component Parts

| Part | Default Element | Purpose |
|------|----------------|---------|
| `DialogRoot` | (renderless) | State container, cascading provider |
| `DialogTrigger` | `<button>` | Opens the dialog on click |
| `DialogPortal` | (renderless) | Renders children via PortalService at body level |
| `DialogOverlay` | `<div>` | Fullscreen backdrop, click-to-close optional |
| `DialogContent` | `<div>` | The dialog panel; focus trap container |
| `DialogClose` | `<button>` | Closes the dialog on click |
| `DialogTitle` | `<h2>` | Accessible title, linked via `aria-labelledby` |
| `DialogDescription` | `<p>` | Accessible description, linked via `aria-describedby` |

### Context Type

```csharp
namespace BlazingSpire.Primitives.Dialog;

public sealed class DialogContext
{
    public required string DialogId { get; init; }
    public required string TitleId { get; init; }
    public required string DescriptionId { get; init; }
    public bool IsOpen { get; set; }
    public required Action Open { get; init; }
    public required Action Close { get; init; }
    public required Action Toggle { get; init; }
}
```

### Parameter Signatures

```csharp
// ── DialogRoot ──────────────────────────────────────────────
public partial class DialogRoot : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Controlled mode: bind to the open state.</summary>
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>Uncontrolled mode: initial open state (default false).</summary>
    [Parameter] public bool DefaultIsOpen { get; set; }

    /// <summary>Called when open state changes (both controlled and uncontrolled).</summary>
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }

    /// <summary>If true, clicking the overlay does not close the dialog.</summary>
    [Parameter] public bool Modal { get; set; } = true;
}

// ── DialogTrigger ───────────────────────────────────────────
public partial class DialogTrigger : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── DialogPortal ────────────────────────────────────────────
public partial class DialogPortal : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>CSS selector for the portal target container. Default: body.</summary>
    [Parameter] public string? Container { get; set; }
}

// ── DialogOverlay ───────────────────────────────────────────
public partial class DialogOverlay : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── DialogContent ───────────────────────────────────────────
public partial class DialogContent : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }

    /// <summary>When true, focus is trapped within the dialog content.</summary>
    [Parameter] public bool TrapFocus { get; set; } = true;

    /// <summary>When true, body scroll is locked while dialog is open.</summary>
    [Parameter] public bool LockScroll { get; set; } = true;

    /// <summary>Called when the user presses Escape. Default: closes dialog.</summary>
    [Parameter] public EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Called when the user clicks/focuses outside the content.</summary>
    [Parameter] public EventCallback OnInteractOutside { get; set; }

    /// <summary>If true, close on Escape. Default true.</summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;

    /// <summary>If true, close on click outside (non-modal only). Default true.</summary>
    [Parameter] public bool CloseOnInteractOutside { get; set; } = true;

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── DialogClose ─────────────────────────────────────────────
public partial class DialogClose : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── DialogTitle ─────────────────────────────────────────────
public partial class DialogTitle : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── DialogDescription ───────────────────────────────────────
public partial class DialogDescription : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
```

### Parent-Child Wiring

```
DialogRoot
  └─ CascadingValue<DialogContext> (IsFixed="false")
       ├─ DialogTrigger   [CascadingParameter]
       ├─ DialogPortal
       │    ├─ DialogOverlay  [CascadingParameter]
       │    └─ DialogContent  [CascadingParameter]
       │         ├─ DialogTitle        [CascadingParameter]
       │         ├─ DialogDescription  [CascadingParameter]
       │         └─ DialogClose        [CascadingParameter]
       └─ (any other user content)
```

Dialog uses Tier 1 (standard `CascadingValue<DialogContext>` with `IsFixed="false"`) because the child count is small (typically 5-8 parts). The context object is replaced on state change, triggering re-render of all parts — acceptable at this scale.

### Two-Way Binding

```razor
@* Controlled mode — consumer owns state *@
<DialogRoot @bind-IsOpen="_dialogOpen">
    ...
</DialogRoot>

@code {
    private bool _dialogOpen;
}
```

`DialogRoot` implements the standard Blazor two-way binding pattern:

```csharp
private bool _isOpen;

private async Task SetIsOpen(bool value)
{
    if (_isOpen == value) return;
    _isOpen = value;
    await IsOpenChanged.InvokeAsync(value);
    await OnOpenChanged.InvokeAsync(value);
}
```

In uncontrolled mode (no `@bind-IsOpen`), `DialogRoot` manages state internally, initializing from `DefaultIsOpen`.

### Event Callbacks

| Callback | Raised When |
|----------|-------------|
| `IsOpenChanged` | Two-way binding notification |
| `OnOpenChanged` | Open state changes (always, both modes) |
| `OnEscapeKeyDown` | Escape pressed while dialog content focused |
| `OnInteractOutside` | Click or focus outside dialog content |

### Keyboard Interactions (WAI-ARIA APG: Dialog Modal)

| Key | Behavior |
|-----|----------|
| `Tab` | Moves focus to next tabbable element inside dialog. Wraps from last to first. |
| `Shift+Tab` | Moves focus to previous tabbable element. Wraps from first to last. |
| `Escape` | Closes the dialog (if `CloseOnEscape` is true). |
| `Enter`/`Space` on trigger | Opens the dialog. |

Implementation: Focus trap and Escape handling are in JS (`focus.js`, `interaction.js`). The JS module is loaded in `DialogContent.OnAfterRenderAsync(firstRender: true)` and disposed in `DisposeAsync`.

### ARIA Attributes

| Part | Element | Attributes |
|------|---------|------------|
| `DialogTrigger` | `<button>` | `aria-haspopup="dialog"`, `aria-expanded="{IsOpen}"`, `aria-controls="{DialogId}"` |
| `DialogOverlay` | `<div>` | `aria-hidden="true"`, `data-state="{open\|closed}"` |
| `DialogContent` | `<div>` | `role="dialog"`, `aria-modal="{Modal}"`, `aria-labelledby="{TitleId}"`, `aria-describedby="{DescriptionId}"`, `id="{DialogId}"`, `data-state="{open\|closed}"` |
| `DialogTitle` | `<h2>` | `id="{TitleId}"` |
| `DialogDescription` | `<p>` | `id="{DescriptionId}"` |
| `DialogClose` | `<button>` | `aria-label="Close"` (when no text content) |

### SSR Fallback

In static SSR mode (no interactivity):
- `DialogTrigger` renders as `<button disabled>` with correct ARIA attributes.
- `DialogContent`, `DialogOverlay` render with `data-state="closed"` and `hidden` attribute (or `style="display:none"`).
- No JS is loaded. The dialog is inert but structurally present in the DOM.
- Consumer can use `<details>`/`<summary>` as a native HTML fallback via AsChild if desired.

```html
<!-- Static SSR output -->
<button aria-haspopup="dialog" aria-expanded="false" aria-controls="dlg-1" disabled>
  Open Dialog
</button>
<div role="dialog" aria-modal="true" aria-labelledby="dlg-1-title"
     aria-describedby="dlg-1-desc" id="dlg-1" data-state="closed" hidden>
  <h2 id="dlg-1-title">Dialog Title</h2>
  <p id="dlg-1-desc">Description text</p>
  <!-- content -->
</div>
```

---

## 2. Select

### Component Parts

| Part | Default Element | Purpose |
|------|----------------|---------|
| `SelectRoot` | (renderless) | State container, cascading provider |
| `SelectTrigger` | `<button>` | Opens the listbox, displays selected value |
| `SelectValue` | `<span>` | Renders the display text of selected item |
| `SelectPortal` | (renderless) | Renders dropdown via PortalService |
| `SelectContent` | `<div>` | Floating dropdown container |
| `SelectViewport` | `<div>` | Scrollable area within content |
| `SelectItem` | `<div>` | Individual option |
| `SelectItemText` | `<span>` | Display text of an item |
| `SelectItemIndicator` | `<span>` | Checkmark or selection indicator |
| `SelectGroup` | `<div>` | Groups items with a label |
| `SelectLabel` | `<div>` | Label for a group |
| `SelectSeparator` | `<div>` | Visual separator between groups |
| `SelectScrollUpButton` | `<div>` | Scroll up indicator/button |
| `SelectScrollDownButton` | `<div>` | Scroll down indicator/button |

### Context Type

```csharp
namespace BlazingSpire.Primitives.Select;

/// <summary>
/// Cascaded as the SelectRoot component reference itself (Tier 2 pattern).
/// Children access state via direct property reads on this reference.
/// </summary>
public partial class SelectRoot : ComponentBase, IAsyncDisposable
{
    // ── State (read by children) ────────────────────────────
    internal string? SelectedValue { get; private set; }
    internal string? SelectedText { get; private set; }
    internal bool IsOpen { get; private set; }
    internal int HighlightedIndex { get; private set; } = -1;
    internal string SelectId { get; } = BlazingSpireId.New("select");
    internal string ContentId { get; } = BlazingSpireId.New("select-content");
    internal string TriggerId { get; } = BlazingSpireId.New("select-trigger");

    // ── Item registry (children register on init) ───────────
    internal List<SelectItemRegistration> Items { get; } = [];

    internal void RegisterItem(SelectItemRegistration item) { ... }
    internal void UnregisterItem(string value) { ... }

    // ── Actions (called by children) ────────────────────────
    internal async Task SelectItem(string value, string? text) { ... }
    internal void SetHighlightedIndex(int index) { ... }
    internal async Task OpenDropdown() { ... }
    internal async Task CloseDropdown() { ... }
}

internal sealed record SelectItemRegistration(
    string Value,
    string? TextValue,
    bool Disabled,
    int Index);
```

### Parameter Signatures

```csharp
// ── SelectRoot ──────────────────────────────────────────────
public partial class SelectRoot : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Controlled value.</summary>
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Uncontrolled default value.</summary>
    [Parameter] public string? DefaultValue { get; set; }

    /// <summary>Called on value change (both modes).</summary>
    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }

    /// <summary>Controlled open state.</summary>
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Uncontrolled default open state.</summary>
    [Parameter] public bool DefaultOpen { get; set; }

    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }

    /// <summary>Disables the entire select.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Name attribute for form submission.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Whether the select is required in a form.</summary>
    [Parameter] public bool Required { get; set; }
}

// ── SelectTrigger ───────────────────────────────────────────
public partial class SelectTrigger : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectValue ─────────────────────────────────────────────
public partial class SelectValue : ComponentBase
{
    /// <summary>Placeholder text shown when no value is selected.</summary>
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectContent ───────────────────────────────────────────
public partial class SelectContent : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Positioning relative to trigger: "item-aligned" or "popper".</summary>
    [Parameter] public string Position { get; set; } = "item-aligned";

    /// <summary>Side for popper positioning.</summary>
    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;

    /// <summary>Alignment for popper positioning.</summary>
    [Parameter] public FloatingAlign Align { get; set; } = FloatingAlign.Start;

    /// <summary>Offset from trigger in pixels.</summary>
    [Parameter] public int SideOffset { get; set; } = 0;

    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectItem ──────────────────────────────────────────────
public partial class SelectItem : ComponentBase
{
    /// <summary>The value submitted when this item is selected.</summary>
    [Parameter, EditorRequired] public string Value { get; set; } = default!;

    /// <summary>Display text for type-ahead. Defaults to trimmed ChildContent text.</summary>
    [Parameter] public string? TextValue { get; set; }

    /// <summary>Whether this item is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectGroup ─────────────────────────────────────────────
public partial class SelectGroup : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectLabel ─────────────────────────────────────────────
public partial class SelectLabel : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectSeparator ─────────────────────────────────────────
public partial class SelectSeparator : ComponentBase
{
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── SelectItemIndicator ─────────────────────────────────────
public partial class SelectItemIndicator : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
```

### Parent-Child Wiring

```
SelectRoot (cascades itself as CascadingValue<SelectRoot> IsFixed="true")
  ├─ SelectTrigger   [CascadingParameter] → reads Root.IsOpen, Root.Disabled
  │    └─ SelectValue  [CascadingParameter] → reads Root.SelectedText
  ├─ SelectPortal
  │    └─ SelectContent  [CascadingParameter] → reads Root.IsOpen, manages positioning
  │         └─ SelectViewport
  │              ├─ SelectGroup
  │              │    ├─ SelectLabel
  │              │    └─ SelectItem*  [CascadingParameter] → registers with Root, reads Root.HighlightedIndex
  │              │         ├─ SelectItemText
  │              │         └─ SelectItemIndicator
  │              └─ SelectSeparator
  └─ (hidden <select> for form submission if Name is set)
```

This is the **Tier 2** pattern. `SelectRoot` cascades itself with `IsFixed="true"`. Items register on `OnInitialized` and unregister on `Dispose`. Items implement `ShouldRender()` to skip unless their highlighted/selected state changed.

### Two-Way Binding

```razor
<SelectRoot @bind-Value="_selectedFruit">
    <SelectTrigger>
        <SelectValue Placeholder="Pick a fruit..." />
    </SelectTrigger>
    <SelectPortal>
        <SelectContent>
            <SelectViewport>
                <SelectItem Value="apple">Apple</SelectItem>
                <SelectItem Value="banana">Banana</SelectItem>
                <SelectItem Value="cherry">Cherry</SelectItem>
            </SelectViewport>
        </SelectContent>
    </SelectPortal>
</SelectRoot>

@code {
    private string? _selectedFruit;
}
```

### Event Callbacks

| Callback | Raised When |
|----------|-------------|
| `ValueChanged` | Two-way binding notification |
| `OnValueChanged` | Value changes (both modes) |
| `OpenChanged` | Two-way binding for open state |
| `OnOpenChanged` | Open state changes |

### Keyboard Interactions (WAI-ARIA APG: Listbox)

**When trigger is focused (closed):**

| Key | Behavior |
|-----|----------|
| `Enter`, `Space`, `ArrowDown`, `ArrowUp` | Open the listbox. ArrowDown highlights first item, ArrowUp highlights last. |
| Type-ahead characters | Open and highlight first matching item |

**When listbox is open:**

| Key | Behavior |
|-----|----------|
| `ArrowDown` | Highlight next item (wraps to first) |
| `ArrowUp` | Highlight previous item (wraps to last) |
| `Home` | Highlight first item |
| `End` | Highlight last item |
| `Enter`, `Space` | Select highlighted item, close listbox |
| `Escape` | Close listbox without selecting, return focus to trigger |
| `Tab` | Select highlighted item, close, move focus naturally |
| Type-ahead characters | Highlight first matching item (350ms reset timer) |

Implementation: Keyboard navigation is handled in JS (`keyboard.js`) via the roving focus / type-ahead module. The JS module receives the list of item text values on open and reports the selected index back to .NET via `DotNetObjectReference`.

### ARIA Attributes

| Part | Element | Attributes |
|------|---------|------------|
| `SelectTrigger` | `<button>` | `role="combobox"`, `aria-expanded="{IsOpen}"`, `aria-haspopup="listbox"`, `aria-controls="{ContentId}"`, `aria-activedescendant="{highlighted item id}"`, `aria-disabled="{Disabled}"`, `aria-required="{Required}"`, `aria-autocomplete="none"` |
| `SelectContent` | `<div>` | `role="listbox"`, `id="{ContentId}"`, `aria-labelledby="{TriggerId}"` |
| `SelectItem` | `<div>` | `role="option"`, `aria-selected="{IsSelected}"`, `aria-disabled="{Disabled}"`, `data-highlighted="{IsHighlighted}"`, `id="{ItemId}"` |
| `SelectGroup` | `<div>` | `role="group"`, `aria-labelledby="{LabelId}"` |
| `SelectLabel` | `<div>` | `id="{LabelId}"` |
| `SelectSeparator` | `<div>` | `role="separator"`, `aria-hidden="true"` |

### SSR Fallback

In static SSR, render a native `<select>` element:

```html
<!-- Static SSR output -->
<select name="fruit" required>
  <option value="" disabled selected>Pick a fruit...</option>
  <option value="apple">Apple</option>
  <option value="banana">Banana</option>
  <option value="cherry">Cherry</option>
</select>
```

The `SelectRoot` detects the absence of an interactive runtime and switches its render tree to emit a native `<select>`. This provides full functionality without JS — form submission, keyboard nav, and screen reader support all come from the browser natively.

Detection mechanism:

```csharp
[CascadingParameter] private HttpContext? HttpContext { get; set; }

private bool IsInteractive => HttpContext is null; // null = interactive, non-null = static SSR

// Alternative for .NET 10:
// RendererInfo.IsInteractive (available on ComponentBase)
```

---

## 3. Combobox

### Component Parts

| Part | Default Element | Purpose |
|------|----------------|---------|
| `ComboboxRoot` | (renderless) | State container, cascading provider |
| `ComboboxAnchor` | `<div>` | Positioning anchor wrapping input + trigger |
| `ComboboxInput` | `<input>` | Text input for filtering |
| `ComboboxTrigger` | `<button>` | Toggle button (chevron icon) |
| `ComboboxPortal` | (renderless) | Renders dropdown via PortalService |
| `ComboboxContent` | `<div>` | Floating dropdown container |
| `ComboboxEmpty` | `<div>` | Shown when filter yields no results |
| `ComboboxGroup` | `<div>` | Groups items with a label |
| `ComboboxLabel` | `<div>` | Label for a group |
| `ComboboxItem` | `<div>` | Individual option |
| `ComboboxItemIndicator` | `<span>` | Checkmark for selected items |
| `ComboboxSeparator` | `<div>` | Visual separator |

### Context Type

```csharp
namespace BlazingSpire.Primitives.Combobox;

/// <summary>
/// Cascaded as the ComboboxRoot reference (Tier 2 pattern).
/// </summary>
public partial class ComboboxRoot : ComponentBase, IAsyncDisposable
{
    // ── State ───────────────────────────────────────────────
    internal string? SelectedValue { get; private set; }
    internal string? SelectedText { get; private set; }
    internal string SearchText { get; private set; } = "";
    internal bool IsOpen { get; private set; }
    internal int HighlightedIndex { get; private set; } = -1;
    internal string ComboboxId { get; } = BlazingSpireId.New("combobox");
    internal string InputId { get; } = BlazingSpireId.New("combobox-input");
    internal string ContentId { get; } = BlazingSpireId.New("combobox-content");

    // ── Item registry ───────────────────────────────────────
    internal List<ComboboxItemRegistration> Items { get; } = [];
    internal List<ComboboxItemRegistration> FilteredItems { get; private set; } = [];

    internal void RegisterItem(ComboboxItemRegistration item) { ... }
    internal void UnregisterItem(string value) { ... }

    // ── Actions ─────────────────────────────────────────────
    internal async Task SelectItem(string value, string? text) { ... }
    internal void SetSearchText(string text) { ... }
    internal void SetHighlightedIndex(int index) { ... }
    internal async Task OpenDropdown() { ... }
    internal async Task CloseDropdown() { ... }
}

internal sealed record ComboboxItemRegistration(
    string Value,
    string? TextValue,
    string[] SearchKeywords,
    bool Disabled,
    int Index);
```

### Parameter Signatures

```csharp
// ── ComboboxRoot ────────────────────────────────────────────
public partial class ComboboxRoot : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Controlled selected value.</summary>
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Uncontrolled default value.</summary>
    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }

    /// <summary>Controlled open state.</summary>
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool DefaultOpen { get; set; }
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }

    /// <summary>Controlled search/filter text.</summary>
    [Parameter] public string? SearchValue { get; set; }
    [Parameter] public EventCallback<string?> SearchValueChanged { get; set; }

    /// <summary>
    /// Custom filter function. Receives (item text, search text) and returns
    /// true if the item should be shown. Default: case-insensitive contains.
    /// </summary>
    [Parameter] public Func<string, string, bool>? FilterFunction { get; set; }

    /// <summary>If false, disables the built-in filter. Consumer handles filtering externally
    /// (e.g., server-side search via OnSearchValueChanged).</summary>
    [Parameter] public bool ShouldFilter { get; set; } = true;

    /// <summary>Disables the entire combobox.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Allow multiple selection.</summary>
    [Parameter] public bool Multiple { get; set; }

    /// <summary>For multiple mode: controlled selected values.</summary>
    [Parameter] public IReadOnlyList<string>? Values { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<string>> ValuesChanged { get; set; }

    /// <summary>Name for form submission.</summary>
    [Parameter] public string? Name { get; set; }
    [Parameter] public bool Required { get; set; }
}

// ── ComboboxInput ───────────────────────────────────────────
public partial class ComboboxInput : ComponentBase
{
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── ComboboxTrigger ─────────────────────────────────────────
public partial class ComboboxTrigger : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── ComboboxContent ─────────────────────────────────────────
public partial class ComboboxContent : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;
    [Parameter] public FloatingAlign Align { get; set; } = FloatingAlign.Start;
    [Parameter] public int SideOffset { get; set; } = 4;
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── ComboboxItem ────────────────────────────────────────────
public partial class ComboboxItem : ComponentBase
{
    [Parameter, EditorRequired] public string Value { get; set; } = default!;

    /// <summary>Text used for filtering and display. Defaults to ChildContent text.</summary>
    [Parameter] public string? TextValue { get; set; }

    /// <summary>Additional keywords for search matching.</summary>
    [Parameter] public string[]? Keywords { get; set; }

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── ComboboxEmpty ───────────────────────────────────────────
public partial class ComboboxEmpty : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ComboboxGroup, ComboboxLabel, ComboboxSeparator, ComboboxItemIndicator
// follow the same pattern as their Select equivalents (omitted for brevity).
```

### Parent-Child Wiring

Same Tier 2 pattern as Select: `ComboboxRoot` cascades itself with `IsFixed="true"`. Items register/unregister and implement `ShouldRender()`.

### Two-Way Binding

```razor
<ComboboxRoot @bind-Value="_selectedFramework" ShouldFilter="true">
    <ComboboxAnchor>
        <ComboboxInput Placeholder="Search frameworks..." />
        <ComboboxTrigger>▼</ComboboxTrigger>
    </ComboboxAnchor>
    <ComboboxPortal>
        <ComboboxContent>
            <ComboboxEmpty>No results found.</ComboboxEmpty>
            <ComboboxGroup>
                <ComboboxLabel>Frontend</ComboboxLabel>
                <ComboboxItem Value="blazor">Blazor</ComboboxItem>
                <ComboboxItem Value="react">React</ComboboxItem>
                <ComboboxItem Value="vue">Vue</ComboboxItem>
            </ComboboxGroup>
        </ComboboxContent>
    </ComboboxPortal>
</ComboboxRoot>

@code {
    private string? _selectedFramework;
}
```

### Event Callbacks

| Callback | Raised When |
|----------|-------------|
| `ValueChanged` | Two-way binding for single selection |
| `ValuesChanged` | Two-way binding for multiple selection |
| `OnValueChanged` | Value changes (both modes) |
| `SearchValueChanged` | Two-way binding for search text |
| `OpenChanged` | Two-way binding for open state |
| `OnOpenChanged` | Open state changes |

### Keyboard Interactions (WAI-ARIA APG: Combobox with Listbox Popup)

**When input is focused (closed):**

| Key | Behavior |
|-----|----------|
| `ArrowDown` | Open popup, highlight first item |
| `ArrowUp` | Open popup, highlight last item |
| `Enter` | If closed, open. If open, select highlighted item. |
| `Escape` | If open, close popup. If closed, clear input. |
| Any printable character | Updates search text, opens popup if closed, filters items |

**When popup is open:**

| Key | Behavior |
|-----|----------|
| `ArrowDown` | Highlight next item (skip disabled, wrap) |
| `ArrowUp` | Highlight previous item (skip disabled, wrap) |
| `Home` | Move cursor to start of input |
| `End` | Move cursor to end of input |
| `Enter` | Select highlighted item, close popup |
| `Escape` | Close popup, restore previous value in input |
| `Tab` | Select highlighted item (if any), close popup, move focus |

**Critical difference from Select:** The input retains DOM focus at all times. Arrow keys move a virtual highlight (managed via `aria-activedescendant`), not actual DOM focus. This is essential — moving DOM focus to list items would disrupt typing.

Implementation: Input events (`oninput`, `onkeydown`) are handled with a custom event type registered via `Blazor.registerCustomEventType`. The custom event handler in JS calls `preventDefault()` for ArrowUp/ArrowDown (to prevent cursor movement) before dispatching to .NET.

### ARIA Attributes

| Part | Element | Attributes |
|------|---------|------------|
| `ComboboxInput` | `<input>` | `role="combobox"`, `aria-expanded="{IsOpen}"`, `aria-haspopup="listbox"`, `aria-controls="{ContentId}"`, `aria-activedescendant="{highlighted item id}"`, `aria-autocomplete="list"`, `autocomplete="off"` |
| `ComboboxTrigger` | `<button>` | `aria-label="Show suggestions"`, `aria-expanded="{IsOpen}"`, `tabindex="-1"` |
| `ComboboxContent` | `<div>` | `role="listbox"`, `id="{ContentId}"`, `aria-label="{input placeholder or label}"` |
| `ComboboxItem` | `<div>` | `role="option"`, `aria-selected="{IsSelected}"`, `aria-disabled="{Disabled}"`, `data-highlighted="{IsHighlighted}"`, `id="{ItemId}"` |
| `ComboboxGroup` | `<div>` | `role="group"`, `aria-labelledby="{LabelId}"` |
| `ComboboxEmpty` | `<div>` | `role="status"`, `aria-live="polite"` |

### SSR Fallback

In static SSR, render a native `<input>` with a `<datalist>`:

```html
<!-- Static SSR output -->
<input type="text" list="combobox-1-list" name="framework"
       placeholder="Search frameworks..." autocomplete="off" />
<datalist id="combobox-1-list">
  <option value="blazor">Blazor</option>
  <option value="react">React</option>
  <option value="vue">Vue</option>
</datalist>
```

`<datalist>` provides native browser autocomplete with filtering. It lacks the custom rendering and grouping of the interactive version, but delivers functional search-and-select without JS.

---

## 4. Menu / Dropdown

### Component Parts

| Part | Default Element | Purpose |
|------|----------------|---------|
| `MenuRoot` | (renderless) | State container |
| `MenuTrigger` | `<button>` | Opens the menu |
| `MenuPortal` | (renderless) | Renders via PortalService |
| `MenuContent` | `<div>` | Floating menu container |
| `MenuItem` | `<div>` | Clickable menu item |
| `MenuCheckboxItem` | `<div>` | Toggle item with checkbox state |
| `MenuRadioGroup` | `<div>` | Groups radio items |
| `MenuRadioItem` | `<div>` | Radio-style exclusive selection |
| `MenuItemIndicator` | `<span>` | Check/radio indicator |
| `MenuGroup` | `<div>` | Visual grouping |
| `MenuLabel` | `<div>` | Group label |
| `MenuSeparator` | `<div>` | Visual divider |
| `MenuSub` | (renderless) | Sub-menu state container |
| `MenuSubTrigger` | `<div>` | Opens sub-menu on hover/keyboard |
| `MenuSubContent` | `<div>` | Sub-menu floating container |

### Context Type

```csharp
namespace BlazingSpire.Primitives.Menu;

/// <summary>
/// Cascaded as the MenuRoot reference (Tier 2 pattern).
/// Sub-menus cascade their own MenuSub reference additionally.
/// </summary>
public partial class MenuRoot : ComponentBase, IAsyncDisposable
{
    internal bool IsOpen { get; private set; }
    internal int HighlightedIndex { get; private set; } = -1;
    internal string MenuId { get; } = BlazingSpireId.New("menu");
    internal string ContentId { get; } = BlazingSpireId.New("menu-content");
    internal string TriggerId { get; } = BlazingSpireId.New("menu-trigger");

    internal List<MenuItemRegistration> Items { get; } = [];

    internal void RegisterItem(MenuItemRegistration item) { ... }
    internal void UnregisterItem(string id) { ... }
    internal void SetHighlightedIndex(int index) { ... }
    internal async Task OpenMenu() { ... }
    internal async Task CloseMenu() { ... }
    internal async Task CloseAll() { ... } // Close menu and all sub-menus
}

/// <summary>
/// Sub-menu state, cascaded additionally within MenuSub.
/// </summary>
public partial class MenuSub : ComponentBase
{
    internal bool IsOpen { get; private set; }
    internal int HighlightedIndex { get; private set; } = -1;
    internal string SubContentId { get; } = BlazingSpireId.New("menu-sub");
    internal List<MenuItemRegistration> Items { get; } = [];

    internal async Task OpenSubMenu() { ... }
    internal async Task CloseSubMenu() { ... }
}

internal sealed record MenuItemRegistration(
    string Id,
    string? TextValue,
    bool Disabled,
    int Index,
    bool IsSubTrigger);
```

### Parameter Signatures

```csharp
// ── MenuRoot ────────────────────────────────────────────────
public partial class MenuRoot : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool DefaultOpen { get; set; }
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }

    /// <summary>
    /// Direction for sub-menu opening. Affects arrow key behavior.
    /// </summary>
    [Parameter] public Direction Dir { get; set; } = Direction.Ltr;

    /// <summary>If true, the menu loops focus from last to first item and vice versa.</summary>
    [Parameter] public bool Loop { get; set; } = false;
}

// ── MenuTrigger ─────────────────────────────────────────────
public partial class MenuTrigger : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuContent ─────────────────────────────────────────────
public partial class MenuContent : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;
    [Parameter] public FloatingAlign Align { get; set; } = FloatingAlign.Start;
    [Parameter] public int SideOffset { get; set; } = 4;

    /// <summary>When true, close on Escape. Default true.</summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;

    /// <summary>When true, close when clicking outside. Default true.</summary>
    [Parameter] public bool CloseOnInteractOutside { get; set; } = true;

    [Parameter] public EventCallback OnEscapeKeyDown { get; set; }
    [Parameter] public EventCallback OnInteractOutside { get; set; }

    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuItem ────────────────────────────────────────────────
public partial class MenuItem : ComponentBase
{
    /// <summary>Text used for type-ahead. Defaults to ChildContent text.</summary>
    [Parameter] public string? TextValue { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Called when the item is selected (click or Enter/Space).</summary>
    [Parameter] public EventCallback OnSelect { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuCheckboxItem ────────────────────────────────────────
public partial class MenuCheckboxItem : ComponentBase
{
    /// <summary>Controlled checked state.</summary>
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter] public string? TextValue { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback OnSelect { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuRadioGroup ──────────────────────────────────────────
public partial class MenuRadioGroup : ComponentBase
{
    /// <summary>Controlled selected value within the group.</summary>
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuRadioItem ───────────────────────────────────────────
public partial class MenuRadioItem : ComponentBase
{
    [Parameter, EditorRequired] public string Value { get; set; } = default!;
    [Parameter] public string? TextValue { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback OnSelect { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuSub ─────────────────────────────────────────────────
public partial class MenuSub : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool DefaultOpen { get; set; }
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }
}

// ── MenuSubTrigger ──────────────────────────────────────────
public partial class MenuSubTrigger : ComponentBase
{
    [Parameter] public string? TextValue { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── MenuSubContent ──────────────────────────────────────────
public partial class MenuSubContent : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public int SideOffset { get; set; } = -4;
    [Parameter] public int AlignOffset { get; set; } = 0;
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// MenuGroup, MenuLabel, MenuSeparator, MenuItemIndicator
// follow the same structural pattern (omitted for brevity).
```

### Parent-Child Wiring

```
MenuRoot (cascades itself as CascadingValue<MenuRoot> IsFixed="true")
  ├─ MenuTrigger  [CascadingParameter]
  ├─ MenuPortal
  │    └─ MenuContent  [CascadingParameter]
  │         ├─ MenuLabel
  │         ├─ MenuItem*  [CascadingParameter] → registers, reads HighlightedIndex
  │         ├─ MenuCheckboxItem*
  │         ├─ MenuRadioGroup
  │         │    └─ MenuRadioItem*
  │         ├─ MenuSeparator
  │         └─ MenuSub (cascades itself as CascadingValue<MenuSub> IsFixed="true")
  │              ├─ MenuSubTrigger  [CascadingParameter] MenuSub
  │              └─ MenuSubContent  [CascadingParameter] MenuSub
  │                   └─ MenuItem* (registered with MenuSub, not MenuRoot)
  └─ (any other user content)
```

Sub-menus introduce a second cascading parameter. `MenuSubTrigger` and items within `MenuSubContent` read from their nearest `MenuSub` ancestor for highlight state, while still accessing `MenuRoot` for global operations like `CloseAll()`.

### Two-Way Binding

```razor
<MenuRoot>
    <MenuTrigger>Options</MenuTrigger>
    <MenuPortal>
        <MenuContent>
            <MenuItem OnSelect="HandleEdit">Edit</MenuItem>
            <MenuItem OnSelect="HandleDuplicate">Duplicate</MenuItem>
            <MenuSeparator />
            <MenuCheckboxItem @bind-Checked="_showHidden">Show Hidden</MenuCheckboxItem>
            <MenuSeparator />
            <MenuRadioGroup @bind-Value="_sortOrder">
                <MenuLabel>Sort By</MenuLabel>
                <MenuRadioItem Value="name">Name</MenuRadioItem>
                <MenuRadioItem Value="date">Date</MenuRadioItem>
                <MenuRadioItem Value="size">Size</MenuRadioItem>
            </MenuRadioGroup>
            <MenuSeparator />
            <MenuSub>
                <MenuSubTrigger>Share</MenuSubTrigger>
                <MenuSubContent>
                    <MenuItem OnSelect="HandleEmail">Email</MenuItem>
                    <MenuItem OnSelect="HandleSlack">Slack</MenuItem>
                </MenuSubContent>
            </MenuSub>
        </MenuContent>
    </MenuPortal>
</MenuRoot>

@code {
    private bool _showHidden;
    private string _sortOrder = "name";
}
```

### Event Callbacks

| Callback | Raised When |
|----------|-------------|
| `OpenChanged` | Two-way binding for menu open state |
| `OnOpenChanged` | Open state changes |
| `MenuItem.OnSelect` | Item is activated (click, Enter, Space) |
| `MenuCheckboxItem.CheckedChanged` | Checkbox state toggles |
| `MenuRadioGroup.ValueChanged` | Radio selection changes |
| `MenuSub.OpenChanged` | Sub-menu open state changes |

### Keyboard Interactions (WAI-ARIA APG: Menu / Menu Bar)

**When trigger is focused:**

| Key | Behavior |
|-----|----------|
| `Enter`, `Space`, `ArrowDown` | Open menu, focus first item |
| `ArrowUp` | Open menu, focus last item |

**When menu content is focused:**

| Key | Behavior |
|-----|----------|
| `ArrowDown` | Focus next item (skip disabled, optionally wrap) |
| `ArrowUp` | Focus previous item (skip disabled, optionally wrap) |
| `Home` | Focus first item |
| `End` | Focus last item |
| `Enter`, `Space` | Activate focused item. For checkbox items, toggle. For sub-trigger, open sub-menu. |
| `ArrowRight` (LTR) | If on sub-trigger, open sub-menu and focus first item |
| `ArrowLeft` (LTR) | If inside sub-menu, close sub-menu, focus sub-trigger |
| `Escape` | Close current menu level. If in sub-menu, close sub-menu only. If in root menu, close and focus trigger. |
| Type-ahead characters | Focus first item matching typed string (350ms reset) |

**Sub-menu hover behavior:** Sub-menus open on pointer enter with a 100ms delay and close on pointer leave with a 300ms delay. This is handled entirely in JS to avoid SignalR round-trips in Server mode.

Implementation: The roving focus module in `keyboard.js` handles arrow navigation, type-ahead, and focus management. Sub-menu open/close timing is in `interaction.js`. The JS module reports actions (item selected, sub-menu opened/closed, menu dismissed) back to .NET.

### ARIA Attributes

| Part | Element | Attributes |
|------|---------|------------|
| `MenuTrigger` | `<button>` | `aria-haspopup="menu"`, `aria-expanded="{IsOpen}"`, `aria-controls="{ContentId}"`, `id="{TriggerId}"` |
| `MenuContent` | `<div>` | `role="menu"`, `id="{ContentId}"`, `aria-labelledby="{TriggerId}"`, `tabindex="-1"` |
| `MenuItem` | `<div>` | `role="menuitem"`, `tabindex="-1"`, `aria-disabled="{Disabled}"`, `data-highlighted="{IsHighlighted}"` |
| `MenuCheckboxItem` | `<div>` | `role="menuitemcheckbox"`, `aria-checked="{Checked}"`, `tabindex="-1"`, `aria-disabled="{Disabled}"` |
| `MenuRadioGroup` | `<div>` | `role="group"` |
| `MenuRadioItem` | `<div>` | `role="menuitemradio"`, `aria-checked="{IsSelected}"`, `tabindex="-1"`, `aria-disabled="{Disabled}"` |
| `MenuSubTrigger` | `<div>` | `role="menuitem"`, `aria-haspopup="menu"`, `aria-expanded="{SubIsOpen}"`, `aria-controls="{SubContentId}"`, `data-highlighted="{IsHighlighted}"`, `tabindex="-1"` |
| `MenuSubContent` | `<div>` | `role="menu"`, `id="{SubContentId}"`, `aria-labelledby="{SubTriggerId}"`, `tabindex="-1"` |
| `MenuLabel` | `<div>` | `id="{LabelId}"` (referenced by group's `aria-labelledby`) |
| `MenuSeparator` | `<div>` | `role="separator"`, `aria-hidden="true"` |
| `MenuItemIndicator` | `<span>` | `aria-hidden="true"` (decorative, state conveyed by parent's `aria-checked`) |

### SSR Fallback

Menus have no native HTML equivalent that provides comparable functionality. In static SSR:
- `MenuTrigger` renders as `<button disabled>` with ARIA attributes.
- `MenuContent` renders with `hidden` attribute and `data-state="closed"`.
- The menu is inert. For pages that must function in static SSR, consumers should provide an alternative UI (e.g., a page with links instead of a dropdown menu).

```html
<!-- Static SSR output -->
<button aria-haspopup="menu" aria-expanded="false" disabled>Options</button>
<div role="menu" hidden data-state="closed">
  <div role="menuitem">Edit</div>
  <div role="menuitem">Duplicate</div>
  <!-- ... -->
</div>
```

---

## 5. Tabs

### Component Parts

| Part | Default Element | Purpose |
|------|----------------|---------|
| `TabsRoot` | `<div>` | State container |
| `TabsList` | `<div>` | Tab strip container |
| `TabsTrigger` | `<button>` | Individual tab button |
| `TabsContent` | `<div>` | Panel associated with a tab |

### Context Type

```csharp
namespace BlazingSpire.Primitives.Tabs;

public sealed class TabsContext
{
    public required string TabsId { get; init; }
    public required string? ActiveValue { get; set; }
    public required Direction Dir { get; init; }
    public required Orientation Orientation { get; init; }
    public required ActivationMode ActivationMode { get; init; }
    public required Func<string, Task> ActivateTab { get; init; }
}

public enum ActivationMode
{
    /// <summary>Tab activates on focus (arrow key navigation).</summary>
    Automatic,

    /// <summary>Tab activates only on explicit selection (Enter/Space).</summary>
    Manual
}

public enum Orientation
{
    Horizontal,
    Vertical
}

public enum Direction
{
    Ltr,
    Rtl
}
```

### Parameter Signatures

```csharp
// ── TabsRoot ────────────────────────────────────────────────
public partial class TabsRoot : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Controlled active tab value.</summary>
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Uncontrolled default active tab.</summary>
    [Parameter] public string? DefaultValue { get; set; }

    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }

    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;
    [Parameter] public Direction Dir { get; set; } = Direction.Ltr;

    /// <summary>
    /// Automatic: tab activates on focus (arrow keys immediately switch).
    /// Manual: tab only activates on Enter/Space after focus.
    /// </summary>
    [Parameter] public ActivationMode ActivationMode { get; set; } = ActivationMode.Automatic;

    /// <summary>If true, arrow key navigation wraps from last to first.</summary>
    [Parameter] public bool Loop { get; set; } = true;

    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── TabsList ────────────────────────────────────────────────
public partial class TabsList : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>If true, wraps focus from last trigger to first and vice versa.</summary>
    [Parameter] public bool Loop { get; set; } = true;

    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── TabsTrigger ─────────────────────────────────────────────
public partial class TabsTrigger : ComponentBase
{
    /// <summary>Unique value identifying this tab. Must match a TabsContent.Value.</summary>
    [Parameter, EditorRequired] public string Value { get; set; } = default!;

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}

// ── TabsContent ─────────────────────────────────────────────
public partial class TabsContent : ComponentBase
{
    /// <summary>Value matching the associated TabsTrigger.Value.</summary>
    [Parameter, EditorRequired] public string Value { get; set; } = default!;

    /// <summary>
    /// If true, content stays in DOM when inactive (hidden with display:none).
    /// If false (default), content is removed from render tree when inactive.
    /// </summary>
    [Parameter] public bool ForceMount { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool AsChild { get; set; }
    [Parameter] public RenderFragment<IReadOnlyDictionary<string, object>>? AsChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
```

### Parent-Child Wiring

```
TabsRoot
  └─ CascadingValue<TabsContext> (IsFixed="false")
       ├─ TabsList
       │    ├─ TabsTrigger (value="account")  [CascadingParameter]
       │    ├─ TabsTrigger (value="password") [CascadingParameter]
       │    └─ TabsTrigger (value="settings") [CascadingParameter]
       ├─ TabsContent (value="account")  [CascadingParameter]
       ├─ TabsContent (value="password") [CascadingParameter]
       └─ TabsContent (value="settings") [CascadingParameter]
```

Tabs uses Tier 1 (`CascadingValue<TabsContext>` with `IsFixed="false"`). Typical tab count is under 20. The context is replaced when the active tab changes, causing all parts to re-evaluate — but `TabsTrigger` and `TabsContent` each only check whether their own `Value` matches `ActiveValue`, and `ShouldRender()` can short-circuit if their active state did not change.

### Two-Way Binding

```razor
<TabsRoot @bind-Value="_activeTab" DefaultValue="account">
    <TabsList>
        <TabsTrigger Value="account">Account</TabsTrigger>
        <TabsTrigger Value="password">Password</TabsTrigger>
    </TabsList>
    <TabsContent Value="account">
        <p>Account settings...</p>
    </TabsContent>
    <TabsContent Value="password">
        <p>Change your password...</p>
    </TabsContent>
</TabsRoot>

@code {
    private string? _activeTab;
}
```

### Event Callbacks

| Callback | Raised When |
|----------|-------------|
| `ValueChanged` | Two-way binding notification |
| `OnValueChanged` | Active tab changes (both modes) |

### Keyboard Interactions (WAI-ARIA APG: Tabs)

**Horizontal orientation (default):**

| Key | Behavior |
|-----|----------|
| `ArrowRight` | Focus next tab (wraps if `Loop`). In Automatic mode, also activates. |
| `ArrowLeft` | Focus previous tab (wraps if `Loop`). In Automatic mode, also activates. |
| `Home` | Focus first non-disabled tab |
| `End` | Focus last non-disabled tab |
| `Enter`, `Space` | In Manual mode, activates the focused tab |
| `Tab` | Moves focus out of the tab list into the active panel |

**Vertical orientation:**

| Key | Behavior |
|-----|----------|
| `ArrowDown` | Focus next tab (replaces ArrowRight) |
| `ArrowUp` | Focus previous tab (replaces ArrowLeft) |
| All other keys | Same as horizontal |

**RTL direction:** ArrowRight and ArrowLeft are swapped.

Implementation: Tab keyboard navigation uses roving tabindex. Only the active (or focused) tab has `tabindex="0"`; all others have `tabindex="-1"`. This is managed entirely in C# — no JS needed for basic tab switching. The `TabsList` handles `@onkeydown` and routes arrow keys to the appropriate `TabsTrigger` by calling `.FocusAsync()` on the `ElementReference`.

Note: `FocusAsync()` on `ElementReference` requires interactive mode. In static SSR, keyboard navigation is not available, but the tabs still function via click (enhanced with standard Blazor navigation if applicable).

### ARIA Attributes

| Part | Element | Attributes |
|------|---------|------------|
| `TabsRoot` | `<div>` | `data-orientation="{Orientation}"`, `dir="{Dir}"` |
| `TabsList` | `<div>` | `role="tablist"`, `aria-orientation="{horizontal\|vertical}"` |
| `TabsTrigger` | `<button>` | `role="tab"`, `aria-selected="{IsActive}"`, `aria-controls="{PanelId}"`, `id="{TriggerId}"`, `tabindex="{IsActive ? 0 : -1}"`, `aria-disabled="{Disabled}"`, `data-state="{active\|inactive}"` |
| `TabsContent` | `<div>` | `role="tabpanel"`, `aria-labelledby="{TriggerId}"`, `id="{PanelId}"`, `tabindex="0"`, `data-state="{active\|inactive}"`, `hidden` (when inactive and ForceMount) |

### SSR Fallback

Tabs can function in static SSR with a clever approach: use URL fragments or query parameters to determine the active tab.

```html
<!-- Static SSR output: all panels present, CSS hides inactive ones -->
<div data-orientation="horizontal">
  <div role="tablist" aria-orientation="horizontal">
    <a href="?tab=account" role="tab" aria-selected="true" id="tab-1-account"
       aria-controls="panel-1-account" tabindex="0" data-state="active">Account</a>
    <a href="?tab=password" role="tab" aria-selected="false" id="tab-1-password"
       aria-controls="panel-1-password" tabindex="-1" data-state="inactive">Password</a>
  </div>
  <div role="tabpanel" aria-labelledby="tab-1-account" id="panel-1-account"
       tabindex="0" data-state="active">
    <p>Account settings...</p>
  </div>
  <div role="tabpanel" aria-labelledby="tab-1-password" id="panel-1-password"
       tabindex="0" data-state="inactive" hidden>
    <p>Change your password...</p>
  </div>
</div>
```

In static SSR mode, `TabsTrigger` renders as an `<a>` tag instead of a `<button>`, linking to the same page with a query parameter (e.g., `?tab=password`). The server reads the query parameter and sets the active tab accordingly. This provides functional tab switching without JS via full-page navigation.

---

## Consumer Usage Examples

These examples show how a consumer would use the headless primitives to create styled components in their copy-pasted component library.

### Styled Dialog Component

```razor
@* Components/UI/Dialog.razor *@

@* Re-export primitives with Tailwind styling *@

<DialogRoot @bind-IsOpen="IsOpen" Modal="Modal">
    @ChildContent
</DialogRoot>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public bool Modal { get; set; } = true;
}
```

```razor
@* Components/UI/DialogContent.razor — styled wrapper *@
@using BlazingSpire.Primitives.Dialog

<Primitives.DialogPortal>
    <Primitives.DialogOverlay
        class="@Cn("fixed inset-0 z-50 bg-black/80 data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0")" />
    <Primitives.DialogContent
        class="@Cn("fixed left-[50%] top-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 border bg-background p-6 shadow-lg duration-200 data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[state=closed]:slide-out-to-left-1/2 data-[state=closed]:slide-out-to-top-[48%] data-[state=open]:slide-in-from-left-1/2 data-[state=open]:slide-in-from-top-[48%] sm:rounded-lg",
            Class)">
        @ChildContent
        <Primitives.DialogClose
            class="absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground">
            <XIcon class="h-4 w-4" />
            <span class="sr-only">Close</span>
        </Primitives.DialogClose>
    </Primitives.DialogContent>
</Primitives.DialogPortal>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
}
```

```razor
@* Usage in a page *@
<Dialog @bind-IsOpen="_deleteConfirmOpen">
    <DialogTrigger AsChild="true">
        <AsChildContent Context="attrs">
            <Button @attributes="attrs" Variant="ButtonVariant.Destructive">
                Delete Account
            </Button>
        </AsChildContent>
    </DialogTrigger>
    <StyledDialogContent>
        <DialogTitle>Are you absolutely sure?</DialogTitle>
        <DialogDescription>
            This action cannot be undone. This will permanently delete your
            account and remove your data from our servers.
        </DialogDescription>
        <div class="flex justify-end gap-2">
            <DialogClose AsChild="true">
                <AsChildContent Context="attrs">
                    <Button @attributes="attrs" Variant="ButtonVariant.Outline">Cancel</Button>
                </AsChildContent>
            </DialogClose>
            <Button Variant="ButtonVariant.Destructive" OnClick="HandleDelete">
                Yes, delete account
            </Button>
        </div>
    </StyledDialogContent>
</Dialog>

@code {
    private bool _deleteConfirmOpen;

    private async Task HandleDelete()
    {
        await AccountService.DeleteAsync();
        _deleteConfirmOpen = false;
    }
}
```

### Styled Select Component

```razor
@* Components/UI/Select.razor *@
@using BlazingSpire.Primitives.Select

<SelectRoot @bind-Value="Value" Disabled="Disabled" Name="Name" Required="Required">
    <SelectTrigger
        class="@Cn("flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&>span]:line-clamp-1",
            TriggerClass)">
        <SelectValue Placeholder="@Placeholder" />
        <ChevronDownIcon class="h-4 w-4 opacity-50" />
    </SelectTrigger>
    <SelectPortal>
        <SelectContent
            class="@Cn("relative z-50 max-h-96 min-w-[8rem] overflow-hidden rounded-md border bg-popover text-popover-foreground shadow-md data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95",
                ContentClass)"
            Position="popper"
            SideOffset="4">
            <SelectViewport class="p-1">
                @ChildContent
            </SelectViewport>
        </SelectContent>
    </SelectPortal>
</SelectRoot>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public string? TriggerClass { get; set; }
    [Parameter] public string? ContentClass { get; set; }
}
```

```razor
@* Components/UI/SelectItem.razor *@
@using BlazingSpire.Primitives.Select

<Primitives.SelectItem
    Value="@Value"
    Disabled="Disabled"
    class="@Cn("relative flex w-full cursor-default select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none focus:bg-accent focus:text-accent-foreground data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
        Class)">
    <span class="absolute left-2 flex h-3.5 w-3.5 items-center justify-center">
        <SelectItemIndicator>
            <CheckIcon class="h-4 w-4" />
        </SelectItemIndicator>
    </span>
    <SelectItemText>@ChildContent</SelectItemText>
</Primitives.SelectItem>

@code {
    [Parameter, EditorRequired] public string Value { get; set; } = default!;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
}
```

```razor
@* Usage in a page *@
<Select @bind-Value="_theme" Placeholder="Select a theme">
    <SelectGroup>
        <SelectLabel>Light Themes</SelectLabel>
        <StyledSelectItem Value="light">Light</StyledSelectItem>
        <StyledSelectItem Value="zinc">Zinc</StyledSelectItem>
        <StyledSelectItem Value="slate">Slate</StyledSelectItem>
    </SelectGroup>
    <SelectSeparator />
    <SelectGroup>
        <SelectLabel>Dark Themes</SelectLabel>
        <StyledSelectItem Value="dark">Dark</StyledSelectItem>
        <StyledSelectItem Value="nord">Nord</StyledSelectItem>
    </SelectGroup>
</Select>

@code {
    private string? _theme;
}
```

### Styled Tabs Component

```razor
@* Components/UI/Tabs.razor *@
@using BlazingSpire.Primitives.Tabs

<TabsRoot @bind-Value="Value" DefaultValue="@DefaultValue"
          Orientation="Orientation" ActivationMode="ActivationMode"
          class="@Cn(Class)">
    @ChildContent
</TabsRoot>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string? DefaultValue { get; set; }
    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;
    [Parameter] public ActivationMode ActivationMode { get; set; } = ActivationMode.Automatic;
    [Parameter] public string? Class { get; set; }
}
```

```razor
@* Usage *@
<Tabs DefaultValue="account" class="w-[400px]">
    <StyledTabsList class="grid w-full grid-cols-2">
        <StyledTabsTrigger Value="account">Account</StyledTabsTrigger>
        <StyledTabsTrigger Value="password">Password</StyledTabsTrigger>
    </StyledTabsList>
    <StyledTabsContent Value="account">
        <Card>
            <CardHeader>
                <CardTitle>Account</CardTitle>
                <CardDescription>
                    Make changes to your account here. Click save when you're done.
                </CardDescription>
            </CardHeader>
            <CardContent class="space-y-2">
                <div class="space-y-1">
                    <Label For="name">Name</Label>
                    <Input Id="name" @bind-Value="_name" />
                </div>
            </CardContent>
            <CardFooter>
                <Button>Save changes</Button>
            </CardFooter>
        </Card>
    </StyledTabsContent>
    <StyledTabsContent Value="password">
        @* Password form... *@
    </StyledTabsContent>
</Tabs>

@code {
    private string _name = "Pedro Duarte";
}
```

---

## Cross-Cutting Design Decisions

### ID Generation

All primitives need stable, unique IDs for ARIA linkage (`aria-controls`, `aria-labelledby`, etc.). We use a deterministic prefix + incremental counter:

```csharp
internal static class BlazingSpireId
{
    private static int _counter;

    public static string New(string prefix) =>
        $"bs-{prefix}-{Interlocked.Increment(ref _counter)}";
}
```

This is trim/AOT-safe, deterministic within a session, and avoids `Guid.NewGuid()` entropy concerns on SSR where deterministic output aids caching.

### Controlled vs. Uncontrolled

Every primitive with user-facing state supports both patterns:

- **Controlled:** Consumer passes `Value` + `ValueChanged`. The primitive never mutates `Value` itself; it always calls `ValueChanged` and waits for the consumer to update.
- **Uncontrolled:** Consumer passes `DefaultValue` (or nothing). The primitive manages state internally.

Detection:

```csharp
private bool IsControlled => ValueChanged.HasDelegate;

private async Task SetValue(string? newValue)
{
    if (IsControlled)
    {
        // Controlled: notify consumer, don't update internal state
        await ValueChanged.InvokeAsync(newValue);
    }
    else
    {
        // Uncontrolled: update internal state
        _internalValue = newValue;
    }

    // Always fire the general callback
    await OnValueChanged.InvokeAsync(newValue);
    StateHasChanged();
}
```

### Data Attributes for Styling

All primitives emit `data-state` and `data-*` attributes that Tailwind CSS can target:

| Attribute | Values | Usage |
|-----------|--------|-------|
| `data-state` | `"open"` / `"closed"` / `"active"` / `"inactive"` | Primary state indicator |
| `data-highlighted` | `""` (present/absent) | Currently highlighted item |
| `data-disabled` | `""` (present/absent) | Disabled items |
| `data-orientation` | `"horizontal"` / `"vertical"` | Tabs, Separator |
| `data-side` | `"top"` / `"bottom"` / `"left"` / `"right"` | Floating content position |
| `data-align` | `"start"` / `"center"` / `"end"` | Floating content alignment |

Tailwind CSS v4 can style these via `data-[state=open]:animate-in`, `data-[highlighted]:bg-accent`, etc.

### Floating UI Integration

`SelectContent`, `ComboboxContent`, `MenuContent`, and `MenuSubContent` all use Floating UI for positioning. The shared integration is:

```csharp
// In OnAfterRenderAsync(firstRender: true)
_positioningCleanup = await PositioningModule.InvokeAsync<IJSObjectReference>(
    "setupPositioning",
    _anchorRef,     // ElementReference to trigger/anchor
    _floatingRef,   // ElementReference to floating content
    new {
        placement = $"{Side.ToJs()}-{Align.ToJs()}",
        offset = SideOffset,
        flip = true,
        shift = true,
        autoUpdate = true
    });

// In DisposeAsync
await _positioningCleanup.InvokeVoidAsync("dispose");
```

### Animation Lifecycle

Opening/closing animations require the element to remain in the DOM during the exit animation. The primitives support this via `data-state` transitions:

1. On open: element is added to DOM (or `hidden` removed) with `data-state="open"`.
2. On close: `data-state` changes to `"closed"`. Element remains in DOM.
3. After CSS animation completes: JS `animationend` event fires, element is removed from DOM.

The JS interop layer listens for `animationend` on elements with `data-state="closed"` and calls back to .NET to complete the removal. This avoids premature removal that would cut animations short.

```csharp
// In the Content component
internal async Task StartClosing()
{
    _state = "closed";
    StateHasChanged();
    // JS will call OnAnimationComplete via DotNetObjectReference
    // when animationend fires (or immediately if no animation)
}

[JSInvokable]
public void OnAnimationComplete()
{
    _isRendered = false;
    StateHasChanged();
}
```

### Form Integration

`Select` and `Combobox` render a hidden `<input type="hidden">` (or native `<select>` in SSR) when `Name` is set, enabling standard HTML form submission without JS:

```csharp
// Inside SelectRoot render tree, when Name is set:
builder.OpenElement(seq, "input");
builder.AddAttribute(seq + 1, "type", "hidden");
builder.AddAttribute(seq + 2, "name", Name);
builder.AddAttribute(seq + 3, "value", SelectedValue ?? "");
if (Required) builder.AddAttribute(seq + 4, "required", true);
builder.CloseElement();
```

---

## Summary of Wiring Patterns by Primitive

| Primitive | Cascading Strategy | Child Count Risk | JS Modules Required |
|-----------|--------------------|------------------|---------------------|
| **Dialog** | Tier 1: `CascadingValue<DialogContext>` `IsFixed="false"` | Low (5-8 parts) | focus.js, interaction.js, scroll.js, portal.js |
| **Select** | Tier 2: `CascadingValue<SelectRoot>` `IsFixed="true"` | High (100+ items) | keyboard.js, positioning.js, portal.js, interaction.js |
| **Combobox** | Tier 2: `CascadingValue<ComboboxRoot>` `IsFixed="true"` | High (100+ items, filtered) | keyboard.js, positioning.js, portal.js, interaction.js |
| **Menu** | Tier 2: `CascadingValue<MenuRoot>` `IsFixed="true"` + nested `CascadingValue<MenuSub>` | Medium (10-30 items + sub-menus) | keyboard.js, positioning.js, portal.js, interaction.js |
| **Tabs** | Tier 1: `CascadingValue<TabsContext>` `IsFixed="false"` | Low (2-20 tabs) | None (pure C# keyboard nav via roving tabindex) |
