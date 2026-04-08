using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialog : OverlayBase
{
    protected override bool ShouldCloseOnEscape => false;
    protected override bool ShouldCloseOnInteractOutside => false;
    protected override string BaseClasses => "";
}
