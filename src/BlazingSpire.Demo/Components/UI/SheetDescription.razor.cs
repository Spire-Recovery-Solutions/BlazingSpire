using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetDescription : ChildOf<Sheet>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Sheet? ParentSheet => Parent;

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
