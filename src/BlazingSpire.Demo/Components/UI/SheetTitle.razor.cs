using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetTitle : BlazingSpireComponentBase
{
    [CascadingParameter] public Sheet? ParentSheet { get; set; }

    protected override string BaseClasses => "text-lg font-semibold text-foreground";
}
