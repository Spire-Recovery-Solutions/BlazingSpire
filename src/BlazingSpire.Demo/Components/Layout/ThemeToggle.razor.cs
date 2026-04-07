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
            _isDark = await JS.InvokeAsync<bool>("blazingSpire.getTheme");
            StateHasChanged();
        }
    }

    private async Task ToggleTheme()
    {
        _isDark = !_isDark;
        await JS.InvokeVoidAsync("blazingSpire.setTheme", _isDark);
    }
}
