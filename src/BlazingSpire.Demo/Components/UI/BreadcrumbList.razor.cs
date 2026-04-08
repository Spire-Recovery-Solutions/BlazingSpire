using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class BreadcrumbList : BlazingSpireComponentBase
{
    protected override string BaseClasses =>
        "flex flex-wrap items-center gap-1.5 break-words text-sm text-muted-foreground sm:gap-2.5";
}
