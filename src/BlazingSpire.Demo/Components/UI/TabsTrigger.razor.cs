using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TabsTrigger : ChildOf<Tabs>, IDisposable
{
    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public bool Disabled { get; set; }

    private bool IsActive => Parent?.ActiveValue == ItemValue;
    private string DataState => IsActive ? "active" : "inactive";
    private string TabIndex => IsActive ? "0" : "-1";

    protected override void OnInitialized()
    {
        Parent?.RegisterTab(ItemValue);
    }

    public void Dispose() => Parent?.UnregisterTab(ItemValue);

    private async Task OnClickAsync()
    {
        if (Disabled || Parent is null) return;
        await Parent.SelectTabAsync(ItemValue);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs e)
    {
        if (Parent is null) return;
        switch (e.Key)
        {
            case "ArrowRight": await Parent.NavigateTabAsync(1); break;
            case "ArrowLeft": await Parent.NavigateTabAsync(-1); break;
            case "Home": await Parent.NavigateToFirstAsync(); break;
            case "End": await Parent.NavigateToLastAsync(); break;
        }
    }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center whitespace-nowrap rounded-sm px-3 py-1.5 text-sm font-medium " +
        "ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 " +
        "data-[state=active]:bg-background data-[state=active]:text-foreground data-[state=active]:shadow-sm";
}
