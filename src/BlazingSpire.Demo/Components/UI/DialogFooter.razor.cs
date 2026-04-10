using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogFooter : ChildOf<Dialog>
{
    protected override string BaseClasses => "flex flex-col-reverse sm:flex-row sm:justify-end sm:space-x-2";
}
