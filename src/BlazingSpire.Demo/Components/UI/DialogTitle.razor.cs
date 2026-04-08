using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogTitle : BlazingSpireComponentBase
{
    [CascadingParameter] public Dialog? ParentDialog { get; set; }

    protected override string BaseClasses => "text-lg font-semibold leading-none tracking-tight";
}
