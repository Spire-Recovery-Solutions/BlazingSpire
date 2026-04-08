using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TooltipProvider : BlazingSpireComponentBase
{
    [Parameter] public int DelayDuration { get; set; } = 200;
    protected override string BaseClasses => "";
}
