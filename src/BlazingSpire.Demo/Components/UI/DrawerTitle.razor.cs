using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerTitle : ChildOf<Drawer>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Drawer? ParentDrawer => Parent;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
