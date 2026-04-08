using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerTitle : BlazingSpireComponentBase
{
    [CascadingParameter] public Drawer? ParentDrawer { get; set; }

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
