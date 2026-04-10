using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class HoverCardTrigger : ChildOf<HoverCard>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public HoverCard? ParentHoverCard => Parent;

    protected override string BaseClasses => "";

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
