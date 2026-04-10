using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PopoverContent : ChildOf<Popover>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Popover? ParentPopover => Parent;

    protected override string BaseClasses =>
        "z-50 w-72 rounded-md border bg-popover p-4 text-popover-foreground shadow-md outline-none";
}
