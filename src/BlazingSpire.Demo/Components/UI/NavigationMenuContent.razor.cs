using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class NavigationMenuContent : ChildOf<NavigationMenuItem>
{
    [CascadingParameter] public NavigationMenuItem? ParentItem { get; set; }

    protected override string BaseClasses =>
        "absolute left-0 top-full mt-1.5 w-max rounded-md border bg-popover p-4 text-popover-foreground shadow-lg";
}
