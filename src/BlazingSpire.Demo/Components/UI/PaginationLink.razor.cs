using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class PaginationLink : BlazingSpireComponentBase
{
    private const string LinkBase = "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 h-10 w-10";

    [Parameter] public bool IsActive { get; set; }
    [Parameter] public string? Href { get; set; }

    protected override string BaseClasses => LinkBase;

    protected override string Classes => BuildClasses(
        LinkBase,
        IsActive
            ? "bg-primary text-primary-foreground"
            : "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        Class);
}
