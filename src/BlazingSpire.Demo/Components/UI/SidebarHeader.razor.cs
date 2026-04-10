using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SidebarHeader : ChildOf<Sidebar>
{
    protected override string BaseClasses => "flex h-14 items-center border-b px-4";
}
