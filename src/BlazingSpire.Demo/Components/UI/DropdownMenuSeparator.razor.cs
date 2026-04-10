using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DropdownMenuSeparator : ChildOf<DropdownMenu>
{
    protected override string BaseClasses => "-mx-1 my-1 h-px bg-muted";
}
