using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for selection form controls (RadioGroup, Select, Combobox).
/// </summary>
public abstract class SelectionBase<T> : FormControlBase<T>
{
    [Parameter] public IReadOnlyList<T>? Items { get; set; }
    [Parameter] public Func<T, string>? OptionText { get; set; }
    [Parameter] public Func<T, object>? OptionValue { get; set; }

    protected string GetOptionText(T item) => OptionText?.Invoke(item) ?? item?.ToString() ?? "";
    protected object GetOptionValue(T item) => OptionValue?.Invoke(item) ?? item!;
}
