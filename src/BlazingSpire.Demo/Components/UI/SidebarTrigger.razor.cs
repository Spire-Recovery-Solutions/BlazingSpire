using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SidebarTrigger : ChildOf<Sidebar>
{
    protected override string BaseClasses =>
        "inline-flex h-8 w-8 items-center justify-center rounded-md text-muted-foreground hover:bg-accent hover:text-accent-foreground";

    private async Task OnClickAsync()
    {
        if (Parent is not null)
            await Parent.ToggleAsync();
    }
}
