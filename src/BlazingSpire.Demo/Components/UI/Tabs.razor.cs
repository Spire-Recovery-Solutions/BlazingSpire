using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Tabs : BlazingSpireComponentBase
{
    [Parameter] public string? ActiveValue { get; set; }
    [Parameter] public EventCallback<string> ActiveValueChanged { get; set; }

    private readonly List<string> _registeredValues = new();

    protected override string BaseClasses => "w-full";

    public async Task SelectTabAsync(string value)
    {
        ActiveValue = value;
        await ActiveValueChanged.InvokeAsync(value);
        StateHasChanged();
    }

    public void RegisterTab(string value)
    {
        if (!_registeredValues.Contains(value))
            _registeredValues.Add(value);
    }

    public void UnregisterTab(string value)
    {
        _registeredValues.Remove(value);
    }

    public async Task NavigateTabAsync(int direction)
    {
        if (_registeredValues.Count == 0 || ActiveValue is null) return;
        var currentIndex = _registeredValues.IndexOf(ActiveValue);
        if (currentIndex < 0) return;
        var nextIndex = (currentIndex + direction + _registeredValues.Count) % _registeredValues.Count;
        await SelectTabAsync(_registeredValues[nextIndex]);
    }

    public async Task NavigateToFirstAsync()
    {
        if (_registeredValues.Count > 0)
            await SelectTabAsync(_registeredValues[0]);
    }

    public async Task NavigateToLastAsync()
    {
        if (_registeredValues.Count > 0)
            await SelectTabAsync(_registeredValues[^1]);
    }
}
