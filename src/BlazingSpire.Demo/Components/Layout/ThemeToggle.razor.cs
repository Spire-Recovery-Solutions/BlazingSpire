using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingSpire.Demo.Components.Layout;

/// <summary>
/// A toggle button that switches between light and dark themes.
/// Persists the user's preference to <c>localStorage</c> and toggles the <c>dark</c> class on the document root.
/// </summary>
public partial class ThemeToggle
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _isDark;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDark = await JS.InvokeAsync<bool>("eval", "document.documentElement.classList.contains('dark')");
            StateHasChanged();
        }
    }

    private async Task ToggleTheme()
    {
        _isDark = !_isDark;
        await JS.InvokeVoidAsync("eval",
            _isDark
                ? "document.documentElement.classList.add('dark'); localStorage.setItem('theme','dark')"
                : "document.documentElement.classList.remove('dark'); localStorage.setItem('theme','light')");
    }
}
