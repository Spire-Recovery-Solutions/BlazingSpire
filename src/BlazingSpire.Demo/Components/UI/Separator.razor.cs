using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum SeparatorOrientation { Horizontal, Vertical }

public partial class Separator : BlazingSpireComponentBase
{
    [Parameter] public SeparatorOrientation Orientation { get; set; } = SeparatorOrientation.Horizontal;
    [Parameter] public bool Decorative { get; set; } = true;

    protected override string BaseClasses => "shrink-0 bg-border";

    protected override string Classes => BuildClasses(
        BaseClasses,
        Orientation == SeparatorOrientation.Horizontal ? "h-[1px] w-full" : "h-full w-[1px]",
        Class);
}
