using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerTrigger : ChildOf<Drawer>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Drawer? ParentDrawer => Parent;

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentDrawer is not null)
            await ParentDrawer.SetIsOpenAsync(true);
    }
}
