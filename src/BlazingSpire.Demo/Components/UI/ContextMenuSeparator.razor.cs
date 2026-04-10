using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ContextMenuSeparator : ChildOf<ContextMenu>
{
    protected override string BaseClasses => "-mx-1 my-1 h-px bg-muted";
}
