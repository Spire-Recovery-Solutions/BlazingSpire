using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Label : BlazingSpireComponentBase
{
    [Parameter] public string? For { get; set; }

    protected override string BaseClasses =>
        "text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70";
}
