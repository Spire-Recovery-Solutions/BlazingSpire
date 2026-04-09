using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A numeric input with type-safe binding, min/max, and optional buttons.</summary>
public partial class NumericField<T> : NumericInputBase<T>
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    /// <summary>Content displayed before the input (e.g., currency symbol).</summary>
    [Parameter] public RenderFragment? Prefix { get; set; }
    /// <summary>Content displayed after the input (e.g., unit label).</summary>
    [Parameter] public RenderFragment? Suffix { get; set; }
    /// <summary>Show increment/decrement buttons.</summary>
    [Parameter] public bool ShowButtons { get; set; }

    private bool HasAdornments => Prefix is not null || Suffix is not null || ShowButtons;

    protected override string BaseClasses =>
        "flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background " +
        "placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring " +
        "focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 h-10";

    protected override string Classes => BuildClasses(
        BaseClasses,
        HasAdornments ? "px-0" : null,
        IsInvalid ? "border-destructive ring-destructive" : null,
        Class);

    private string WrapperClasses => BuildClasses(
        "flex items-center rounded-md border border-input bg-background h-10 ring-offset-background " +
        "has-[input:focus-visible]:ring-2 has-[input:focus-visible]:ring-ring has-[input:focus-visible]:ring-offset-2",
        IsEffectivelyDisabled ? "cursor-not-allowed opacity-50" : null,
        IsInvalid ? "border-destructive ring-destructive" : null,
        Class);

    private string InnerInputClasses =>
        "flex-1 min-w-0 bg-transparent px-3 py-2 text-sm placeholder:text-muted-foreground " +
        "focus-visible:outline-none disabled:cursor-not-allowed [appearance:textfield] " +
        "[&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none";

    private string ButtonClasses =>
        "inline-flex h-full items-center justify-center px-2 text-muted-foreground hover:text-foreground " +
        "disabled:pointer-events-none disabled:opacity-50 transition-colors";

    private T TypedValue => Value is T v ? v : default;

    private string DisplayValue => TypedValue.ToString(null, CultureInfo.InvariantCulture) ?? "";

    private async Task OnChangeAsync(ChangeEventArgs e)
    {
        var text = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            await SetValueAsync(default);
            return;
        }

        if (T.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            await SetValueAsync(Clamp(parsed));
        }
    }

    private async Task IncrementAsync()
    {
        if (IsEffectivelyDisabled) return;
        var step = Step ?? T.One;
        await SetValueAsync(Clamp(TypedValue + step));
    }

    private async Task DecrementAsync()
    {
        if (IsEffectivelyDisabled) return;
        var step = Step ?? T.One;
        await SetValueAsync(Clamp(TypedValue - step));
    }
}
