using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectValue : ChildOf<SelectTrigger>
{
    [CascadingParameter] private Select? SelectRoot { get; set; }

    public Select? ParentSelect => SelectRoot;

    protected override string BaseClasses => "";
}
