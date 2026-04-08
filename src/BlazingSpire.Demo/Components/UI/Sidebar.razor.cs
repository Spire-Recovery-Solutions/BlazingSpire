using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Sidebar : BlazingSpireComponentBase
{
    [Parameter] public bool IsCollapsed { get; set; }
    [Parameter] public EventCallback<bool> IsCollapsedChanged { get; set; }

    protected override string BaseClasses => "flex h-full flex-col border-r bg-background transition-all duration-300";

    protected override string Classes => BuildClasses(
        BaseClasses,
        IsCollapsed ? "w-16" : "w-64",
        Class);

    public async Task ToggleAsync()
    {
        IsCollapsed = !IsCollapsed;
        await IsCollapsedChanged.InvokeAsync(IsCollapsed);
        StateHasChanged();
    }
}
