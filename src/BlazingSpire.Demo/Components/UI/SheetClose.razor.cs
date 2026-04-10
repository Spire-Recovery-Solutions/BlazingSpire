using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetClose : ChildOf<SheetContent>
{
    // ChildOf<SheetContent> declares visual nesting for the playground's
    // tree walk. The runtime needs Sheet-root state, which cascades from
    // the outer Sheet component independently.
    [CascadingParameter] private Sheet? SheetRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Sheet? ParentSheet => SheetRoot;

    protected override string BaseClasses =>
        "absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity " +
        "hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 " +
        "disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground";

    private async Task OnClickAsync()
    {
        if (ParentSheet is not null)
            await ParentSheet.RequestCloseAsync();
    }
}
