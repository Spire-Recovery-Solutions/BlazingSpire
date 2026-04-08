using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PopoverContent : BlazingSpireComponentBase
{
    [CascadingParameter] public Popover? ParentPopover { get; set; }

    protected override string BaseClasses =>
        "z-50 w-72 rounded-md border bg-popover p-4 text-popover-foreground shadow-md outline-none";
}
