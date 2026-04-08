using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectValue : BlazingSpireComponentBase
{
    [CascadingParameter] public Select? ParentSelect { get; set; }

    protected override string BaseClasses => "";
}
