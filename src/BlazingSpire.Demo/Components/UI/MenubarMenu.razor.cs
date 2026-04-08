using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class MenubarMenu : BlazingSpireComponentBase
{
    public bool IsOpen { get; private set; }

    protected override string BaseClasses => "relative";

    public void Toggle()
    {
        IsOpen = !IsOpen;
        StateHasChanged();
    }

    public void Close()
    {
        IsOpen = false;
        StateHasChanged();
    }
}
