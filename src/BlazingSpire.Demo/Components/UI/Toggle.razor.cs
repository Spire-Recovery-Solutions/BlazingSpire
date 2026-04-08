using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum ToggleVariant { Default, Outline }

public partial class Toggle : PresentationalBase<ToggleVariant>
{
    [Parameter] public bool Pressed { get; set; }
    [Parameter] public EventCallback<bool> PressedChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private string DataState => Pressed ? "on" : "off";

    private async Task ToggleAsync()
    {
        if (Disabled) return;
        Pressed = !Pressed;
        await PressedChanged.InvokeAsync(Pressed);
    }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background " +
        "transition-colors hover:bg-muted hover:text-muted-foreground focus-visible:outline-none " +
        "focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:pointer-events-none disabled:opacity-50 " +
        "data-[state=on]:bg-accent data-[state=on]:text-accent-foreground h-10 px-3";

    private static readonly FrozenDictionary<ToggleVariant, string> s_variants = new Dictionary<ToggleVariant, string>
    {
        [ToggleVariant.Default] = "bg-transparent",
        [ToggleVariant.Outline] = "border border-input bg-transparent hover:bg-accent hover:text-accent-foreground",
    }.ToFrozenDictionary();

    protected override FrozenDictionary<ToggleVariant, string> VariantClassMap => s_variants;
}
