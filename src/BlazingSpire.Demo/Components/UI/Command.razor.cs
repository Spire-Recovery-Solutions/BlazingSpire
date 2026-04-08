using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Command : BlazingSpireComponentBase
{
    public string SearchText { get; set; } = "";

    protected override string BaseClasses =>
        "flex h-full w-full flex-col overflow-hidden rounded-md bg-popover text-popover-foreground";

    public void UpdateSearch(string text)
    {
        SearchText = text;
        StateHasChanged();
    }
}
