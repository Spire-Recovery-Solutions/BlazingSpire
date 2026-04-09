using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for expand/collapse components (Accordion, Collapsible, Tabs).
/// Provides controlled/uncontrolled open state pattern.
/// </summary>
public abstract class DisclosureBase : InteractiveBase
{
    /// <summary>Whether the component is expanded (controlled).</summary>
    [Parameter] public bool IsOpen { get; set; }
    /// <summary>Callback for two-way binding of IsOpen.</summary>
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    /// <summary>Initial open state for uncontrolled usage.</summary>
    [Parameter] public bool DefaultIsOpen { get; set; }
    /// <summary>Callback invoked when the open state changes.</summary>
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }

    private bool _internalIsOpen;
    private bool IsControlled => IsOpenChanged.HasDelegate;
    protected bool CurrentIsOpen => IsControlled ? IsOpen : _internalIsOpen;
    protected string DataState => CurrentIsOpen ? "open" : "closed";

    protected override void OnInitialized()
    {
        if (!IsControlled)
            _internalIsOpen = DefaultIsOpen;
    }

    protected async Task SetIsOpenAsync(bool value)
    {
        if (CurrentIsOpen == value) return;
        if (IsControlled)
            await IsOpenChanged.InvokeAsync(value);
        else
            _internalIsOpen = value;
        await OnOpenChanged.InvokeAsync(value);
        StateHasChanged();
    }

    protected Task ToggleAsync() => SetIsOpenAsync(!CurrentIsOpen);
}
