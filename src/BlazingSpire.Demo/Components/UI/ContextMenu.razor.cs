using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ContextMenu : PopoverBase
{
    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => true;
    protected override string BaseClasses => "";
}
