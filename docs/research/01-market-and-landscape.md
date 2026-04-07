# Market Opportunity & Competitive Landscape

## The Gap

No Blazor component framework combines all three pillars:

1. **High-quality headless primitives** (a11y, keyboard, ARIA)
2. **Copy-paste CLI distribution** (you own the code)
3. **Comprehensive Tailwind-styled components**

## Why Now

- Tailwind v4 auto-detects `.razor` files — the build pipeline friction is largely solved
- .NET 10 Blazor is stable with mature render mode support (SSR, Server, WASM, Auto)
- shadcn/ui has proven massive demand for this model in React
- Existing Blazor ports are all early-stage, solo-developer efforts or chose the wrong architecture (RCL packages instead of copy-paste)

---

## Major Blazor Component Libraries

| Library | Stars | Components | Approach | License | Weakness |
|---------|-------|------------|----------|---------|----------|
| **MudBlazor** | ~14K | 80+ | NuGet RCL, Material Design | MIT | Locked to Material aesthetic, ~500KB CSS |
| **Radzen** | ~3K | 70+ | NuGet RCL + paid IDE | MIT (core) | Dated styling |
| **Fluent UI Blazor** | ~4K | 40+ | Web Components wrappers | MIT | Breaking changes, component breadth |
| **Blazorise** | 3.5K | 80+ | Multi-CSS framework adapter | Dual | No first-class Tailwind |

## shadcn-Inspired Blazor Projects

| Project | Stars | Model | Maturity | Key Insight |
|---------|-------|-------|----------|-------------|
| **Blazor Blueprint** | 351 | NuGet RCL (NOT copy-paste) | Most mature (115+ components, 26 primitives) | Wrong distribution model; bus factor 1; zero behavioral tests |
| **Pango UI** | 52 | **True copy-paste CLI** | Early (v0.0.2) | Best CLI/registry architecture; limited components |
| **ShadcnBlazor** (bryjen) | 20 | Copy-paste CLI | Very early (2 months) | Best dependency resolution; `RequiredActions` pattern |
| **ShellUI** | 5 | Hybrid CLI + NuGet | Alpha | MSBuild Tailwind integration; standalone CLI binary download |
| **LumexUI** | 502 | NuGet RCL, Tailwind-native | Mature for its scope (30+) | Experimenting with composable model |

### Blazor Blueprint — Detailed Gap Analysis

**Architecture issues:**
- No tree-shaking — 1.11 MB Components package ships everything
- Monolithic CSS — all component styles bundled together
- Forced dependencies — Markdig, HtmlSanitizer, ECharts pulled in even when unused
- Customization ceiling — can change colors/radius via CSS vars but can't change component structure

**Testing is nearly nonexistent:**
- Only API surface snapshot tests
- Zero bUnit rendering tests, zero behavioral tests, zero a11y tests, zero keyboard tests

**Missing components/primitives:** ToggleGroup, Toolbar, Roving Focus Group, Toast primitive, Combobox primitive, nested dropdowns, progress circles, stepper

**Bus factor: 1** — Single maintainer (400 of ~415 commits), no Discord, no sponsorship

## Headless Primitive Libraries (Radix equivalent)

**Nothing production-ready exists.** This is the biggest gap:
- HeadlessBlazor — dead (3 components, stalled 14 months)
- SummitUI — alpha (~1 documented component, bits-ui inspired, screen reader tested)
- Blazix / Bladix — barely started

## Key Infrastructure Libraries

- **TailwindMerge.NET** v1.3.0 — MIT, 37K downloads, supports Tailwind v4.2. Essential dependency.
- **Tailwind.MSBuild** v2.x — MSBuild integration, auto-downloads standalone CLI
- **Benchmark.Blazor** — by bUnit creator, BenchmarkDotNet for Blazor components
- **BlazingStory** — Storybook clone for Blazor, pure C#/.NET
