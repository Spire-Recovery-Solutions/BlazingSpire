using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogClose : ChildOf<DialogContent>
{
    // ChildOf<DialogContent> declares the visual nesting for the playground's tree
    // walk. The runtime needs access to the outer Dialog for close actions and state
    // attributes, which comes from the Dialog root via its own CascadingValue.
    [CascadingParameter] private Dialog? DialogRoot { get; set; }

    // Backwards-compat alias; .razor files still read ParentDialog?.X
    public Dialog? ParentDialog => DialogRoot;

    protected override string BaseClasses =>
        "absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity " +
        "hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 " +
        "disabled:pointer-events-none data-[state=open]:bg-accent data-[state=open]:text-muted-foreground";

    private async Task OnClickAsync()
    {
        if (ParentDialog is not null)
            await ParentDialog.RequestCloseAsync();
    }
}
