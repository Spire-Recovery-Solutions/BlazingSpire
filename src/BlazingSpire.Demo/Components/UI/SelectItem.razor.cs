using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SelectItem : ChildOf<Select>, IDisposable
{
    public Select? ParentSelect => Parent;

    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public string? ItemText { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private bool IsSelected => ParentSelect?.Value == ItemValue;
    public bool IsHighlighted => ParentSelect?.HighlightedValue == ItemValue;

    protected override string BaseClasses =>
        "relative flex w-full cursor-default select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none " +
        "focus:bg-accent focus:text-accent-foreground data-[disabled]:pointer-events-none data-[disabled]:opacity-50";

    protected override void OnInitialized() => ParentSelect?.RegisterItem(ItemValue, ItemText ?? ItemValue);

    public void Dispose() => ParentSelect?.UnregisterItem(ItemValue);

    private async Task OnClickAsync()
    {
        if (Disabled) return;
        if (ParentSelect is not null)
            await ParentSelect.SelectItemAsync(ItemValue, ItemText ?? ItemValue);
    }
}
