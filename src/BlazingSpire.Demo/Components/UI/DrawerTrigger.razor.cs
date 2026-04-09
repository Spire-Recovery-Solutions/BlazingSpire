using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Drawer? ParentDrawer { get; set; }

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentDrawer is not null)
            await ParentDrawer.SetIsOpenAsync(true);
    }
}
