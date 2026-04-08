using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TabsContent : BlazingSpireComponentBase
{
    [CascadingParameter] public Tabs? Parent { get; set; }
    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";

    private bool IsActive => Parent?.ActiveValue == ItemValue;

    protected override string BaseClasses =>
        "mt-2 ring-offset-background focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2";
}
