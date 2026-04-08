# Styled Component Patterns

How the styled (copy-paste) component layer wraps headless primitives in BlazingSpire.

---

## The Two-Layer Model

BlazingSpire uses a two-layer architecture:

```
┌─────────────────────────────────────────┐
│  Styled Components (copy-paste)          │  ← User-owned .razor/.razor.cs files
│  Button, Dialog, Select, Tabs, etc.      │  ← Tailwind CSS classes, full source
├─────────────────────────────────────────┤
│  Headless Primitives (NuGet package)     │  ← BlazingSpire.Primitives
│  Focus trap, keyboard nav, ARIA,         │  ← Immutable, versioned dependency
│  positioning, portals, scroll lock       │
└─────────────────────────────────────────┘
```

**Primitives** ship as a NuGet package (`BlazingSpire.Primitives`). They handle behavior, accessibility, and keyboard interactions. They emit no visual styling — only semantic HTML, ARIA attributes, and `data-state` attributes.

**Styled components** are copied into the consumer's project by the CLI (`dotnet blazingspire add button`). They wrap primitives with Tailwind utility classes, define variant systems, and are fully editable. The consumer owns this code.

The separation is strict: primitives never reference Tailwind classes; styled components never implement ARIA logic or keyboard handling.

### When There Is No Primitive

Simple visual-only components (Badge, Card, CardHeader, etc.) have no corresponding primitive. They are pure styled components — a thin wrapper around a native HTML element with Tailwind classes. The styled component *is* the entire component.

### When There Is a Primitive

Interactive components (Dialog, Select, Tabs, etc.) always wrap a primitive. The styled component:
1. Renders the primitive as its root element
2. Passes `Class` and `AdditionalAttributes` through to the primitive
3. Supplies default Tailwind classes via the primitive's `class` attribute
4. Uses `data-state` attributes emitted by primitives to drive CSS transitions

---

## File Structure Convention

Every styled component is a `.razor` + `.razor.cs` pair in the `Components/UI/` directory:

```
Components/
  UI/
    Button.razor          ← Markup template
    Button.razor.cs       ← Parameters, variant logic, class composition
    Badge.razor
    Badge.razor.cs
    Dialog.razor          ← Wraps DialogRoot + styled sub-parts
    Dialog.razor.cs
    DialogContent.razor
    DialogContent.razor.cs
    DialogOverlay.razor
    DialogOverlay.razor.cs
    ...
```

**Naming rules:**
- Component name matches the primitive it wraps (e.g., `Dialog` wraps `Primitives.DialogRoot`)
- Multi-part primitives get one styled component per part (`DialogContent`, `DialogOverlay`, `DialogTitle`, etc.)
- Names are PascalCase, no prefix or suffix
- All components share the namespace `{ProjectName}.Components.UI`

**No `.razor.css` files.** Blazor CSS isolation is incompatible with Tailwind (see Anti-Patterns).

---

## Base Component Pattern

Every styled component shares a minimal skeleton of three parameters:

```csharp
public partial class ExampleComponent
{
    /// <summary>The component content.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes appended to the root element.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Captures any HTML attributes not explicitly defined as parameters.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

The corresponding `.razor` template follows the pattern:

```razor
@namespace BlazingSpire.Demo.Components.UI

<div class="@($"base-classes-here {Class}")"
     @attributes="AdditionalAttributes">
    @ChildContent
</div>
```

**Key points:**
- `Class` is always a `string?`, never `CssClass` or a custom type. Consumers pass raw Tailwind strings.
- `AdditionalAttributes` is splatted via `@attributes` on the root element so consumers can pass `id`, `data-*`, `aria-*`, or any other HTML attribute.
- `ChildContent` is the standard Blazor slot for nested content.
- The `.razor.cs` file uses `partial class` — no inheritance from a custom base class. Components inherit from `ComponentBase` implicitly.

### Minimal Example: CardFooter

**CardFooter.razor**
```razor
@namespace BlazingSpire.Demo.Components.UI

<div class="@($"flex items-center p-6 pt-0 {Class}")">
    @ChildContent
</div>
```

**CardFooter.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class CardFooter
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
}
```

This is the simplest form — no variants, no primitive, just a styled `<div>`.

---

## Variant System

Components with visual variants use the enum + `switch` expression pattern. The production-ready path uses `FrozenDictionary` for zero-lookup-overhead at render time; the POC currently uses `switch` expressions which are equally fast for small enum cardinality.

### Pattern: Enum Definition + Class Mapping

```csharp
public partial class Button
{
    [Parameter] public ButtonVariant Variant { get; set; } = ButtonVariant.Default;
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Default;

    // ── Variant → CSS mapping ──────────────────────────────
    private string VariantClass => Variant switch
    {
        ButtonVariant.Default     => "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm",
        ButtonVariant.Destructive => "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        ButtonVariant.Outline     => "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Secondary   => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ButtonVariant.Ghost       => "hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Link        => "text-primary underline-offset-4 hover:underline",
        _ => ""
    };

    private string SizeClass => Size switch
    {
        ButtonSize.Default => "h-10 px-4 py-2",
        ButtonSize.Sm      => "h-9 px-3",
        ButtonSize.Lg      => "h-11 px-8",
        ButtonSize.Icon    => "h-10 w-10",
        _ => ""
    };

    public enum ButtonVariant { Default, Destructive, Outline, Secondary, Ghost, Link }
    public enum ButtonSize { Default, Sm, Lg, Icon }
}
```

### Production Upgrade: FrozenDictionary

For components with many variants or compound variant combinations, use `FrozenDictionary` for O(1) lookup:

```csharp
using System.Collections.Frozen;

private static readonly FrozenDictionary<ButtonVariant, string> VariantClasses =
    new Dictionary<ButtonVariant, string>
    {
        [ButtonVariant.Default]     = "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm",
        [ButtonVariant.Destructive] = "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        [ButtonVariant.Outline]     = "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        [ButtonVariant.Secondary]   = "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        [ButtonVariant.Ghost]       = "hover:bg-accent hover:text-accent-foreground",
        [ButtonVariant.Link]        = "text-primary underline-offset-4 hover:underline",
    }.ToFrozenDictionary();

private static readonly FrozenDictionary<ButtonSize, string> SizeClasses =
    new Dictionary<ButtonSize, string>
    {
        [ButtonSize.Default] = "h-10 px-4 py-2",
        [ButtonSize.Sm]      = "h-9 px-3",
        [ButtonSize.Lg]      = "h-11 px-8",
        [ButtonSize.Icon]    = "h-10 w-10",
    }.ToFrozenDictionary();
```

**When to use which:**
- `switch` expression: components with < 10 variant values (Button, Badge). Simpler, equally fast.
- `FrozenDictionary`: components with compound variants or many values. Static allocation, dictionary lookup.

### Enum Placement

Variant enums are nested inside the component class. This keeps the namespace clean and makes the relationship explicit:

```razor
<Button Variant="Button.ButtonVariant.Outline" />
```

Or with a `using static`:
```razor
@using static BlazingSpire.Demo.Components.UI.Button
<Button Variant="ButtonVariant.Outline" />
```

---

## Class Merging with Cn()

The `Cn()` utility is the Blazor equivalent of shadcn/ui's `cn()` function. It joins class strings, filters nulls, and passes the result through `TwMerge` to resolve Tailwind conflicts (e.g., `bg-red-500` overrides `bg-blue-500`).

### Signature

```csharp
// C# 13 params ReadOnlySpan<string?> avoids array allocation
public static string Cn(params ReadOnlySpan<string?> inputs)
{
    return TwMerge.Merge(
        string.Join(" ", inputs.ToArray().Where(s => !string.IsNullOrWhiteSpace(s)))
    );
}
```

### Usage in Styled Components

```csharp
private string Classes => Cn(
    "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium",
    "ring-offset-background transition-colors",
    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
    "disabled:pointer-events-none disabled:opacity-50",
    VariantClasses[Variant],
    SizeClasses[Size],
    Class   // consumer override — last wins via TwMerge
);
```

### Performance Rules

`Cn()` is a hot path — called every render of every component.

1. **Cache in `OnParametersSet`.** Do not call `Cn()` in the render tree. Compute the merged class string when parameters change and store it in a field:

   ```csharp
   private string _classes = "";

   protected override void OnParametersSet()
   {
       _classes = Cn("base-classes", VariantClasses[Variant], SizeClasses[Size], Class);
   }
   ```

2. **Pre-compute variant dictionaries as `static readonly`.** The variant-to-string mappings never change at runtime.

3. **Never build class strings at render time** using string concatenation in the `.razor` template. The POC currently uses `$"..."` interpolation in templates for simplicity — the production path should move to `OnParametersSet` caching.

4. **TailwindMerge.NET has an internal LRU cache.** Repeated identical inputs are near-free. But avoid creating unique strings on every render (e.g., by including volatile data in the class string).

---

## Wrapping a Primitive

### Example 1: Button (Simplest — No Primitive)

Button is currently a pure styled component with no headless primitive. It renders a native `<button>` directly.

**Button.razor**
```razor
@namespace BlazingSpire.Demo.Components.UI

<button class="@Classes"
        type="@Type"
        disabled="@Disabled"
        @attributes="AdditionalAttributes"
        @onclick="OnClick">
    @ChildContent
</button>
```

**Button.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingSpire.Demo.Components.UI;

public partial class Button
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public ButtonVariant Variant { get; set; } = ButtonVariant.Default;
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Default;
    [Parameter] public string Type { get; set; } = "button";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes =>
        $"inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium " +
        $"ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 " +
        $"focus-visible:ring-ring focus-visible:ring-offset-2 " +
        $"disabled:pointer-events-none disabled:opacity-50 " +
        $"{VariantClass} {SizeClass} {Class}";

    private string VariantClass => Variant switch
    {
        ButtonVariant.Default     => "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm",
        ButtonVariant.Destructive => "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        ButtonVariant.Outline     => "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Secondary   => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ButtonVariant.Ghost       => "hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Link        => "text-primary underline-offset-4 hover:underline",
        _ => ""
    };

    private string SizeClass => Size switch
    {
        ButtonSize.Default => "h-10 px-4 py-2",
        ButtonSize.Sm      => "h-9 px-3",
        ButtonSize.Lg      => "h-11 px-8",
        ButtonSize.Icon    => "h-10 w-10",
        _ => ""
    };

    public enum ButtonVariant { Default, Destructive, Outline, Secondary, Ghost, Link }
    public enum ButtonSize { Default, Sm, Lg, Icon }
}
```

**What to notice:** Button is the reference pattern for any component that does not need a primitive (no ARIA state management, no keyboard navigation beyond native `<button>` behavior). The styled component is self-contained.

### Example 2: Dialog (Wrapping a Complex Primitive)

Dialog is a multi-part primitive with state management (open/close), focus trapping, scroll locking, and portal rendering. The styled layer wraps each primitive part with its own styled component file.

**Dialog.razor** — wraps `Primitives.DialogRoot`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Dialog

<DialogRoot @bind-IsOpen="IsOpen"
            IsOpenChanged="IsOpenChanged"
            OnOpenChanged="OnOpenChanged"
            Modal="@Modal">
    @ChildContent
</DialogRoot>
```

**Dialog.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class Dialog
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }
    [Parameter] public bool Modal { get; set; } = true;
}
```

**DialogOverlay.razor** — wraps `Primitives.DialogOverlay`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Dialog

<Primitives.Dialog.DialogOverlay
    class="@Classes"
    @attributes="AdditionalAttributes" />
```

**DialogOverlay.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogOverlay
{
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => Cn(
        "fixed inset-0 z-50 bg-black/80",
        // Animation via data-state attribute from primitive
        "data-[state=open]:animate-in data-[state=open]:fade-in-0",
        "data-[state=closed]:animate-out data-[state=closed]:fade-out-0",
        Class
    );
}
```

**DialogContent.razor** — wraps `Primitives.DialogContent`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Dialog

<DialogPortal>
    <DialogOverlay />
    <Primitives.Dialog.DialogContent
        class="@Classes"
        @attributes="AdditionalAttributes">
        @ChildContent
        <DialogClose class="absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background
                            transition-opacity hover:opacity-100
                            focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2
                            disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                 fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M18 6 6 18"/><path d="m6 6 12 12"/>
            </svg>
            <span class="sr-only">Close</span>
        </DialogClose>
    </Primitives.Dialog.DialogContent>
</DialogPortal>
```

**DialogContent.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogContent
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => Cn(
        "fixed left-1/2 top-1/2 z-50 grid w-full max-w-lg -translate-x-1/2 -translate-y-1/2",
        "gap-4 border bg-background p-6 shadow-lg sm:rounded-lg",
        // Enter/exit animation driven by data-state from primitive
        "data-[state=open]:animate-in data-[state=open]:fade-in-0 data-[state=open]:zoom-in-95",
        "data-[state=open]:slide-in-from-left-1/2 data-[state=open]:slide-in-from-top-[48%]",
        "data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95",
        "data-[state=closed]:slide-out-to-left-1/2 data-[state=closed]:slide-out-to-top-[48%]",
        "duration-200",
        Class
    );
}
```

**DialogHeader.razor** / **DialogFooter.razor** — pure styled, no primitive
```razor
@* DialogHeader.razor *@
@namespace BlazingSpire.Demo.Components.UI

<div class="@($"flex flex-col space-y-1.5 text-center sm:text-left {Class}")">
    @ChildContent
</div>
```

**DialogTitle.razor** — wraps `Primitives.DialogTitle`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Dialog

<Primitives.Dialog.DialogTitle
    class="@($"text-lg font-semibold leading-none tracking-tight {Class}")"
    @attributes="AdditionalAttributes">
    @ChildContent
</Primitives.Dialog.DialogTitle>
```

**DialogDescription.razor** — wraps `Primitives.DialogDescription`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Dialog

<Primitives.Dialog.DialogDescription
    class="@($"text-sm text-muted-foreground {Class}")"
    @attributes="AdditionalAttributes">
    @ChildContent
</Primitives.Dialog.DialogDescription>
```

**Consumer usage:**
```razor
<Dialog @bind-IsOpen="_open">
    <DialogTrigger AsChild="true">
        <AsChildContent Context="attrs">
            <Button @attributes="attrs">Edit Profile</Button>
        </AsChildContent>
    </DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Edit Profile</DialogTitle>
            <DialogDescription>Make changes to your profile here.</DialogDescription>
        </DialogHeader>
        <div class="grid gap-4 py-4">
            @* form fields *@
        </div>
        <DialogFooter>
            <Button Type="submit">Save changes</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>

@code {
    private bool _open;
}
```

**What to notice:**
- The styled `Dialog` passes through all primitive parameters (`@bind-IsOpen`, `Modal`).
- `DialogContent` bundles the overlay, close button, and portal — the consumer writes less markup than they would with raw primitives.
- Animations are driven entirely by `data-[state=open]` / `data-[state=closed]` CSS selectors targeting the `data-state` attribute the primitive emits.
- `DialogHeader` and `DialogFooter` have no primitive counterpart — they are layout helpers.

### Example 3: Select (Wrapping a Collection Primitive)

Select uses the Tier 2 cascading pattern: `SelectRoot` cascades itself with `IsFixed="true"`. Items implement `ShouldRender()` to avoid O(n) re-renders when the highlighted index changes.

**Select.razor** — wraps `Primitives.SelectRoot`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Select

<SelectRoot @bind-Value="Value"
            ValueChanged="ValueChanged"
            OnValueChanged="OnValueChanged"
            @bind-Open="Open"
            OpenChanged="OpenChanged"
            Disabled="@Disabled"
            Name="@Name"
            Required="@Required">
    @ChildContent
</SelectRoot>
```

**Select.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class Select
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback<string?> OnValueChanged { get; set; }
    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public bool Required { get; set; }
}
```

**SelectTrigger.razor** — wraps `Primitives.SelectTrigger`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Select

<Primitives.Select.SelectTrigger
    class="@Classes"
    @attributes="AdditionalAttributes">
    @ChildContent
    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
         fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
         class="opacity-50" aria-hidden="true">
        <path d="m6 9 6 6 6-6"/>
    </svg>
</Primitives.Select.SelectTrigger>
```

**SelectTrigger.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectTrigger
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => Cn(
        "flex h-10 w-full items-center justify-between rounded-md border border-input",
        "bg-background px-3 py-2 text-sm ring-offset-background",
        "placeholder:text-muted-foreground",
        "focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "[&>span]:line-clamp-1",
        Class
    );
}
```

**SelectContent.razor** — wraps `Primitives.SelectContent` + portal
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Select

<SelectPortal>
    <Primitives.Select.SelectContent
        class="@Classes"
        Position="@Position"
        Side="@Side"
        SideOffset="@SideOffset"
        @attributes="AdditionalAttributes">
        <SelectScrollUpButton />
        <SelectViewport class="p-1">
            @ChildContent
        </SelectViewport>
        <SelectScrollDownButton />
    </Primitives.Select.SelectContent>
</SelectPortal>
```

**SelectContent.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectContent
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string Position { get; set; } = "popper";
    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;
    [Parameter] public int SideOffset { get; set; } = 4;
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => Cn(
        "relative z-50 max-h-96 min-w-[8rem] overflow-hidden rounded-md border",
        "bg-popover text-popover-foreground shadow-md",
        // Animations
        "data-[state=open]:animate-in data-[state=open]:fade-in-0 data-[state=open]:zoom-in-95",
        "data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95",
        // Directional slide based on side
        "data-[side=bottom]:slide-in-from-top-2 data-[side=top]:slide-in-from-bottom-2",
        "data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2",
        // Popper sizing
        Position == "popper"
            ? "w-full min-w-[var(--select-trigger-width)] max-h-[var(--select-content-available-height)]"
            : null,
        Class
    );
}
```

**SelectItem.razor** — wraps `Primitives.SelectItem`
```razor
@namespace BlazingSpire.Demo.Components.UI
@using BlazingSpire.Primitives.Select

<Primitives.Select.SelectItem
    Value="@Value"
    Disabled="@Disabled"
    TextValue="@TextValue"
    class="@Classes"
    @attributes="AdditionalAttributes">
    <span class="absolute left-2 flex h-3.5 w-3.5 items-center justify-center">
        <SelectItemIndicator>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                 fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M20 6 9 17l-5-5"/>
            </svg>
        </SelectItemIndicator>
    </span>
    <SelectItemText>@ChildContent</SelectItemText>
</Primitives.Select.SelectItem>
```

**SelectItem.razor.cs**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectItem
{
    [Parameter, EditorRequired] public string Value { get; set; } = default!;
    [Parameter] public string? TextValue { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => Cn(
        "relative flex w-full cursor-default select-none items-center",
        "rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none",
        "focus:bg-accent focus:text-accent-foreground",
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
        "data-[highlighted]:bg-accent data-[highlighted]:text-accent-foreground",
        Class
    );
}
```

**Consumer usage:**
```razor
<Select @bind-Value="_fruit">
    <SelectTrigger class="w-[180px]">
        <SelectValue Placeholder="Pick a fruit..." />
    </SelectTrigger>
    <SelectContent>
        <SelectGroup>
            <SelectLabel>Fruits</SelectLabel>
            <SelectItem Value="apple">Apple</SelectItem>
            <SelectItem Value="banana">Banana</SelectItem>
            <SelectItem Value="cherry">Cherry</SelectItem>
        </SelectGroup>
    </SelectContent>
</Select>

@code {
    private string? _fruit;
}
```

**What to notice:**
- The styled `SelectItem` bundles the indicator (checkmark) and text span — the consumer just writes `<SelectItem Value="x">Label</SelectItem>`.
- The styled `SelectContent` bundles the portal, viewport, and scroll buttons.
- Directional slide animations use `data-[side=*]` attributes emitted by the primitive's floating positioning logic.
- The `Position == "popper"` conditional class demonstrates how styled components can add conditional Tailwind classes based on configuration.

---

## Animation Patterns

Animations are CSS-first, driven by `data-state` and `data-side` attributes that primitives emit. No JS animation library is needed.

### The `@starting-style` Pattern

For native `<dialog>` elements or CSS-only transitions:

```css
dialog[open] {
    opacity: 1;
    transform: scale(1);
    transition: opacity 0.3s, transform 0.3s,
                overlay 0.3s allow-discrete,
                display 0.3s allow-discrete;
    @starting-style { opacity: 0; transform: scale(0.95); }
}
dialog { opacity: 0; transform: scale(0.95); }
```

### The `data-state` Attribute Pattern

Primitives emit `data-state="open"` or `data-state="closed"` on their root elements. Styled components target these with Tailwind's data attribute selector:

```
data-[state=open]:animate-in data-[state=open]:fade-in-0 data-[state=open]:zoom-in-95
data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95
```

These utility classes come from **tw-animate-css** and map to keyframe animations.

### Standard Animation Pairs

| Component | Enter | Exit |
|-----------|-------|------|
| Dialog overlay | `fade-in-0` | `fade-out-0` |
| Dialog content | `fade-in-0 zoom-in-95 slide-in-from-left-1/2 slide-in-from-top-[48%]` | reverse |
| Select/Dropdown | `fade-in-0 zoom-in-95` + directional `slide-in-from-*` | reverse |
| Tooltip | `fade-in-0 zoom-in-95` + directional `slide-in-from-*` | reverse |
| Sheet | `slide-in-from-{edge}` | `slide-out-to-{edge}` |

### Directional Slide

Floating elements (Select, Dropdown, Tooltip) use `data-[side=*]` to animate from the correct direction:

```
data-[side=bottom]:slide-in-from-top-2
data-[side=top]:slide-in-from-bottom-2
data-[side=left]:slide-in-from-right-2
data-[side=right]:slide-in-from-left-2
```

### Reduced Motion

Replace directional motion with simple opacity fades:

```css
@media (prefers-reduced-motion: reduce) {
    [data-state] {
        animation: none;
        transition: opacity 0.15s ease;
    }
}
```

---

## Dark Mode

Dark mode uses the Tailwind class strategy: `.dark` on the `<html>` element.

### How It Works

1. CSS custom properties define all color tokens in `:root` (light) and `.dark` (dark):

   ```css
   @theme {
       --color-primary: oklch(0.42 0.18 25);
       --color-primary-foreground: oklch(1 0 0);
   }

   .dark {
       --color-primary: oklch(0.55 0.18 25);
       --color-primary-foreground: oklch(0.97 0 0);
   }
   ```

2. Tailwind resolves `bg-primary` to `var(--color-primary)`, which automatically changes when `.dark` is present.

3. Styled components use only semantic token names (`bg-primary`, `text-foreground`, `border-border`). They never use raw color values.

### Styled Component Rules

- **Always use semantic tokens**: `bg-background`, `text-foreground`, `bg-primary`, `text-muted-foreground`, etc. Never `bg-white`, `text-gray-900`, or raw `oklch(...)`.
- **No `dark:` prefix needed** for token-based colors. The custom properties switch automatically.
- **Use `dark:` only** for non-token adjustments (e.g., `dark:shadow-none` to remove a shadow in dark mode, or `dark:border-white/10` for a specific opacity override).
- **Test both modes.** Every styled component must be visually verified in both light and dark modes.

### Flash Prevention

A blocking inline script in `<head>` reads the theme from `localStorage` before first paint:

```html
<script>
(function() {
    var theme = localStorage.getItem('theme');
    if (theme === 'dark' || (!theme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark');
    }
})();
</script>
```

This runs before Blazor boots, preventing a flash of the wrong theme.

---

## Consumer Customization

The styled component layer is designed for modification. Consumers customize components through three mechanisms, in order of invasiveness:

### 1. Class Parameter (Non-Breaking)

Pass additional Tailwind classes via the `Class` parameter. `TwMerge` resolves conflicts, so consumer classes override defaults:

```razor
@* Override the default rounded corners *@
<Button Class="rounded-none">Square Button</Button>

@* Add a width constraint *@
<SelectTrigger class="w-[300px]">...</SelectTrigger>
```

### 2. Direct Source Editing (Copy-Paste Model)

Since consumers own the source, they can directly edit the `.razor` and `.razor.cs` files:

- Change the base classes in the `Classes` property
- Add new variants to the enum and dictionary
- Remove unused variants
- Modify the HTML structure in the `.razor` template
- Add new parameters

This is the intended workflow. The styled components are **starting points**, not locked-down library code.

### 3. AdditionalAttributes (Escape Hatch)

Any HTML attribute can be passed through and splatted onto the root element:

```razor
<Button data-testid="submit-btn" aria-label="Submit form">Submit</Button>
```

---

## Anti-Patterns

### CSS Isolation (`.razor.css`)

**Never use it.** Blazor CSS isolation generates scoped selectors that cannot interact with Tailwind's utility classes. All styling must be done via utility classes directly in the `.razor` markup.

### Inline Styles

**Never use `style="..."`.** Inline styles cannot be overridden by the `Class` parameter and bypass the design system's token layer. Use Tailwind utilities for everything, including one-off values via arbitrary value syntax (`w-[137px]`).

### Render-Time String Building

**Avoid string concatenation in the `.razor` template** for class composition:

```razor
@* BAD: allocates on every render, no TwMerge conflict resolution *@
<div class="@($"{BaseClass} {ConditionalClass()} {Class}")">

@* GOOD: pre-computed in OnParametersSet, merged via Cn() *@
<div class="@_classes">
```

The POC currently uses the simpler interpolation pattern for expediency. Production components should cache via `OnParametersSet`.

### Inheriting from a Custom Base Class

**Don't create a `BlazingSpireComponentBase`.** Each styled component is a plain `partial class` inheriting from `ComponentBase`. A shared base class creates coupling between components that are supposed to be independent copy-paste units.

### Conditional Rendering of Wrapper Elements

**Don't add wrapper `<div>` elements** that only exist for styling. Use Tailwind utilities on the semantic element itself. Extra wrapper elements break the primitive's ARIA tree and add unnecessary DOM nodes.

### Hardcoded Colors

**Never use raw colors** (`bg-red-500`, `text-[#333]`, `oklch(0.5 0.2 30)`). Always use semantic tokens (`bg-destructive`, `text-foreground`). Raw colors do not respond to dark mode or theme changes.

### Duplicating Primitive Logic

**Never re-implement ARIA, keyboard handling, or focus management** in a styled component. If the primitive does not expose something you need, the correct fix is to add it to the primitive — not to work around it in the styled layer.
