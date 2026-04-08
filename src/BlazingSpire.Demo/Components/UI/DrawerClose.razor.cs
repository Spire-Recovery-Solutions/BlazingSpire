using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerClose : BlazingSpireComponentBase
{
    [CascadingParameter] public Drawer? ParentDrawer { get; set; }

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
