# CLI Tooling

## Commands

```bash
dotnet blazingspire init          # Scaffold config, Tailwind, utilities
dotnet blazingspire add button    # Copy component + dependencies
dotnet blazingspire add --all     # Copy all components
dotnet blazingspire list          # List available components
dotnet blazingspire diff button   # Show upstream changes vs local
dotnet blazingspire update button # Re-copy (with backup)
```

## Config File (`blazingspire.json`)

```json
{
  "$schema": "https://blazingspire.dev/schema/config.json",
  "componentsPath": "Components/UI",
  "rootNamespace": "MyApp.Components.UI",
  "style": "default",
  "tailwind": {
    "method": "standalone",
    "inputCss": "wwwroot/input.css",
    "outputCss": "wwwroot/app.css"
  },
  "installed": {
    "button": { "version": "1.0.0", "installedAt": "2026-04-07" }
  }
}
```

## Init Flow

1. Detect .csproj, SDK type, RootNamespace
2. Create `blazingspire.json`
3. Create `Components/UI/` directory
4. Install base utilities (`Cn.cs`, theme CSS variables)
5. Update `_Imports.razor` with `@using` directives
6. Add NuGet reference to `BlazingSpire.Primitives`
7. Set up Tailwind (standalone CLI via Tailwind.MSBuild or direct download)
8. Optionally remove Bootstrap defaults

## Registry Format (per-component JSON)

```json
{
  "name": "dialog",
  "description": "A modal dialog",
  "version": "1.0.0",
  "registryDependencies": ["button", "portal"],
  "nugetDependencies": ["BlazingSpire.Primitives"],
  "files": [
    { "path": "Dialog/Dialog.razor", "type": "component" },
    { "path": "Dialog/Dialog.razor.cs", "type": "component" },
    { "path": "Dialog/Dialog.razor.js", "type": "interop" }
  ],
  "actions": [
    { "type": "mergeImports", "namespaces": ["BlazingSpire.Components.UI.Dialog"] },
    { "type": "addNuget", "package": "BlazingSpire.Primitives", "version": "1.0.0" }
  ]
}
```

## Dependency Resolution

Recursive depth-first tree — install leaves first. Track installed set to avoid duplicates.

**Best references:**
- **ShadcnBlazor** — `ComponentDependencyTree` with polymorphic `RequiredActions` (CopyJs, AddNuget, MergeImports, AddService)
- **Pango UI** — Remote registry with Brotli-compressed files over HTTP
- **ShellUI** — MSBuild targets auto-generation, standalone Tailwind CLI download

## CLI Tech Stack

- **Spectre.Console.Cli** — command framework (mature, good DI via TypeRegistrar)
- **Component templates bundled with tool DLL** via `CopyToOutputDirectory`
- **Remote registry support** — static JSON on CDN for third-party registries
- **Namespace rewriting** — regex replace of `@namespace`/`namespace` declarations

## `dotnet new` Templates

Ship alongside the CLI in a separate `BlazingSpire.Templates` NuGet package:

### `blazingspire-app` (project template)

- Full Blazor Web App pre-configured with primitives, Tailwind, theme tokens
- Parameters: `Framework` (net10.0/net8.0), `RenderMode` (Server/WASM/Auto), `IncludeExamples` (bool)
- `sourceName` handles namespace substitution automatically

### `blazingspire-component` (item template)

- Scaffolds `.razor` + `.razor.cs` + optional `.razor.js`
- Includes variant enum, Cn() usage, ARIA attributes boilerplate
- `sourceName` substitution for component name

### Template vs CLI

| Scenario | Use |
|----------|-----|
| New project from scratch | `dotnet new blazingspire-app` |
| Add components to existing project | `dotnet blazingspire add button` |
| Scaffold a new custom component | `dotnet new blazingspire-component` |

## Update/Diff Mechanism

No Blazor CLI implementation handles this well yet. Recommended approach:
- Track installed version + hash in `blazingspire.json`
- `diff` command shows upstream changes vs local file
- `update --force` overwrites with backup
- Auto-detect customization by comparing file hash against original
