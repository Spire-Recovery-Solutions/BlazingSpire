using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CollapsibleTrigger : ChildOf<Collapsible>
{
    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (Parent is not null)
            await Parent.ToggleAsync();
    }
}
