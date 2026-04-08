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
    [Parameter] public TVariant Variant { get; set; }
    [Parameter] public TSize Size { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public string? Href { get; set; }
    [Parameter] public string? Target { get; set; }
    [Parameter] public string? Rel { get; set; }
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
