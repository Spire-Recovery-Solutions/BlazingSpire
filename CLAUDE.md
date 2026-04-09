# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

BlazingSpire is a .NET 10 Blazor WebAssembly component framework inspired by shadcn/ui. Users copy-paste component source into their projects — no NuGet package. Tailwind CSS v4 with OKLCH color tokens, dark mode via `localStorage`.

Live demo: https://blazingspire.pages.dev

Currently a POC with `src/BlazingSpire.Demo/` and test projects. Future plans include a headless primitives NuGet package and a CLI tool.

## Build & Run

```bash
# From solution root
dotnet build                                                        # all projects + tests

# Demo app
cd src/BlazingSpire.Demo
npm ci                                                              # install Tailwind v4 + Prism
npx @tailwindcss/cli -i wwwroot/app.css -o wwwroot/app.build.css    # build CSS (CI does --minify)
dotnet watch                                                        # dev server with hot reload
dotnet publish -c Release -o publish                                # production build

# Tests
dotnet test test/BlazingSpire.Tests.Unit                            # bUnit + xUnit
dotnet test test/BlazingSpire.Tests.E2E                             # Playwright
```

Prerequisites: .NET 10 SDK (`~/.dotnet/dotnet`), Node.js 22+, `wasm-tools` workload (`dotnet workload install wasm-tools`). Add `export PATH="$HOME/.dotnet:$PATH"` if `dotnet` is not on PATH.

Solution file: `BlazingSpire.sln`. Central Package Management via `Directory.Packages.props`. Build artifacts output to `artifacts/` via `ArtifactsPath`.

## Deployment

Pushes to `main` (under `src/BlazingSpire.Demo/`) trigger `.github/workflows/deploy.yml`:
1. Build Tailwind CSS
2. `dotnet publish` Blazor WASM
3. Deploy `publish/wwwroot` to Cloudflare Pages via `wrangler-action@v3`

Secrets: `CLOUDFLARE_API_TOKEN`, `CLOUDFLARE_ACCOUNT_ID`. Project name: `blazingspire`.

`app.build.css` is gitignored — CI regenerates it. Never commit it.

## Architecture

**Base component hierarchy:** All components extend a tiered base class hierarchy in `Components/Shared/`. The base classes use template method pattern, `FrozenDictionary<TEnum, string>` for variant/size→CSS mappings, and abstract members. Hierarchy:

```
BlazingSpireComponentBase              → ChildContent, Class, AdditionalAttributes, BuildClasses(), abstract BaseClasses, virtual Classes
├── PresentationalBase<TVariant>       → Variant, abstract VariantClassMap (FrozenDictionary)
├── InteractiveBase                    → Disabled, virtual IsEffectivelyDisabled
│   ├── ButtonBase<TVariant, TSize>    → Variant, Size, Loading, Href/Target/Rel, OnClick, VariantClassMap, SizeClassMap
│   ├── FormControlBase<TValue>        → Value/ValueChanged/ValueExpression, EditContext wiring, validation
│   │   ├── TextInputBase, BooleanInputBase, NumericInputBase<T>, SelectionBase<T>
│   └── DisclosureBase                 → IsOpen, controlled/uncontrolled, ToggleAsync()
└── OverlayBase                        → IsOpen, focus trap, scroll lock, click outside, portal (JS interop)
    └── PopoverBase                    → Floating UI positioning
        └── MenuBase                   → Item registry, roving focus, keyboard nav
```

**Component pattern:** Each UI component lives in `Components/UI/` as a `.razor` + `.razor.cs` pair. The `.razor` file uses `@inherits` to specify the base class. The `.razor.cs` provides `BaseClasses`, variant/size `FrozenDictionary` mappings, and component-specific parameters. Enums are at namespace scope (e.g., `ButtonVariant.Default`, not `Button.ButtonVariant.Default`). No component sets its own `@rendermode`.

**Theming:** All colors defined as OKLCH tokens in `wwwroot/app.css` under `@theme` (light) and `.dark` (dark override). Dark mode uses `@custom-variant dark (&:where(.dark, .dark *))`. Theme toggle persists to `localStorage` via `wwwroot/js/theme.js` (no eval).

**Boot sequence (index.html):**
1. Inline `<script>` reads `localStorage` theme before paint (no flash)
2. Skeleton div renders outside `#app` for instant LCP
3. `Blazor.start()` deferred; on resolve: skeleton removed, `#app` shown, Prism runs
4. `#blazor-error-ui` hidden by default, shown only on unhandled errors

**Performance strategy:** No AOT (Jiterpreter instead for smaller payload), full trimming, invariant globalization/timezone, stripped subsystems. Target: Lighthouse 100/100/100/100 desktop.

## Agent Workflow

Use `/team <component-name>` to orchestrate a multi-agent team for building components. Domain experts (blazor-architecture, design-and-styling, performance, tooling) hold embedded research knowledge. Workers (component-builder, test-writer, docs-writer) implement, test, and document. See `.claude/agents/` for definitions and `.claude/skills/team/` for the orchestration skill.

## Self-Documenting Playground

Components self-document via `///` XML doc comments and `[Description]` attributes on `[Parameter]` properties. Three outputs flow automatically from the C# source:

1. **Source Generator** (`BlazingSpire.SourceGenerator`) — emits `PlaygroundFactories.g.cs` with closed-generic render factories per component
2. **DocGen Tool** (`tools/BlazingSpire.DocGen`) — runs post-build, generates `docs/openapi.json`, `docs/examples/*.tonl`, and `wwwroot/components.json`
3. **ComponentPlayground** (`Components/Demo/`) — interactive playground with live preview, auto-generated controls, and live code snippet

Demo pages use: `<ComponentPlayground ComponentName="Alert" />` — that's the entire page.

## Key Files

- `BlazingSpire.sln` — Solution file
- `Directory.Packages.props` — Central Package Management
- `Directory.Build.props` / `.targets` / `.rsp` — Shared build configuration
- `wwwroot/app.css` — Tailwind v4 source with OKLCH theme tokens (light + dark)
- `wwwroot/index.html` — Skeleton, boot sequence, script loading order
- `wwwroot/components.json` — Generated component metadata for playground (auto-generated, do not hand-edit)
- `Components/Shared/` — Base component hierarchy (13 files)
- `Components/UI/` — All UI components (Alert, Button, Badge, Card, Input, etc.)
- `Components/UI/CodeBlock.razor` — Code snippet renderer (Prism highlighting)
- `Components/Demo/` — ComponentPlayground, PlaygroundControl, ComponentMetaService
- `Components/Layout/` — MainLayout, NavMenu, ThemeToggle
- `src/BlazingSpire.SourceGenerator/` — Roslyn source generator for render factories
- `tools/BlazingSpire.DocGen/` — Post-build tool: OpenAPI + TONL + components.json generation
- `docs/openapi.json` — Generated OpenAPI 3.0 spec for all components (auto-generated)
- `docs/examples/*.tonl` — Generated TONL files per component for AI/MCP indexing (auto-generated)
- `test/BlazingSpire.Tests.Unit/` — bUnit component tests (base class in `Shared/BlazingSpireTestBase.cs`)
- `test/BlazingSpire.Tests.E2E/` — Playwright E2E tests
- `test/BlazingSpire.Tests.Performance/` — BenchmarkDotNet
- `.claude/agents/` — Agent definitions (domain experts + workers)
- `.claude/skills/team/` — `/team` orchestration skill
- `docs/research/` — Architecture research documents

## Conventions

- Tailwind v4 classes only — no inline styles in components (inline styles are acceptable in `index.html` skeleton for LCP)
- OKLCH color system — use semantic tokens (`primary`, `muted`, `destructive`, etc.), never raw color values in components
- Every component class and `[Parameter]` property must have a `/// <summary>` doc comment — this drives the playground, OpenAPI spec, and TONL output
- Demo pages use `<ComponentPlayground ComponentName="..." />` — never hand-write code examples
- `docs/openapi.json`, `docs/examples/*.tonl`, and `wwwroot/components.json` are auto-generated — do not hand-edit
- Prism.js runs in manual mode — `CodeBlock` triggers `Prism.highlightElement` via JS interop
