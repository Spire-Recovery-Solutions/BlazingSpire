using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

public enum FloatingSide { Top, Bottom, Left, Right }
public enum FloatingAlign { Start, Center, End }

/// <summary>
/// Base for positioned floating components (Popover, Tooltip, HoverCard, DropdownMenu).
/// Extends OverlayBase with Floating UI positioning and popover-appropriate defaults.
/// </summary>
public abstract class PopoverBase : OverlayBase
{
    [Parameter] public FloatingSide Side { get; set; } = FloatingSide.Bottom;
    [Parameter] public FloatingAlign Align { get; set; } = FloatingAlign.Start;
    [Parameter] public int SideOffset { get; set; } = 4;
    [Parameter] public int AlignOffset { get; set; }

    // Override overlay defaults for popover behavior
    protected override bool ShouldTrapFocus => false;
    protected override bool ShouldLockScroll => false;
    protected override bool IsModal => false;
}
