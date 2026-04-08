using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TooltipContent : BlazingSpireComponentBase
{
    [CascadingParameter] public Tooltip? ParentTooltip { get; set; }

    protected override string BaseClasses =>
        "z-50 overflow-hidden rounded-md border bg-popover px-3 py-1.5 text-sm text-popover-foreground shadow-md animate-in fade-in-0 zoom-in-95";
}
