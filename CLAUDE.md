# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Mission

**BlazingSpire is an AI-first, test-driven Blazor component framework.** The visual inspiration comes from shadcn/ui, Radix UI, and the modern copy-paste component ecosystem — but the *purpose* is different.

Our primary consumer is not a human developer copying markup from a docs site. Our primary consumer is **an AI coding agent** (Claude, Copilot, custom agents, MCP servers, IDE integrations) that needs a complete, machine-readable specification of every component so it can generate correct Blazor applications without human review.

Three things flow from this mission, and they are inseparable:

1. **The components' public contract is emitted as structured AI-consumable documentation.** `docs/openapi.json` (OpenAPI 3.0) and `docs/examples/*.tonl` (Token-Optimized Notation Language, ~40% fewer tokens than JSON) describe every component's parameters, types, defaults, constraints, composition tree, variants, and ARIA semantics. These files are the canonical specification. Agents read them instead of scraping prose.

2. **The interactive playground is the live end-to-end proof** that what the TONL files describe is what the components actually do. Every top-level component has a `/components/{name}` page that renders the real component through a generated render factory, exposes every `[Parameter]` as a live control, and is covered by automated Playwright tests that run on every build. There is no separate demo directory or hand-written example — the playground *is* the demo, and its auto-generated content *is* the source of truth for what agents will consume.

3. **The test suite enforces the contract end-to-end with zero human review.** Every test must be metadata-driven off `components.json` or the `ChildOf<T>` type graph, and must catch regressions automatically. Baseline screenshot testing is explicitly rejected because it requires human approval. Tests instead use metamorphic assertions, universal structural invariants, and type-graph-derived fixtures that can't drift out of sync with the components. If a test can't run unattended in CI and report pass/fail with full confidence, it doesn't belong in our suite.

**The payoff**: a user can hand an AI agent the TONL files for BlazingSpire and say "build me an inventory management UI using these components," and the agent produces correct, compiling, semantically-valid Blazor code without ever visiting a docs site or reading human prose. The framework's value is measured by how often the AI's first output is correct.

**Corollary**: every architectural decision in this repo trades off against that measurement. Sparse playground coverage = AI gets wrong signals. Hand-authored demo samples = drift risk. Untested parameters = agents generate dead code. Hidden runtime cascades = agents can't infer the API from the type graph. *When in doubt, optimize for "what does the AI see?"*

Currently a POC with `src/BlazingSpire.Demo/` and test projects. Live demo: https://blazingspire.pages.dev. Future plans include a headless primitives NuGet package, a CLI tool, and an MCP server that exposes the TONL files directly to agent runtimes.

## Technical Summary

.NET 10 Blazor WebAssembly. Tailwind CSS v4 with OKLCH color tokens, dark mode via `localStorage`. Users can also copy-paste component source into their own projects — the same mechanism that makes shadcn/ui work — but this is a *secondary* consumption model. The primary model is AI-driven code generation against the TONL spec.

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

**Every agent in this project must internalize the mission**: we are building for AI consumers, not human users. When a worker agent ships a component, the success criterion is "can another AI agent read the generated TONL file and write correct Blazor markup that uses this component?" — not "does a human think this looks nice." When an expert agent advises on a design, the success criterion is "does this decision produce a cleaner, more machine-readable type graph?" — not "does this match Radix UI's API exactly." Use the reference implementations as inspiration for visual behavior and ARIA patterns, but diverge wherever divergence produces better machine-consumable output.

## Self-Documenting Playground = AI Specification

**The playground is not a marketing demo — it's the end-to-end proof that the AI-consumable specification is accurate.** Every output below exists to be consumed by machines first and humans second. When designing any component feature, ask: "Does this flow cleanly into the TONL file that an agent will read?"

Components self-document via `///` XML doc comments and `[Description]` attributes on `[Parameter]` properties. Four outputs flow automatically from the C# source:

1. **Source Generator** (`BlazingSpire.SourceGenerator`) — emits `PlaygroundFactories.g.cs` with closed-generic render factories per top-level component. For composites, the emitter is a recursive tree walker over the `ChildOf<T>` graph: each level produces a nested `RenderFragment` closure that mirrors the visual hierarchy. No suffix heuristics, no default-content maps, no hardcoded component lists — the composition *is* the type graph. Repeating slots implementing `IRepeatingSlot<TRoot>` become runtime `for`-loops driven by the static `GetSampleCount(root)` method, evaluated against the live root instance via `AddComponentReferenceCapture`, so parameter toggles re-drive the loop.
2. **DocGen Tool** (`tools/BlazingSpire.DocGen`) — runs post-build, generates `docs/openapi.json`, `docs/examples/*.tonl`, and `wwwroot/components.json`. Walks `ChildOf<T>` chains the same way the generator does. **This is the AI-facing API.** The TONL files are 40% smaller than JSON and include every parameter, type, default, constraint, variant value, composition tree link, and ARIA id wiring that an agent needs to generate correct Blazor markup.
3. **ComponentPlayground** (`Components/Demo/`) — interactive playground with live preview, auto-generated controls, and live code snippet. The live rendering is the round-trip proof that the TONL spec is accurate: if the playground renders wrong, the TONL output is lying, and any AI consumer will generate broken code.
4. **Playwright E2E suite** (`test/BlazingSpire.Tests.E2E/`) — runs against every playground page on every build, asserts that the rendered output matches the spec without any baseline files or human review. See "Test-Driven Correctness" below.

Demo pages use: `<ComponentPlayground ComponentName="Alert" />` — that's the entire page. There are **no** hand-written examples, **no** separate demo files, and **no** prose documentation that a human has to maintain. The playground, the TONL output, and the tests all regenerate from the same C# source on every build.

**Mission check per change**: when adding a new component or modifying an existing one, verify all four outputs are correct:
- Does `PlaygroundFactories.g.cs` emit a usable factory? (open generated output, read the `Render{Name}` method)
- Does `docs/examples/{name}.tonl` contain every parameter with the right type and default?
- Does the live playground render without errors and show every parameter as a working control?
- Do the tests catch regressions to any of the above without human intervention?

If any answer is "no," the mission is not served.

**Generator trimming safety:** every factory opens components via `builder.OpenComponent<TComponent>(0)` (closed generic). The trimmer statically sees every component type that can flow through the playground, so `TrimMode=full` works without manual `[DynamicallyAccessedMembers]` annotations.

**Frame ordering rule (important):** Blazor's `RenderTreeBuilder` requires all `AddAttribute` calls to come immediately after `OpenComponent`, before any other frame type. The generator emits the `ChildContent` attribute *before* the root's `AddComponentReferenceCapture` so this rule holds. The reference-capture fires before the `ChildContent` RenderFragment closures run, so `rootRef[0]` is populated by the time any `IRepeatingSlot.GetSampleCount(rootRef[0])` call executes.

## Test-Driven Correctness (zero human review)

Every test in this repo exists to protect the AI-consumable contract end-to-end **without any human having to review output**. If a test requires a human to approve a baseline, overwrite a `.verified` file, or squint at a screenshot diff, it does not belong here. That rules out:

- Visual regression (`toHaveScreenshot`, Chromatic, Percy, Applitools) — requires baseline approval
- Snapshot libraries (`Verify.*`) — requires `.received` → `.verified` promotion
- LLM-based visual verification — non-deterministic, not a gate

The techniques we use instead derive their oracle from first principles so there is nothing to approve:

1. **Metadata-driven parameterization.** Every theory's `[MemberData]` source enumerates `components.json` or walks the `ChildOf<T>` type graph. New components automatically get test coverage. Removed components automatically drop coverage. The fixture can never drift out of sync with the code.

2. **Metamorphic assertions.** Tests compare a component against *itself*, not against a baseline. Parameter involution (`snap(A) == snap(A_again) != snap(B)`) catches dead parameters without any reference to "what it looked like yesterday." Dark/light toggle involution catches theme breakage. Toggle-any-param-twice is an identity that holds for every correct component.

3. **Universal structural invariants.** One injected JavaScript function walks the DOM and returns a list of violated invariants: dangling `aria-labelledby` targets, duplicate ids, `<th>` inside `<tbody>`, elements with empty `class=""` from a broken variant map, `role="dialog"` hidden without `aria-hidden`. The invariants are true for *any* correct component, so no baseline is needed.

4. **Geometric relationships.** Floating elements are tested by asserting `content.bottom <= trigger.top` when `Side="Top"`, etc. The geometric contract is fixed by the laws of CSS and the intent of the `Side` enum — it does not depend on any stored state.

5. **Type-graph-derived runtime assertions.** `IRepeatingSlot<TRoot>` components are validated by toggling their count parameter and asserting the a11y tree contains exactly N nodes with the expected role. The oracle (N) comes from the parameter itself, not from a baseline.

**What a test failure means.** Any failure is a mission-critical bug: the AI-consumable spec and the actual runtime no longer agree. Either the spec (TONL/OpenAPI/playground factory) is lying, or the component is broken, or the test's invariant is wrong. There is no "this is just a baseline churn, approve and move on" category.

**What this means operationally.** The E2E suite is parallelized to 10 threads via `xunit.runner.json` and runs the full 300+ test suite in well under a minute. CI will add it as a required gate on every PR. Local `dotnet test` should pass cleanly before any push to `main`. If it doesn't, do not ship — the AI consumers of the next deploy will get broken signals.

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

**Mission-critical (non-negotiable):**
- Every component class and every `[Parameter]` property must have a `/// <summary>` doc comment. This text flows directly into the TONL/OpenAPI spec that AI consumers read. A component without summaries is lying to its consumers.
- Composition is expressed via `ChildOf<TImmediateParent>` — never via naming conventions, attributes, or hand-maintained registries. The type graph is the composition tree.
- Repeating structural children implement `IRepeatingSlot<TRoot>` with C# 11 static abstract members so the playground re-drives at runtime.
- `docs/openapi.json`, `docs/examples/*.tonl`, and `wwwroot/components.json` are auto-generated — do not hand-edit. They regenerate on every build.
- Demo pages are `<ComponentPlayground ComponentName="..." />` — nothing else. If a component needs a more elaborate example, that is a sign the component API isn't discoverable enough by AI consumers; fix the component, not the demo.
- Every new component or parameter must have corresponding automated test coverage before merge. Tests must be metadata-driven and require zero human review.

**Styling:**
- Tailwind v4 classes only — no inline styles in components (inline styles are acceptable in `index.html` skeleton for LCP)
- OKLCH color system — use semantic tokens (`primary`, `muted`, `destructive`, etc.), never raw color values in components
- Prism.js runs in manual mode — `CodeBlock` triggers `Prism.highlightElement` via JS interop

**When in doubt:**
- Ask "what does the AI agent reading this component's TONL file see?" Design decisions that produce richer, more accurate TONL output are correct. Design decisions that improve only the human-facing site are not.
