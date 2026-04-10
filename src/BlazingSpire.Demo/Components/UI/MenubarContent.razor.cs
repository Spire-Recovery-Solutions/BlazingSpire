using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class MenubarContent : ChildOf<MenubarMenu>
{
    public MenubarMenu? ParentMenu => Parent;

    protected override string BaseClasses =>
        "absolute left-0 top-full z-50 mt-1 min-w-[12rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-md";
}
