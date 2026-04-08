using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerDescription : BlazingSpireComponentBase
{
    [CascadingParameter] public Drawer? ParentDrawer { get; set; }

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
