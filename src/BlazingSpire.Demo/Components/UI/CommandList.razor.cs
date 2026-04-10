using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandList : ChildOf<Command>
{
    protected override string BaseClasses =>
        "max-h-[300px] overflow-y-auto overflow-x-hidden";
}
