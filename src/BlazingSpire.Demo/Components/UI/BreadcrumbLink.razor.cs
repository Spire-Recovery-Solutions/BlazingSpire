using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class BreadcrumbLink : ChildOf<Breadcrumb>
{
    [Parameter] public string? Href { get; set; }

    protected override string BaseClasses => "transition-colors hover:text-foreground";
}
