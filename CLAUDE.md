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
dotnet test test/BlazingSpire.Tests.E2E                             # Playwright, ~32s for 253 tests
```

The E2E suite is parallelized to 10 threads via `xunit.runner.json`. Each test class takes `IClassFixture<BlazorAppFixture>` (a static-singleton demo app shared across all classes) and `IClassFixture<PlaywrightBrowserFixture>` (a class-scoped browser + page so WASM only boots once per class). `BlazorAppFixture` auto-kills any orphaned `blazor-devserver` on :5299 at startup — a killed test run can't poison the next one. `ParameterPermutationTests` is sharded into 8 classes (`ParameterPermutationTestsShard0..7`) to saturate the 10-thread cap.

Prerequisites: .NET 10 SDK (`~/.dotnet/dotnet`), Node.js 22+, `wasm-tools` workload (`dotnet workload install wasm-tools`). Add `export PATH="$HOME/.dotnet:$PATH"` if `dotnet` is not on PATH.

Solution file: `BlazingSpire.sln`. Central Package Management via `Directory.Packages.props`. Build artifacts output to `artifacts/` via `ArtifactsPath`.

## Deployment

Pushes to `main` trigger `.github/workflows/deploy.yml` on any change under:
- `src/BlazingSpire.Demo/**`
- `src/BlazingSpire.SourceGenerator/**` (generator output is baked into the published WASM)
- `tools/BlazingSpire.DocGen/**` (DocGen runs post-build and emits `components.json`)
- `Directory.Build.*` / `Directory.Packages.props` / the workflow file itself

The workflow:
1. Build Tailwind CSS (`app.build.css`, minified)
2. Pre-highlight code blocks (`highlight-code.js`)
3. `dotnet publish` Blazor WASM in `Release`
4. Deploy `publish/wwwroot` to Cloudflare Pages via `wrangler-action@v3`

Secrets: `CLOUDFLARE_API_TOKEN`, `CLOUDFLARE_ACCOUNT_ID`. Project name: `blazingspire`.

`app.build.css` is gitignored — CI regenerates it. Never commit it.

The workflow's path filter is intentionally broader than just the demo project: a generator-only change (e.g., fixing the playground factory emission) still affects the final WASM payload, and the old narrower filter silently skipped deploys on those commits.

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

**Parent/child composition:** Sub-components inherit from `ChildOf<TParent>` in `Components/Shared/`, where `TParent` is the **immediate visual container**, not the outer composite root. The type graph literally encodes the composition tree:

```csharp
public partial class DialogContent     : ChildOf<Dialog>         { }
public partial class DialogHeader      : ChildOf<DialogContent>  { }
public partial class DialogTitle       : ChildOf<DialogHeader>   { }
public partial class DialogDescription : ChildOf<DialogHeader>   { }
public partial class DialogFooter      : ChildOf<DialogContent>  { }
public partial class DialogClose       : ChildOf<DialogFooter>   { }
```

`ChildOf<T>` exposes `Parent` as a `[CascadingParameter]` matching the immediate container. When a child needs access to the outer root's state (e.g., `DialogTitle` reading `Dialog.TitleId`), it adds an explicit `[CascadingParameter] Dialog? DialogRoot` alongside the `ChildOf<DialogHeader>` declaration — the root component cascades itself via `<CascadingValue Value="this">` so any descendant can resolve it regardless of nesting depth. Visual nesting (`ChildOf<T>`) and data-flow cascading are two orthogonal type-system signals.

The source generator and DocGen discover composition by walking `ChildOf<T>` base-type chains — no naming conventions, no attributes, no registries. The playground's tree-walk factory emitter (see `src/BlazingSpire.SourceGenerator/PlaygroundGenerator.cs`) recursively descends the graph, producing nested `RenderFragment` closures that mirror the visual hierarchy. Leaf children with a `ChildContent` parameter get placeholder text derived uniformly from class names — `PopoverTrigger` → `"Trigger"`, `AlertDialogAction` → `"Action"` — computed as `childName.Substring(rootName.Length)` with zero hand-maintained maps.

**Repeating slots:** Components that should emit N instances (driven by a runtime parameter) implement `IRepeatingSlot<TRoot>` with C# 11 static abstract members:

```csharp
public partial class InputOTPSlot : ChildOf<InputOTPGroup>, IRepeatingSlot<InputOTP>
{
    public static int GetSampleCount(InputOTP root) => root.MaxLength;
    public static string IndexParameterName => nameof(Index);
    [Parameter] public int Index { get; set; }
}
```

The generator detects the interface via type-graph walk and emits a runtime `for`-loop against the live root instance (captured via `AddComponentReferenceCapture`). Toggling `MaxLength` in the playground re-drives the loop — the count is live, not static.

Playground and smoke tests skip `Child`-role components automatically (they're rendered via their parent's playground).

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

1. **Source Generator** (`BlazingSpire.SourceGenerator`) — emits `PlaygroundFactories.g.cs` with closed-generic render factories per top-level component. For composites, the emitter is a recursive tree walker over the `ChildOf<T>` graph: each level produces a nested `RenderFragment` closure that mirrors the visual hierarchy. No suffix heuristics, no default-content maps, no hardcoded component lists — the composition *is* the type graph. Repeating slots implementing `IRepeatingSlot<TRoot>` become runtime `for`-loops driven by the static `GetSampleCount(root)` method, evaluated against the live root instance via `AddComponentReferenceCapture`, so parameter toggles re-drive the loop.
2. **DocGen Tool** (`tools/BlazingSpire.DocGen`) — runs post-build, generates `docs/openapi.json`, `docs/examples/*.tonl`, and `wwwroot/components.json`. Walks `ChildOf<T>` chains the same way the generator does.
3. **ComponentPlayground** (`Components/Demo/`) — interactive playground with live preview, auto-generated controls, and live code snippet.

Demo pages use: `<ComponentPlayground ComponentName="Alert" />` — that's the entire page.

**Generator trimming safety:** every factory opens components via `builder.OpenComponent<TComponent>(0)` (closed generic). The trimmer statically sees every component type that can flow through the playground, so `TrimMode=full` works without manual `[DynamicallyAccessedMembers]` annotations.

**Frame ordering rule (important):** Blazor's `RenderTreeBuilder` requires all `AddAttribute` calls to come immediately after `OpenComponent`, before any other frame type. The generator emits the `ChildContent` attribute *before* the root's `AddComponentReferenceCapture` so this rule holds. The reference-capture fires before the `ChildContent` RenderFragment closures run, so `rootRef[0]` is populated by the time any `IRepeatingSlot.GetSampleCount(rootRef[0])` call executes.

## Key Files

- `BlazingSpire.sln` — Solution file
- `Directory.Packages.props` — Central Package Management
- `Directory.Build.props` / `.targets` / `.rsp` — Shared build configuration
- `wwwroot/app.css` — Tailwind v4 source with OKLCH theme tokens (light + dark)
- `wwwroot/index.html` — Skeleton, boot sequence, script loading order
- `wwwroot/components.json` — Generated component metadata for playground (auto-generated, do not hand-edit)
- `Components/Shared/` — Base component hierarchy + `ChildOf<T>` + `IRepeatingSlot<T>` (14 files)
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
