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
