using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Slider : BlazingSpireComponentBase
{
    [Parameter] public double Value { get; set; } = 50;
    [Parameter] public EventCallback<double> ValueChanged { get; set; }
    [Parameter] public double Min { get; set; } = 0;
    [Parameter] public double Max { get; set; } = 100;
    [Parameter] public double Step { get; set; } = 1;
    [Parameter] public bool Disabled { get; set; }

    private double Percentage => Max > Min ? (Value - Min) / (Max - Min) * 100 : 0;

    private async Task OnInput(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var val))
        {
            Value = val;
            await ValueChanged.InvokeAsync(Value);
        }
    }

    protected override string BaseClasses =>
        "relative flex w-full touch-none select-none items-center";
}
