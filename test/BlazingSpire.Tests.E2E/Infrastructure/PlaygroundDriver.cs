using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Page object for navigating and interacting with the BlazingSpire component playground.
/// Provides stable selectors backed by data-* attributes on playground controls.
/// </summary>
internal sealed class PlaygroundDriver
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    private readonly List<string> _consoleErrors = new();
    private readonly List<string> _pageErrors = new();

    public PlaygroundDriver(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;

        _page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                var text = msg.Text;
                // Suppress known benign noise
                if (text.Contains("favicon")) return;
                if (text.Contains("MonoWasm: Debugging")) return;
                if (text.Contains("The WebAssembly exception handling 'try'")) return;
                _consoleErrors.Add(text);
            }
        };

        _page.PageError += (_, err) => _pageErrors.Add(err);
    }

    public IReadOnlyList<string> ConsoleErrors => _consoleErrors;
    public IReadOnlyList<string> PageErrors => _pageErrors;

    /// <summary>Navigate to a component's playground page and wait for the preview to render.</summary>
    public async Task NavigateTo(string componentName)
    {
        _consoleErrors.Clear();
        _pageErrors.Clear();

        // Components use kebab-case routes for multi-word names
        var route = ToKebabCase(componentName);
        await _page.GotoAsync($"{_baseUrl}/components/{route}");

        // Wait for the playground card to render (it appears after components.json loads)
        await _page.Locator($"[data-playground='{componentName}']").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30_000,
        });

        // Wait for the preview pane to settle
        await _page.Locator("[data-playground-preview]").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5_000,
        });
    }

    /// <summary>Set an enum parameter value via the playground control (works for both ToggleGroup and Select variants).</summary>
    public async Task SetEnumParam(string paramName, string value)
    {
        var control = _page.Locator($"[data-param='{paramName}']");
        await control.WaitForAsync(new() { Timeout = 5_000 });

        // Try select first (enums with >4 values)
        var select = control.Locator("select");
        if (await select.CountAsync() > 0)
        {
            await select.SelectOptionAsync(value);
            return;
        }

        // Otherwise it's a toggle button group
        await control.Locator($"button:has-text('{value}')").First.ClickAsync();
    }

    /// <summary>Toggle a bool parameter via the Switch control.</summary>
    public async Task SetBoolParam(string paramName, bool value)
    {
        var control = _page.Locator($"[data-param='{paramName}']");
        await control.WaitForAsync(new() { Timeout = 5_000 });
        var switchRole = control.GetByRole(AriaRole.Switch).First;

        var isChecked = await switchRole.GetAttributeAsync("aria-checked") == "true";
        if (isChecked != value)
        {
            await switchRole.ClickAsync();
        }
    }

    /// <summary>Locator for the live preview pane.</summary>
    public ILocator Preview => _page.Locator("[data-playground-preview]");

    /// <summary>Locator for a specific parameter control wrapper.</summary>
    public ILocator ParamControl(string paramName) => _page.Locator($"[data-param='{paramName}']");

    /// <summary>Clear captured errors (call before an interaction step).</summary>
    public void ClearErrors()
    {
        _consoleErrors.Clear();
        _pageErrors.Clear();
    }

    private static string ToKebabCase(string name)
    {
        // DatePicker -> date-picker, AlertDialog -> alert-dialog
        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                sb.Append('-');
            sb.Append(char.ToLowerInvariant(name[i]));
        }
        return sb.ToString();
    }
}
