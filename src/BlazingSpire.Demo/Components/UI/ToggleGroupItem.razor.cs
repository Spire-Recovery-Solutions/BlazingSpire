using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ToggleGroupItem : BlazingSpireComponentBase
{
    [CascadingParameter] public ToggleGroup? Group { get; set; }
    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public bool Disabled { get; set; }

    private bool IsSelected => Group?.IsSelected(ItemValue) == true;
    private string DataState => IsSelected ? "on" : "off";
    private bool IsDisabled => Disabled || (Group?.Disabled == true);

    private async Task OnClickAsync()
    {
        if (IsDisabled || Group is null) return;
        await Group.ToggleItemAsync(ItemValue);
    }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background " +
        "transition-colors hover:bg-muted hover:text-muted-foreground focus-visible:outline-none " +
        "focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:pointer-events-none disabled:opacity-50 " +
        "data-[state=on]:bg-accent data-[state=on]:text-accent-foreground h-10 px-3";
}
