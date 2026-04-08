using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PaginationPrevious : BlazingSpireComponentBase
{
    [Parameter] public string? Href { get; set; }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 h-10 border border-input bg-background hover:bg-accent hover:text-accent-foreground gap-1 pl-2.5 pr-3";
}
