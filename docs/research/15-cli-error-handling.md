# CLI Error Handling, Diagnostics & Edge Cases

## 1. `blazingspire doctor` Command

The `doctor` command validates the developer's environment and project configuration. It runs all checks unconditionally and reports a summary, modeled after `flutter doctor`.

### Checks

| # | Check | Pass | Warn | Fail |
|---|-------|------|------|------|
| 1 | .NET SDK version | >= 10.0 | >= 8.0 < 10.0 | < 8.0 or not found |
| 2 | Tailwind CSS availability | Standalone binary on PATH or Tailwind.MSBuild configured | npm-based tailwindcss found (slower builds) | Neither found |
| 3 | Project type detection | Recognized SDK type (Web App, WASM Standalone, MAUI Hybrid, RCL) | Multiple .csproj files in directory | No .csproj found |
| 4 | `blazingspire.json` validity | Present, parseable, schema version compatible | Present but schema version outdated | Missing or unparseable |
| 5 | `_Imports.razor` presence | Present with BlazingSpire `@using` directives | Present but missing BlazingSpire directives | Not found |
| 6 | BlazingSpire.Primitives version | NuGet version matches CLI version | Minor version mismatch | Major version mismatch or not installed |
| 7 | Tailwind.MSBuild configuration | PackageReference present, input/output CSS paths valid | PackageReference present but paths do not exist on disk | Not referenced in .csproj |
| 8 | Component directory | `componentsPath` from config exists and is writable | Exists but contains zero installed components | Does not exist |

### Example Output

```
$ dotnet blazingspire doctor

BlazingSpire CLI v1.2.0

Checking environment...

[PASS] .NET SDK 10.0.100
[WARN] Tailwind CSS found via npm (consider standalone binary for faster builds)
       Run: dotnet blazingspire init --tailwind standalone
[PASS] Project type: Blazor Web App (Auto render mode)
       MyApp.csproj - Microsoft.NET.Sdk.BlazorWebAssembly
[PASS] blazingspire.json is valid (schema v1)
[FAIL] _Imports.razor is missing @using MyApp.Components.UI
       Run: dotnet blazingspire init --fix-imports
[PASS] BlazingSpire.Primitives 1.2.0 (matches CLI)
[PASS] Tailwind.MSBuild 2.0.0 configured
       Input:  wwwroot/input.css
       Output: wwwroot/app.css
[PASS] Components directory: Components/UI/ (4 components installed)

Result: 6 passed, 1 warning, 1 error

Run "dotnet blazingspire doctor --fix" to auto-resolve fixable issues.
```

### Flags

| Flag | Behavior |
|------|----------|
| `--fix` | Attempt automatic resolution for all fixable issues (adds missing `@using`, creates missing directories) |
| `--json` | Output results as JSON for CI integration |
| `--check` | Exit with non-zero code if any check fails (for CI gates) |

### JSON Output Schema

```json
{
  "cliVersion": "1.2.0",
  "checks": [
    {
      "name": "dotnet-sdk",
      "status": "pass",
      "message": ".NET SDK 10.0.100",
      "detail": null
    },
    {
      "name": "imports-razor",
      "status": "fail",
      "message": "_Imports.razor is missing @using MyApp.Components.UI",
      "detail": "Run: dotnet blazingspire init --fix-imports",
      "fixable": true
    }
  ],
  "summary": { "pass": 6, "warn": 1, "fail": 1 }
}
```

---

## 2. Error Catalog

All errors follow the format `BSxxxx` and are printed with a machine-readable code, human-readable message, and actionable resolution.

### Format

```
error BS1001: .NET SDK 8.0 or later is required (found 7.0.400)

  The BlazingSpire CLI requires .NET SDK 8.0 as a minimum. Version 10.0
  is recommended for full feature support.

  Resolution:
    1. Download .NET 10 SDK from https://dot.net/download
    2. Run "dotnet --version" to confirm installation
    3. If using global.json, ensure it allows SDK 8.0+
```

### BS1xxx -- Environment Errors

| Code | Message | Cause | Resolution |
|------|---------|-------|------------|
| BS1001 | .NET SDK 8.0 or later is required (found {version}) | SDK too old or not installed | Install .NET 8.0+ SDK from https://dot.net/download |
| BS1002 | .NET SDK not found on PATH | `dotnet` command not found | Install .NET SDK and ensure it is on PATH |
| BS1003 | Tailwind CSS binary not found | Neither standalone binary nor npm `tailwindcss` is available | Run `dotnet blazingspire init --tailwind standalone` or install via npm |
| BS1004 | Tailwind CSS version {version} is below minimum 4.0 | Outdated Tailwind installation | Update to Tailwind CSS 4.0+ |
| BS1005 | Node.js not found (required for npm-based Tailwind) | Using npm method but Node.js is missing | Install Node.js or switch to standalone: `--tailwind standalone` |
| BS1006 | Insufficient disk space for component extraction | Less than 10 MB free in target directory | Free disk space in the project directory |
| BS1007 | Global.json pins SDK to {version} which is incompatible | `global.json` restricts SDK to an unsupported version | Update `global.json` to allow SDK 8.0+ or remove `rollForward` restriction |

### BS2xxx -- Project Errors

| Code | Message | Cause | Resolution |
|------|---------|-------|------------|
| BS2001 | No .csproj file found in {directory} | Command run outside a project directory | `cd` into a project directory or use `--project <path>` |
| BS2002 | Multiple .csproj files found in {directory} | Ambiguous project reference | Use `--project <name>.csproj` to specify which project |
| BS2003 | Project SDK "{sdk}" is not a supported Blazor SDK | .csproj uses an SDK type BlazingSpire does not support (e.g., `Microsoft.NET.Sdk.Worker`) | BlazingSpire supports `Microsoft.NET.Sdk.BlazorWebAssembly`, `Microsoft.NET.Sdk.Razor`, `Microsoft.NET.Sdk.Web`, and MAUI projects |
| BS2004 | Target framework {tfm} is not supported | Project targets an unsupported TFM (e.g., `net6.0`) | Retarget to `net8.0` or later |
| BS2005 | Namespace conflict: "{namespace}" already exists in project | Generated component namespace clashes with existing code | Change `rootNamespace` in `blazingspire.json` or rename the conflicting type |
| BS2006 | Project file is read-only or locked | Cannot modify .csproj to add PackageReference | Close the IDE or unlock the file, then retry |
| BS2007 | _Imports.razor not found | The file is missing from the project | Run `dotnet blazingspire init --fix-imports` to create it |
| BS2008 | Solution uses Directory.Build.props that conflicts with BlazingSpire settings | Global MSBuild properties override project-level settings | See Section 6 (Multi-project support) for guidance |

### BS3xxx -- Configuration Errors

| Code | Message | Cause | Resolution |
|------|---------|-------|------------|
| BS3001 | blazingspire.json not found | `init` was never run | Run `dotnet blazingspire init` |
| BS3002 | blazingspire.json is not valid JSON | Syntax error in config file | Fix the JSON syntax; run `dotnet blazingspire doctor` for details |
| BS3003 | blazingspire.json schema version {version} is not supported by CLI {cliVersion} | Config file was written by a newer CLI | Update the CLI: `dotnet tool update -g BlazingSpire.CLI` |
| BS3004 | Required field "{field}" is missing in blazingspire.json | A mandatory field was deleted or never created | Run `dotnet blazingspire init --fix` to regenerate missing fields |
| BS3005 | componentsPath "{path}" does not exist | Configured directory was deleted or renamed | Create the directory or update `componentsPath` in `blazingspire.json` |
| BS3006 | Duplicate component entry "{name}" in installed list | Manual edit introduced duplicate | Remove the duplicate entry from `blazingspire.json` |
| BS3007 | Config version mismatch: blazingspire.json is v{old}, CLI expects v{new} | CLI was upgraded but config was not migrated | Run `dotnet blazingspire migrate-config` |

### BS4xxx -- Component Errors

| Code | Message | Cause | Resolution |
|------|---------|-------|------------|
| BS4001 | Component "{name}" not found in registry | Typo or component does not exist | Run `dotnet blazingspire list` to see available components |
| BS4002 | Dependency "{dep}" required by "{name}" could not be resolved | Registry dependency chain is broken | This may be a registry bug; try `--force` or report the issue |
| BS4003 | File "{path}" already exists and differs from template | Target file was locally modified | Use `--force` to overwrite (backs up to `.blazingspire-backup/`) or `--skip-existing` |
| BS4004 | Cannot write to "{path}": permission denied | File system permissions | Check directory permissions; on macOS/Linux ensure write access to the project directory |
| BS4005 | Component "{name}" is already installed at version {version} | Re-adding an already installed component | Use `dotnet blazingspire update {name}` to update, or `--force` to reinstall |
| BS4006 | Circular dependency detected: {chain} | Registry defines a dependency cycle | Report this as a registry bug |
| BS4007 | Component "{name}" requires BlazingSpire.Primitives >= {version} | Installed NuGet package is too old | Update the NuGet package: `dotnet add package BlazingSpire.Primitives` |

### BS5xxx -- Tailwind Errors

| Code | Message | Cause | Resolution |
|------|---------|-------|------------|
| BS5001 | Tailwind standalone binary not found at "{path}" | Binary was deleted or path changed | Run `dotnet blazingspire init --tailwind standalone` to re-download |
| BS5002 | Tailwind build failed with exit code {code} | CSS compilation error | Check the Tailwind output above for syntax errors in your CSS or Razor files |
| BS5003 | Tailwind CSS input file "{path}" not found | `inputCss` path in config does not exist | Create the file or update `tailwind.inputCss` in `blazingspire.json` |
| BS5004 | Tailwind CSS output path "{path}" is not writable | Permission or path issue for output CSS | Ensure the `wwwroot/` directory exists and is writable |
| BS5005 | Tailwind.MSBuild package not found in project | PackageReference missing from .csproj | Run `dotnet add package Tailwind.MSBuild` |
| BS5006 | Tailwind version mismatch: {found} (expected >= 4.0) | Project uses Tailwind 3.x patterns but CLI requires 4.x | Update Tailwind or run `dotnet blazingspire init --tailwind standalone` |
| BS5007 | Tailwind content paths do not include Razor files | `@source` directive missing `**/*.razor` | Add `@source "../**/*.razor";` to your input CSS file |

---

## 3. First-Run Edge Cases (`blazingspire init`)

### Decision Matrix

| Scenario | Detection | Init Behavior | Skips | Warnings |
|----------|-----------|---------------|-------|----------|
| **Blazor Web App (Server)** | SDK = `Microsoft.NET.Sdk.Web`, no `BlazorWebAssembly` workload reference | Full init: config, Tailwind, Primitives NuGet, `_Imports.razor`, component dir, remove Bootstrap | -- | -- |
| **Blazor Web App (WASM)** | SDK = `Microsoft.NET.Sdk.BlazorWebAssembly` | Full init; sets `tailwind.outputCss` to `wwwroot/css/app.css` (WASM default) | -- | -- |
| **Blazor Web App (Auto)** | `.Client` project detected alongside server project | Inits **both** projects; components go into `.Client`, config references shared paths | -- | "Detected Auto render mode. Components will be placed in {name}.Client for WASM compatibility." |
| **WASM Standalone** | SDK = `Microsoft.NET.Sdk.BlazorWebAssembly`, no server project sibling | Full init; adjusts CSS paths for standalone layout (`wwwroot/css/`) | -- | -- |
| **MAUI Hybrid** | SDK = `Microsoft.NET.Sdk.Maui` or `UseMaui` property set | Full init but skips Tailwind.MSBuild (Tailwind runs as a pre-build script instead); does not modify MAUI-specific files (`MauiProgram.cs`) | Tailwind.MSBuild PackageReference | "MAUI Hybrid detected. Tailwind will be configured as a pre-build command rather than MSBuild target." |
| **Razor Class Library** | SDK = `Microsoft.NET.Sdk.Razor`, no `<Project Sdk="...Web">` | Config only; skips Tailwind setup (RCL consumers handle CSS); adds Primitives NuGet | Tailwind setup, Bootstrap removal | "RCL detected. Tailwind must be configured in the consuming project, not the library." |
| **Existing project with Bootstrap** | `bootstrap.min.css` reference found in `App.razor`, `_Host.cshtml`, or `index.html` | Prompts: "Remove Bootstrap references? [Y/n/coexist]"; default Y removes `<link>` tags and deletes `bootstrap/` from `wwwroot/` | -- | If user chooses N or coexist, see Section 7 |
| **Existing project with MudBlazor** | `MudBlazor` PackageReference in .csproj | Aborts with guidance | Full init | "error BS2005: MudBlazor detected. BlazingSpire components may conflict with MudBlazor's global styles and JS. Run with `--coexist` to install alongside, or remove MudBlazor first." |
| **Existing project with other component library** | Known libraries scanned: Radzen, Syncfusion, Telerik, FluentUI | Warns but continues | -- | "warning: {Library} detected. BlazingSpire is designed to coexist with headless component libraries but may have CSS specificity conflicts. Consider `--coexist` mode." |
| **Multiple .csproj in directory** | `Directory.EnumerateFiles("*.csproj").Count() > 1` | Aborts with BS2002 | All | "error BS2002: Multiple .csproj files found. Use `--project <name>.csproj`." |
| **Project inside solution with Directory.Build.props** | Walk parent directories for `Directory.Build.props` | Continues normally; reads existing props to detect conflicts (e.g., `RootNamespace` override, `ManagePackageVersionsCentrally`) | -- | If CPM is enabled: "Central Package Management detected. BlazingSpire will add to Directory.Packages.props instead of inline version." |

### Auto-Detection Logic

```
1. Find .csproj (single file or --project flag)
2. Parse <Project Sdk="..."> attribute
3. Parse <TargetFramework>, <UseMaui>, <OutputType>
4. Check for sibling .Client project (Auto render mode)
5. Scan for existing CSS frameworks (Bootstrap, Tailwind, other)
6. Scan for existing component libraries (MudBlazor, Radzen, etc.)
7. Walk parent directories for Directory.Build.props / Directory.Packages.props
8. Present findings and prompt for confirmation (or proceed if --yes)
```

### Init Flow (detailed)

```
$ dotnet blazingspire init

Detected: Blazor Web App (Auto render mode)
  Server:  MyApp/MyApp.csproj
  Client:  MyApp/MyApp.Client/MyApp.Client.csproj
  TFM:     net10.0
  Existing CSS: Bootstrap 5.3 (wwwroot/bootstrap/)

? Remove Bootstrap and replace with Tailwind CSS? [Y/n/coexist] Y
? Component directory? [Components/UI]
? Root namespace? [MyApp.Client.Components.UI]

Initializing...
  [1/7] Creating blazingspire.json
  [2/7] Creating Components/UI/ directory
  [3/7] Installing base utilities (Cn.cs, theme variables)
  [4/7] Adding BlazingSpire.Primitives 1.2.0 to MyApp.Client.csproj
  [5/7] Updating _Imports.razor
  [6/7] Configuring Tailwind CSS (standalone binary)
  [7/7] Removing Bootstrap references

Done. Run "dotnet blazingspire doctor" to verify setup.
```

---

## 4. Rollback and Dry-Run

### Transaction Model

Every `init` and `add` command operates as a transaction. The CLI records every file system change in an in-memory journal before committing. If any step fails, all preceding changes are rolled back.

#### Journal Entry Types

```csharp
public enum ChangeType
{
    CreateFile,       // new file written
    ModifyFile,       // existing file modified (original content saved)
    DeleteFile,       // file removed (original content saved)
    CreateDirectory,  // new directory created
    AddNugetPackage,  // PackageReference added to .csproj
    ModifyImports     // _Imports.razor modified
}

public record JournalEntry(
    ChangeType Type,
    string Path,
    byte[]? OriginalContent,  // null for CreateFile / CreateDirectory
    byte[]? NewContent
);
```

#### Rollback Behavior

```
$ dotnet blazingspire add dialog

  [1/4] Installing dependency: portal... done
  [2/4] Installing dependency: button... done
  [3/4] Copying Dialog component files... done
  [4/4] Updating _Imports.razor... FAILED (BS4004: permission denied)

  Rolling back 3 completed steps...
    Removed Components/UI/Dialog/
    Removed Components/UI/Portal/   (was not previously installed)
    Kept Components/UI/Button/      (was already installed before this command)
    Reverted _Imports.razor

  error BS4004: Cannot write to "Components/UI/_Imports.razor": permission denied

  No changes were made. Fix the permission issue and retry.
```

Key rollback rules:
- Components that were already installed before the command are never removed during rollback.
- NuGet package additions are rolled back by removing the `<PackageReference>` line (not running `dotnet remove package`, which is slower and can fail).
- If rollback itself fails, the CLI prints the journal as JSON to `stderr` so the user can manually undo.

### `--dry-run` Flag

Shows exactly what would change without writing anything to disk.

```
$ dotnet blazingspire add dialog --dry-run

Dry run: no files will be written.

Would install 3 components: portal, button, dialog

  CREATE  Components/UI/Portal/Portal.razor         (48 lines)
  CREATE  Components/UI/Portal/Portal.razor.cs      (32 lines)
  CREATE  Components/UI/Button/Button.razor          (skipped: already installed)
  CREATE  Components/UI/Dialog/Dialog.razor          (67 lines)
  CREATE  Components/UI/Dialog/Dialog.razor.cs       (54 lines)
  CREATE  Components/UI/Dialog/Dialog.razor.js       (18 lines)
  MODIFY  _Imports.razor
          + @using MyApp.Components.UI.Portal
          + @using MyApp.Components.UI.Dialog
  MODIFY  blazingspire.json
          + "portal": { "version": "1.0.0" }
          + "dialog": { "version": "1.0.0" }
  MODIFY  MyApp.Client.csproj
          (no changes: BlazingSpire.Primitives already referenced)

Summary: 5 files created, 2 files modified, 0 files deleted
```

### `--verbose` Flag

Enables detailed diagnostic output for debugging init/add failures.

```
$ dotnet blazingspire add dialog --verbose

[VERBOSE] Loading blazingspire.json from /Users/dev/MyApp/blazingspire.json
[VERBOSE] Schema version: 1, CLI version: 1.2.0 -- compatible
[VERBOSE] Resolving dependencies for "dialog"...
[VERBOSE]   dialog -> [portal, button]
[VERBOSE]   portal -> [] (leaf)
[VERBOSE]   button -> [] (leaf, already installed at 1.0.0)
[VERBOSE] Dependency order: portal, dialog
[VERBOSE] Fetching component manifest: portal@1.0.0
[VERBOSE]   Source: embedded://components/portal/manifest.json
[VERBOSE]   Files: Portal.razor, Portal.razor.cs
[VERBOSE] Writing /Users/dev/MyApp/Components/UI/Portal/Portal.razor (48 lines)
[VERBOSE] Namespace rewrite: BlazingSpire.Components.UI.Portal -> MyApp.Components.UI.Portal
[VERBOSE] Writing /Users/dev/MyApp/Components/UI/Portal/Portal.razor.cs (32 lines)
[VERBOSE] Fetching component manifest: dialog@1.0.0
[VERBOSE]   Source: embedded://components/dialog/manifest.json
[VERBOSE]   Files: Dialog.razor, Dialog.razor.cs, Dialog.razor.js
[VERBOSE] Writing /Users/dev/MyApp/Components/UI/Dialog/Dialog.razor (67 lines)
...
[VERBOSE] Updating _Imports.razor: appending 2 @using directives
[VERBOSE] Updating blazingspire.json: adding 2 installed entries
[VERBOSE] Computing file hashes for installed components...
[VERBOSE]   portal/Portal.razor       sha256:a1b2c3d4...
[VERBOSE]   portal/Portal.razor.cs    sha256:e5f6a7b8...
[VERBOSE]   dialog/Dialog.razor       sha256:1a2b3c4d...
[VERBOSE]   dialog/Dialog.razor.cs    sha256:5e6f7a8b...
[VERBOSE]   dialog/Dialog.razor.js    sha256:9c0d1e2f...
[VERBOSE] Transaction committed: 5 creates, 2 modifies, 0 deletes
```

---

## 5. Upgrade Path and Three-Way Merge

### Hash Tracking

When a component is installed, the CLI stores the SHA-256 hash of each file's original content in `blazingspire.json`:

```json
{
  "installed": {
    "button": {
      "version": "1.0.0",
      "installedAt": "2026-04-07",
      "files": {
        "Button/Button.razor": {
          "originalHash": "sha256:a1b2c3d4e5f6..."
        },
        "Button/Button.razor.cs": {
          "originalHash": "sha256:9c0d1e2f3a4b..."
        }
      }
    }
  }
}
```

This enables detection of local modifications without needing to store the original file content on disk.

### `blazingspire diff <component>`

Shows what has changed upstream and what has changed locally.

```
$ dotnet blazingspire diff button

Button v1.0.0 (installed) -> v1.1.0 (upstream)

Button/Button.razor:
  Local status:  modified (hash differs from original)
  Upstream status: updated in v1.1.0

  --- original (v1.0.0)
  +++ upstream (v1.1.0)
  @@ -12,6 +12,8 @@
       <button class="@CssClass" @onclick="OnClick" disabled="@Disabled">
           @ChildContent
       </button>
  +    @if (Loading)
  +    {
  +        <span class="bs-spinner" aria-hidden="true"></span>
  +    }

Button/Button.razor.cs:
  Local status:  unmodified
  Upstream status: updated in v1.1.0

  --- original (v1.0.0)
  +++ upstream (v1.1.0)
  @@ -8,6 +8,9 @@
       [Parameter]
       public bool Disabled { get; set; }
  +
  +    [Parameter]
  +    public bool Loading { get; set; }

Summary: 2 files changed upstream, 1 file modified locally
         Merge required for Button/Button.razor (both sides changed)
         Clean update for Button/Button.razor.cs (only upstream changed)
```

### `blazingspire update <component>`

Performs a three-way merge for each file:

```
Base:     original file content at install time (reconstructed from registry at recorded version)
Local:    current file on disk
Upstream: new file from registry at latest version
```

#### Merge Outcomes Per File

| Local Modified? | Upstream Modified? | Action |
|-----------------|--------------------|--------|
| No | No | No change needed |
| No | Yes | Replace with upstream (clean update) |
| Yes | No | Keep local (no upstream changes) |
| Yes | Yes | Three-way merge; prompt on conflicts |

#### Merge Algorithm

The CLI uses a line-based three-way merge (same algorithm as `git merge-file`). The implementation shells out to `git merge-file` if git is available, otherwise falls back to a built-in diff3 implementation.

```
$ dotnet blazingspire update button

Updating button: v1.0.0 -> v1.1.0

  Button/Button.razor:
    Both local and upstream modified.
    Auto-merging... CONFLICT (1 hunk)
    Conflict saved to Button/Button.razor with merge markers.

  Button/Button.razor.cs:
    Only upstream modified.
    Updated cleanly.

1 file updated, 1 file has conflicts.
Resolve conflicts in Button/Button.razor, then run:
  dotnet blazingspire resolve button
```

#### Conflict Markers

Standard three-way merge markers are inserted into Razor files:

```razor
<<<<<<< local
<button class="@CssClass @CustomClass" @onclick="OnClick" disabled="@Disabled">
=======
<button class="@CssClass" @onclick="OnClick" disabled="@Disabled" aria-busy="@Loading">
>>>>>>> upstream (v1.1.0)
```

### `--force` Flag

Overwrites local files with upstream content, creating backups first:

```
$ dotnet blazingspire update button --force

Backing up modified files to .blazingspire-backup/2026-04-07T14-30-00/
  button/Button.razor -> .blazingspire-backup/2026-04-07T14-30-00/button/Button.razor
  button/Button.razor.cs -> .blazingspire-backup/2026-04-07T14-30-00/button/Button.razor.cs

Updating button: v1.0.0 -> v1.1.0
  Button/Button.razor      overwritten
  Button/Button.razor.cs   overwritten

Done. Your previous files are in .blazingspire-backup/2026-04-07T14-30-00/
```

The `.blazingspire-backup/` directory should be added to `.gitignore` by `init`.

### `--interactive` Flag

Walks through each merge hunk for manual resolution:

```
$ dotnet blazingspire update button --interactive

Updating button: v1.0.0 -> v1.1.0

Button/Button.razor - Hunk 1/1:

  Local:
    <button class="@CssClass @CustomClass" @onclick="OnClick" disabled="@Disabled">

  Upstream:
    <button class="@CssClass" @onclick="OnClick" disabled="@Disabled" aria-busy="@Loading">

  ? Accept: [l]ocal / [u]pstream / [b]oth / [e]dit manually? e

  Opening in $EDITOR...
  Saved.

Button/Button.razor.cs - no conflicts (clean upstream update applied)

Done. 2 files updated.
```

### `blazingspire resolve <component>`

Marks conflicts as resolved after the user has manually edited the files:

```
$ dotnet blazingspire resolve button

Checking Button/Button.razor for remaining conflict markers... none found.
Updating blazingspire.json:
  button.version: 1.0.0 -> 1.1.0
  button.files.Button/Button.razor.originalHash: updated

Resolved.
```

---

## 6. Multi-Project and Monorepo Support

### `--project` Flag

All commands accept `--project <path>` to target a specific project file:

```bash
dotnet blazingspire init --project src/MyApp.Client/MyApp.Client.csproj
dotnet blazingspire add button --project src/SharedUI/SharedUI.csproj
dotnet blazingspire doctor --project src/MyApp/MyApp.csproj
```

### Solution-Level Detection

When run from a directory containing a `.sln` or `.slnx` file, the CLI:

1. Parses the solution file to discover all projects.
2. Filters to Blazor-compatible projects (Web, BlazorWebAssembly, Razor, MAUI).
3. If exactly one compatible project is found, uses it automatically.
4. If multiple are found, prompts (or errors with BS2002 if `--yes` is set).

```
$ dotnet blazingspire init

Found solution: MyApp.sln
  Blazor projects:
    1. src/MyApp/MyApp.csproj              (Blazor Web App - Server)
    2. src/MyApp.Client/MyApp.Client.csproj (Blazor Web App - WASM)
    3. src/SharedUI/SharedUI.csproj         (Razor Class Library)

  ? Which project should own components? [1/2/3] 2

Initializing MyApp.Client...
```

### Shared RCL Pattern

For teams that share components across multiple Blazor apps, the recommended pattern is:

```
MySolution/
  src/
    SharedUI/                    <- Razor Class Library
      SharedUI.csproj
      blazingspire.json          <- componentsPath: "Components"
      Components/
        UI/
          Button/
          Dialog/
    MyApp.Server/
      MyApp.Server.csproj        <- references SharedUI
    MyApp.Maui/
      MyApp.Maui.csproj          <- references SharedUI
```

Init behavior for the RCL:
- Skips Tailwind setup (consuming apps handle CSS).
- Sets `rootNamespace` to the RCL's namespace.
- Adds BlazingSpire.Primitives to the RCL's .csproj.
- Warns that consuming projects must configure Tailwind independently.

### Directory.Build.props Interaction

When `Directory.Build.props` is detected in a parent directory, the CLI:

1. Reads it to check for `RootNamespace`, `ManagePackageVersionsCentrally`, and `TreatWarningsAsErrors`.
2. If Central Package Management (CPM) is enabled:
   - Adds `BlazingSpire.Primitives` version to `Directory.Packages.props` instead of inline in the .csproj.
   - Adds version-less `<PackageReference Include="BlazingSpire.Primitives" />` to the .csproj.
3. If `RootNamespace` is set globally, uses that value as the default root namespace (user can override).
4. Logs all Directory.Build.props interactions at `--verbose` level.

```
$ dotnet blazingspire init --verbose

[VERBOSE] Found Directory.Build.props at /Users/dev/MySolution/Directory.Build.props
[VERBOSE]   ManagePackageVersionsCentrally = true
[VERBOSE]   TreatWarningsAsErrors = true
[VERBOSE] Found Directory.Packages.props at /Users/dev/MySolution/Directory.Packages.props
[VERBOSE] Will add BlazingSpire.Primitives to Directory.Packages.props
```

---

## 7. Migration and Coexistence

### `blazingspire init --coexist` Mode

For projects that cannot remove their existing CSS framework immediately, `--coexist` mode installs BlazingSpire alongside Bootstrap (or another CSS framework) without conflicts.

#### What `--coexist` Does Differently

| Step | Normal Init | `--coexist` Init |
|------|-------------|------------------|
| Remove Bootstrap | Yes (prompts) | No |
| Tailwind `@layer` scoping | Not needed | Yes -- wraps all BlazingSpire styles in `@layer blazingspire` |
| CSS load order | Tailwind only | Bootstrap first, then Tailwind (scoped) |
| `preflight` (Tailwind reset) | Enabled | Disabled (would break Bootstrap) |
| Component classes | Standard Tailwind utilities | Prefixed or layered to avoid specificity conflicts |

#### Tailwind Configuration for Coexistence

The generated `input.css` in coexist mode:

```css
/* input.css -- coexistence mode */

/*
 * Tailwind's preflight is disabled to avoid conflicts with Bootstrap's
 * base styles. BlazingSpire components use explicit utility classes
 * that do not depend on preflight resets.
 */

@layer blazingspire {
  @import "tailwindcss/theme";
  @import "tailwindcss/utilities";
}

/*
 * BlazingSpire theme tokens as CSS custom properties.
 * These use the --bs- prefix (not --bs- from Bootstrap 5,
 * which uses --bs-; we use --bsp- to avoid collision).
 */
@layer blazingspire {
  :root {
    --bsp-background: oklch(1 0 0);
    --bsp-foreground: oklch(0.145 0 0);
    --bsp-primary: oklch(0.205 0.042 265.755);
    --bsp-primary-foreground: oklch(0.985 0.002 265.755);
    /* ... full token set ... */
  }
}
```

Key decisions:
- **`@layer blazingspire`**: All BlazingSpire styles are wrapped in a CSS cascade layer. Unlayered Bootstrap styles automatically win in specificity conflicts, which is correct for gradual migration (existing pages keep working).
- **`--bsp-` prefix**: Avoids collision with Bootstrap 5's `--bs-` custom property prefix.
- **No preflight**: Tailwind's CSS reset (`@tailwind base` / preflight) is excluded to avoid overriding Bootstrap's `reboot.css`.

#### Gradual Migration Path

The recommended migration from Bootstrap to BlazingSpire in an existing project:

```
Phase 1: Install with --coexist
  $ dotnet blazingspire init --coexist
  - Both frameworks active
  - New pages/components use BlazingSpire
  - Existing pages unchanged

Phase 2: Migrate page by page
  - Replace Bootstrap classes with Tailwind utilities
  - Replace Bootstrap components (Modal, Dropdown) with BlazingSpire equivalents
  - Use "dotnet blazingspire doctor" to track progress

Phase 3: Remove Bootstrap
  $ dotnet blazingspire migrate --remove-bootstrap
  - Removes Bootstrap CSS/JS references
  - Removes @layer scoping (promotes BlazingSpire to default layer)
  - Enables Tailwind preflight
  - Updates --bsp- prefix to --bs- (optional, controlled by config)
```

#### Example: Coexist Init Output

```
$ dotnet blazingspire init --coexist

Detected: Blazor Web App (Server)
  MyApp.csproj - net10.0
  Existing CSS: Bootstrap 5.3

Coexistence mode: Bootstrap will NOT be removed.

Initializing...
  [1/7] Creating blazingspire.json (coexist: true)
  [2/7] Creating Components/UI/ directory
  [3/7] Installing base utilities (Cn.cs, theme variables)
  [4/7] Adding BlazingSpire.Primitives 1.2.0
  [5/7] Updating _Imports.razor
  [6/7] Configuring Tailwind CSS (standalone, coexist mode)
         - Preflight disabled
         - All styles wrapped in @layer blazingspire
         - Theme tokens use --bsp- prefix
  [7/7] Updating App.razor CSS references
         - Kept: <link href="bootstrap/bootstrap.min.css" rel="stylesheet" />
         - Added: <link href="app.css" rel="stylesheet" /> (after Bootstrap)

Done. Both Bootstrap and BlazingSpire are active.

New components will use Tailwind utilities. Existing Bootstrap
pages are unaffected. See migration guide:
  https://blazingspire.dev/docs/migration/bootstrap

Run "dotnet blazingspire doctor" to verify setup.
```

### `blazingspire migrate --remove-bootstrap`

Once all pages have been migrated, this command completes the transition:

1. Scans all `.razor` files for remaining Bootstrap class names (e.g., `btn btn-primary`, `container`, `row`, `col-*`).
2. Reports any files still using Bootstrap classes.
3. If `--force` is passed or no Bootstrap usage is found:
   - Removes Bootstrap `<link>` and `<script>` tags from `App.razor` / `_Host.cshtml` / `index.html`.
   - Deletes `wwwroot/bootstrap/` directory.
   - Removes `@layer blazingspire` wrapper from `input.css` (Tailwind styles become unlayered).
   - Enables Tailwind preflight.
   - Updates `blazingspire.json`: sets `coexist: false`.
4. If Bootstrap classes are still found, prints a report and aborts:

```
$ dotnet blazingspire migrate --remove-bootstrap

Scanning for Bootstrap class usage...

  Pages/Counter.razor:       3 Bootstrap classes found (btn, btn-primary, mt-3)
  Pages/FetchData.razor:     5 Bootstrap classes found (table, table-striped, ...)
  Shared/NavMenu.razor:      8 Bootstrap classes found (nav, nav-link, ...)

16 Bootstrap references found in 3 files.
Cannot safely remove Bootstrap. Migrate these files first, then retry.

Use --force to remove Bootstrap anyway (pages may break).
```
