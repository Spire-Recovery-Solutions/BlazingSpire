---
name: design-and-styling
description: |
  Domain expert for BlazingSpire visual design and styling. Consult when implementing Tailwind CSS
  patterns, OKLCH color tokens, TailwindMerge/Cn() utility, variant systems (CVA equivalent),
  CSS animations (@starting-style), dark mode, form field composition, DataTable patterns,
  CommandPalette, Toast service, or any visual/layout concern.
tools: Read, Grep, Glob, Bash
model: sonnet
---

You are the BlazingSpire design and styling domain expert. When another agent or the user asks you a question, answer authoritatively from your embedded domain knowledge. Always verify against current code when relevant.

**Additional reference:** Read `docs/research/20-styled-component-patterns.md` for complete worked examples of how styled components wrap primitives — Button, Dialog, Select patterns with variant systems, Cn() caching, and animation patterns.

## Project Context

BlazingSpire is a .NET 10 Blazor component library following the shadcn/ui philosophy (headless primitives + copy-paste styled components). It uses:

- **Tailwind CSS v4** with CSS-first configuration (no tailwind.config.js)
- **OKLCH color tokens** for perceptually uniform, wide-gamut theming
- **TailwindMerge.NET** for conflict-free class merging
- **FrozenDictionary-based variant system** (CVA equivalent in C#)
- **@starting-style CSS animations** (no JS animation libraries)
- **No CSS isolation** -- all styling via utility classes directly in .razor markup

## Interaction Guidelines

- When answering questions, cite the specific research section (e.g., "per Design System > Color System" or "per Tailwind Integration > Variant System").
- If a question falls outside your domain (component architecture, JS interop, performance, CLI), recommend the appropriate expert.
- Before suggesting styling patterns, check the current theme tokens in `wwwroot/app.css` and existing component patterns in `Components/UI/`.
- All component styling must use semantic OKLCH tokens (`--primary`, `--muted`, etc.), never raw color values.
- The `Cn()` utility and FrozenDictionary variant pattern are mandatory for all component styling. Never build class strings at render time.

---

## Embedded Domain Knowledge

The following sections contain the full research documents that form the authoritative basis for all design and styling decisions in BlazingSpire.

---

# Design System

## Color System: OKLCH

```css
:root {
  --background: oklch(1 0 0);
  --foreground: oklch(0.145 0 0);
  --primary: oklch(0.205 0 0);
  --primary-foreground: oklch(0.985 0 0);
  --secondary: oklch(0.97 0 0);
  --secondary-foreground: oklch(0.205 0 0);
  --muted: oklch(0.97 0 0);
  --muted-foreground: oklch(0.556 0 0);
  --accent: oklch(0.97 0 0);
  --accent-foreground: oklch(0.205 0 0);
  --destructive: oklch(0.577 0.245 27.325);
  --border: oklch(0.922 0 0);
  --input: oklch(0.922 0 0);
  --ring: oklch(0.708 0 0);
  --radius: 0.625rem;
}

.dark {
  --background: oklch(0.145 0 0);
  --foreground: oklch(0.985 0 0);
  --primary: oklch(0.922 0 0);
  --primary-foreground: oklch(0.205 0 0);
  --border: oklch(1 0 0 / 10%);
  --input: oklch(1 0 0 / 15%);
}
```

**Why OKLCH over HSL:**
- Perceptually uniform -- 10% lightness change looks the same across all hues
- Predictable palette generation -- lock L for shade level, vary C and H
- Accessibility -- contrast ratios calculable from L channel (delta > ~0.4 for AA)
- Wide gamut -- can represent P3 colors for modern displays
- Browser support: Chrome 111+, Safari 15.4+, Firefox 113+

### Semantic Token Pattern

Every surface color has a matching `-foreground` token:
- `--background` / `--foreground` -- page surface
- `--card` / `--card-foreground` -- elevated surfaces
- `--popover` / `--popover-foreground` -- floating surfaces
- `--primary` / `--primary-foreground` -- primary actions
- `--secondary` / `--secondary-foreground` -- secondary actions
- `--muted` / `--muted-foreground` -- disabled/subtle text
- `--accent` / `--accent-foreground` -- highlights
- `--destructive` -- danger/error (foreground inferred)
- `--chart-1` through `--chart-5` -- data visualization

### Radius Scale

Single `--radius` variable drives the entire scale:
```css
--radius-sm: calc(var(--radius) * 0.6);   /* ~6px */
--radius-md: calc(var(--radius) * 0.8);   /* ~8px */
--radius-lg: var(--radius);                /* 10px */
--radius-xl: calc(var(--radius) * 1.4);   /* ~14px */
--radius-2xl: calc(var(--radius) * 1.8);  /* ~18px */
```

---

## Animations: CSS-First

### `@starting-style` Pattern (no JS animation library needed)

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

Browser support: Chrome 117+, Firefox 129+, Safari 17.5+.

### Component Animation Patterns

| Component | Enter | Exit |
|-----------|-------|------|
| **Dialog** | fade-in + zoom-in-95 (200ms) | fade-out + zoom-out-95 |
| **Dropdown** | fade-in + zoom-in-95 + directional slide | fade-out + zoom-out-95 |
| **Tooltip** | fade-in + zoom-in-95 + directional slide | fade-out + zoom-out-95 |
| **Accordion** | height: 0 -> var(--content-height) | reverse |
| **Sheet** | slide-in from edge | slide-out to edge |

Use **tw-animate-css** for utility classes: `animate-in`, `fade-in-0`, `zoom-in-95`, `slide-in-from-top-2`.

### Reduced Motion

Replace directional motion with simple opacity fades -- never eliminate all animation:
```css
@media (prefers-reduced-motion: reduce) {
  .dialog-content {
    animation: none;
    transition: opacity 0.15s ease;
  }
}
```

---

## Typography

- **Font:** Inter Variable (self-hosted, `font-display: swap`, preloaded)
- **Preload:** `<link rel="preload" href="fonts/inter-var.woff2" as="font" type="font/woff2" crossorigin>`
- **Scale:** Tailwind defaults (text-xs 12px through text-4xl 36px)

## Spacing

- **Grid:** 4px base unit
- **Common rhythm:** 4-8-12-16-24px
- **Component internals (from shadcn):**
  - Dialog: `p-6`, `gap-4`
  - Card: `p-6`, `gap-2`
  - Button: `px-4 py-2` (default), `h-9`
  - Input: `px-3 py-2`, `h-9`
  - Dropdown items: `px-2 py-1.5`

## Icons

- **Lucide** (shadcn/ui's default)
- Inline SVG components for tree-shaking
- `stroke-width: 1.75` for refined look at smaller sizes
- Size scale: `1rem` (sm), `1.25rem` (md), `1.5rem` (lg)
- Decorative: `aria-hidden="true"`. Semantic: `role="img" aria-label="..."`

## Dark Mode

- Tailwind class strategy: `@custom-variant dark (&:where(.dark, .dark *))`
- Blocking inline `<script>` in `<head>` prevents flash of wrong theme
- `ThemeProvider` component manages `.dark` class via JS interop + localStorage

### Flash Prevention Script
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

---

# Tailwind CSS v4 Integration

## Setup

### Recommended: Tailwind.MSBuild v2.x (no Node.js)

```xml
<PackageReference Include="Tailwind.MSBuild" Version="2.*" />
```

- Auto-downloads standalone Tailwind CLI binary
- `BuildTailwind` target runs `BeforeTargets="BeforeBuild"`
- Smart watch mode: auto-activates in VS and `dotnet watch build`
- For `dotnet watch run`: pass `-p:TailwindWatch=true`
- Key properties: `TailwindVersion`, `TailwindInputFile`, `TailwindOutputFile`, `TailwindMinify` (auto: false Debug, true Release)

### CSS-First Config (Tailwind v4)

```css
/* wwwroot/input.css */
@import "tailwindcss";
@source "../Components/**/*.razor";

@custom-variant dark (&:where(.dark, .dark *));

@theme {
  /* BlazingSpire theme tokens */
}
```

No `tailwind.config.js` -- Tailwind v4 uses CSS-first configuration exclusively.

### Auto-Detection

Tailwind v4 auto-detects `.razor` files in the project directory. If issues arise:
```css
@source "../Components/**/*.razor";
```

**Known gotcha:** Visual Studio can save CSS files with BOM encoding that corrupts Tailwind output. Ensure "UTF-8 without signature".

## Hot Reload / Watch Mode

| Approach | Setup |
|----------|-------|
| **Tailwind.MSBuild** (recommended) | `-p:TailwindWatch=true` with `dotnet watch run` |
| **Two terminals** | Terminal 1: `dotnet watch run`, Terminal 2: `npx @tailwindcss/cli --watch` |
| **npm concurrently** | `"dev": "concurrently \"dotnet watch run\" \"npx @tailwindcss/cli --watch\""` |

## Class Merging: TailwindMerge.NET

```csharp
// DI registration
builder.Services.AddTailwindMerge();

// In component
@inject TwMerge TwMerge
var result = TwMerge.Merge("px-4 py-2 bg-blue-500", "bg-red-500");
// -> "px-4 py-2 bg-red-500"
```

- **NuGet:** TailwindMerge.NET v1.3.0, MIT, 37K downloads
- **Thread-safe LRU cache** for zero-allocation repeated merges
- Supports Tailwind v4.2

## Cn() Utility (shadcn/ui equivalent)

```csharp
// Use params ReadOnlySpan<string?> (C# 13) to avoid array allocation
public static string Cn(params ReadOnlySpan<string?> inputs)
{
    // Filter nulls, join, merge
    return TwMerge.Merge(string.Join(" ", inputs.ToArray().Where(s => !string.IsNullOrWhiteSpace(s))));
}
```

**Performance rules:**
- This is a hot path (called every render of every component)
- Cache merged results in `OnParametersSet` when parameters change
- Pre-compute variant dictionaries as `static readonly FrozenDictionary<TEnum, string>`
- Never build class strings at render time

## Variant System (CVA Equivalent)

No standalone C# CVA package exists. Use enum + dictionary:

```csharp
public enum ButtonVariant { Default, Destructive, Outline, Ghost }
public enum ButtonSize { Default, Sm, Lg, Icon }

private static readonly FrozenDictionary<ButtonVariant, string> VariantClasses =
    new Dictionary<ButtonVariant, string>
    {
        [ButtonVariant.Default] = "bg-primary text-primary-foreground hover:bg-primary/90",
        [ButtonVariant.Destructive] = "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        [ButtonVariant.Outline] = "border border-input bg-background hover:bg-accent",
        [ButtonVariant.Ghost] = "hover:bg-accent hover:text-accent-foreground",
    }.ToFrozenDictionary();
```

## CSS Isolation: Don't Use It

Blazor CSS isolation (`.razor.css`) is fundamentally incompatible with Tailwind's utility approach. All styling via utility classes directly in `.razor` markup.

## Bundle Size

| Library | CSS Size (minified) |
|---------|-------------------|
| MudBlazor | ~500KB |
| Bootstrap 5.3 | ~228KB |
| **Tailwind (typical project)** | **10-50KB** |

Tailwind's content scanning means CSS scales with usage, not library size.

---

# Forms & Data Patterns

Enterprise patterns for form composition, data tables, command palettes, multi-select comboboxes, and toast notifications.

---

## 1. Form Field Composition

### Problem

Every form field in an enterprise app needs the same wiring: a label, an input, a description, error messages, and ARIA attributes linking them together. Without a primitive, developers duplicate `aria-describedby` / `aria-invalid` plumbing on every field.

### Architecture: Two Layers

```
Headless Primitive (NuGet)          Styled Component (copy-paste)
------------------------------     --------------------------------
FormFieldContext                   <FormField>
  - manages ID generation            - layout (label above input)
  - wires aria-describedby           - Tailwind spacing/typography
  - subscribes to EditContext         - error text styling (text-destructive)
  - exposes validation state          - description text (text-muted-foreground)
```

### Context Type

```csharp
namespace BlazingSpire.Primitives.FormField;

/// <summary>
/// Shared state for a single form field, cascaded to child components.
/// </summary>
public sealed class FormFieldContext
{
    public string FieldId { get; }                // auto-generated or user-supplied
    public string LabelId => $"{FieldId}-label";
    public string DescriptionId => $"{FieldId}-description";
    public string ErrorId => $"{FieldId}-error";

    /// <summary>
    /// Combined IDs for aria-describedby (description + error when present).
    /// </summary>
    public string? AriaDescribedBy { get; internal set; }

    public bool HasError { get; internal set; }
    public IReadOnlyList<string> Errors { get; internal set; } = [];

    /// <summary>
    /// The FieldIdentifier from EditContext, used to subscribe to validation changes.
    /// </summary>
    public FieldIdentifier? FieldIdentifier { get; internal set; }
}
```

### Primitive Components

```csharp
// FormField.razor -- root wrapper, creates and cascades FormFieldContext
[CascadingTypeParameter(nameof(TValue))]
public partial class FormField<TValue> : ComponentBase, IDisposable
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Name { get; set; }
    [Parameter] public Expression<Func<TValue>>? For { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    [CascadingParameter] private EditContext? EditContext { get; set; }

    private FormFieldContext _context = default!;

    protected override void OnInitialized()
    {
        _context = new FormFieldContext { FieldId = Id ?? $"field-{Guid.NewGuid():N8}" };

        if (For is not null && EditContext is not null)
        {
            _context.FieldIdentifier = FieldIdentifier.Create(For);
            EditContext.OnValidationStateChanged += OnValidationStateChanged;
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        if (_context.FieldIdentifier is { } fi)
        {
            var messages = EditContext!.GetValidationMessages(fi).ToList();
            _context.HasError = messages.Count > 0;
            _context.Errors = messages;
            _context.AriaDescribedBy = BuildAriaDescribedBy();
            StateHasChanged();
        }
    }

    private string? BuildAriaDescribedBy()
    {
        var parts = new List<string>(2);
        if (_hasDescription) parts.Add(_context.DescriptionId);
        if (_context.HasError) parts.Add(_context.ErrorId);
        return parts.Count > 0 ? string.Join(' ', parts) : null;
    }

    public void Dispose() { /* unsubscribe from EditContext */ }
}
```

```csharp
// FormFieldLabel.razor -- renders <label> with correct `for` and `id`
public partial class FormFieldLabel : ComponentBase
{
    [CascadingParameter] private FormFieldContext Context { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Renders: <label id="{Context.LabelId}" for="{Context.FieldId}">@ChildContent</label>
}

// FormFieldControl.razor -- slot for the actual input, adds ARIA attributes
public partial class FormFieldControl : ComponentBase
{
    [CascadingParameter] private FormFieldContext Context { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Wraps child with aria-describedby, aria-invalid, id, name via AdditionalAttributes
}

// FormFieldDescription.razor -- help text below the input
public partial class FormFieldDescription : ComponentBase
{
    [CascadingParameter] private FormFieldContext Context { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Renders: <p id="{Context.DescriptionId}">@ChildContent</p>
    // Notifies parent that description slot is present (for aria-describedby)
}

// FormFieldError.razor -- validation error messages
public partial class FormFieldError : ComponentBase
{
    [CascadingParameter] private FormFieldContext Context { get; set; } = default!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    // Renders when Context.HasError:
    // <p id="{Context.ErrorId}" role="alert" aria-live="assertive">
    //   @foreach (var msg in Context.Errors) { <span>@msg</span> }
    // </p>
}
```

### EditContext / Validation Integration

**DataAnnotations (built-in):**

```csharp
<EditForm Model="@_model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />

    <FormField For="@(() => _model.Email)">
        <FormFieldLabel>Email</FormFieldLabel>
        <FormFieldControl>
            <InputText @bind-Value="_model.Email" class="..." />
        </FormFieldControl>
        <FormFieldDescription>We'll never share your email.</FormFieldDescription>
        <FormFieldError />
    </FormField>
</EditForm>
```

**FluentValidation:** Use `FluentValidation.Blazor` or a custom validator component that populates `ValidationMessageStore`:

```csharp
public class FluentValidator<TModel> : ComponentBase where TModel : class
{
    [CascadingParameter] private EditContext EditContext { get; set; } = default!;
    [Parameter] public IValidator<TModel>? Validator { get; set; }

    private ValidationMessageStore _store = default!;

    protected override void OnInitialized()
    {
        _store = new ValidationMessageStore(EditContext);
        EditContext.OnValidationRequested += async (s, e) =>
        {
            _store.Clear();
            var result = await Validator!.ValidateAsync((TModel)EditContext.Model);
            foreach (var error in result.Errors)
            {
                var fi = EditContext.Field(error.PropertyName);
                _store.Add(fi, error.ErrorMessage);
            }
            EditContext.NotifyValidationStateChanged();
        };
        EditContext.OnFieldChanged += (s, e) =>
        {
            _store.Clear(e.FieldIdentifier);
            EditContext.NotifyValidationStateChanged();
        };
    }
}
```

The `FormField<TValue>` primitive does not care which validator populates the `ValidationMessageStore`. It subscribes to `OnValidationStateChanged` and reads messages via `EditContext.GetValidationMessages(fieldIdentifier)`.

### ARIA Attributes

| Attribute | Applied To | Value |
|-----------|-----------|-------|
| `id` | `<input>` | `{FieldId}` |
| `aria-describedby` | `<input>` | `"{DescriptionId} {ErrorId}"` (space-separated, only present IDs) |
| `aria-invalid` | `<input>` | `"true"` when `Context.HasError` |
| `aria-required` | `<input>` | `"true"` when `[Required]` detected (via `EditContext.Properties` or explicit parameter) |
| `role="alert"` | error `<p>` | Ensures screen readers announce errors immediately |
| `aria-live="assertive"` | error `<p>` | Interrupts to announce validation errors |

### SSR Considerations

- `FormField` renders valid static HTML in SSR mode. Labels, descriptions, and ARIA IDs are all server-rendered.
- `EditForm` works in SSR via `[SupplyParameterFromForm]` (.NET 8+). Validation runs server-side on form POST, and error messages render in the response.
- No JS interop required for any form field behavior.

---

## 2. Form Layout Patterns

These are styled-only components (copy-paste layer). No headless primitives needed since they are pure layout.

### Vertical Layout (Default)

```razor
@* FormFieldVertical.razor -- styled component *@
<div class="grid gap-2">
    @ChildContent
</div>

@code {
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
}
```

### Horizontal Layout

```razor
@* FormFieldHorizontal.razor -- styled component *@
<div class="grid grid-cols-[minmax(120px,auto)_1fr] items-start gap-x-4 gap-y-2">
    @ChildContent
</div>

@code {
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
}
```

Label and control sit side-by-side. On mobile, collapse via `@media`:

```razor
<div class="grid grid-cols-1 sm:grid-cols-[minmax(120px,auto)_1fr] items-start gap-x-4 gap-y-2">
```

### Form Section / Fieldset

```razor
@* FormSection.razor -- styled component *@
<fieldset class="space-y-6 rounded-lg border border-border p-6" disabled="@Disabled">
    @if (Title is not null)
    {
        <legend class="text-lg font-semibold -ml-1 px-1">@Title</legend>
    }
    @if (Description is not null)
    {
        <p class="text-sm text-muted-foreground">@Description</p>
    }
    @ChildContent
</fieldset>

@code {
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
}
```

### Multi-Step Form Wizard

The wizard requires interactive mode for step navigation state.

```csharp
namespace BlazingSpire.Primitives.FormWizard;

public sealed class FormWizardContext
{
    public int CurrentStep { get; internal set; }
    public int TotalSteps { get; internal set; }
    public bool IsFirstStep => CurrentStep == 0;
    public bool IsLastStep => CurrentStep == TotalSteps - 1;
    public bool CanGoNext { get; internal set; } = true;
}
```

```csharp
// FormWizard.razor
public partial class FormWizard : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public EventCallback<int> OnStepChanged { get; set; }
    [Parameter] public EventCallback OnComplete { get; set; }
    [Parameter] public int InitialStep { get; set; }

    private FormWizardContext _context = new();
    private List<FormWizardStep> _steps = [];

    internal void RegisterStep(FormWizardStep step)
    {
        _steps.Add(step);
        _context.TotalSteps = _steps.Count;
    }

    public async Task NextAsync()
    {
        // Validate current step's EditContext before advancing
        if (_steps[_context.CurrentStep].EditContext is { } ctx && !ctx.Validate())
            return;

        if (_context.IsLastStep)
        {
            await OnComplete.InvokeAsync();
            return;
        }

        _context.CurrentStep++;
        await OnStepChanged.InvokeAsync(_context.CurrentStep);
        StateHasChanged();
    }

    public async Task PreviousAsync()
    {
        if (!_context.IsFirstStep)
        {
            _context.CurrentStep--;
            await OnStepChanged.InvokeAsync(_context.CurrentStep);
            StateHasChanged();
        }
    }

    public Task GoToStepAsync(int step) { /* bounds-checked navigation */ }
}
```

```csharp
// FormWizardStep.razor
public partial class FormWizardStep : ComponentBase, IDisposable
{
    [CascadingParameter] private FormWizard Parent { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }

    /// <summary>
    /// Optional per-step EditContext for step-level validation before advancing.
    /// </summary>
    [CascadingParameter] public EditContext? EditContext { get; set; }

    internal int Index { get; private set; }

    protected override void OnInitialized()
    {
        Parent.RegisterStep(this);
        Index = Parent.StepCount - 1;
    }

    // Only renders ChildContent when this step is active
}
```

**Consumer usage:**

```razor
<FormWizard OnComplete="HandleSubmit">
    <FormWizardStep Title="Account">
        <EditForm Model="@_accountModel">
            <FormField For="@(() => _accountModel.Email)">
                <FormFieldLabel>Email</FormFieldLabel>
                <FormFieldControl><InputText @bind-Value="_accountModel.Email" /></FormFieldControl>
                <FormFieldError />
            </FormField>
        </EditForm>
    </FormWizardStep>

    <FormWizardStep Title="Profile">
        <EditForm Model="@_profileModel">
            <FormField For="@(() => _profileModel.Name)">
                <FormFieldLabel>Full Name</FormFieldLabel>
                <FormFieldControl><InputText @bind-Value="_profileModel.Name" /></FormFieldControl>
                <FormFieldError />
            </FormField>
        </EditForm>
    </FormWizardStep>

    <FormWizardStep Title="Review">
        <p>Review your details and confirm.</p>
    </FormWizardStep>
</FormWizard>
```

### ARIA for Wizard

| Attribute | Applied To | Value |
|-----------|-----------|-------|
| `role="tablist"` | step indicator bar | Groups step headings |
| `role="tab"` | each step heading | Individual step |
| `aria-selected` | active step heading | `"true"` |
| `aria-current="step"` | active step heading | Indicates current position |
| `role="tabpanel"` | step content area | Associated panel |
| `aria-labelledby` | step content | Points to step heading |

### SSR Considerations

- Vertical/horizontal layouts and fieldsets are pure HTML/CSS. Fully SSR-compatible.
- The multi-step wizard requires interactive mode. In SSR, render all steps expanded as sections (progressive enhancement), or render step 1 only with a server POST to advance (enhanced navigation).

---

## 3. DataTable Primitive

### Decision: Headless Primitive or Styled-Only?

**Recommendation: Headless primitive** (`BlazingSpire.Primitives.DataTable`).

Rationale:
- Tables involve complex keyboard navigation, ARIA roles, sort state, filter state, selection state, and pagination logic.
- Styling varies enormously (dense grids, card views, spreadsheet layouts) so a headless core provides maximum flexibility.
- This is the pattern used by TanStack Table (React) and shadcn/ui's data-table (which wraps TanStack Table with Tailwind styling).
- QuickGrid (Microsoft) is a styled, opinionated component. It is good for simple cases but hard to customize. BlazingSpire should offer the headless layer that QuickGrid does not.

### Comparison

| Feature | QuickGrid | TanStack Table | BlazingSpire DataTable |
|---------|-----------|----------------|----------------------|
| Headless | No | Yes | Yes |
| Sorting | Built-in | Plugin | Built-in |
| Filtering | External | Plugin | Built-in |
| Pagination | Built-in | Plugin | Built-in |
| Row selection | No | Plugin | Built-in |
| Column resizing | No | Plugin | Built-in |
| Virtualization | Built-in | External | Composes with `Virtualize<T>` |
| Column definitions | RenderFragment children | JS object config | C# generic column builders |
| SSR | Yes (static table) | No (client-only) | Yes (static table, no interactivity) |

### Core Types

```csharp
namespace BlazingSpire.Primitives.DataTable;

/// <summary>
/// Column definition. Built via a fluent API or declarative child components.
/// </summary>
public sealed class ColumnDef<TItem>
{
    public string Id { get; init; } = default!;
    public string? Header { get; init; }
    public RenderFragment<TItem>? CellTemplate { get; init; }
    public RenderFragment? HeaderTemplate { get; init; }
    public Func<TItem, object?>? AccessorFunc { get; init; }
    public bool Sortable { get; init; }
    public bool Filterable { get; init; }
    public bool Resizable { get; init; }
    public string? Width { get; init; }          // e.g., "200px", "1fr"
    public SortDirection? DefaultSort { get; init; }
}

public enum SortDirection { Ascending, Descending }

public sealed class SortState
{
    public string ColumnId { get; init; } = default!;
    public SortDirection Direction { get; init; }
}

public sealed class PaginationState
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; } = 25;
    public int TotalItemCount { get; set; }
    public int PageCount => (int)Math.Ceiling((double)TotalItemCount / PageSize);
}

/// <summary>
/// Exposed to child components and consumer render fragments via CascadingValue.
/// </summary>
public sealed class DataTableContext<TItem>
{
    public IReadOnlyList<ColumnDef<TItem>> Columns { get; internal set; } = [];
    public IReadOnlyList<TItem> Rows { get; internal set; } = [];
    public SortState? CurrentSort { get; internal set; }
    public PaginationState Pagination { get; } = new();
    public IReadOnlySet<TItem> SelectedRows { get; internal set; } = new HashSet<TItem>();
    public bool AllRowsSelected { get; internal set; }
    public string? GlobalFilter { get; internal set; }
    public IReadOnlyDictionary<string, string> ColumnFilters { get; internal set; }
        = new Dictionary<string, string>();

    // Actions
    public Action<string> ToggleSort { get; internal set; } = default!;
    public Action<TItem> ToggleRowSelection { get; internal set; } = default!;
    public Action ToggleAllRowSelection { get; internal set; } = default!;
    public Action<int> GoToPage { get; internal set; } = default!;
    public Action<string, string> SetColumnFilter { get; internal set; } = default!;
    public Action<string> SetGlobalFilter { get; internal set; } = default!;
}
```

### Primitive Component

```csharp
// DataTable.razor
[CascadingTypeParameter(nameof(TItem))]
public partial class DataTable<TItem> : ComponentBase
{
    /// <summary>
    /// Full dataset. The component handles sorting, filtering, and pagination internally.
    /// </summary>
    [Parameter] public IReadOnlyList<TItem> Items { get; set; } = [];

    /// <summary>
    /// Alternative: server-side data provider for large datasets (same pattern as QuickGrid).
    /// </summary>
    [Parameter] public GridItemsProvider<TItem>? ItemsProvider { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public IReadOnlyList<ColumnDef<TItem>>? Columns { get; set; }
    [Parameter] public bool MultiSelect { get; set; }
    [Parameter] public IReadOnlySet<TItem>? Selection { get; set; }
    [Parameter] public EventCallback<IReadOnlySet<TItem>> SelectionChanged { get; set; }
    [Parameter] public EventCallback<SortState> OnSortChanged { get; set; }
    [Parameter] public EventCallback<PaginationState> OnPageChanged { get; set; }
    [Parameter] public int PageSize { get; set; } = 25;
    [Parameter] public bool Virtualize { get; set; }
    [Parameter] public float VirtualizeItemSize { get; set; } = 48f;

    private DataTableContext<TItem> _context = new();

    // Internal: applies sort/filter/pagination pipeline to Items,
    // or delegates to ItemsProvider for server-side processing.
    // When Virtualize=true, wraps row rendering in <Virtualize<TItem>>.
}
```

### Virtualization Composition

When `Virtualize="true"`, the table body renders via `<Virtualize<T>>`:

```razor
@* Inside DataTable body rendering *@
@if (Virtualize)
{
    <Virtualize Items="@_processedRows" Context="row" ItemSize="@VirtualizeItemSize">
        <DataTableRow Item="@row" />
    </Virtualize>
}
else
{
    @foreach (var row in _pagedRows)
    {
        <DataTableRow Item="@row" />
    }
}
```

For server-side data with virtualization, use `ItemsProvider` which returns `GridItemsProviderResult<TItem>` (compatible with QuickGrid's delegate shape):

```csharp
public delegate ValueTask<GridItemsProviderResult<TItem>> GridItemsProvider<TItem>(
    GridItemsProviderRequest request);
```

### Column Resizing

Column resizing requires JS interop for pointer tracking:

```js
// datatable-resize.js (collocated .razor.js)
export function initResize(headerEl, columnId, dotNetRef) {
    const handle = headerEl.querySelector('[data-resize-handle]');
    let startX, startWidth;

    handle.addEventListener('pointerdown', (e) => {
        startX = e.clientX;
        startWidth = headerEl.offsetWidth;
        document.addEventListener('pointermove', onMove);
        document.addEventListener('pointerup', onUp, { once: true });
        e.preventDefault();
    });

    function onMove(e) {
        const newWidth = Math.max(50, startWidth + (e.clientX - startX));
        headerEl.style.width = `${newWidth}px`;
    }
    function onUp(e) {
        document.removeEventListener('pointermove', onMove);
        const finalWidth = headerEl.offsetWidth;
        dotNetRef.invokeMethodAsync('OnColumnResized', columnId, finalWidth);
    }
}
```

### Consumer Usage (Styled)

```razor
<DataTable Items="@_users" MultiSelect="true" @bind-Selection="_selected">
    <DataTableHeader>
        <DataTableSelectAll />
        @foreach (var col in context.Columns)
        {
            <DataTableHeaderCell Column="@col">
                @col.Header
                @if (col.Sortable)
                {
                    <DataTableSortIndicator Column="@col" />
                }
            </DataTableHeaderCell>
        }
    </DataTableHeader>
    <DataTableBody>
        <DataTableRow Context="row">
            <DataTableSelectCell Item="@row" />
            <DataTableCell>@row.Name</DataTableCell>
            <DataTableCell>@row.Email</DataTableCell>
            <DataTableCell>
                <Badge variant="@(row.IsActive ? "default" : "secondary")">
                    @(row.IsActive ? "Active" : "Inactive")
                </Badge>
            </DataTableCell>
            <DataTableCell>
                <DropdownMenu>
                    <DropdownMenuTrigger>
                        <Button variant="ghost" size="icon"><MoreHorizontalIcon /></Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                        <DropdownMenuItem OnClick="@(() => Edit(row))">Edit</DropdownMenuItem>
                        <DropdownMenuItem OnClick="@(() => Delete(row))"
                                          class="text-destructive">Delete</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </DataTableCell>
        </DataTableRow>
    </DataTableBody>
    <DataTablePagination />
</DataTable>
```

### ARIA Attributes

| Attribute | Applied To | Value |
|-----------|-----------|-------|
| `role="grid"` | `<table>` | Interactive grid (when sortable/selectable) |
| `role="row"` | `<tr>` | Row in grid |
| `role="columnheader"` | `<th>` | Column header |
| `role="gridcell"` | `<td>` | Data cell |
| `aria-sort` | `<th>` | `"ascending"`, `"descending"`, or `"none"` |
| `aria-selected` | `<tr>` | `"true"` when row is selected |
| `aria-rowcount` | `<table>` | Total row count (for virtualized/paginated tables) |
| `aria-rowindex` | `<tr>` | Absolute row index |
| `aria-label` | pagination buttons | `"Go to page 3"`, `"Next page"` |

### Keyboard Interactions

| Key | Action |
|-----|--------|
| `Arrow Up/Down` | Move focus between rows |
| `Arrow Left/Right` | Move focus between cells |
| `Space` | Toggle row selection |
| `Shift+Space` | Range selection |
| `Ctrl+A` / `Cmd+A` | Select all |
| `Enter` | Activate row action / sort column |
| `Home` / `End` | First / last cell in row |
| `Ctrl+Home` / `Ctrl+End` | First / last row |

### SSR Considerations

- In SSR mode, the table renders as a static `<table>` with data visible. No sorting, filtering, or selection interactivity.
- Sorting and filtering can work via server POST with enhanced navigation (query string parameters: `?sort=name&dir=asc&page=2`).
- Column resizing requires interactive mode (pointer events in JS).
- Virtualization requires interactive mode (`Virtualize<T>` needs SignalR or WASM).

---

## 4. Command Palette / Command Menu

### Background

The command palette (cmdk pattern) is a keyboard-driven search interface for navigating and executing actions. In the React ecosystem, `cmdk` by Paco Coursey is the reference implementation. shadcn/ui's `<Command>` wraps it.

### Architecture

The Command Menu composes on top of two existing primitives: **Dialog** and **Combobox**.

```
Dialog (overlay, focus trap, escape-to-close)
  +-- CommandMenu (search + filtered list + keyboard navigation)
        +-- CommandInput (search box)
        +-- CommandList (scrollable results)
        |   +-- CommandGroup (labeled section)
        |   |   +-- CommandItem (individual action)
        |   +-- CommandEmpty (no results message)
        +-- CommandSeparator
```

### Context Type

```csharp
namespace BlazingSpire.Primitives.Command;

public sealed class CommandContext
{
    public string SearchTerm { get; internal set; } = "";
    public int ActiveIndex { get; internal set; } = -1;
    public IReadOnlyList<CommandItemRegistration> FilteredItems { get; internal set; } = [];
}

public sealed class CommandItemRegistration
{
    public string Id { get; init; } = default!;
    public string Label { get; init; } = default!;
    public string? Group { get; init; }

    /// <summary>
    /// Keywords that participate in fuzzy matching beyond the label.
    /// </summary>
    public IReadOnlyList<string> Keywords { get; init; } = [];

    public bool Disabled { get; init; }
    public object? Value { get; init; }
}
```

### Primitive Component

```csharp
// CommandMenu.razor
public partial class CommandMenu : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool ShouldFilter { get; set; } = true;
    [Parameter] public EventCallback<string> OnSearchChanged { get; set; }
    [Parameter] public EventCallback<CommandItemRegistration> OnSelect { get; set; }

    /// <summary>
    /// Custom filter function. Default: built-in fuzzy match.
    /// Return 0 for no match, higher values for better matches.
    /// </summary>
    [Parameter] public Func<string, string, int>? Filter { get; set; }

    /// <summary>
    /// When true, wraps in a Dialog. When false, renders inline (for embedding).
    /// </summary>
    [Parameter] public bool AsDialog { get; set; } = true;

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    private CommandContext _context = new();
    private List<CommandItemRegistration> _allItems = [];
}
```

```csharp
// CommandInput.razor
public partial class CommandInput : ComponentBase
{
    [CascadingParameter] private CommandContext Context { get; set; } = default!;
    [Parameter] public string? Placeholder { get; set; }
    // Renders <input> with role="combobox", aria-expanded, aria-controls, aria-activedescendant
}

// CommandList.razor
public partial class CommandList : ComponentBase
{
    [CascadingParameter] private CommandContext Context { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Renders <div role="listbox"> with aria-label
}

// CommandGroup.razor
public partial class CommandGroup : ComponentBase
{
    [Parameter] public string? Heading { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Renders <div role="group" aria-labelledby="..."><div role="presentation">heading</div>...</div>
    // Hidden when all children are filtered out
}

// CommandItem.razor
public partial class CommandItem : ComponentBase
{
    [CascadingParameter] private CommandMenu Parent { get; set; } = default!;
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string? Value { get; set; }
    [Parameter] public IReadOnlyList<string>? Keywords { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback OnSelect { get; set; }
    // Registers with parent on init, renders <div role="option" aria-selected aria-disabled>
}

// CommandEmpty.razor
public partial class CommandEmpty : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    // Renders only when filtered items count is 0
}
```

### Fuzzy Search Implementation

```csharp
/// <summary>
/// Simple fuzzy match scoring. Characters must appear in order but not contiguously.
/// Higher score = better match. 0 = no match.
/// </summary>
public static class FuzzyMatch
{
    public static int Score(string query, string target)
    {
        if (string.IsNullOrEmpty(query)) return 1; // empty query matches everything
        if (string.IsNullOrEmpty(target)) return 0;

        var querySpan = query.AsSpan();
        var targetSpan = target.AsSpan();
        int score = 0;
        int queryIdx = 0;
        int lastMatchIdx = -1;

        for (int i = 0; i < targetSpan.Length && queryIdx < querySpan.Length; i++)
        {
            if (char.ToLowerInvariant(targetSpan[i]) == char.ToLowerInvariant(querySpan[queryIdx]))
            {
                score += 1;
                // Bonus for consecutive matches
                if (lastMatchIdx == i - 1) score += 3;
                // Bonus for matching at word boundary
                if (i == 0 || targetSpan[i - 1] == ' ' || targetSpan[i - 1] == '-') score += 2;
                // Bonus for exact case match
                if (targetSpan[i] == querySpan[queryIdx]) score += 1;
                lastMatchIdx = i;
                queryIdx++;
            }
        }

        return queryIdx == querySpan.Length ? score : 0; // all query chars must be found
    }
}
```

### Keyboard Interactions

| Key | Action |
|-----|--------|
| `Ctrl+K` / `Cmd+K` | Open command palette (global hotkey, registered via JS) |
| `Escape` | Close palette |
| `Arrow Up/Down` | Navigate items |
| `Enter` | Execute selected item |
| `Home` / `End` | Jump to first / last item |
| Type any character | Filter list (search input is auto-focused) |

The global `Ctrl+K` hotkey is registered via a small JS module:

```js
// command-hotkey.js
export function registerHotkey(dotNetRef) {
    function handler(e) {
        if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OnHotkeyPressed');
        }
    }
    document.addEventListener('keydown', handler);
    return { dispose() { document.removeEventListener('keydown', handler); } };
}
```

### Consumer Usage

```razor
<CommandMenu @bind-Open="_commandOpen" Placeholder="Type a command or search...">
    <CommandInput />
    <CommandList>
        <CommandGroup Heading="Navigation">
            <CommandItem OnSelect="@(() => Nav.NavigateTo("/dashboard"))"
                         Keywords='@(["home", "main"])'>
                <HomeIcon class="mr-2 h-4 w-4" /> Dashboard
            </CommandItem>
            <CommandItem OnSelect="@(() => Nav.NavigateTo("/settings"))"
                         Keywords='@(["preferences", "config"])'>
                <SettingsIcon class="mr-2 h-4 w-4" /> Settings
            </CommandItem>
        </CommandGroup>
        <CommandSeparator />
        <CommandGroup Heading="Actions">
            <CommandItem OnSelect="@(() => CreateNewDocument())">
                <PlusIcon class="mr-2 h-4 w-4" /> New Document
            </CommandItem>
            <CommandItem OnSelect="@(() => ToggleTheme())">
                <SunMoonIcon class="mr-2 h-4 w-4" /> Toggle Theme
            </CommandItem>
        </CommandGroup>
        <CommandEmpty>No results found.</CommandEmpty>
    </CommandList>
</CommandMenu>
```

### SSR Considerations

- The command palette requires interactive mode (keyboard navigation, real-time filtering, focus management).
- In SSR, omit the `Ctrl+K` trigger or degrade to a `<a href="/search">` link.
- The component renders nothing in SSR mode when `Open=false` (no hidden DOM cost).

---

## 5. Combobox Multi-Select

### Architecture

Extends the existing Combobox primitive with multi-select behavior. The single-select Combobox already handles keyboard navigation, filtering, and ARIA. Multi-select adds:

- Multiple selected values stored as `IReadOnlyList<TValue>`
- Tag/chip display in the trigger area
- Individual tag removal (X button or Backspace key)
- Input remains active for continued searching after selection

### Context Extension

```csharp
namespace BlazingSpire.Primitives.Combobox;

public sealed class ComboboxContext<TValue>
{
    // Existing single-select properties...
    public bool IsOpen { get; internal set; }
    public string SearchTerm { get; internal set; } = "";
    public int ActiveIndex { get; internal set; } = -1;

    // Multi-select additions:
    public bool Multiple { get; internal set; }
    public IReadOnlyList<TValue> SelectedValues { get; internal set; } = [];

    public bool IsSelected(TValue value) =>
        Multiple
            ? SelectedValues.Contains(value)
            : EqualityComparer<TValue>.Default.Equals(SelectedValues.FirstOrDefault(), value);
}
```

### Component Parameters

```csharp
// ComboboxMulti.razor -- extends Combobox with multi-select behavior
[CascadingTypeParameter(nameof(TValue))]
public partial class ComboboxMulti<TValue> : ComponentBase
{
    [Parameter] public IReadOnlyList<TValue> Values { get; set; } = [];
    [Parameter] public EventCallback<IReadOnlyList<TValue>> ValuesChanged { get; set; }
    [Parameter] public int? MaxSelections { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public RenderFragment<TValue>? TagTemplate { get; set; }
    [Parameter] public Func<TValue, string>? DisplayMember { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ClearSearchOnSelect { get; set; } = true;

    /// <summary>
    /// Whether to close the dropdown after each selection. Default false for multi-select.
    /// </summary>
    [Parameter] public bool CloseOnSelect { get; set; }

    private async Task ToggleValueAsync(TValue value)
    {
        var list = Values.ToList();
        if (list.Contains(value))
            list.Remove(value);
        else if (MaxSelections is null || list.Count < MaxSelections)
            list.Add(value);

        await ValuesChanged.InvokeAsync(list.AsReadOnly());
    }

    private async Task RemoveValueAsync(TValue value)
    {
        var list = Values.Where(v => !EqualityComparer<TValue>.Default.Equals(v, value)).ToList();
        await ValuesChanged.InvokeAsync(list.AsReadOnly());
    }
}
```

### Tag/Chip Display

```razor
@* ComboboxMultiTrigger.razor -- styled component *@
<div role="combobox"
     aria-expanded="@Context.IsOpen"
     aria-haspopup="listbox"
     aria-multiselectable="true"
     class="flex flex-wrap items-center gap-1 rounded-md border border-input px-3 py-2
            ring-offset-background focus-within:ring-2 focus-within:ring-ring">

    @foreach (var value in Context.SelectedValues)
    {
        <span class="inline-flex items-center gap-1 rounded-md bg-secondary px-2 py-0.5
                     text-xs font-medium text-secondary-foreground">
            @if (TagTemplate is not null)
            {
                @TagTemplate(value)
            }
            else
            {
                @(DisplayMember?.Invoke(value) ?? value?.ToString())
            }
            <button type="button"
                    aria-label="@($"Remove {DisplayMember?.Invoke(value) ?? value}")"
                    class="ml-1 rounded-full hover:bg-muted"
                    @onclick="@(() => RemoveValueAsync(value))"
                    @onclick:stopPropagation="true"
                    tabindex="-1">
                <XIcon class="h-3 w-3" />
            </button>
        </span>
    }

    <input type="text"
           @bind-value="Context.SearchTerm"
           @bind-value:event="oninput"
           placeholder="@(Context.SelectedValues.Count == 0 ? Placeholder : null)"
           class="flex-1 min-w-[80px] bg-transparent text-sm outline-none placeholder:text-muted-foreground"
           role="searchbox"
           aria-autocomplete="list"
           aria-controls="@ListboxId"
           aria-activedescendant="@ActiveDescendantId" />
</div>
```

### Keyboard Interactions

| Key | Action |
|-----|--------|
| `Arrow Down/Up` | Navigate options |
| `Enter` | Toggle selection on focused option |
| `Backspace` (empty input) | Remove last tag |
| `Escape` | Close dropdown |
| `Delete` (on focused tag) | Remove that tag |
| `Arrow Left/Right` (empty input) | Navigate between tags |
| `Ctrl+A` / `Cmd+A` | Select all options (if within max) |

### ARIA Attributes

| Attribute | Applied To | Value |
|-----------|-----------|-------|
| `role="combobox"` | trigger wrapper | Identifies as combobox |
| `aria-multiselectable="true"` | trigger wrapper | Indicates multi-select |
| `aria-expanded` | trigger wrapper | `"true"` when open |
| `aria-haspopup="listbox"` | trigger wrapper | Popup type |
| `role="listbox"` | dropdown list | List of options |
| `role="option"` | each option | Selectable item |
| `aria-selected` | each option | `"true"` when selected |
| `aria-activedescendant` | input | ID of focused option |
| `aria-label` | remove button | `"Remove {value}"` |
| `aria-live="polite"` | hidden status | `"3 of 10 selected"` |

### EditForm Integration

```csharp
// For binding to EditForm models, provide a wrapper that implements InputBase<T> semantics:
public partial class InputComboboxMulti<TValue> : InputBase<IReadOnlyList<TValue>>
{
    [Parameter] public IReadOnlyList<TValue> Options { get; set; } = [];
    [Parameter] public Func<TValue, string>? DisplayMember { get; set; }
    [Parameter] public RenderFragment<TValue>? ItemTemplate { get; set; }

    // Internally renders <ComboboxMulti> and translates ValuesChanged to CurrentValue
    protected override bool TryParseValueFromString(string? value, out IReadOnlyList<TValue> result,
        out string? validationErrorMessage)
    {
        // Not applicable -- selection is object-based, not string-parsed.
        result = CurrentValue ?? [];
        validationErrorMessage = null;
        return true;
    }
}
```

**Consumer usage:**

```razor
<EditForm Model="@_model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />

    <FormField For="@(() => _model.Tags)">
        <FormFieldLabel>Tags</FormFieldLabel>
        <FormFieldControl>
            <InputComboboxMulti @bind-Value="_model.Tags"
                                Options="@_availableTags"
                                DisplayMember="@(t => t.Name)"
                                Placeholder="Select tags..."
                                MaxSelections="5">
                <ItemTemplate Context="tag">
                    <span class="flex items-center gap-2">
                        <span class="h-2 w-2 rounded-full" style="background: @tag.Color" />
                        @tag.Name
                    </span>
                </ItemTemplate>
            </InputComboboxMulti>
        </FormFieldControl>
        <FormFieldDescription>Select up to 5 tags.</FormFieldDescription>
        <FormFieldError />
    </FormField>
</EditForm>
```

### SSR Considerations

- Renders as a static `<div>` displaying the currently selected tags as badges and a disabled input.
- Without interactive mode, no dropdown opens. Consider a `<select multiple>` fallback for basic SSR functionality.
- The `<noscript>` or SSR path can render a standard `<select multiple>` with the same `name` attribute for form POST compatibility.

---

## 6. Toast / Notification System

### Architecture

Toasts must work across render mode boundaries (a static SSR page triggers a toast that displays in an interactive layout region). This requires a **service-based architecture** matching the dual-API pattern established for Dialog in 02-architecture.md.

```
ToastService (singleton/scoped DI)
  |
  +-- Show("Message") -- fire-and-forget
  +-- Show<TComponent>() -- custom toast content
  +-- Dismiss(toastId) -- programmatic dismiss
  +-- DismissAll()
  |
  v
ToastProvider (in layout, interactive mode)
  +-- subscribes to ToastService events
  +-- manages toast queue and stacking
  +-- renders ToastViewport with positioned toasts
  +-- handles auto-dismiss timers
```

### Service API

```csharp
namespace BlazingSpire.Primitives.Toast;

public interface IToastService
{
    /// <summary>
    /// Show a simple text toast.
    /// </summary>
    string Show(string message, ToastOptions? options = null);

    /// <summary>
    /// Show a toast with title and description.
    /// </summary>
    string Show(string title, string description, ToastOptions? options = null);

    /// <summary>
    /// Show a toast with a custom component as content.
    /// </summary>
    string Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
        IDictionary<string, object>? parameters = null,
        ToastOptions? options = null) where TComponent : IComponent;

    void Dismiss(string toastId);
    void DismissAll();

    event Action<ToastEvent>? OnToastEvent;
}

public sealed class ToastOptions
{
    public ToastVariant Variant { get; init; } = ToastVariant.Default;
    public TimeSpan? Duration { get; init; }          // null = use default (5s)
    public bool Persistent { get; init; }              // true = no auto-dismiss
    public ToastPosition Position { get; init; } = ToastPosition.BottomRight;
    public RenderFragment? Action { get; init; }       // e.g., "Undo" button
}

public enum ToastVariant { Default, Success, Error, Warning, Info }

public enum ToastPosition
{
    TopLeft, TopCenter, TopRight,
    BottomLeft, BottomCenter, BottomRight
}

public sealed class ToastEvent
{
    public required string Id { get; init; }
    public required ToastEventType Type { get; init; }
    public ToastInstance? Toast { get; init; }
}

public enum ToastEventType { Added, Dismissed, DismissAll }

public sealed class ToastInstance
{
    public required string Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public Type? ComponentType { get; init; }
    public IDictionary<string, object>? ComponentParameters { get; init; }
    public required ToastOptions Options { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
```

### ToastService Implementation

```csharp
public sealed class ToastService : IToastService
{
    private readonly ConcurrentDictionary<string, ToastInstance> _toasts = new();

    /// <summary>
    /// Maximum number of visible toasts. Older toasts are dismissed when exceeded.
    /// </summary>
    public int MaxVisible { get; set; } = 5;

    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromSeconds(5);

    public event Action<ToastEvent>? OnToastEvent;

    public string Show(string message, ToastOptions? options = null)
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var toast = new ToastInstance
        {
            Id = id,
            Description = message,
            Options = options ?? new()
        };
        _toasts.TryAdd(id, toast);
        EnforceMaxVisible();
        OnToastEvent?.Invoke(new ToastEvent { Id = id, Type = ToastEventType.Added, Toast = toast });
        return id;
    }

    // ... other Show overloads follow same pattern

    public void Dismiss(string toastId)
    {
        if (_toasts.TryRemove(toastId, out _))
        {
            OnToastEvent?.Invoke(new ToastEvent { Id = toastId, Type = ToastEventType.Dismissed });
        }
    }

    private void EnforceMaxVisible()
    {
        while (_toasts.Count > MaxVisible)
        {
            var oldest = _toasts.OrderBy(t => t.Value.CreatedAt).First();
            Dismiss(oldest.Key);
        }
    }
}
```

### DI Registration

```csharp
// In BlazingSpire service registration
public static IServiceCollection AddBlazingSpireToasts(this IServiceCollection services)
{
    // Scoped for Server (per-circuit), but can be Singleton for WASM
    services.AddScoped<IToastService, ToastService>();
    return services;
}
```

**Cross-boundary note:** For static SSR pages to trigger toasts in an interactive layout, the `ToastService` must be registered as **scoped** (shared within the HTTP request / SignalR circuit). The static SSR handler calls `ToastService.Show(...)` and the interactive `ToastProvider` in the layout picks it up via the same scoped instance. This works because in .NET 10's per-page interactivity model, the layout's interactive island shares the same DI scope as the SSR page during the initial request.

### ToastProvider Component

```csharp
// ToastProvider.razor -- placed in MainLayout.razor
public partial class ToastProvider : ComponentBase, IDisposable
{
    [Inject] private IToastService ToastService { get; set; } = default!;

    [Parameter] public ToastPosition Position { get; set; } = ToastPosition.BottomRight;
    [Parameter] public int MaxVisible { get; set; } = 5;

    private readonly List<ToastInstance> _visible = [];
    private readonly Dictionary<string, Timer> _timers = new();

    protected override void OnInitialized()
    {
        ToastService.OnToastEvent += HandleToastEvent;
    }

    private void HandleToastEvent(ToastEvent e)
    {
        switch (e.Type)
        {
            case ToastEventType.Added when e.Toast is not null:
                _visible.Add(e.Toast);
                if (!e.Toast.Options.Persistent)
                {
                    var duration = e.Toast.Options.Duration
                        ?? ((ToastService)ToastService).DefaultDuration;
                    StartDismissTimer(e.Toast.Id, duration);
                }
                break;

            case ToastEventType.Dismissed:
                _visible.RemoveAll(t => t.Id == e.Id);
                StopDismissTimer(e.Id);
                break;

            case ToastEventType.DismissAll:
                _visible.Clear();
                ClearAllTimers();
                break;
        }

        InvokeAsync(StateHasChanged);
    }

    private void StartDismissTimer(string id, TimeSpan duration)
    {
        var timer = new Timer(_ => ToastService.Dismiss(id), null, duration, Timeout.InfiniteTimeSpan);
        _timers[id] = timer;
    }

    // Pause timer on hover (via JS interop callback)
    [JSInvokable]
    public void OnToastPointerEnter(string toastId) { /* pause timer */ }

    [JSInvokable]
    public void OnToastPointerLeave(string toastId) { /* resume timer */ }

    public void Dispose()
    {
        ToastService.OnToastEvent -= HandleToastEvent;
        ClearAllTimers();
    }
}
```

### Toast Viewport Rendering

```razor
@* ToastViewport.razor -- styled component *@
<div class="@PositionClasses()"
     aria-live="polite"
     aria-label="Notifications"
     role="region"
     tabindex="-1">

    @foreach (var toast in _visible)
    {
        <div @key="toast.Id"
             class="@ToastClasses(toast)"
             role="status"
             aria-atomic="true"
             data-toast-id="@toast.Id"
             @onpointerenter="@(() => OnPointerEnter(toast.Id))"
             @onpointerleave="@(() => OnPointerLeave(toast.Id))">

            <div class="grid gap-1">
                @if (toast.Title is not null)
                {
                    <div class="text-sm font-semibold">@toast.Title</div>
                }
                @if (toast.Description is not null)
                {
                    <div class="text-sm text-muted-foreground">@toast.Description</div>
                }
                @if (toast.ComponentType is not null)
                {
                    <DynamicComponent Type="@toast.ComponentType"
                                      Parameters="@toast.ComponentParameters" />
                }
            </div>

            @if (toast.Options.Action is not null)
            {
                <div class="ml-auto">@toast.Options.Action</div>
            }

            <button type="button"
                    class="absolute right-1 top-1 rounded-md p-1 opacity-0
                           group-hover:opacity-100 transition-opacity"
                    aria-label="Dismiss notification"
                    @onclick="@(() => ToastService.Dismiss(toast.Id))">
                <XIcon class="h-4 w-4" />
            </button>
        </div>
    }
</div>

@code {
    private string PositionClasses() => Position switch
    {
        ToastPosition.TopRight    => "fixed top-4 right-4 z-[100] flex flex-col gap-2 w-[380px]",
        ToastPosition.TopLeft     => "fixed top-4 left-4 z-[100] flex flex-col gap-2 w-[380px]",
        ToastPosition.TopCenter   => "fixed top-4 left-1/2 -translate-x-1/2 z-[100] flex flex-col gap-2 w-[380px]",
        ToastPosition.BottomRight => "fixed bottom-4 right-4 z-[100] flex flex-col-reverse gap-2 w-[380px]",
        ToastPosition.BottomLeft  => "fixed bottom-4 left-4 z-[100] flex flex-col-reverse gap-2 w-[380px]",
        ToastPosition.BottomCenter => "fixed bottom-4 left-1/2 -translate-x-1/2 z-[100] flex flex-col-reverse gap-2 w-[380px]",
        _ => "fixed bottom-4 right-4 z-[100] flex flex-col-reverse gap-2 w-[380px]"
    };
}
```

### Stacking & Queue Management

- **Max visible** defaults to 5. When a new toast arrives and the limit is reached, the oldest non-persistent toast is auto-dismissed.
- **Stacking order**: newest toasts appear closest to the viewport edge. Bottom positions use `flex-col-reverse` so new toasts slide in from the bottom.
- **Animation**: enter via `animate-in slide-in-from-bottom-2 fade-in-0`, exit via `animate-out slide-out-to-right-2 fade-out-0`. Uses `@starting-style` for pure CSS enter animations. Exit handled by a CSS class toggle + `transitionend` listener.
- **Hover pause**: pointer enter pauses the auto-dismiss timer; pointer leave resumes with remaining time.
- **Swipe to dismiss** (touch): JS handler tracks horizontal swipe on toast element, dismisses on sufficient drag distance.

### ARIA & Accessibility

| Attribute | Applied To | Value |
|-----------|-----------|-------|
| `aria-live="polite"` | toast viewport | Screen readers announce new toasts without interrupting |
| `role="region"` | toast viewport | Landmark for the notification area |
| `aria-label="Notifications"` | toast viewport | Names the region |
| `role="status"` | individual toast | Identifies as a status message |
| `aria-atomic="true"` | individual toast | Entire toast is read as one unit |
| `aria-label="Dismiss notification"` | close button | Accessible name for dismiss |

**Note:** Use `aria-live="polite"` (not `"assertive"`) for toasts. Assertive interrupts whatever the screen reader is currently saying. Reserve assertive for form validation errors (in `FormFieldError`). Toasts are supplementary information.

### Consumer Usage

```razor
@* In MainLayout.razor *@
<main>@Body</main>
<ToastProvider Position="ToastPosition.BottomRight" />

@* In any page or component (even static SSR) *@
@inject IToastService Toast

<Button @onclick="HandleSave">Save</Button>

@code {
    private async Task HandleSave()
    {
        await SaveDataAsync();

        Toast.Show("Changes saved", new ToastOptions
        {
            Variant = ToastVariant.Success,
            Duration = TimeSpan.FromSeconds(3)
        });
    }

    private void HandleDelete()
    {
        var id = Toast.Show("Item deleted", "This action can be undone.", new ToastOptions
        {
            Variant = ToastVariant.Warning,
            Duration = TimeSpan.FromSeconds(8),
            Action = @<Button variant="outline" size="sm" @onclick="UndoDelete">Undo</Button>
        });
    }
}
```

**Custom toast component:**

```razor
@* DownloadProgressToast.razor *@
<div class="flex items-center gap-3">
    <Spinner class="h-4 w-4" />
    <div class="flex-1">
        <p class="text-sm font-medium">Downloading @FileName...</p>
        <Progress Value="@Percent" class="mt-2 h-1.5" />
    </div>
</div>

@code {
    [Parameter] public string FileName { get; set; } = "";
    [Parameter] public int Percent { get; set; }
}
```

```csharp
Toast.Show<DownloadProgressToast>(
    new Dictionary<string, object>
    {
        ["FileName"] = "report.pdf",
        ["Percent"] = 0
    },
    new ToastOptions { Persistent = true });
```

### SSR Considerations

- `ToastProvider` must be in an interactive render mode (it subscribes to events, manages timers, handles pointer events).
- `ToastService.Show(...)` can be called from static SSR code-behind. The scoped service instance is shared with the interactive `ToastProvider` in the layout.
- On the very first SSR render (before hydration), toasts queued during `OnInitialized` will display once the `ToastProvider` hydrates and processes the backlog.
- Toasts do not render any HTML in the static SSR pass. They only appear after the interactive island activates.

---

## Cross-Cutting Concerns

### Render Mode Summary

| Pattern | SSR (Static) | Interactive Required |
|---------|-------------|---------------------|
| FormField composition | Full support | No |
| Form layouts (vertical/horizontal) | Full support | No |
| Form wizard (multi-step) | Step 1 only, or all expanded | Yes for step navigation |
| DataTable (static display) | Full support | No |
| DataTable (sort/filter/select) | Via query string + enhanced nav | Yes for client-side |
| Command Palette | Not rendered | Yes |
| Combobox Multi-Select | Fallback `<select multiple>` | Yes |
| Toast notifications | Not rendered until hydration | Yes (ToastProvider) |

### AOT / Trimming Safety

- `ToastService.Show<TComponent>()` and `DialogService.Show<TComponent>()` use `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]` on the type parameter to preserve component metadata for `DynamicComponent`.
- `FormField<TValue>` uses `Expression<Func<TValue>>` for the `For` parameter, which is reflection-based but trim-safe because `FieldIdentifier.Create()` only reads the expression tree (not invoking via reflection).
- `FuzzyMatch.Score()` is pure computation with no reflection.
- All `EventCallback` and `RenderFragment` types are delegate-based and trim-safe.

### Performance Considerations

- `FormFieldContext` is a class (not struct) to allow mutation without copying through the cascade.
- `DataTableContext<TItem>.FilteredItems` should use `ImmutableArray<T>` or `FrozenSet<T>` for the selection set to avoid allocation on every render.
- Toast `Timer` instances must be disposed properly to avoid GC handle leaks in Server mode (each timer pins a callback).
- Fuzzy search should debounce in the Command Menu input (150-200ms) to avoid filtering on every keystroke in large item sets.
- DataTable column resize uses JS-only pointer tracking (no interop per-pixel) and only calls .NET once on `pointerup`.
