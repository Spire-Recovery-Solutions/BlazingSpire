using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DrawerContent : BlazingSpireComponentBase
{
    [CascadingParameter] public Drawer? ParentDrawer { get; set; }

    protected override string BaseClasses =>
        "fixed inset-x-0 bottom-0 z-50 mt-24 flex h-auto flex-col rounded-t-[10px] border bg-background";
}
