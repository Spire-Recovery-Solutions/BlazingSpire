using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogAction : BlazingSpireComponentBase
{
    [CascadingParameter] public AlertDialog? ParentDialog { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium cursor-pointer " +
        "ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0 " +
        "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm h-10 px-4 py-2";

    private async Task OnClickAsync()
    {
        await OnClick.InvokeAsync();
        if (ParentDialog is not null)
            await ParentDialog.RequestCloseAsync();
    }
}
