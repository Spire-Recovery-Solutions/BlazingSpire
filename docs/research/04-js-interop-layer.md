# JS Interop Layer

## Minimal JS Surface (~300 lines + Floating UI ~3KB)

```
blazingspire-interop.js
├── focus.js        — getFocusable, focusFirst/Last, createFocusTrap, save/restore
├── positioning.js  — wraps Floating UI, computePosition, autoUpdate
├── interaction.js  — clickOutside (pointerdown+pointerup pair), escapeKey, focusOutside
├── scroll.js       — lockBodyScroll/unlock with lock counting + scrollbar compensation
├── keyboard.js     — menuKeyboard (arrow nav + type-ahead), roving focus helpers
└── portal.js       — DOM reparenting for escaping stacking contexts
```

## Focus Management

### Focus Trap Patterns

Two approaches found in the ecosystem:

**Sentinel divs (MudBlazor):** Invisible tabbable divs before/after content. When focus lands on a sentinel, redirect via JS `focusFirst()`/`focusLast()`. Track Shift key state for reverse-tab. More DOM, less JS.

**JS keydown handler (Blazor Blueprint):** JS-side event listener intercepts Tab, calls `preventDefault()` + `element.focus()` directly. Cleaner, less DOM.

**Recommended: JS keydown handler** — aligns with our JS-first approach for DOM operations.

### Tabbable Element Selector

```js
"a[href]:not([tabindex='-1']), button:not([disabled]):not([tabindex='-1']),
 input:not([disabled]):not([tabindex='-1']):not([type='hidden']),
 select:not([disabled]):not([tabindex='-1']), textarea:not([disabled]):not([tabindex='-1']),
 [tabindex]:not([tabindex='-1']), [contentEditable=true]:not([tabindex='-1'])"
```

### Focus Restore

Save `document.activeElement` before opening overlay, restore on close.

## Floating Element Positioning

**Use Floating UI** (~3KB minified). Blazor Blueprint bundles it as an ES module and wraps with:

```js
// positioning.js
import { computePosition, offset, flip, shift, arrow, size, autoUpdate } from './floating-ui-dom.esm.min.js';

export function setupPositioning(referenceId, floatingId, options) {
    // Wait for elements (handles portal timing)
    // Return cleanup disposable
}
```

C# service: `IPositioningService.AutoUpdateAsync(ElementReference ref, ElementReference float, options)` → `IAsyncDisposable`

## Click-Outside Detection

**Use pointerdown+pointerup pair** (Blazor Blueprint's approach):
- Only trigger close if BOTH mousedown AND mouseup were outside
- Handles nested portals via `data-portal-content` attribute
- `setTimeout(0)` to avoid triggering on the opening click
- Also provides `onFocusOutside()` and `onEscapeKey()`

## Scroll Locking

```js
export function lockBodyScroll() {
    const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
    document.body.style.overflow = 'hidden';
    if (scrollbarWidth > 0) {
        document.body.style.paddingRight = `${scrollbarWidth}px`;  // prevent layout shift
    }
    return { dispose() { /* restore */ } };
}
```

Lock counting required for nested modals. MudBlazor's known issue: doesn't work on touch devices where `innerWidth === clientWidth`.

## Keyboard Navigation (Roving Focus)

Based on Blazor Blueprint's `menu-keyboard.js`:
- Arrow keys navigate between `[role="menuitem"]` elements
- Home/End jump to first/last
- Enter/Space activate
- Escape calls back to .NET
- **Type-ahead search:** accumulates typed characters, focuses matching item (350ms reset)
- Double `requestAnimationFrame` for timing after Blazor render cycles

## Portal Rendering

**Two mechanisms:**

1. **C# Portal Service** — `PortalService` holds `ConcurrentDictionary<string, RenderFragment>`. `PortalHost` in layout renders them. No DOM manipulation.

2. **JS DOM reparenting** — `portal.js` physically moves elements to a `position: fixed` container at body level. Needed for escaping CSS stacking contexts (e.g., overflow: hidden parents).

## JS Interop Performance Rules

- Create `DotNetObjectReference` once in `OnAfterRenderAsync(firstRender)`, dispose in `DisposeAsync`
- Use `ValueTask` for wrappers that frequently complete synchronously
- Never `await` interop inside `BuildRenderTree`
- Batch interop calls (especially in Server mode where each crosses SignalR)
- Use collocated `.razor.js` files — loaded via `IJSObjectReference` on demand
- All setup functions return disposable cleanup objects (`{ dispose() }` or `{ _cleanupId }`)
