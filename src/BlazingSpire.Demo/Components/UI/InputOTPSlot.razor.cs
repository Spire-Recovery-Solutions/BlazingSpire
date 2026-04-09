using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class InputOTPSlot : BlazingSpireComponentBase
{
    [CascadingParameter] public InputOTP? Parent { get; set; }
    [Parameter] public int Index { get; set; }

    private char? DisplayChar => Parent?.GetChar(Index);
    private bool IsActive => Parent?.Value.Length == Index;
    private bool ShowRing => IsActive && (Parent?.IsFocused ?? false);

    protected override string BaseClasses =>
        "relative flex h-10 w-10 items-center justify-center border-y border-r text-sm transition-all " +
        "first:rounded-l-md first:border-l last:rounded-r-md";

    protected override string Classes => ShowRing
        ? base.Classes + " ring-2 ring-ring ring-offset-background"
        : base.Classes;
}
