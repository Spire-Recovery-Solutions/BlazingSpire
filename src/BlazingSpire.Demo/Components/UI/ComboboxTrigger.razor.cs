using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ComboboxTrigger : ChildOf<Combobox>
{
    public Combobox? ParentCombobox => Parent;

    protected override string BaseClasses =>
        "flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 " +
        "text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 " +
        "focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&>span]:line-clamp-1 " +
        "cursor-pointer";

    private async Task OnClickAsync()
    {
        if (ParentCombobox is not null)
            await ParentCombobox.SetIsOpenAsync(!ParentCombobox.CurrentIsOpen);
    }
}
