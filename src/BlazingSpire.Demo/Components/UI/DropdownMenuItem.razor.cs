using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DropdownMenuItem : ChildOf<DropdownMenu>
{
    public DropdownMenu? ParentMenu => Parent;

    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses =>
        "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none " +
        "transition-colors focus:bg-accent focus:text-accent-foreground " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50";

    private async Task OnClickAsync()
    {
        if (Disabled) return;
        await OnClick.InvokeAsync();
        if (ParentMenu is not null)
            await ParentMenu.RequestCloseAsync();
    }
}
