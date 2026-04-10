using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogTitle : ChildOf<AlertDialog>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public AlertDialog? ParentDialog => Parent;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
