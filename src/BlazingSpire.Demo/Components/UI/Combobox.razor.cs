using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Combobox : PopoverBase
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string SearchPlaceholder { get; set; } = "Search...";

    public string SearchText { get; set; } = "";
    public string? SelectedText { get; set; }

    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => true;
    protected override string BaseClasses => "";

    public async Task SelectItemAsync(string value, string text)
    {
        Value = value;
        SelectedText = text;
        SearchText = "";
        await ValueChanged.InvokeAsync(value);
        await SetIsOpenAsync(false);
        StateHasChanged();
    }

    public void UpdateSearch(string text)
    {
        SearchText = text;
        StateHasChanged();
    }
}
