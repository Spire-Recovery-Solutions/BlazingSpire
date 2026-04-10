using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandSeparator : ChildOf<CommandList>
{
    protected override string BaseClasses =>
        "-mx-1 h-px bg-border";
}
