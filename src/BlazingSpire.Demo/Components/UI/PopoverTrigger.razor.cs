using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PopoverTrigger : ChildOf<Popover>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Popover? ParentPopover => Parent;

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentPopover is not null)
            await ParentPopover.SetIsOpenAsync(!ParentPopover.CurrentIsOpen);
    }
}
