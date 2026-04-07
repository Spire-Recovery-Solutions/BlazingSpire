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
- Perceptually uniform — 10% lightness change looks the same across all hues
- Predictable palette generation — lock L for shade level, vary C and H
- Accessibility — contrast ratios calculable from L channel (delta > ~0.4 for AA)
- Wide gamut — can represent P3 colors for modern displays
- Browser support: Chrome 111+, Safari 15.4+, Firefox 113+

### Semantic Token Pattern

Every surface color has a matching `-foreground` token:
- `--background` / `--foreground` — page surface
- `--card` / `--card-foreground` — elevated surfaces
- `--popover` / `--popover-foreground` — floating surfaces
- `--primary` / `--primary-foreground` — primary actions
- `--secondary` / `--secondary-foreground` — secondary actions
- `--muted` / `--muted-foreground` — disabled/subtle text
- `--accent` / `--accent-foreground` — highlights
- `--destructive` — danger/error (foreground inferred)
- `--chart-1` through `--chart-5` — data visualization

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
| **Accordion** | height: 0 → var(--content-height) | reverse |
| **Sheet** | slide-in from edge | slide-out to edge |

Use **tw-animate-css** for utility classes: `animate-in`, `fade-in-0`, `zoom-in-95`, `slide-in-from-top-2`.

### Reduced Motion

Replace directional motion with simple opacity fades — never eliminate all animation:
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
