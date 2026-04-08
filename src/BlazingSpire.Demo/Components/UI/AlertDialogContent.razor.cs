using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogContent : BlazingSpireComponentBase
{
    [CascadingParameter] public AlertDialog? ParentDialog { get; set; }

    protected override string BaseClasses =>
        "fixed left-[50%] top-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 " +
        "border bg-background p-6 shadow-lg duration-200 sm:rounded-lg";
}
