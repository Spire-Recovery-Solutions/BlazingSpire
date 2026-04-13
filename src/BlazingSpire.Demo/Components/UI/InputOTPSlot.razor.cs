using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class InputOTPSlot : ChildOf<InputOTPGroup>, IRepeatingSlot<InputOTP>
{
    // ChildOf<InputOTPGroup> declares visual nesting for the playground's tree walk.
    // InputOTPSlot accesses state on the outer InputOTP root via its own CascadingValue.
    [CascadingParameter] private InputOTP? InputOTPRoot { get; set; }

    /// <summary>Zero-based index of this slot within the OTP input.</summary>
    [Parameter] public int Index { get; set; }

    // IRepeatingSlot: the playground emits a for-loop driven by InputOTP.MaxLength,
    // setting the Index parameter to the loop variable on each iteration.
    public static int GetSampleCount(InputOTP root) => root.MaxLength;
    public static string IndexParameterName => nameof(Index);
    public static string CountParameterName => nameof(InputOTP.MaxLength);

    private char? DisplayChar => InputOTPRoot?.GetChar(Index);
    private bool IsActive => InputOTPRoot?.Value.Length == Index;
    private bool ShowRing => IsActive && (InputOTPRoot?.IsFocused ?? false);

    protected override string BaseClasses =>
        "relative flex h-10 w-10 items-center justify-center border-y border-r text-sm transition-all " +
        "first:rounded-l-md first:border-l last:rounded-r-md";

    protected override string Classes => ShowRing
        ? base.Classes + " ring-2 ring-ring ring-offset-background"
        : base.Classes;
}
