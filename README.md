# BlazingSpire

**An AI-first, test-driven Blazor component framework.** Inspired by shadcn/ui for visual design, built for machine consumption.

**Live demo:** [blazingspire.pages.dev](https://blazingspire.pages.dev)

## Why BlazingSpire

AI agents (Claude, Copilot, custom MCP servers) generate Blazor code at scale, but they need a machine-readable spec to do it correctly. Prose docs are ambiguous; agents hallucinate component APIs and generate broken code.

BlazingSpire solves this by making every component self-documenting. Each component's `///` XML comments and `[Parameter]` attributes flow automatically into three machine-readable outputs: [`docs/openapi.json`](docs/openapi.json) (OpenAPI 3.0), [`docs/examples/*.tonl`](docs/examples/) (Token-Optimized Notation, 40% smaller than JSON), and [`src/BlazingSpire.Demo/wwwroot/components.json`](src/BlazingSpire.Demo/wwwroot/components.json). An AI agent reads these specs instead of scraping prose, so it generates correct Blazor code without human review.

The proof is an end-to-end tested playground: [`test/BlazingSpire.Tests.E2E/Generated/`](test/BlazingSpire.Tests.E2E/Generated/) contains 300+ baseline-free tests covering metamorphic assertions, universal structural invariants, geometric relationships, and runtime type-graph validation. Zero screenshots, zero human approval — tests catch regressions automatically using first-principles oracles.

## For AI Agents / MCP Servers

Three machine-readable specifications, regenerated on every build:

- **[`docs/openapi.json`](docs/openapi.json)** — OpenAPI 3.0 schema for all components, with parameter types, defaults, constraints, and composition tree (`x-composition-*` extensions).
- **[`docs/examples/*.tonl`](docs/examples/)** — Token-Optimized Notation files per component. Smaller than JSON, optimized for AI tokenization and retrieval by semantic search / MCP.
- **[`src/BlazingSpire.Demo/wwwroot/components.json`](src/BlazingSpire.Demo/wwwroot/components.json)** — Metadata-driven component registry, consumed by the interactive playground.

All three are auto-generated from C# source by a Roslyn source generator ([`src/BlazingSpire.SourceGenerator/`](src/BlazingSpire.SourceGenerator/)) and a post-build DocGen tool ([`tools/BlazingSpire.DocGen/`](tools/BlazingSpire.DocGen/)). They never drift from the code.

## For Human Developers

- **Copy-paste model** — Source code lives in your repo; you own the code. Visual inspiration from shadcn/ui, Radix UI, and WAI-ARIA APG.
- **Tailwind CSS v4** with OKLCH color tokens, dark mode via `localStorage` (no eval).
- **Interactive playground** at `/components/{name}` with live preview, auto-generated parameter controls, and live code snippet.

## Architecture Highlights

- **Hierarchical `ChildOf<TParent>` composition** — The type graph *is* the composition tree. Walk `ChildOf<T>` base-type chains and you have the visual nesting structure.
- **`IRepeatingSlot<TRoot>`** — Runtime-driven repeating slots (e.g., `InputOTPSlot` counts driven by `InputOTP.MaxLength`). Playground re-counts on parameter toggle.
- **Roslyn source generator** — Emits trim-safe closed-generic render factories (`PlaygroundFactories.g.cs`) from the type graph. Supports hierarchical composition and repeating slots.
- **Post-build DocGen** — Runs after build, emits OpenAPI + TONL + `components.json`. Walks the same type graph as the generator so specs never drift.
- **Baseline-free E2E tests** — Metamorphic involution (parameter snap twice = identity), universal structural invariants (no dangling `aria-labelledby`, no empty `class=""`), geometric assertions (`Side="Top"` → `content.bottom <= trigger.top`). Zero human review.

## Build & Run

```bash
# All projects + tests
dotnet build

# Demo app (dev server with hot reload)
cd src/BlazingSpire.Demo
npm ci
npx @tailwindcss/cli -i wwwroot/app.css -o wwwroot/app.build.css
dotnet watch

# Tests (300+ E2E tests, ~32s on 10 threads)
dotnet test
```

Prerequisites: .NET 10 SDK, Node.js 22+. See [CLAUDE.md](CLAUDE.md) for the full development guide.

## Mission

**BlazingSpire is an AI-first, test-driven Blazor component framework.** The visual inspiration comes from shadcn/ui, Radix UI, and the modern copy-paste component ecosystem — but the *purpose* is different.

Our primary consumer is not a human developer copying markup from a docs site. Our primary consumer is **an AI coding agent** (Claude, Copilot, custom agents, MCP servers, IDE integrations) that needs a complete, machine-readable specification of every component so it can generate correct Blazor applications without human review.

Three things flow from this mission:

1. **The components' public contract is emitted as structured AI-consumable documentation.** `docs/openapi.json` and `docs/examples/*.tonl` describe every component's parameters, types, defaults, constraints, composition tree, variants, and ARIA semantics. Agents read them instead of scraping prose.

2. **The interactive playground is the live end-to-end proof** that what the spec describes is what the components actually do. Every top-level component has a `/components/{name}` page rendered through a generated factory. There is no separate demo directory or hand-written example — the playground *is* the demo, and its auto-generated content *is* the source of truth for what agents will consume.

3. **The test suite enforces the contract end-to-end with zero human review.** Every test is metadata-driven off `components.json` or the `ChildOf<T>` type graph, using metamorphic assertions and universal structural invariants. Baseline screenshot testing is explicitly rejected. If a test can't run unattended in CI and report pass/fail with full confidence, it doesn't belong here.

**The payoff**: an agent reads the TONL files and says "build me an inventory management UI using these components" — and the agent produces correct, compiling, semantically-valid Blazor code on the first try.

## Status

Current POC: **~180 top-level components, 800+ total components** (including composition children), **300+ baseline-free E2E tests**, full playground coverage.

Future: headless primitives NuGet package, CLI tool for scaffolding, MCP server exposing TONL specs directly to agent runtimes.

## License

MIT
