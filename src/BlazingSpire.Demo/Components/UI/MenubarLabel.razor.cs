using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class MenubarLabel : ChildOf<MenubarMenu>
{
    protected override string BaseClasses => "px-2 py-1.5 text-sm font-semibold";
}
