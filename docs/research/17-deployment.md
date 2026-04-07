# Deployment & CI/CD

## Cloudflare Pages

**Live URL:** https://blazingspire.pages.dev
**Project name:** `blazingspire`

### Static Files

| File | Purpose |
|------|---------|
| `wwwroot/_headers` | `Cache-Control: immutable` for `/_framework/*` |
| `wwwroot/_redirects` | SPA fallback: `/* /index.html 200` |
| `wwwroot/robots.txt` | Allow all crawlers |

### Cloudflare Settings to Disable

These Cloudflare features can corrupt Blazor WASM integrity checks:

| Feature | Location | Why |
|---------|----------|-----|
| Auto Minify (JS/HTML) | Speed > Optimization | Corrupts Blazor's integrity hashes |
| Rocket Loader | Speed > Optimization | Defers script loading, breaks blazor.webassembly.js |
| Email Obfuscation | Security > Scrape Shield | Injects scripts that break integrity |

### Cloudflare Features That Help

- **Brotli compression** — enabled by default on Pages, serves `.br` files automatically
- **Early Hints** — auto-enabled, extracts `<link rel="preload">` from HTML into 103 responses
- **HTTP/2** — enabled by default, parallelizes WASM assembly downloads

---

## GitHub Actions Pipeline

**File:** `.github/workflows/deploy.yml`

### Trigger

- Push to `main` with changes in `src/BlazingSpire.Demo/**`
- Manual via `workflow_dispatch` (for workflow-only changes)

### Steps

1. `actions/checkout@v5`
2. `actions/setup-dotnet@v5` (.NET 10.x)
3. `dotnet workload install wasm-tools` (required even without AOT — trimming needs it)
4. `actions/setup-node@v6` (Node 22)
5. `npm ci` (Tailwind CSS dependencies)
6. `npx @tailwindcss/cli` (build CSS)
7. `dotnet publish -c Release` (WASM publish with trimming)
8. `cloudflare/wrangler-action@v3` (deploy to Pages)

### Secrets

| Secret | Purpose |
|--------|---------|
| `CLOUDFLARE_API_TOKEN` | Wrangler deploy auth |
| `CLOUDFLARE_ACCOUNT_ID` | Cloudflare account identifier |

### Runner

Uses `ubuntu-latest` (GitHub-hosted). The org's self-hosted runners on Proxmox are not accessible to this repo — runner groups are restricted. If needed, add BlazingSpire in **Org Settings > Actions > Runner groups**.

### Notes

- Workflow file changes don't auto-trigger (not in `paths` filter). Use `gh workflow run deploy.yml`.
- Wrangler `--commit-dirty=true` flag suppresses warning from Tailwind build output in working directory.
- `wasm-tools` workload is needed even without AOT because the publish pipeline uses it for WebCIL packaging and IL stripping.
