using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerHeader : ChildOf<Drawer>
{
    protected override string BaseClasses => "grid gap-1.5 p-4 text-center sm:text-left";
}
