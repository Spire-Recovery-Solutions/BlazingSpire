using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A toggle checkbox control.</summary>
public partial class Checkbox : BlazingSpireComponentBase
{
    /// <summary>Whether the checkbox is checked.</summary>
    [Parameter] public bool Checked { get; set; }
    /// <summary>Callback invoked when the checked state changes.</summary>
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    /// <summary>Whether the checkbox is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    private string DataState => Checked ? "checked" : "unchecked";

    private async Task ToggleAsync()
    {
        if (Disabled) return;
        Checked = !Checked;
        await CheckedChanged.InvokeAsync(Checked);
    }

    protected override string BaseClasses =>
        "peer h-4 w-4 shrink-0 rounded-sm border border-primary ring-offset-background " +
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:cursor-not-allowed disabled:opacity-50 " +
        "data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground";
}
