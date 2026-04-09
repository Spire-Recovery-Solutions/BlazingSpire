using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for button-like components with variant + size styling, loading state,
/// and polymorphic element rendering (button vs anchor).
/// </summary>
public abstract class ButtonBase<TVariant, TSize> : InteractiveBase
    where TVariant : struct, Enum
    where TSize : struct, Enum
{
    /// <summary>The visual style variant.</summary>
    [Parameter] public TVariant Variant { get; set; }
    /// <summary>The size of the button.</summary>
    [Parameter] public TSize Size { get; set; }
    /// <summary>Whether the button shows a loading spinner.</summary>
    [Parameter] public bool Loading { get; set; }
    /// <summary>URL to navigate to (renders as anchor).</summary>
    [Parameter] public string? Href { get; set; }
    /// <summary>Link target attribute (e.g., _blank).</summary>
    [Parameter] public string? Target { get; set; }
    /// <summary>Link rel attribute (e.g., noopener).</summary>
    [Parameter] public string? Rel { get; set; }
    /// <summary>Callback invoked when the button is clicked.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected abstract FrozenDictionary<TVariant, string> VariantClassMap { get; }
    protected abstract FrozenDictionary<TSize, string> SizeClassMap { get; }

    protected override bool IsEffectivelyDisabled => Disabled || Loading;
    protected bool IsLink => Href is not null;

    protected override string Classes => BuildClasses(
        BaseClasses,
        VariantClassMap.GetValueOrDefault(Variant, ""),
        SizeClassMap.GetValueOrDefault(Size, ""),
        Class);
}
