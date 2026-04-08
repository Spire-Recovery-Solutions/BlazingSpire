using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class HoverCardContent : BlazingSpireComponentBase
{
    [CascadingParameter] public HoverCard? ParentHoverCard { get; set; }

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
