using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class BreadcrumbItem : ChildOf<BreadcrumbList>
{
    protected override string BaseClasses => "inline-flex items-center gap-1.5";
}
