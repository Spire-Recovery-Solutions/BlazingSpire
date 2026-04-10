using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogTitle : ChildOf<Dialog>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Dialog? ParentDialog => Parent;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
