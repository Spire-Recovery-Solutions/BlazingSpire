using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandInput : BlazingSpireComponentBase
{
    [CascadingParameter] public Command? ParentCommand { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    protected override string BaseClasses =>
        "flex h-11 w-full rounded-md bg-transparent py-3 text-sm outline-none placeholder:text-muted-foreground disabled:cursor-not-allowed disabled:opacity-50";
}
