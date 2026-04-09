# Demo Page Template

Standard layout and conventions for all 47 component demo pages in `src/BlazingSpire.Demo/`.

---

## 1. Route Convention

```
/components/<lowercase-hyphenated>
```

Examples: `/components/button`, `/components/alert-dialog`, `/components/toggle-group`, `/components/dropdown-menu`

---

## 2. File Location

```
src/BlazingSpire.Demo/Components/Pages/<PascalCase>Page.razor
```

Examples:
- `ButtonPage.razor` for `/components/button`
- `AlertDialogPage.razor` for `/components/alert-dialog`
- `ToggleGroupPage.razor` for `/components/toggle-group`

One file per page. No `.razor.cs` code-behind unless the page needs significant C# logic (rare -- prefer `@code` blocks).

---

## 3. Standard Page Structure

Every demo page follows this exact structure with these exact Tailwind classes:

```razor
@page "/components/<lowercase-hyphenated>"
@using BlazingSpire.Demo.Components.UI

<PageTitle><ComponentName> - BlazingSpire</PageTitle>

<div class="space-y-8">

    @* ── Header ─────────────────────────────── *@
    <div class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight">Component Name</h1>
        <p class="text-lg text-muted-foreground">One-line description of what the component does.</p>
    </div>

    @* ── Preview ────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Preview</h2>
        <div class="flex items-center justify-center rounded-xl border bg-muted/30 p-8">
            @* Live component example here *@
        </div>
    </div>

    @* ── Code ───────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Code</h2>
        <pre class="language-xml"><code class="language-xml">@* HTML-entity-encoded Blazor markup *@</code></pre>
    </div>

    @* ── Variants ───────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Variants</h2>
        <div class="flex flex-wrap items-center gap-4 rounded-xl border bg-muted/30 p-8">
            @* Each variant with a small label *@
        </div>
    </div>

    @* ── Sizes (if applicable) ──────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Sizes</h2>
        <div class="flex flex-wrap items-center gap-4 rounded-xl border bg-muted/30 p-8">
            @* Each size with a small label *@
        </div>
    </div>

    @* ── Additional examples (optional) ─────── *@

</div>
```

### Key layout rules

- Outer wrapper: `<div class="space-y-8">` -- consistent vertical rhythm between sections
- Section wrapper: `<div class="space-y-3">` -- tight spacing between heading and content
- Section heading: `<h2 class="text-xl font-semibold tracking-tight">`
- Demo container: `<div class="... rounded-xl border bg-muted/30 p-8">` -- bordered, muted background, generous padding
- Use `flex items-center justify-center` for single-component previews
- Use `flex flex-wrap items-center gap-4` for variant/size grids
- Use `grid` layouts when examples need more structure (e.g., form components)

### Variant labels

Show small labels above or beside each variant using a wrapper:

```razor
<div class="flex flex-col items-center gap-2">
    <span class="text-xs text-muted-foreground">Destructive</span>
    <Badge Variant="BadgeVariant.Destructive">Destructive</Badge>
</div>
```

---

## 4. Code Snippet Format

Code snippets use `<pre class="language-xml"><code class="language-xml">` with Prism.js highlighting (loaded at build time). All Blazor markup must be HTML-entity-encoded.

### Encoding rules

| Character | Encode as |
|-----------|-----------|
| `<` | `&lt;` |
| `>` | `&gt;` |
| `"` | `&quot;` |
| `&` | `&amp;` |
| `@` | `@@` — **MUST be doubled** — Razor interprets `@` everywhere, including inside `<pre>` |

### Example: simple component

```razor
<pre class="language-xml"><code class="language-xml">&lt;Badge&gt;Default&lt;/Badge&gt;
&lt;Badge Variant=&quot;BadgeVariant.Secondary&quot;&gt;Secondary&lt;/Badge&gt;
&lt;Badge Variant=&quot;BadgeVariant.Destructive&quot;&gt;Destructive&lt;/Badge&gt;
&lt;Badge Variant=&quot;BadgeVariant.Outline&quot;&gt;Outline&lt;/Badge&gt;</code></pre>
```

### Critical: `@` escaping inside `<pre>` blocks

Razor processes `@` tokens **everywhere** in `.razor` files — including inside `<pre>` and `<code>` elements. The Razor parser runs before HTML parsing, so there is no "safe" HTML context.

**Any `@` in a code snippet must be escaped.** Use `&#64;` (HTML entity for `@`) as the **preferred escape** — it is always treated as literal text by Razor. The `@@` escape works for most cases but **fails for `@bind-` prefixed attributes**: Razor recognizes `bind-` as a reserved keyword even after `@@`, causing compile errors like `CS0103: The name 'bind' does not exist`.

| What you want to show | Preferred (always safe) | `@@` alternative |
|-----------------------|-------------------------|-----------------|
| `@bind-Value="..."` | `&#64;bind-Value="..."` | ❌ `@@bind-Value` — **FAILS** |
| `@bind-IsOpen="..."` | `&#64;bind-IsOpen="..."` | ❌ `@@bind-IsOpen` — **FAILS** |
| `@onclick="..."` | `&#64;onclick="..."` | `@@onclick` (works) |
| `@code { ... }` | `&#64;code { ... }` | `@@code` (works) |
| `@inject SomeService` | `&#64;inject SomeService` | `@@inject` (works) |
| `@if (condition) { }` | `&#64;if (condition) { }` | `@@if` (works) |
| `@foreach (var x in xs)` | `&#64;foreach (var x in xs)` | `@@foreach` (works) |

**Rule of thumb:** Use `&#64;` for all `@` in code snippets — it avoids the `@@bind-` failure case entirely.

This applies **only** to `@` inside code snippets (`<pre><code>`). In live component markup outside `<pre>`, use `@bind-Value` and `@onclick` normally — Razor processes them as real bindings.

### Example: component with events/binding

```razor
<pre class="language-xml"><code class="language-xml">&lt;Button @@onclick=&quot;HandleClick&quot;&gt;Click me&lt;/Button&gt;
&lt;Button Variant=&quot;ButtonVariant.Outline&quot; Disabled=&quot;true&quot;&gt;Disabled&lt;/Button&gt;</code></pre>
```

### Example: composition (nested components)

```razor
<pre class="language-xml"><code class="language-xml">&lt;Dialog @@bind-IsOpen=&quot;_isOpen&quot;&gt;
    &lt;DialogTrigger&gt;
        &lt;Button Variant=&quot;ButtonVariant.Outline&quot;&gt;Open Dialog&lt;/Button&gt;
    &lt;/DialogTrigger&gt;
    &lt;DialogContent&gt;
        &lt;DialogHeader&gt;
            &lt;DialogTitle&gt;Edit Profile&lt;/DialogTitle&gt;
            &lt;DialogDescription&gt;Make changes to your profile.&lt;/DialogDescription&gt;
        &lt;/DialogHeader&gt;
        &lt;DialogFooter&gt;
            &lt;DialogClose&gt;
                &lt;Button Variant=&quot;ButtonVariant.Outline&quot;&gt;Cancel&lt;/Button&gt;
            &lt;/DialogClose&gt;
            &lt;Button&gt;Save changes&lt;/Button&gt;
        &lt;/DialogFooter&gt;
    &lt;/DialogContent&gt;
&lt;/Dialog&gt;</code></pre>
```

### Formatting rules

- No leading whitespace on the first line (starts immediately after `<code ...>`)
- Indent nested elements with 4 spaces
- No trailing whitespace on the last line (ends immediately before `</code>`)
- Keep snippets concise -- show the minimal usage, not every parameter

---

## 5. NavMenu Integration

The sidebar lives in `src/BlazingSpire.Demo/Components/Layout/DocsSidebar.razor`. Components are organized into 5 category groups. To add a new component page, add a `<SidebarLink>` in the correct category group:

```razor
@* ── Forms ───────────────────────────────── *@
<div>
    <h4 class="mb-1 px-6 text-xs font-semibold uppercase tracking-wider text-muted-foreground">Forms</h4>
    <div class="space-y-0.5 px-3">
        <SidebarLink Href="/components/button">Button</SidebarLink>
        @* ... existing entries ... *@
    </div>
</div>
```

The `<SidebarLink>` component handles active styling automatically via `NavLink` with `NavLinkMatch.All`. The categories are:

1. **Layout** -- Aspect Ratio, Scroll Area, Separator, Sidebar
2. **Forms** -- Button, Checkbox, Combobox, Form Field, Input, Label, Select, Switch, etc.
3. **Data Display** -- Alert, Avatar, Badge, Breadcrumb, Card, Table, etc.
4. **Feedback** -- Alert Dialog, Dialog, Drawer, Popover, Sheet, Toast, Tooltip, etc.
5. **Navigation** -- Accordion, Carousel, Collapsible, Command, Context Menu, Dropdown Menu, Tabs, etc.

All 47 components already have sidebar entries. No new entries need to be added -- just create the page file at the correct route.

---

## 6. Copy-Paste Template

Complete template for a new demo page. Replace all `<!-- FILL -->` comments:

```razor
@page "/components/<!-- FILL: lowercase-hyphenated -->"
@using BlazingSpire.Demo.Components.UI

<PageTitle><!-- FILL: Component Name --> - BlazingSpire</PageTitle>

<div class="space-y-8">

    @* ── Header ─────────────────────────────── *@
    <div class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight"><!-- FILL: Component Name --></h1>
        <p class="text-lg text-muted-foreground"><!-- FILL: One-line description --></p>
    </div>

    @* ── Preview ────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Preview</h2>
        <div class="flex items-center justify-center rounded-xl border bg-muted/30 p-8">
            <!-- FILL: Primary live example -->
        </div>
    </div>

    @* ── Code ───────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Code</h2>
        <pre class="language-xml"><code class="language-xml"><!-- FILL: HTML-entity-encoded markup matching the Preview above --></code></pre>
    </div>

    @* ── Variants ───────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Variants</h2>
        <div class="flex flex-wrap items-center gap-4 rounded-xl border bg-muted/30 p-8">
            <!-- FILL: One example per variant enum value, each with a label -->
        </div>
    </div>

</div>

@code {
    // FILL: Add any state needed for interactive examples (e.g., bool _isOpen)
    // Delete this @code block entirely if the page has no interactive state
}
```

---

## 7. Worked Example: Badge (Simple, Static)

File: `src/BlazingSpire.Demo/Components/Pages/BadgePage.razor`

Badge has 4 variants (`Default`, `Secondary`, `Destructive`, `Outline`) and no interactive state.

```razor
@page "/components/badge"
@using BlazingSpire.Demo.Components.UI

<PageTitle>Badge - BlazingSpire</PageTitle>

<div class="space-y-8">

    @* ── Header ─────────────────────────────── *@
    <div class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight">Badge</h1>
        <p class="text-lg text-muted-foreground">Displays a small status indicator or label.</p>
    </div>

    @* ── Preview ────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Preview</h2>
        <div class="flex items-center justify-center rounded-xl border bg-muted/30 p-8">
            <Badge>Badge</Badge>
        </div>
    </div>

    @* ── Code ───────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Code</h2>
        <pre class="language-xml"><code class="language-xml">&lt;Badge&gt;Badge&lt;/Badge&gt;</code></pre>
    </div>

    @* ── Variants ───────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Variants</h2>
        <div class="flex flex-wrap items-center gap-4 rounded-xl border bg-muted/30 p-8">
            <div class="flex flex-col items-center gap-2">
                <span class="text-xs text-muted-foreground">Default</span>
                <Badge>Default</Badge>
            </div>
            <div class="flex flex-col items-center gap-2">
                <span class="text-xs text-muted-foreground">Secondary</span>
                <Badge Variant="BadgeVariant.Secondary">Secondary</Badge>
            </div>
            <div class="flex flex-col items-center gap-2">
                <span class="text-xs text-muted-foreground">Destructive</span>
                <Badge Variant="BadgeVariant.Destructive">Destructive</Badge>
            </div>
            <div class="flex flex-col items-center gap-2">
                <span class="text-xs text-muted-foreground">Outline</span>
                <Badge Variant="BadgeVariant.Outline">Outline</Badge>
            </div>
        </div>
    </div>

</div>
```

---

## 8. Worked Example: Dialog (Complex, Interactive)

File: `src/BlazingSpire.Demo/Components/Pages/DialogPage.razor`

Dialog requires `@code` state (`_isOpen`), uses composition with sub-components, and needs a trigger button.

```razor
@page "/components/dialog"
@using BlazingSpire.Demo.Components.UI

<PageTitle>Dialog - BlazingSpire</PageTitle>

<div class="space-y-8">

    @* ── Header ─────────────────────────────── *@
    <div class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight">Dialog</h1>
        <p class="text-lg text-muted-foreground">A modal window that interrupts the user with important content.</p>
    </div>

    @* ── Preview ────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Preview</h2>
        <div class="flex items-center justify-center rounded-xl border bg-muted/30 p-8">
            <Dialog @bind-IsOpen="_isOpen">
                <DialogTrigger>
                    <Button Variant="ButtonVariant.Outline">Edit Profile</Button>
                </DialogTrigger>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Edit Profile</DialogTitle>
                        <DialogDescription>Make changes to your profile here. Click save when you're done.</DialogDescription>
                    </DialogHeader>
                    <div class="grid gap-4 py-4">
                        <div class="grid grid-cols-4 items-center gap-4">
                            <Label Class="text-right">Name</Label>
                            <Input Class="col-span-3" Placeholder="Enter your name" />
                        </div>
                        <div class="grid grid-cols-4 items-center gap-4">
                            <Label Class="text-right">Username</Label>
                            <Input Class="col-span-3" Placeholder="johndoe" />
                        </div>
                    </div>
                    <DialogFooter>
                        <Button>Save changes</Button>
                    </DialogFooter>
                    <DialogClose />
                </DialogContent>
            </Dialog>
        </div>
    </div>

    @* ── Code ───────────────────────────────── *@
    <div class="space-y-3">
        <h2 class="text-xl font-semibold tracking-tight">Code</h2>
        <pre class="language-xml"><code class="language-xml">&lt;Dialog @@bind-IsOpen=&quot;_isOpen&quot;&gt;
    &lt;DialogTrigger&gt;
        &lt;Button Variant=&quot;ButtonVariant.Outline&quot;&gt;Edit Profile&lt;/Button&gt;
    &lt;/DialogTrigger&gt;
    &lt;DialogContent&gt;
        &lt;DialogHeader&gt;
            &lt;DialogTitle&gt;Edit Profile&lt;/DialogTitle&gt;
            &lt;DialogDescription&gt;Make changes to your profile here.&lt;/DialogDescription&gt;
        &lt;/DialogHeader&gt;
        &lt;div class=&quot;grid gap-4 py-4&quot;&gt;
            &lt;div class=&quot;grid grid-cols-4 items-center gap-4&quot;&gt;
                &lt;Label Class=&quot;text-right&quot;&gt;Name&lt;/Label&gt;
                &lt;Input Class=&quot;col-span-3&quot; Placeholder=&quot;Enter your name&quot; /&gt;
            &lt;/div&gt;
        &lt;/div&gt;
        &lt;DialogFooter&gt;
            &lt;Button&gt;Save changes&lt;/Button&gt;
        &lt;/DialogFooter&gt;
        &lt;DialogClose /&gt;
    &lt;/DialogContent&gt;
&lt;/Dialog&gt;

@@code {
    private bool _isOpen;
}</code></pre>
    </div>

</div>

@code {
    private bool _isOpen;
}
```

---

## 9. What NOT to Do

- **No API tables** -- don't list parameters, types, defaults. This is a demo site, not API docs.
- **No installation instructions** -- no "how to add this component" sections.
- **No accessibility documentation** -- no ARIA tables, keyboard shortcut lists, or screen reader notes.
- **No prose explanations** -- don't write paragraphs about when to use a component. Show it.
- **No external links** -- no links to shadcn/ui, MDN, or other references.
- **No `@rendermode` directives** -- pages inherit the app's render mode.
- **No CSS isolation files** -- all styling via Tailwind utility classes in markup.
- **No inline styles** -- use Tailwind classes exclusively.
- **No raw color values** -- use semantic tokens (`primary`, `muted`, `destructive`, etc.) via Tailwind classes.
