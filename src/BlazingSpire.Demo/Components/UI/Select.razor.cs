using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Select : PopoverBase
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }

    public string? SelectedText { get; set; }

    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => true;
    protected override string BaseClasses => "";

    public async Task SelectItemAsync(string value, string text)
    {
        Value = value;
        SelectedText = text;
        await ValueChanged.InvokeAsync(value);
        await SetIsOpenAsync(false);
        StateHasChanged();
    }
}
