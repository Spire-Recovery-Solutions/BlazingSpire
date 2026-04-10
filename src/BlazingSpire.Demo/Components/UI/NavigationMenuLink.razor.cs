using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class NavigationMenuLink : ChildOf<NavigationMenu>
{
    [Parameter] public string? Href { get; set; }

    protected override string BaseClasses =>
        "block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors " +
        "hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground";
}
