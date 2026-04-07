# BlazingSpire Research Overview

> A .NET 10 Blazor UI Component Framework inspired by shadcn/ui, built with Tailwind CSS v4.

## Research Documents

### Foundation (Phase 1)
| # | Document | Domain |
|---|----------|--------|
| 01 | [Market & Landscape](01-market-and-landscape.md) | Competitive analysis, existing projects, gaps |
| 02 | [Architecture](02-architecture.md) | Distribution model, component layers, tech stack |
| 03 | [Blazor Limitations](03-blazor-limitations.md) | Platform constraints and proven workarounds |
| 04 | [JS Interop Layer](04-js-interop-layer.md) | Focus, positioning, scroll lock, click-outside, keyboard |
| 05 | [Design System](05-design-system.md) | Color, typography, animation, icons, theming |
| 06 | [Tailwind Integration](06-tailwind-integration.md) | Build pipeline, class merging, variants, dark mode |
| 07 | [Testing](07-testing.md) | Test pyramid, tools, anti-patterns, CI |
| 08 | [Performance](08-performance.md) | Source generators, trimming/AOT, rendering, allocations |
| 09 | [CLI Tooling](09-cli-tooling.md) | Commands, registry, dependency resolution, templates |
| 10 | [MSBuild & Repo](10-msbuild-and-repo.md) | Directory.Build files, CPM, repo layout |
| 11 | [Open Questions](11-open-questions.md) | Unresolved decisions |

### Gap Analysis (Phase 2 — from quorum review)
| # | Document | Domain | Gap Addressed |
|---|----------|--------|---------------|
| 12 | [Primitive API Design](12-primitive-api-design.md) | C# API specs for Dialog, Select, Combobox, Menu, Tabs | C1: No API surface defined |
| 13 | [Performance Targets](13-performance-targets.md) | Concrete benchmarks, budgets, scaling model | C3, H2, H3: No perf targets |
| 14 | [Forms & Data Patterns](14-forms-and-data-patterns.md) | FormField, DataTable, CommandPalette, Toast | H4: Enterprise patterns missing |
| 15 | [CLI Error Handling](15-cli-error-handling.md) | Doctor command, error catalog, edge cases, upgrade path | H5, H6, H1: CLI gaps |
| 16 | [SSR & Versioning](16-ssr-and-versioning.md) | SSR fallback matrix, API stability, compatibility | H7, H1, M1, M2: SSR + versioning |

## The Thesis

No Blazor component framework combines all three pillars:
1. High-quality headless primitives (a11y, keyboard, ARIA)
2. Copy-paste CLI distribution (you own the code)
3. Comprehensive Tailwind-styled components

BlazingSpire fills this gap.

## Gap Resolution Status

| ID | Gap | Severity | Status |
|----|-----|----------|--------|
| C1 | No primitive API surface defined | Critical | Resolved → doc 12 |
| C2 | CascadingValue perf for mutable state | Critical | Resolved → docs 12, 13 |
| C3 | No performance targets or benchmarks | Critical | Resolved → doc 13 |
| H1 | Versioning / breaking changes unsolved | High | Resolved → docs 15, 16 |
| H2 | Server-mode interop budget unquantified | High | Resolved → doc 13 |
| H3 | WASM payload misleadingly incomplete | High | Resolved → doc 13 |
| H4 | Forms, validation, DataTable absent | High | Resolved → doc 14 |
| H5 | CLI error handling completely absent | High | Resolved → doc 15 |
| H6 | First-run edge cases undesigned | High | Resolved → doc 15 |
| H7 | SSR graceful degradation unspecified | High | Resolved → doc 16 |
| M1 | Portal dual-mechanism complexity | Medium | Resolved → doc 16 |
| M2 | @starting-style unmount strategy | Medium | Resolved → doc 16 |
| M3 | JS module loading waterfall | Medium | Resolved → doc 13 |
| M4 | TailwindMerge.NET actual cost | Medium | Resolved → doc 13 |
| M5 | IDE integration | Medium | Resolved → doc 15 |
| M6 | Multi-project / monorepo | Medium | Resolved → doc 15 |
| M7 | Migration / coexistence | Medium | Resolved → doc 15 |
| M8 | Documentation strategy | Medium | Partially in doc 15 |
| M9 | Memory per Server connection | Medium | Resolved → doc 13 |
| M10 | JS build pipeline for Floating UI | Medium | Resolved → doc 13 |
