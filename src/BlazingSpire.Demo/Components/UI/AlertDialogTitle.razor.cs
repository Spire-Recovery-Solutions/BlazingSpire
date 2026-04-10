using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogTitle : ChildOf<AlertDialogHeader>
{
    [CascadingParameter] private AlertDialog? AlertDialogRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public AlertDialog? ParentDialog => AlertDialogRoot;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
