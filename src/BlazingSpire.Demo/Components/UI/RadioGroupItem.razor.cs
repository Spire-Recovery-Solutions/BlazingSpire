using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class RadioGroupItem : ChildOf<RadioGroup>
{
    public RadioGroup? Group => Parent;
    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public bool Disabled { get; set; }

    private bool IsSelected => Group?.Value == ItemValue;
    private string DataState => IsSelected ? "checked" : "unchecked";

    private async Task SelectAsync()
    {
        if (Disabled || Group is null) return;
        await Group.SelectAsync(ItemValue);
    }

    protected override string BaseClasses =>
        "aspect-square h-4 w-4 rounded-full border border-primary text-primary ring-offset-background " +
        "focus:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:cursor-not-allowed disabled:opacity-50";
}
