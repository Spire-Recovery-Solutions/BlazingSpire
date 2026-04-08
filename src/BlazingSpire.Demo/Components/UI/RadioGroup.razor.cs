using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class RadioGroup : BlazingSpireComponentBase
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    protected override string BaseClasses => "grid gap-2";

    public async Task SelectAsync(string itemValue)
    {
        Value = itemValue;
        await ValueChanged.InvokeAsync(Value);
        StateHasChanged();
    }
}
