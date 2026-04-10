using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogTrigger : ChildOf<Dialog>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Dialog? ParentDialog => Parent;

    protected override string BaseClasses => "inline-block";

    private async Task OnClickAsync()
    {
        if (ParentDialog is not null)
            await ParentDialog.SetIsOpenAsync(true);
    }
}
