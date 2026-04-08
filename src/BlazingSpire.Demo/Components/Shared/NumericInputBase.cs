using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for numeric form controls (Slider). Uses generic math for type-safe clamping.
/// </summary>
public abstract class NumericInputBase<T> : FormControlBase<T>
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    [Parameter] public T? Min { get; set; }
    [Parameter] public T? Max { get; set; }
    [Parameter] public T? Step { get; set; }

    /// <summary>Clamp a value within Min/Max bounds using generic math.</summary>
    protected T Clamp(T value)
    {
        if (Min.HasValue) value = T.Max(value, Min.Value);
        if (Max.HasValue) value = T.Min(value, Max.Value);
        return value;
    }
}
