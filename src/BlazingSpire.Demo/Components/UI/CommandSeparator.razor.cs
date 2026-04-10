using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandSeparator : ChildOf<Command>
{
    protected override string BaseClasses =>
        "-mx-1 h-px bg-border";
}
