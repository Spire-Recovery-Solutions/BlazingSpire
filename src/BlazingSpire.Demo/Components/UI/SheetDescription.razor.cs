using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetDescription : BlazingSpireComponentBase
{
    [CascadingParameter] public Sheet? ParentSheet { get; set; }

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
