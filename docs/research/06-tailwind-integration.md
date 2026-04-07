# Tailwind CSS v4 Integration

## Setup

### Recommended: Tailwind.MSBuild v2.x (no Node.js)

```xml
<PackageReference Include="Tailwind.MSBuild" Version="2.*" />
```

- Auto-downloads standalone Tailwind CLI binary
- `BuildTailwind` target runs `BeforeTargets="BeforeBuild"`
- Smart watch mode: auto-activates in VS and `dotnet watch build`
- For `dotnet watch run`: pass `-p:TailwindWatch=true`
- Key properties: `TailwindVersion`, `TailwindInputFile`, `TailwindOutputFile`, `TailwindMinify` (auto: false Debug, true Release)

### CSS-First Config (Tailwind v4)

```css
/* wwwroot/input.css */
@import "tailwindcss";
@source "../Components/**/*.razor";

@custom-variant dark (&:where(.dark, .dark *));

@theme {
  /* BlazingSpire theme tokens */
}
```

No `tailwind.config.js` — Tailwind v4 uses CSS-first configuration exclusively.

### Auto-Detection

Tailwind v4 auto-detects `.razor` files in the project directory. If issues arise:
```css
@source "../Components/**/*.razor";
```

**Known gotcha:** Visual Studio can save CSS files with BOM encoding that corrupts Tailwind output. Ensure "UTF-8 without signature".

## Hot Reload / Watch Mode

| Approach | Setup |
|----------|-------|
| **Tailwind.MSBuild** (recommended) | `-p:TailwindWatch=true` with `dotnet watch run` |
| **Two terminals** | Terminal 1: `dotnet watch run`, Terminal 2: `npx @tailwindcss/cli --watch` |
| **npm concurrently** | `"dev": "concurrently \"dotnet watch run\" \"npx @tailwindcss/cli --watch\""` |

## Class Merging: TailwindMerge.NET

```csharp
// DI registration
builder.Services.AddTailwindMerge();

// In component
@inject TwMerge TwMerge
var result = TwMerge.Merge("px-4 py-2 bg-blue-500", "bg-red-500");
// → "px-4 py-2 bg-red-500"
```

- **NuGet:** TailwindMerge.NET v1.3.0, MIT, 37K downloads
- **Thread-safe LRU cache** for zero-allocation repeated merges
- Supports Tailwind v4.2

## Cn() Utility (shadcn/ui equivalent)

```csharp
// Use params ReadOnlySpan<string?> (C# 13) to avoid array allocation
public static string Cn(params ReadOnlySpan<string?> inputs)
{
    // Filter nulls, join, merge
    return TwMerge.Merge(string.Join(" ", inputs.ToArray().Where(s => !string.IsNullOrWhiteSpace(s))));
}
```

**Performance rules:**
- This is a hot path (called every render of every component)
- Cache merged results in `OnParametersSet` when parameters change
- Pre-compute variant dictionaries as `static readonly FrozenDictionary<TEnum, string>`
- Never build class strings at render time

## Variant System (CVA Equivalent)

No standalone C# CVA package exists. Use enum + dictionary:

```csharp
public enum ButtonVariant { Default, Destructive, Outline, Ghost }
public enum ButtonSize { Default, Sm, Lg, Icon }

private static readonly FrozenDictionary<ButtonVariant, string> VariantClasses =
    new Dictionary<ButtonVariant, string>
    {
        [ButtonVariant.Default] = "bg-primary text-primary-foreground hover:bg-primary/90",
        [ButtonVariant.Destructive] = "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        [ButtonVariant.Outline] = "border border-input bg-background hover:bg-accent",
        [ButtonVariant.Ghost] = "hover:bg-accent hover:text-accent-foreground",
    }.ToFrozenDictionary();
```

## CSS Isolation: Don't Use It

Blazor CSS isolation (`.razor.css`) is fundamentally incompatible with Tailwind's utility approach. All styling via utility classes directly in `.razor` markup.

## Bundle Size

| Library | CSS Size (minified) |
|---------|-------------------|
| MudBlazor | ~500KB |
| Bootstrap 5.3 | ~228KB |
| **Tailwind (typical project)** | **10-50KB** |

Tailwind's content scanning means CSS scales with usage, not library size.
