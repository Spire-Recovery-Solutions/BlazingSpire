using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandGroup : ChildOf<CommandList>
{
    [Parameter] public string? Heading { get; set; }

    protected override string BaseClasses =>
        "overflow-hidden p-1 text-foreground " +
        "[&_[data-command-group-heading]]:px-2 [&_[data-command-group-heading]]:py-1.5 " +
        "[&_[data-command-group-heading]]:text-xs [&_[data-command-group-heading]]:font-medium " +
        "[&_[data-command-group-heading]]:text-muted-foreground";
}
