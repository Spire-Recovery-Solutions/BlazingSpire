using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Switch : BlazingSpireComponentBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private string DataState => Checked ? "checked" : "unchecked";

    private async Task ToggleAsync()
    {
        if (Disabled) return;
        Checked = !Checked;
        await CheckedChanged.InvokeAsync(Checked);
    }

    protected override string BaseClasses =>
        "peer inline-flex h-6 w-11 shrink-0 cursor-pointer items-center rounded-full border-2 border-transparent " +
        "transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring " +
        "focus-visible:ring-offset-2 focus-visible:ring-offset-background " +
        "disabled:cursor-not-allowed disabled:opacity-50 " +
        "data-[state=checked]:bg-primary data-[state=unchecked]:bg-input";
}
