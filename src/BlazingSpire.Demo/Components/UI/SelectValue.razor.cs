using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectValue : ChildOf<Select>
{
    public Select? ParentSelect => Parent;

    protected override string BaseClasses => "";
}
