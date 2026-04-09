using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for navigable menu components (DropdownMenu, ContextMenu, Menubar).
/// Provides item registration, roving focus, and keyboard navigation.
/// </summary>
public abstract class MenuBase : PopoverBase
{
    /// <summary>Whether keyboard navigation wraps around.</summary>
    [Parameter] public bool Loop { get; set; }

    protected List<MenuItemRegistration> RegisteredItems { get; } = [];
    protected int HighlightedIndex { get; private set; } = -1;

    internal void RegisterItem(MenuItemRegistration item) => RegisteredItems.Add(item);
    internal void UnregisterItem(string id) => RegisteredItems.RemoveAll(i => i.Id == id);

    [JSInvokable]
    public void SetHighlightedIndex(int index)
    {
        HighlightedIndex = index;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task HandleItemSelected(int index)
    {
        if (index < 0 || index >= RegisteredItems.Count) return;
        var item = RegisteredItems[index];
        if (item.IsDisabled) return;
        await RequestCloseAsync();
    }
}

public sealed record MenuItemRegistration(
    string Id,
    string? TextValue,
    bool IsDisabled,
    int Index,
    bool IsSubTrigger);
