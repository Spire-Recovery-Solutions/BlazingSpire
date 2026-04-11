using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableFooterCell : ChildOf<TableFooterRow>
{
    protected override string BaseClasses => "p-4 align-middle [&:has([role=checkbox])]:pr-0";
}
