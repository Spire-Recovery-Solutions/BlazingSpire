using System.Text.RegularExpressions;
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

        // Wait for the playground card to render (it appears after components.json loads).
        // Generous timeout because 10 parallel Playwright browsers all hitting the dev
        // server for the WASM bundle can push the first navigation past 30s.
        await _page.Locator($"[data-playground='{componentName}']").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 90_000,
        });

        // Wait for the preview pane to settle
        await _page.Locator("[data-playground-preview]").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10_000,
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

    /// <summary>
    /// Returns normalized innerHTML of the scope, with non-deterministic attributes
    /// stripped (id, aria-labelledby, aria-describedby, aria-controls, for, data-floating-ui-*).
    /// Two snapshots of identical component state will compare equal; two snapshots of
    /// different states will compare differently — use this for involution/liveness assertions.
    /// </summary>
    public async Task<string> InvolutionSnapshotAsync(ILocator scope)
    {
        var html = await scope.InnerHTMLAsync();
        // Strip id attributes (component-generated, may contain random values)
        html = Regex.Replace(html, @"\s*id=""[^""]*""", "");
        // Strip ARIA reference attributes (reference generated IDs)
        html = Regex.Replace(html, @"\s*(aria-labelledby|aria-describedby|aria-controls|for)=""[^""]*""", "");
        // Strip Floating UI nonce attributes
        html = Regex.Replace(html, @"\s*data-floating-ui-[^\s=]+=(?:""[^""]*""|\S+)", "");
        return html;
    }

    /// <summary>Set a numeric parameter value via the playground control's number/text input.</summary>
    public async Task SetNumberParam(string paramName, int value)
    {
        var control = _page.Locator($"[data-param='{paramName}']");
        await control.WaitForAsync(new() { Timeout = 5_000 });
        var input = control.Locator("input[type='number'], input[type='text']").First;
        await input.FillAsync(value.ToString());
        // Blur the input to fire the 'change' event that Blazor @bind listens on.
        // PressAsync("Enter") on number inputs does NOT fire 'change' in Chromium.
        await input.PressAsync("Tab");
        await _page.WaitForTimeoutAsync(400);
    }

    /// <summary>
    /// Returns the floating content locator (element with [data-side] attribute set by Floating UI).
    /// Use BoundingBoxAsync() on the result to measure position after the floating layer opens.
    /// </summary>
    public ILocator FloatingContent => _page.Locator("[data-side]").First;

    /// <summary>Close an open floating element by pressing Escape.</summary>
    public async Task CloseFloatingAsync()
    {
        await _page.Keyboard.PressAsync("Escape");
        await _page.WaitForTimeoutAsync(200);
    }

    /// <summary>Clear captured errors (call before an interaction step).</summary>
    public void ClearErrors()
    {
        _consoleErrors.Clear();
        _pageErrors.Clear();
    }

    private static string ToKebabCase(string name)
    {
        // DatePicker -> date-picker, AlertDialog -> alert-dialog
        // InputOTP -> input-otp (treat consecutive caps as one acronym)
        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (i > 0 && char.IsUpper(c))
            {
                // Insert dash only at boundary between lowercase→uppercase
                // OR between uppercase→uppercase followed by lowercase (acronym → word)
                var prev = name[i - 1];
                var nextLower = i + 1 < name.Length && char.IsLower(name[i + 1]);
                if (char.IsLower(prev) || (char.IsUpper(prev) && nextLower))
                    sb.Append('-');
            }
            sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }
}
