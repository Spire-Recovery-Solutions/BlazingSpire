using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerClose : ChildOf<DrawerContent>
{
    // ChildOf<DrawerContent> declares visual nesting for the playground's
    // tree walk. The runtime needs Drawer-root state, which cascades from
    // the outer Drawer component independently.
    [CascadingParameter] private Drawer? DrawerRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Drawer? ParentDrawer => DrawerRoot;

    protected override string BaseClasses =>
        "absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity " +
        "hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 " +
        "disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground";

    private async Task OnClickAsync()
    {
        if (ParentDrawer is not null)
            await ParentDrawer.RequestCloseAsync();
    }
}
