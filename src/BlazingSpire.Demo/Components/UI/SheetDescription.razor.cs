using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetDescription : ChildOf<SheetHeader>
{
    // ChildOf<SheetHeader> declares visual nesting for the playground's
    // tree walk. The runtime needs Sheet-root state, which cascades from
    // the outer Sheet component independently.
    [CascadingParameter] private Sheet? SheetRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Sheet? ParentSheet => SheetRoot;

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
