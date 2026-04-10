using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogDescription : ChildOf<Dialog>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Dialog? ParentDialog => Parent;

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
