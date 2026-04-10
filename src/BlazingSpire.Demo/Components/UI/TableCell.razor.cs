using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableCell : ChildOf<Table>
{
    protected override string BaseClasses => "p-4 align-middle [&:has([role=checkbox])]:pr-0";
}
