using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ContextMenuLabel : BlazingSpireComponentBase
{
    [Parameter] public bool Inset { get; set; }

    protected override string BaseClasses => "px-2 py-1.5 text-sm font-semibold";

    protected override string Classes => BuildClasses(BaseClasses, Inset ? "pl-8" : null, Class);
}
