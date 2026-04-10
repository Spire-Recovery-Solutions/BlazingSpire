using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TableRow : ChildOf<Table>
{
    protected override string BaseClasses => "border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted";
}
