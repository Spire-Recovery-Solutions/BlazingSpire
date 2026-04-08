using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TabsTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Tabs? Parent { get; set; }
    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public bool Disabled { get; set; }

    private bool IsActive => Parent?.ActiveValue == ItemValue;
    private string DataState => IsActive ? "active" : "inactive";

    private async Task OnClickAsync()
    {
        if (Disabled || Parent is null) return;
        await Parent.SelectTabAsync(ItemValue);
    }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center whitespace-nowrap rounded-sm px-3 py-1.5 text-sm font-medium " +
        "ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 " +
        "data-[state=active]:bg-background data-[state=active]:text-foreground data-[state=active]:shadow-sm";
}
