using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CollapsibleContent : ChildOf<Collapsible>
{
    protected override string BaseClasses => "space-y-2";
}
