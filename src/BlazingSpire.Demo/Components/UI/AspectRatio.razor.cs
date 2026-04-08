using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AspectRatio : BlazingSpireComponentBase
{
    [Parameter] public double Ratio { get; set; } = 16.0 / 9.0;

    protected override string BaseClasses => "relative w-full overflow-hidden";
}
