using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Tabs : BlazingSpireComponentBase
{
    [Parameter] public string? ActiveValue { get; set; }
    [Parameter] public EventCallback<string> ActiveValueChanged { get; set; }

    protected override string BaseClasses => "w-full";

    public async Task SelectTabAsync(string value)
    {
        ActiveValue = value;
        await ActiveValueChanged.InvokeAsync(value);
        StateHasChanged();
    }
}
