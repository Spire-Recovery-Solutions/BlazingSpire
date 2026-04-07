# Open Questions

## Architecture

1. **Multiple styles?** Should we support "default" and "new-york" variants like shadcn/ui, or start with one style and expand later?

2. **net8.0 compatibility?** Should the primitives package target `net8.0` for broader adoption, or `net10.0` only? API surface is stable since .NET 8, but `[PersistentState]` and `field` keyword require .NET 10.

3. **`Cn()` signature?** Should it use `params ReadOnlySpan<string?>` (C# 13+, zero-alloc) or `params string?[]` (broader compatibility)? This depends on the TFM decision.

## Distribution

4. **Templates vs CLI overlap?** `dotnet new blazingspire-app` creates a full project; `dotnet blazingspire init` configures an existing project. Is both needed or is one redundant?

5. **Component updates?** How do we handle updates for users who've customized their copies? Options: diff command, hash-based customization detection, manual re-add with backup.

## Tooling

6. **Tailwind class validation source generator?** Novel (none exists). Validate utility classes at compile time. Worth the investment or premature?

7. **Theme builder web tool?** From day one, or post-MVP? Competitors point users to third-party tools (tweakcn).

## Documentation

8. **BlazingStory or custom?** Ship a BlazingStory-based documentation site with component playground, or build a custom docs site?

## Testing

9. **Test coverage target?** What's the minimum coverage before first release? Suggestion: 90%+ on primitives, 70%+ on styled components (since users modify them).

10. **Playwright visual regression in CI?** Screenshot comparison across OS/browser can be flaky. Worth the maintenance cost or defer to manual review?

## Community

11. **Governance model?** Start with a single maintainer (like most competitors) or establish multi-maintainer structure from day one to avoid bus factor 1?

12. **Sponsorship/funding?** Set up GitHub Sponsors or Open Collective before launch?
