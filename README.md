# BlazingSpire

A .NET 10 Blazor component framework inspired by [shadcn/ui](https://ui.shadcn.com). Copy-paste components, Tailwind CSS v4, OKLCH color system. You own the code.

**Live demo:** [blazingspire.pages.dev](https://blazingspire.pages.dev)

## Components

- **Button** — 6 variants (Default, Destructive, Outline, Secondary, Ghost, Link), 4 sizes
- **Badge** — 4 variants (Default, Secondary, Destructive, Outline)
- **Card** — Composable with CardHeader, CardTitle, CardDescription, CardContent, CardFooter

## Tech Stack

- .NET 10 Blazor WebAssembly (standalone)
- Tailwind CSS v4 with OKLCH color system
- SRS brand theme (red primary, orange accent, warm grays)
- Dark mode with `localStorage` persistence

## Performance

Lighthouse scores (desktop): **100 / 100 / 100 / 100**

| Metric | Value |
|--------|-------|
| FCP | 0.4s |
| LCP | 0.5s |
| TBT | 50ms |
| CLS | 0 |
| Framework payload (brotli) | 1.35 MB |

Key optimizations:
- Interpreter + Jiterpreter (no AOT — smaller payload, faster boot)
- Full trimming with trimmer root descriptors
- Invariant globalization/timezone, stripped unused subsystems
- Pre-rendered skeleton outside Blazor root for instant LCP
- Deferred Blazor start with skeleton-to-app swap

## Project Structure

```
src/
  BlazingSpire.Demo/
    Components/
      UI/           # Reusable components (Button, Badge, Card, etc.)
      Layout/        # MainLayout, ThemeToggle, NavMenu
      Pages/         # Demo pages
    wwwroot/
      app.css        # Tailwind source with OKLCH theme tokens
      index.html     # Skeleton + Blazor loader
      js/theme.js    # Theme toggle (no eval)
docs/
  research/          # Architecture research documents (Phase 1 & 2)
```

## Development

```bash
# Prerequisites: .NET 10 SDK, Node.js 22+

cd src/BlazingSpire.Demo
npm ci                    # Install Tailwind
dotnet watch              # Dev server with hot reload
```

## Deployment

Pushes to `main` auto-deploy to [Cloudflare Pages](https://blazingspire.pages.dev) via GitHub Actions.

## License

MIT
