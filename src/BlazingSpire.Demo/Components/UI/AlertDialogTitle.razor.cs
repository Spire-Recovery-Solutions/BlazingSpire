using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDialogTitle : BlazingSpireComponentBase
{
    [CascadingParameter] public AlertDialog? ParentDialog { get; set; }

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
