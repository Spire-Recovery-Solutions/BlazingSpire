using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PaginationContent : ChildOf<Pagination>
{
    protected override string BaseClasses => "flex flex-row items-center gap-1";
}
