using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogDescription : BlazingSpireComponentBase
{
    [CascadingParameter] public Dialog? ParentDialog { get; set; }

    protected override string BaseClasses => "text-sm text-muted-foreground";
}
