using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Textarea : BlazingSpireComponentBase
{
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses =>
        "flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background " +
        "placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring " +
        "focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50";
}
