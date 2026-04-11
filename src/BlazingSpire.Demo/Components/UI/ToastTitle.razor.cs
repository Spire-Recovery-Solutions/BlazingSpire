using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ToastTitle : ChildOf<ToastHeader>
{
    protected override string BaseClasses => "text-sm font-semibold";
}
