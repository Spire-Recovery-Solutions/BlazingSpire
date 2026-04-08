using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CollapsibleContent : BlazingSpireComponentBase
{
    [CascadingParameter] public Collapsible? Parent { get; set; }

    protected override string BaseClasses => "space-y-2";
}
