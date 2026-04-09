using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public AlertDialog? ParentDialog { get; set; }

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentDialog is not null)
            await ParentDialog.SetIsOpenAsync(true);
    }
}
