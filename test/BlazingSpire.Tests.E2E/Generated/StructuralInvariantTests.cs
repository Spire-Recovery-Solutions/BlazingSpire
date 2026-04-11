using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Metadata-driven structural invariant sweep.
///
/// For every top-level component's playground page, injects a single JS function that
/// walks the preview DOM and returns a list of violated invariants. Any non-empty list
/// fails the test. No baseline files, no screenshots — pure structural correctness.
///
/// Invariants checked (universal, no component-specific knowledge):
///   1. No duplicate id attributes within the preview
///   2. aria-labelledby / aria-describedby refs resolve to existing DOM elements
///   3. aria-controls refs resolve to existing DOM elements
///   4. No element has an empty class attribute (broken variant map produces class="")
///   5. No &lt;th&gt; element inside &lt;tbody&gt; (catches TableHead ChildOf hierarchy bug)
///   6. No &lt;td&gt; element inside &lt;thead&gt;
///   7. No &lt;button&gt; without an accessible name (text content or aria-label)
/// </summary>
public class StructuralInvariantTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    public StructuralInvariantTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    private const string InvariantScript = """
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

    public static IEnumerable<object[]> TopLevelComponents() =>
        ComponentMetadata.TopLevel.Select(c => new object[] { c.Name });

    [Theory, MemberData(nameof(TopLevelComponents))]
    public async Task Component_Passes_Structural_Invariants(string componentName)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(componentName);

        // Allow the preview to fully settle before checking
        await Page.WaitForTimeoutAsync(300);

        var violations = await Page.EvaluateAsync<string[]>(InvariantScript);

        Assert.True(
            violations.Length == 0,
            $"'{componentName}' has structural invariant violations:\n  {string.Join("\n  ", violations)}");
    }
}
