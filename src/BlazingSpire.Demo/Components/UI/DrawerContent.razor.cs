using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerContent : ChildOf<Drawer>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Drawer? ParentDrawer => Parent;

    protected override string BaseClasses =>
        "fixed inset-x-0 bottom-0 z-50 mt-24 flex h-auto flex-col rounded-t-[10px] border bg-background";
}
