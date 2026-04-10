using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class NavigationMenuTrigger : ChildOf<NavigationMenuItem>
{
    [CascadingParameter] public NavigationMenuItem? ParentItem { get; set; }

    protected override string BaseClasses =>
        "group inline-flex h-10 w-max items-center justify-center rounded-md bg-background px-4 py-2 " +
        "text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground " +
        "focus:bg-accent focus:text-accent-foreground focus:outline-none " +
        "disabled:pointer-events-none disabled:opacity-50 data-[state=open]:bg-accent/50";

    private string DataState => ParentItem?.IsOpen == true ? "open" : "closed";

    private async Task OnClickAsync()
    {
        if (ParentItem is not null)
            await ParentItem.ToggleAsync();
    }
}
