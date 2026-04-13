using BlazingSpire.Tests.E2E;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for metadata-driven structural invariant sweep.
/// Concrete class is emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class StructuralInvariantTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected StructuralInvariantTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    /// <summary>
    /// JavaScript function injected into each playground page to check universal
    /// structural invariants. Returns a list of violation strings; empty = pass.
    /// </summary>
    protected const string InvariantScript = """
        (function checkInvariants() {
          const preview = document.querySelector('[data-playground-preview]');
          if (!preview) return ['NO_PREVIEW'];
          const errors = [];
          const all = Array.from(preview.querySelectorAll('*'));

          // 1. Duplicate IDs within the preview pane
          const ids = all.map(el => el.id).filter(Boolean);
          const seen = {};
          ids.forEach(id => {
            if (seen[id]) {
              errors.push('DUPLICATE_ID:' + id);
            }
            seen[id] = true;
          });

          // 2. Dangling aria-labelledby refs
          all.forEach(el => {
            const v = el.getAttribute('aria-labelledby');
            if (v) v.split(' ').forEach(id => {
              if (id && !document.getElementById(id))
                errors.push('DANGLING_LABELLEDBY:' + id);
            });
          });

          // 3. Dangling aria-describedby refs
          all.forEach(el => {
            const v = el.getAttribute('aria-describedby');
            if (v) v.split(' ').forEach(id => {
              if (id && !document.getElementById(id))
                errors.push('DANGLING_DESCRIBEDBY:' + id);
            });
          });

          // 4. Dangling aria-controls refs
          all.forEach(el => {
            const v = el.getAttribute('aria-controls');
            if (v && !document.getElementById(v))
              errors.push('DANGLING_CONTROLS:' + v);
          });

          // 5. Empty class attributes (broken variant map yields class="")
          all.forEach(el => {
            if (el.hasAttribute('class') && el.getAttribute('class').trim() === '')
              errors.push('EMPTY_CLASS:' + el.tagName.toLowerCase());
          });

          // 6. <th> inside <tbody> — catches TableHead ChildOf<TableRow> structural bug
          preview.querySelectorAll('tbody th').forEach(th =>
            errors.push('TH_IN_TBODY:' + th.textContent.trim().substring(0, 30))
          );

          // 7. <td> inside <thead>
          preview.querySelectorAll('thead td').forEach(() =>
            errors.push('TD_IN_THEAD')
          );

          return errors;
        })()
        """;

    protected static System.Collections.Generic.IEnumerable<object[]> TopLevelComponentsData() =>
        ComponentMetadata.TopLevel.Select(c => new object[] { c.Name });

    protected async Task RunStructuralInvariantAsync(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        await Page.WaitForTimeoutAsync(300);

        var violations = await Page.EvaluateAsync<string[]>(InvariantScript);

        Assert.True(
            violations.Length == 0,
            $"'{componentName}' has structural invariant violations:\n  {string.Join("\n  ", violations)}");
    }
}
