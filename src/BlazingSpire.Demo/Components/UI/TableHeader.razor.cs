using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableHeader : ChildOf<Table>
{
    protected override string BaseClasses => "[&_tr]:border-b";
}
