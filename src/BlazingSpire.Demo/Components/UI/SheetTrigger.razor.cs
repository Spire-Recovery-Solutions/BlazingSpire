using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Sheet? ParentSheet { get; set; }

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentSheet is not null)
            await ParentSheet.SetIsOpenAsync(true);
    }
}
