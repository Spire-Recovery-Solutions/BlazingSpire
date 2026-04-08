using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PopoverTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Popover? ParentPopover { get; set; }

    protected override string BaseClasses => "";

    private async Task OnClickAsync()
    {
        if (ParentPopover is not null)
            await ParentPopover.SetIsOpenAsync(!ParentPopover.CurrentIsOpen);
    }
}
