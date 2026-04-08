using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum ToggleGroupType { Single, Multiple }

public partial class ToggleGroup : BlazingSpireComponentBase
{
    [Parameter] public ToggleGroupType Type { get; set; } = ToggleGroupType.Single;
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public HashSet<string> Values { get; set; } = [];
    [Parameter] public EventCallback<HashSet<string>> ValuesChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses => "flex items-center justify-center gap-1";

    public async Task ToggleItemAsync(string itemValue)
    {
        if (Type == ToggleGroupType.Single)
        {
            Value = Value == itemValue ? null : itemValue;
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            if (!Values.Remove(itemValue))
                Values.Add(itemValue);
            await ValuesChanged.InvokeAsync(Values);
        }
        StateHasChanged();
    }

    public bool IsSelected(string itemValue) =>
        Type == ToggleGroupType.Single ? Value == itemValue : Values.Contains(itemValue);
}
