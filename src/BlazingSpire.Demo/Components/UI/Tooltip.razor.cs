using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Tooltip : PopoverBase
{
    [CascadingParameter(Name = "TooltipDelay")] public int DelayDuration { get; set; } = 200;

    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => false;

    protected override string BaseClasses => "";
}
