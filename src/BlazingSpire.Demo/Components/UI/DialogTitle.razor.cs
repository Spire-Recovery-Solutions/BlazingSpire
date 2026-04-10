using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogTitle : ChildOf<DialogHeader>
{
    // ChildOf<DialogHeader> declares visual nesting for the playground's
    // tree walk. The runtime needs Dialog-root state, which cascades from
    // the outer Dialog component independently.
    [CascadingParameter] private Dialog? DialogRoot { get; set; }

    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Dialog? ParentDialog => DialogRoot;

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
