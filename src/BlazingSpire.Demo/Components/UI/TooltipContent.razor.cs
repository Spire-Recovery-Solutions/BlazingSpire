using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TooltipContent : ChildOf<Tooltip>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Tooltip? ParentTooltip => Parent;

    protected override string BaseClasses =>
        "z-50 overflow-hidden rounded-md border bg-popover px-3 py-1.5 text-sm text-popover-foreground shadow-md animate-in fade-in-0 zoom-in-95";
}
