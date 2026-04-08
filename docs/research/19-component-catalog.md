# Component Catalog & Build Order

A prioritized inventory of every component BlazingSpire will ship, organized by complexity tier, with a dependency graph and phased build order. This is the roadmap for implementation sequencing.

---

## Component Inventory

### Complexity Tiers

| Tier | Description | Examples |
|------|-------------|----------|
| **Tier 0** | Static — pure HTML + Tailwind, no JS, no state | Button, Badge, Card, Alert |
| **Tier 1** | Simple interactive — C# state, no JS interop | Accordion (`<details>`), Toggle, Collapsible |
| **Tier 2** | JS-assisted — needs collocated `.razor.js` | Tooltip, Popover, Sheet, Scroll Area |
| **Tier 3** | Complex — focus trap, positioning, keyboard nav, portals | Dialog, Select, Combobox, Menu, Command Palette |

---

## Dependency Graph

```
Portal ─────────────────────────────────────────────────────┐
  (JS DOM reparenting, escapes stacking contexts)           │
                                                            │
Floating UI ────────────────────────────────────────────┐   │
  (JS positioning engine, ~3KB)                         │   │
                                                        │   │
Focus Trap ─────────────────────────────────────────┐   │   │
  (JS focus management, Tab/Shift+Tab cycling)      │   │   │
                                                    │   │   │
Click Outside ──────────────────────────────────┐   │   │   │
  (JS interaction detection)                    │   │   │   │
                                                │   │   │   │
Scroll Lock ────────────────────────────────┐   │   │   │   │
  (JS body scroll prevention)               │   │   │   │   │
                                            │   │   │   │   │
                                            ▼   ▼   ▼   ▼   ▼
                                          ┌───────────────────┐
                                          │ Dialog             │
                                          │ Sheet              │
                                          └───────────────────┘
                                            ▼       ▼
                                   ┌────────┐  ┌─────────────┐
                                   │ Alert  │  │ Command     │
                                   │ Dialog │  │ Palette     │
                                   └────────┘  │ (+ Combobox)│
                                               └─────────────┘

Floating UI + Click Outside + Portal ──────────────────────┐
                                                           ▼
                                                   ┌──────────────┐
                                                   │ Popover       │
                                                   │ Tooltip       │
                                                   │ Dropdown Menu │
                                                   │ Context Menu  │
                                                   │ Hover Card    │
                                                   └──────────────┘
                                                           │
                                          ┌────────────────┤
                                          ▼                ▼
                                   ┌────────────┐  ┌──────────────┐
                                   │ Select     │  │ Combobox     │
                                   │ (+ roving  │  │ (+ filtering │
                                   │  focus)    │  │  + keyboard) │
                                   └────────────┘  └──────────────┘
                                                           │
                                                           ▼
                                                   ┌──────────────┐
                                                   │ Multi-Select │
                                                   │ Combobox     │
                                                   └──────────────┘

FormField ──────────────────────────────────────────────────┐
  (EditContext integration, ARIA wiring)                     │
                                                            ▼
                                                   ┌──────────────┐
                                                   │ All form     │
                                                   │ controls:    │
                                                   │ Input, Select│
                                                   │ Checkbox,    │
                                                   │ Radio, etc.  │
                                                   └──────────────┘

DataTable ──────────────────────────────────────────────────┐
  (sort, filter, pagination, selection, virtualization)     │
  Composes: Checkbox, Button, DropdownMenu, Badge, Input    │
                                                            ▼
                                                   ┌──────────────┐
                                                   │ DataTable    │
                                                   │ (Tier 3)     │
                                                   └──────────────┘

Toast ──────────────────────────────────────────────────────┐
  (service-based, cross-render-boundary, auto-dismiss)      │
  Requires: Portal (C# PortalService for cross-boundary)    │
                                                            ▼
                                                   ┌──────────────┐
                                                   │ Sonner-style │
                                                   │ Toast system │
                                                   └──────────────┘
```

### Key Dependency Chains

| Component | Depends On |
|-----------|-----------|
| Dialog | Portal, Focus Trap, Click Outside, Scroll Lock |
| Sheet | Dialog (same primitives, slide-in variant) |
| Alert Dialog | Dialog (non-dismissible variant) |
| Dropdown Menu | Popover (Floating UI + Click Outside + Portal), Roving Focus |
| Context Menu | Dropdown Menu (right-click trigger variant) |
| Select | Floating UI, Portal, Click Outside, Roving Focus |
| Combobox | Select mechanics + text filtering |
| Multi-Select Combobox | Combobox + tag/chip display |
| Command Palette | Dialog + Combobox (search + filtered list) |
| Tooltip | Floating UI, Portal |
| Popover | Floating UI, Portal, Click Outside |
| Hover Card | Popover (hover-triggered variant) |
| DataTable | Checkbox, Button, Badge, DropdownMenu, Input (composes styled components) |
| Toast | Portal (C# PortalService), JS for animations/timers |
| Navigation Menu | Popover + Roving Focus (horizontal nav with dropdown panels) |

---

## Suggested Build Order

### Phase 1: Foundation (Tier 0 — Static Components)

**Goal:** Ship the most-used components fast. Zero JS interop complexity. Establish project conventions, base classes (`BlazingSpireStaticComponent`), Tailwind class merging patterns, CLI scaffolding.

| Component | Notes |
|-----------|-------|
| Button | `<button>` / `<a>`, variants (default, destructive, outline, secondary, ghost, link), sizes |
| Badge | `<span>`, variants (default, secondary, destructive, outline) |
| Card | `<div>` container — Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter |
| Alert | `<div role="alert">`, variants (default, destructive) |
| Avatar | `<img>` + fallback initials `<span>`, fallback via CSS or conditional render |
| Separator | `<hr>` or `<div role="separator">`, horizontal/vertical |
| Skeleton | `<div>` with CSS pulse animation |
| Typography | `<h1>`–`<h6>`, `<p>`, `<blockquote>`, `<code>`, `<lead>` utility classes |
| Label | `<label>`, associates with form controls via `for` |
| Input | `<input>` with Tailwind styling, file input variant |
| Textarea | `<textarea>` with Tailwind styling |
| Aspect Ratio | `<div>` with `aspect-ratio` CSS property |
| Table | `<table>` with semantic sub-components (TableHeader, TableBody, TableRow, TableHead, TableCell) |
| Progress | `<progress>` or `<div role="progressbar">` with CSS width |
| Checkbox | `<input type="checkbox">`, custom styling via CSS |
| Radio Group | `<fieldset>` + `<input type="radio">`, custom styling via CSS |
| Switch (visual only) | `<button role="switch">`, CSS-only toggle appearance |

**Estimated effort:** 1–2 weeks. These are the "free wins" — they establish patterns and give users something immediately useful.

### Phase 2: Simple Interactive + JS Infrastructure (Tier 1–2)

**Goal:** Build the JS interop infrastructure modules that all Tier 2–3 components depend on. Ship simple interactive components that prove the patterns.

#### 2a: JS Interop Infrastructure (ships in `BlazingSpire.Primitives` NuGet)

| Module | Purpose | Used By |
|--------|---------|---------|
| `portal.js` | DOM reparenting to body-level container | Dialog, Sheet, Popover, Tooltip, Select, Menu |
| `focus-trap.js` | Tab/Shift+Tab cycling within container | Dialog, Sheet, Alert Dialog |
| `interaction.js` | Click-outside, pointer-down-outside detection | Popover, Menu, Select, Combobox, Dialog |
| `scroll-lock.js` | Prevent body scrolling when overlay open | Dialog, Sheet |
| `floating.js` | Floating UI wrapper — positioning, flip, shift, arrow | Tooltip, Popover, Menu, Select, Combobox, HoverCard |
| `keyboard.js` | Roving focus group, typeahead, arrow key navigation | Menu, Select, Tabs, Radio Group, Toolbar |

#### 2b: Simple Interactive Components

| Component | Notes |
|-----------|-------|
| Accordion | Native `<details>`/`<summary>` with animated height via CSS grid trick. SSR-compatible (expanded by default). |
| Collapsible | Single open/close panel. Simpler than Accordion. |
| Toggle | `<button aria-pressed>`, stateful on/off. |
| Toggle Group | Group of Toggle buttons, single or multi-select, roving focus. |
| Tabs | Tab list + tab panels, roving focus in tab list, keyboard arrow navigation. |
| Scroll Area | Custom scrollbar styling via JS (`scrollbar-width` detection + overlay scrollbar). |

**Estimated effort:** 2–3 weeks. The JS infrastructure is the real investment here — it unlocks everything in Phase 3.

### Phase 3: Core Primitives (Tier 3 — The 5 Hardest + Overlays)

**Goal:** Ship the headless primitives that define BlazingSpire's value proposition. These are the components no other Blazor library does well. Build in the order that maximizes reuse.

#### 3a: Overlay Foundation

| Component | Notes |
|-----------|-------|
| Dialog | The canonical overlay. Focus trap, scroll lock, portal, click-outside, Escape. `data-state` open/closing for CSS animations. |
| Alert Dialog | Dialog variant — no click-outside dismiss, requires explicit action. |
| Sheet | Dialog variant — slide-in from edge (top/right/bottom/left). Same primitive, different CSS. |

#### 3b: Floating Components

| Component | Notes |
|-----------|-------|
| Tooltip | Floating UI positioning, delay open/close, `title` fallback in SSR. |
| Popover | Floating positioned panel, click-outside to close, portal. |
| Hover Card | Popover variant — hover-triggered, with open/close delay. |
| Dropdown Menu | Popover + roving focus + typeahead + sub-menus. Full ARIA menu pattern. |
| Context Menu | Dropdown Menu variant — right-click trigger, positioned at pointer. |

#### 3c: Selection Components

| Component | Notes |
|-----------|-------|
| Select | Floating listbox, roving focus, typeahead, keyboard arrow nav, single selection. Native `<select>` fallback in SSR. |
| Combobox | Select + text input filtering. Native `<input>` + `<datalist>` fallback in SSR. |

**Estimated effort:** 4–6 weeks. These are the highest-complexity, highest-value components. Extensive a11y and keyboard testing required.

### Phase 4: Form Components

**Goal:** Enterprise form composition layer. Depends on Phase 1 (inputs) and Phase 3 (Select, Combobox).

| Component | Notes |
|-----------|-------|
| FormField | Primitive — ID generation, ARIA wiring, EditContext/validation subscription. Sub-components: FormFieldLabel, FormFieldControl, FormFieldDescription, FormFieldError. |
| FormSection | Styled `<fieldset>` with legend, description, disabled state. |
| Form Layouts | Styled vertical/horizontal layout wrappers. |
| Multi-Select Combobox | Combobox + tag/chip display, Backspace to remove, max selections. |
| Date Picker | Calendar + Popover composition. Consider wrapping a date library or building from scratch. |
| Slider | `<input type="range">` with custom styling + ARIA, optional JS for dual-thumb range. |
| Form Wizard | Multi-step form with per-step validation, step indicator, prev/next navigation. |

**Estimated effort:** 3–4 weeks.

### Phase 5: Data & Advanced Components

**Goal:** Ship the remaining high-value components that compose on top of everything built so far.

#### 5a: Data Components

| Component | Notes |
|-----------|-------|
| DataTable | Headless primitive — sort, filter, pagination, row selection, column resize, virtualization via `Virtualize<T>`. Composes: Checkbox, Button, Badge, DropdownMenu. |
| Command Palette | Dialog + Combobox composition. Fuzzy search, grouped results, `Ctrl+K` global hotkey. |
| Pagination | Standalone pagination control (also used inside DataTable). |

#### 5b: Feedback & Notification

| Component | Notes |
|-----------|-------|
| Toast / Sonner | Service-based (`IToastService`), cross-render-boundary via scoped DI, auto-dismiss, stacking, swipe-to-dismiss (JS). Requires `ToastProvider` in layout. |
| Alert Dialog (service) | Imperative `AlertDialogService.Confirm()` for cross-boundary confirmation dialogs. |

#### 5c: Navigation & Layout

| Component | Notes |
|-----------|-------|
| Navigation Menu | Horizontal nav bar with dropdown panels on hover/click. Radix-style viewport animation. |
| Breadcrumb | Static `<nav>` + `<ol>` with separator, collapsible middle items. |
| Menubar | Horizontal menu bar (File/Edit/View pattern). Composes Dropdown Menu. |
| Sidebar | Collapsible sidebar layout. State managed in C#, CSS transition for width. |
| Resizable | Drag-to-resize panels. JS pointer tracking for handle. |

#### 5d: Remaining Components

| Component | Notes |
|-----------|-------|
| Calendar | Month/year grid, date selection, range selection. Used by Date Picker. |
| Carousel | Slide navigation with touch/swipe (JS), keyboard arrows, ARIA live region. |
| Chart | Wrapper/guidance for charting libraries (not a custom chart engine). |
| Drawer | Mobile-friendly bottom sheet variant. Touch-drag-to-dismiss (JS). |
| InputOTP | One-time-password input with auto-advance between digits (JS). |

**Estimated effort:** 5–8 weeks total for Phase 5.

---

## Per-Component Reference Table

### Legend

- **Primitive**: Headless component in `BlazingSpire.Primitives` NuGet package
- **Styled**: Copy-paste component via CLI
- **JS Interop**: Requires collocated `.razor.js` module
- **SSR Fallback**: Behavior when rendered in Static SSR (no JS, no interactivity)
- **Status**: Exists (in demo) / Planned / Stretch

### Tier 0 — Static

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| Button | No | Yes | No | None | Fully functional | **Exists** |
| Badge | No | Yes | No | None | Fully functional | **Exists** |
| Card | No | Yes | No | None | Fully functional | **Exists** |
| Alert | No | Yes | No | None | Fully functional | Planned |
| Avatar | No | Yes | No | None | `<img>` + CSS fallback | Planned |
| Separator | No | Yes | No | None | `<hr>` | Planned |
| Skeleton | No | Yes | No | None | CSS pulse animation | Planned |
| Typography | No | Yes | No | None | Semantic HTML elements | Planned |
| Label | No | Yes | No | None | `<label>` | Planned |
| Input | No | Yes | No | None | `<input>` | Planned |
| Textarea | No | Yes | No | None | `<textarea>` | Planned |
| Table | No | Yes | No | None | `<table>` | Planned |
| Progress | No | Yes | No | None | `<progress>` | Planned |
| Checkbox | No | Yes | No | None | `<input type="checkbox">` | Planned |
| Radio Group | No | Yes | No | None | `<fieldset>` + `<input type="radio">` | Planned |
| Aspect Ratio | No | Yes | No | None | `<div>` with CSS `aspect-ratio` | Planned |
| Breadcrumb | No | Yes | No | None | `<nav>` + `<ol>` | Planned |

### Tier 1 — Simple Interactive

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| Accordion | Yes | Yes | No | None | Native `<details open>`, all panels expanded | Planned |
| Collapsible | Yes | Yes | No | None | Native `<details>` | Planned |
| Toggle | Yes | Yes | No | None | `<button aria-pressed>` visible but non-functional | Planned |
| Toggle Group | Yes | Yes | No | Roving Focus | Buttons visible, non-functional | Planned |
| Tabs | Yes | Yes | No | Roving Focus | Default panel visible, others `hidden` | Planned |
| Switch | Yes | Yes | No | None | `<button role="switch">` visible but non-functional | Planned |

### Tier 2 — JS-Assisted

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| Tooltip | Yes | Yes | `floating.js` | Floating UI, Portal | `title` attribute (browser-native) | Planned |
| Popover | Yes | Yes | `floating.js`, `interaction.js` | Floating UI, Portal, Click Outside | Trigger visible, content `hidden` | Planned |
| Hover Card | Yes | Yes | `floating.js` | Floating UI, Portal | Link/trigger functional, no card on hover | Planned |
| Scroll Area | Yes | Yes | `scroll-area.js` | None | Browser-native scrollbar | Planned |
| Slider | Yes | Yes | `slider.js` | None | `<input type="range">` | Planned |
| Resizable | Yes | Yes | `resizable.js` | None | Fixed layout, no drag | Planned |
| Carousel | Yes | Yes | `carousel.js` | None | All slides visible or first slide only | Stretch |
| InputOTP | Yes | Yes | `input-otp.js` | None | Multiple `<input>` fields | Stretch |

### Tier 3 — Complex

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| Dialog | Yes | Yes | `portal.js`, `focus-trap.js`, `interaction.js`, `scroll-lock.js` | Portal, Focus Trap, Click Outside, Scroll Lock | `<dialog>` closed, trigger visible, content in DOM for SEO | Planned |
| Alert Dialog | Yes | Yes | Same as Dialog | Dialog | Same as Dialog, no dismiss on backdrop | Planned |
| Sheet | Yes | Yes | Same as Dialog | Dialog | Trigger visible, content `hidden` | Planned |
| Dropdown Menu | Yes | Yes | `floating.js`, `interaction.js`, `portal.js`, `keyboard.js` | Floating UI, Portal, Click Outside, Roving Focus | Trigger visible, menu `hidden` | Planned |
| Context Menu | Yes | Yes | Same as Dropdown Menu | Dropdown Menu | Not rendered (requires right-click JS) | Planned |
| Select | Yes | Yes | `floating.js`, `interaction.js`, `portal.js`, `keyboard.js` | Floating UI, Portal, Click Outside, Roving Focus | Native `<select>` with `<option>` children | Planned |
| Combobox | Yes | Yes | Same as Select | Select mechanics + filtering | `<input>` + `<datalist>` | Planned |
| Multi-Select Combobox | Yes | Yes | Same as Combobox | Combobox | Tags as badges + disabled input, or `<select multiple>` | Planned |
| Navigation Menu | Yes | Yes | `floating.js`, `interaction.js`, `keyboard.js` | Floating UI, Roving Focus | Links functional, dropdowns hidden | Planned |
| Menubar | Yes | Yes | Same as Dropdown Menu | Dropdown Menu | Menu titles visible, dropdowns hidden | Planned |
| Command Palette | Yes | Yes | `command-hotkey.js` + Dialog JS + Combobox JS | Dialog, Combobox | Not rendered (`Open=false`) or degrade to search link | Planned |
| DataTable | Yes | Yes | `datatable-resize.js` | Checkbox, Button, Badge, DropdownMenu, Input | Static `<table>` with data, no sort/filter/selection | Planned |
| Toast | Yes (service) | Yes | `toast.js` (swipe, timers) | Portal (C# PortalService), Focus management | Not rendered (service-based, requires interactivity) | Planned |
| Date Picker | Yes | Yes | `floating.js`, `interaction.js` | Calendar, Popover, Button, Select | Native `<input type="date">` | Stretch |

### Form Components (Tier 1–2, depend on Phase 1 + 3)

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| FormField | Yes | Yes | No | EditContext integration | Static HTML with ARIA IDs, validation via form POST | Planned |
| FormFieldLabel | Yes | Yes | No | FormField | `<label>` | Planned |
| FormFieldControl | Yes | Yes | No | FormField | Pass-through ARIA attrs | Planned |
| FormFieldDescription | Yes | Yes | No | FormField | `<p>` | Planned |
| FormFieldError | Yes | Yes | No | FormField | `<p role="alert">` | Planned |
| FormSection | No | Yes | No | None | `<fieldset>` + `<legend>` | Planned |
| Form Layouts | No | Yes | No | None | Grid CSS | Planned |
| Form Wizard | Yes | Yes | No | Tabs, FormField | All steps expanded as sections, or step 1 with POST to advance | Planned |

### Layout Components (Tier 1–2)

| Component | Primitive | Styled | JS Interop | Dependencies | SSR Fallback | Status |
|-----------|-----------|--------|------------|--------------|--------------|--------|
| Sidebar | No | Yes | Optional JS for collapse animation | None | Expanded by default | Stretch |
| Drawer | Yes | Yes | `drawer.js` (touch drag) | Dialog | Trigger visible, content `hidden` | Stretch |
| Calendar | Yes | Yes | No (keyboard nav in C#) | Button | Static month grid, no selection | Planned |
| Pagination | No | Yes | No | Button | Links with `?page=N` query strings | Planned |

---

## Comparison to shadcn/ui Component List

Every shadcn/ui component is accounted for. Components not listed above that exist in shadcn/ui:

| shadcn/ui Component | BlazingSpire Mapping | Notes |
|---------------------|---------------------|-------|
| Sonner | Toast | Same concept, service-based for Blazor |
| Command | Command Palette | Built on Dialog + Combobox |
| Data Table | DataTable | Headless primitive (TanStack Table equivalent) |
| Form | FormField | Blazor's `EditForm` + our FormField primitive |
| Dropdown Menu | Dropdown Menu | Direct mapping |
| Sheet | Sheet | Dialog variant with slide-in animation |
| Alert Dialog | Alert Dialog | Dialog variant, non-dismissible |
| Input OTP | InputOTP | JS for auto-advance between digits |
| Toggle | Toggle + Toggle Group | Split into two components for composability |
| Scroll Area | Scroll Area | Custom scrollbar styling |
| Resizable | Resizable | Panel resize via drag handle |

---

## Summary

| Phase | Components | Unlocks |
|-------|-----------|---------|
| **Phase 1** | 17 static components | CLI workflow, base classes, Tailwind patterns, immediate user value |
| **Phase 2** | 6 interactive + JS infrastructure | Interop modules that all Tier 2–3 depend on |
| **Phase 3** | 11 core primitives | BlazingSpire's unique value prop — the components nobody else does well |
| **Phase 4** | 8 form components | Enterprise form patterns, EditContext integration |
| **Phase 5** | ~13 data, nav, and advanced components | Full shadcn/ui parity |

**Total: ~55 components** covering the full shadcn/ui catalog plus Blazor-specific additions (FormField, DataTable headless primitive, Toast service, Form Wizard).

Build order rationale: each phase's outputs are direct inputs to the next. Tier 0 components are both the easiest to ship and the most composed-into by later tiers (Button appears inside Dialog, DataTable, Pagination, etc.). The JS infrastructure in Phase 2 is the gate for everything interactive. Phase 3's primitives are what differentiate BlazingSpire from every competitor.
