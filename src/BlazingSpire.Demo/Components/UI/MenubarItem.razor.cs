using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class MenubarItem : ChildOf<MenubarMenu>
{
    public MenubarMenu? ParentMenu => Parent;

    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses =>
        "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none " +
        "focus:bg-accent focus:text-accent-foreground " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50";

    private async Task OnClickAsync()
    {
        if (Disabled) return;
        await OnClick.InvokeAsync();
        ParentMenu?.Close();
    }
}
