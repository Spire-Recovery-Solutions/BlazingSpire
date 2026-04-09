using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum SpinnerSize { Sm, Default, Lg }

/// <summary>A circular loading indicator for async operations.</summary>
public partial class Spinner : BlazingSpireComponentBase
{
    /// <summary>The size of the spinner.</summary>
    [Parameter] public SpinnerSize Size { get; set; } = SpinnerSize.Default;

    protected override string BaseClasses => "animate-spin text-primary";

    private static readonly FrozenDictionary<SpinnerSize, string> s_sizes = new Dictionary<SpinnerSize, string>
    {
        [SpinnerSize.Sm]      = "h-4 w-4",
        [SpinnerSize.Default] = "h-6 w-6",
        [SpinnerSize.Lg]      = "h-10 w-10",
    }.ToFrozenDictionary();

    protected override string Classes => BuildClasses(
        BaseClasses,
        s_sizes.GetValueOrDefault(Size, "h-6 w-6"),
        Class);
}
