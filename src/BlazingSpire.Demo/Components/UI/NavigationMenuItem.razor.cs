using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class NavigationMenuItem : ChildOf<NavigationMenu>
{
    public bool IsOpen { get; private set; }

    protected override string BaseClasses => "";

    public Task ToggleAsync()
    {
        IsOpen = !IsOpen;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public Task CloseAsync()
    {
        if (!IsOpen) return Task.CompletedTask;
        IsOpen = false;
        StateHasChanged();
        return Task.CompletedTask;
    }
}
