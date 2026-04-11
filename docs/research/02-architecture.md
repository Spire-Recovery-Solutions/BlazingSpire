# Architecture Decisions

## Distribution: Copy-Paste CLI (like shadcn/ui)

```
dotnet tool install --global BlazingSpire.CLI
dotnet blazingspire init
dotnet blazingspire add button dialog tabs
```

**Why copy-paste over NuGet RCL:**
- Users own and can modify component source code
- Tailwind v4 content scanner automatically picks up `.razor` files in the project
- No bundle bloat — only used components exist
- No forced transitive dependencies
- NuGet RCLs can't ship Tailwind utilities properly (compiled DLLs, not source for scanning)

**What ships as a NuGet package:**
- `BlazingSpire.Primitives` — thin headless primitives (the only dependency)
- `BlazingSpire.CLI` — the dotnet tool
- `BlazingSpire.Templates` — `dotnet new` project/item templates

## Component Architecture: Two Layers

```
┌─────────────────────────────────────────┐
│  Styled Components (copy-paste)          │
│  Button, Dialog, Select, Tabs, etc.      │
│  Tailwind CSS classes, user-owned code   │
├─────────────────────────────────────────┤
│  Headless Primitives (NuGet package)     │
│  Focus trap, keyboard nav, ARIA,         │
│  positioning, portals, scroll lock       │
│  C# API + collocated .razor.js modules   │
└─────────────────────────────────────────┘
```

## Base Component Hierarchy

Styled components extend a tiered base class hierarchy defined in `Components/Shared/`. The hierarchy uses template method pattern, `FrozenDictionary` for variant/size mappings, and abstract members. See `docs/research/20-styled-component-patterns.md` for the full tree and choosing-a-base-class decision guide.

Key tiers: `BlazingSpireComponentBase` (structural) → `PresentationalBase<T>` (variants) → `InteractiveBase` (interactive) → `ButtonBase<V,S>` / `FormControlBase<T>` / `DisclosureBase` → `OverlayBase` → `PopoverBase` → `MenuBase`.

## Composition: Hierarchical `ChildOf<T>` + `IRepeatingSlot<T>`

Sub-components of a composite declare their immediate visual parent through the type system via `ChildOf<TImmediateContainer>`. The type argument is the component that directly wraps them in the rendered output — not the outer composite root. The source generator walks this chain to build a composition tree, and the playground factory emits a recursive `RenderFragment` closure that mirrors the visual hierarchy. There are no suffix heuristics, default-content maps, or hand-maintained child registries: the composition is the type graph, full stop.

```csharp
public partial class DialogContent     : ChildOf<Dialog>         { }
public partial class DialogHeader      : ChildOf<DialogContent>  { }
public partial class DialogTitle       : ChildOf<DialogHeader>   { }
public partial class DialogDescription : ChildOf<DialogHeader>   { }
public partial class DialogFooter      : ChildOf<DialogContent>  { }
public partial class DialogClose       : ChildOf<DialogFooter>   { }
```

`ChildOf<T>.Parent` is a `[CascadingParameter]` matching the immediate container. When a child needs state from an outer root (e.g., `DialogTitle` reading `Dialog.TitleId` for ARIA), it declares a second `[CascadingParameter] Dialog? DialogRoot` alongside `ChildOf<DialogHeader>`. Visual nesting (for the generator) and data flow (for the runtime) are two orthogonal type-system signals — do not conflate them.

Repeating slots implement `IRepeatingSlot<TRoot>` with C# 11 static abstract members. The generator emits a runtime `for`-loop that calls `GetSampleCount(root)` against a ref-captured root instance, so parameter toggles in the playground re-drive the loop count at render time:

```csharp
public partial class InputOTPSlot : ChildOf<InputOTPGroup>, IRepeatingSlot<InputOTP>
{
    public static int GetSampleCount(InputOTP root) => root.MaxLength;
    public static string IndexParameterName => nameof(Index);
    [Parameter] public int Index { get; set; }
}
```

See `.claude/agents/blazor-architecture.md` and `.claude/agents/component-builder.md` for the full authoring guide.

## Render Mode Strategy: Islands Architecture

- Components never set their own `@rendermode`
- Most content renders as static SSR
- Interactive primitives (Dialog, Combobox, Select) require an interactive render mode — consumer sets it
- Graceful degradation: components render valid static HTML with ARIA in SSR mode
- Dual API for overlays: declarative `<Dialog>` (same render mode) AND imperative `DialogService.Show<T>()` (cross-boundary)

## Technology Stack

### Core Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Components.Web` | 10.0.x | Blazor component infrastructure |
| `TailwindMerge.NET` | 1.3.x | CSS class conflict resolution |
| `Floating UI` (JS) | ~3KB | Floating element positioning |

### Dev Dependencies

| Package | Purpose |
|---------|---------|
| `bUnit` 2.7.x | Unit testing |
| `Playwright` 1.58+ | E2E + visual regression |
| `Deque.AxeCore.Playwright` | Accessibility testing |
| `Benchmark.Blazor` | Performance benchmarking |
| `Coverlet` | Code coverage |
| `Spectre.Console.Cli` | CLI framework |
| `Tailwind.MSBuild` 2.x | Build integration |

### Target Frameworks

- **Primary:** `net10.0`
- **Consider:** `net8.0` compatibility for wider adoption (stable API surface since .NET 8)
