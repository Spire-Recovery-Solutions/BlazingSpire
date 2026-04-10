using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableFooter : ChildOf<Table>
{
    protected override string BaseClasses => "border-t bg-muted/50 font-medium [&>tr]:last:border-b-0";
}
