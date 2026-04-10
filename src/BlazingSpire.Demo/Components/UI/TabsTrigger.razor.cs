using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TabsTrigger : ChildOf<TabsList>, IDisposable
{
    // ChildOf<TabsList> declares visual nesting for the playground's tree walk.
    // TabsTrigger interacts with the outer Tabs root for selection, navigation, and
    // registration state. Tabs cascades itself so all descendants can pick it up here.
    [CascadingParameter] private Tabs? TabsRoot { get; set; }

    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public bool Disabled { get; set; }

    private bool IsActive => TabsRoot?.ActiveValue == ItemValue;
    private string DataState => IsActive ? "active" : "inactive";
    private string TabIndex => IsActive ? "0" : "-1";

    protected override void OnInitialized()
    {
        TabsRoot?.RegisterTab(ItemValue);
    }

    public void Dispose() => TabsRoot?.UnregisterTab(ItemValue);

    private async Task OnClickAsync()
    {
        if (Disabled || TabsRoot is null) return;
        await TabsRoot.SelectTabAsync(ItemValue);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs e)
    {
        if (TabsRoot is null) return;
        switch (e.Key)
        {
            case "ArrowRight": await TabsRoot.NavigateTabAsync(1); break;
            case "ArrowLeft": await TabsRoot.NavigateTabAsync(-1); break;
            case "Home": await TabsRoot.NavigateToFirstAsync(); break;
            case "End": await TabsRoot.NavigateToLastAsync(); break;
        }
    }

    protected override string BaseClasses =>
        "inline-flex items-center justify-center whitespace-nowrap rounded-sm px-3 py-1.5 text-sm font-medium " +
        "ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 " +
        "data-[state=active]:bg-background data-[state=active]:text-foreground data-[state=active]:shadow-sm";
}
