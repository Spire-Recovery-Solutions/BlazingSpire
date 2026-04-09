using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A dropdown select control for choosing from a list.</summary>
public partial class Select : PopoverBase
{
    /// <summary>The currently selected value.</summary>
    [Parameter] public string? Value { get; set; }
    /// <summary>Callback invoked when the selection changes.</summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    /// <summary>Placeholder text when no value is selected.</summary>
    [Parameter] public string? Placeholder { get; set; }

    public string? SelectedText { get; set; }

    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => true;
    protected override string BaseClasses => "";

    private readonly List<string> _itemValues = new();
    private readonly Dictionary<string, string> _itemTexts = new();
    private int _highlightedIndex = -1;
    private int? _pendingHighlightDirection;

    public string? HighlightedValue =>
        _highlightedIndex >= 0 && _highlightedIndex < _itemValues.Count
            ? _itemValues[_highlightedIndex] : null;

    public void RegisterItem(string value, string text)
    {
        if (!_itemValues.Contains(value)) _itemValues.Add(value);
        _itemTexts[value] = text;
    }

    public void UnregisterItem(string value)
    {
        var idx = _itemValues.IndexOf(value);
        if (idx >= 0)
        {
            _itemValues.RemoveAt(idx);
            if (_highlightedIndex >= _itemValues.Count)
                _highlightedIndex = _itemValues.Count - 1;
        }
        _itemTexts.Remove(value);
    }

    public void MoveHighlight(int direction)
    {
        if (_itemValues.Count == 0)
        {
            // Items not yet rendered (select just opened) — apply after next render
            _pendingHighlightDirection = direction;
            return;
        }
        if (_highlightedIndex < 0)
            _highlightedIndex = direction > 0 ? 0 : _itemValues.Count - 1;
        else
            _highlightedIndex = (_highlightedIndex + direction + _itemValues.Count) % _itemValues.Count;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (_pendingHighlightDirection.HasValue && _itemValues.Count > 0)
        {
            var dir = _pendingHighlightDirection.Value;
            _pendingHighlightDirection = null;
            if (_highlightedIndex < 0)
                _highlightedIndex = dir > 0 ? 0 : _itemValues.Count - 1;
            else
                _highlightedIndex = (_highlightedIndex + dir + _itemValues.Count) % _itemValues.Count;
            StateHasChanged();
        }
    }

    public async Task SelectHighlightedAsync()
    {
        if (HighlightedValue is { } val)
        {
            var text = _itemTexts.TryGetValue(val, out var t) ? t : val;
            await SelectItemAsync(val, text);
        }
    }

    public new async Task SetIsOpenAsync(bool value)
    {
        await base.SetIsOpenAsync(value);
        if (value)
        {
            _highlightedIndex = Value is not null ? _itemValues.IndexOf(Value) : -1;
            StateHasChanged();
        }
    }

    public async Task SelectItemAsync(string value, string text)
    {
        Value = value;
        SelectedText = text;
        await ValueChanged.InvokeAsync(value);
        await base.SetIsOpenAsync(false);
        StateHasChanged();
    }
}
