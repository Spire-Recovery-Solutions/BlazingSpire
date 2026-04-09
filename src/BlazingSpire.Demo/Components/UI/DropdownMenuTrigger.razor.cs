using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DropdownMenuTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public DropdownMenu? ParentMenu { get; set; }

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentMenu is not null)
            await ParentMenu.SetIsOpenAsync(!ParentMenu.CurrentIsOpen);
    }
}
