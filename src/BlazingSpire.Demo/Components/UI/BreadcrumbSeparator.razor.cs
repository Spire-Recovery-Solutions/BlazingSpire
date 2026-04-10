using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class BreadcrumbSeparator : ChildOf<Breadcrumb>
{
    protected override string BaseClasses => "[&>svg]:h-3.5 [&>svg]:w-3.5";
}
