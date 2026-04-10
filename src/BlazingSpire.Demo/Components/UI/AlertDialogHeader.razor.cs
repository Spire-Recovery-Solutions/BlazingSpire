using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogHeader : ChildOf<AlertDialog>
{
    protected override string BaseClasses => "flex flex-col space-y-1.5 text-center sm:text-left";
}
