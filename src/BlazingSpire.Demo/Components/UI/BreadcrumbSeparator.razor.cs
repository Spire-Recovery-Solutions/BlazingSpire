using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class BreadcrumbSeparator : ChildOf<BreadcrumbList>
{
    protected override string BaseClasses => "[&>svg]:h-3.5 [&>svg]:w-3.5";
}
