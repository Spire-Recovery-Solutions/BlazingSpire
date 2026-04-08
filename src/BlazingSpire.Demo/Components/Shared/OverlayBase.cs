using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for overlay components (Dialog, AlertDialog, Sheet, Drawer).
/// Provides focus trapping, click-outside, scroll locking, escape key, and portal plumbing.
/// </summary>
public abstract class OverlayBase : BlazingSpireComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public bool DefaultIsOpen { get; set; }
    [Parameter] public EventCallback<bool> OnOpenChanged { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    protected virtual bool ShouldTrapFocus => true;
    protected virtual bool ShouldLockScroll => true;
    protected virtual bool IsModal => true;
    protected virtual bool ShouldCloseOnEscape => true;
    protected virtual bool ShouldCloseOnInteractOutside => true;

    private bool _internalIsOpen;
    private bool IsControlled => IsOpenChanged.HasDelegate;
    public bool CurrentIsOpen => IsControlled ? IsOpen : _internalIsOpen;
    public string DataState => CurrentIsOpen ? "open" : "closed";

    private static int s_counter;
    protected string OverlayId { get; } = $"bs-overlay-{Interlocked.Increment(ref s_counter)}";
    public string TitleId => $"{OverlayId}-title";
    public string DescriptionId => $"{OverlayId}-desc";

    public ElementReference ContentRef;
    protected ElementReference TriggerRef;

    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _focusTrapHandle;
    private IJSObjectReference? _scrollLockHandle;
    private IJSObjectReference? _clickOutsideHandle;
    private DotNetObjectReference<OverlayBase>? _selfRef;
    private bool _isJsReady;

    protected override void OnInitialized()
    {
        if (!IsControlled)
            _internalIsOpen = DefaultIsOpen;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./js/overlay.js");
            _selfRef = DotNetObjectReference.Create(this);
            _isJsReady = true;
        }

        if (!_isJsReady) return;

        if (CurrentIsOpen)
            await ActivateOverlayAsync();
        else
            await DeactivateOverlayAsync();
    }

    private async Task ActivateOverlayAsync()
    {
        if (_jsModule is null || _selfRef is null) return;

        if (ShouldTrapFocus && _focusTrapHandle is null)
            _focusTrapHandle = await _jsModule.InvokeAsync<IJSObjectReference>(
                "createFocusTrap", ContentRef, _selfRef);

        if (ShouldLockScroll && _scrollLockHandle is null)
            _scrollLockHandle = await _jsModule.InvokeAsync<IJSObjectReference>(
                "lockBodyScroll");

        if (ShouldCloseOnInteractOutside && _clickOutsideHandle is null)
            _clickOutsideHandle = await _jsModule.InvokeAsync<IJSObjectReference>(
                "onClickOutside", ContentRef, _selfRef);
    }

    private async Task DeactivateOverlayAsync()
    {
        if (_focusTrapHandle is not null)
        {
            await _focusTrapHandle.InvokeVoidAsync("dispose");
            _focusTrapHandle = null;
        }
        if (_scrollLockHandle is not null)
        {
            await _scrollLockHandle.InvokeVoidAsync("dispose");
            _scrollLockHandle = null;
        }
        if (_clickOutsideHandle is not null)
        {
            await _clickOutsideHandle.InvokeVoidAsync("dispose");
            _clickOutsideHandle = null;
        }
    }

    [JSInvokable] public async Task HandleEscapeKey()
    {
        if (ShouldCloseOnEscape)
            await RequestCloseAsync();
    }

    [JSInvokable] public async Task HandleInteractOutside()
    {
        if (ShouldCloseOnInteractOutside)
            await RequestCloseAsync();
    }

    public async Task SetIsOpenAsync(bool value)
    {
        if (CurrentIsOpen == value) return;
        if (IsControlled)
            await IsOpenChanged.InvokeAsync(value);
        else
            _internalIsOpen = value;
        await OnOpenChanged.InvokeAsync(value);
        StateHasChanged();
    }

    public async Task RequestCloseAsync()
    {
        await SetIsOpenAsync(false);
        await OnClose.InvokeAsync();
    }

    protected Task RequestOpenAsync() => SetIsOpenAsync(true);

    public virtual async ValueTask DisposeAsync()
    {
        await DeactivateOverlayAsync();
        _selfRef?.Dispose();
        if (_jsModule is not null)
            await _jsModule.DisposeAsync();
    }
}
