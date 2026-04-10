using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerTitle : ChildOf<DrawerHeader>
{
    // ChildOf<DrawerHeader> declares visual nesting for the playground's
    // tree walk. The runtime needs Drawer-root state, which cascades from
    // the outer Drawer component independently.
    [CascadingParameter] private Drawer? DrawerRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Drawer? ParentDrawer => DrawerRoot;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
