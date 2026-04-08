using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public enum SheetSide { Top, Right, Bottom, Left }

public partial class Sheet : OverlayBase
{
    [Parameter] public SheetSide Side { get; set; } = SheetSide.Right;

    protected override string BaseClasses => "";
}
