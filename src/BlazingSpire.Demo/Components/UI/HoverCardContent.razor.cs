using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class HoverCardContent : ChildOf<HoverCard>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public HoverCard? ParentHoverCard => Parent;

    protected override string BaseClasses =>
        "z-50 w-64 rounded-md border bg-popover p-4 text-popover-foreground shadow-md outline-none";

    private async Task OnMouseEnterAsync()
    {
        if (ParentHoverCard is not null)
            await ParentHoverCard.HandleMouseEnterAsync();
    }

    private async Task OnMouseLeaveAsync()
    {
        if (ParentHoverCard is not null)
            await ParentHoverCard.HandleMouseLeaveAsync();
    }
}
