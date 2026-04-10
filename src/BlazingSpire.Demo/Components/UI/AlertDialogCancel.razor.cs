using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogCancel : ChildOf<AlertDialog>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public AlertDialog? ParentDialog => Parent;

    protected override string BaseClasses =>
        "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium cursor-pointer " +
        "ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0 " +
        "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground h-10 px-4 py-2";

    private async Task OnClickAsync()
    {
        if (ParentDialog is not null)
            await ParentDialog.RequestCloseAsync();
    }
}
