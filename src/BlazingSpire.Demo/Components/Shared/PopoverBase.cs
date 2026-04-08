using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingSpire.Demo.Components.Shared;

public enum FloatingSide { Top, Bottom, Left, Right }
public enum FloatingAlign { Start, Center, End }

/// <summary>
/// Base for positioned floating components (Popover, Tooltip, HoverCard, DropdownMenu).
/// Extends OverlayBase with Floating UI positioning and popover-appropriate defaults.
/// </summary>
public abstract class PopoverBase : OverlayBase
{
    [Inject] private IJSRuntime FloatingJS { get; set; } = default!;

    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;
    [Parameter] public FloatingAlign Align { get; set; } = FloatingAlign.Start;
    [Parameter] public int SideOffset { get; set; } = 4;
    [Parameter] public int AlignOffset { get; set; }

    // Override overlay defaults for popover behavior
    protected override bool ShouldTrapFocus => false;
    protected override bool ShouldLockScroll => false;
    protected override bool IsModal => false;

    private IJSObjectReference? _floatingModule;
    private IJSObjectReference? _positionHandle;

    public ElementReference AnchorRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _floatingModule = await FloatingJS.InvokeAsync<IJSObjectReference>(
                "import", "./js/floating.js");
        }

        if (_floatingModule is null) return;

        if (CurrentIsOpen && _positionHandle is null)
        {
            _positionHandle = await _floatingModule.InvokeAsync<IJSObjectReference>(
                "computePosition", AnchorRef, ContentRef, new
                {
                    side = Side.ToString().ToLowerInvariant(),
                    align = Align.ToString().ToLowerInvariant(),
                    sideOffset = SideOffset,
                    alignOffset = AlignOffset,
                });
        }
        else if (!CurrentIsOpen && _positionHandle is not null)
        {
            await _positionHandle.InvokeVoidAsync("dispose");
            _positionHandle = null;
        }
    }

    public override async ValueTask DisposeAsync()
    {
        if (_positionHandle is not null)
            await _positionHandle.InvokeVoidAsync("dispose");
        if (_floatingModule is not null)
            await _floatingModule.DisposeAsync();
        await base.DisposeAsync();
    }
}
