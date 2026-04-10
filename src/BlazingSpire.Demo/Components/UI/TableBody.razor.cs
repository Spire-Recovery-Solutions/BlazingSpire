using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableBody : ChildOf<Table>
{
    protected override string BaseClasses => "[&_tr:last-child]:border-0";
}
