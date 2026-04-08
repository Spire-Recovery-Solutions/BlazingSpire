using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class HoverCardTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public HoverCard? ParentHoverCard { get; set; }

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
