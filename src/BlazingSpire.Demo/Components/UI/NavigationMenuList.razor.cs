using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class NavigationMenuList : ChildOf<NavigationMenu>
{
    protected override string BaseClasses =>
        "group flex flex-1 list-none items-center justify-center space-x-1";
}
