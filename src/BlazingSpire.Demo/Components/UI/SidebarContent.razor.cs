using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SidebarContent : ChildOf<Sidebar>
{
    protected override string BaseClasses => "flex-1 overflow-auto py-2";
}
