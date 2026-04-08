using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Input : BlazingSpireComponentBase
{
    [Parameter] public string Type { get; set; } = "text";
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses =>
        "flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background " +
        "file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground " +
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:cursor-not-allowed disabled:opacity-50";
}
