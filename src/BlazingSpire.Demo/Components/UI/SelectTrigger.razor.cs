using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Select? ParentSelect { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses =>
        "flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm " +
        "ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring " +
        "focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";

    private async Task OnClickAsync()
    {
        if (Disabled) return;
        if (ParentSelect is not null)
            await ParentSelect.SetIsOpenAsync(!ParentSelect.CurrentIsOpen);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs e)
    {
        if (ParentSelect is null || Disabled) return;
        switch (e.Key)
        {
            case "ArrowDown":
                if (!ParentSelect.CurrentIsOpen) await ParentSelect.SetIsOpenAsync(true);
                ParentSelect.MoveHighlight(1);
                break;
            case "ArrowUp":
                if (!ParentSelect.CurrentIsOpen) await ParentSelect.SetIsOpenAsync(true);
                ParentSelect.MoveHighlight(-1);
                break;
            case "Enter":
            case " ":
                if (ParentSelect.CurrentIsOpen) await ParentSelect.SelectHighlightedAsync();
                else await ParentSelect.SetIsOpenAsync(true);
                break;
            case "Escape":
                await ParentSelect.SetIsOpenAsync(false);
                break;
        }
    }
}
