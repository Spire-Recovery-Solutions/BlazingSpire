using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for non-interactive components with visual variants (Badge, Alert, etc.).
/// Template method override inserts variant class between base and consumer.
/// </summary>
public abstract class PresentationalBase<TVariant> : BlazingSpireComponentBase
    where TVariant : struct, Enum
{
    [Parameter] public TVariant Variant { get; set; }

    /// <summary>Declarative variant→CSS mapping. Subclasses return a static FrozenDictionary.</summary>
    protected abstract FrozenDictionary<TVariant, string> VariantClassMap { get; }

    protected override string Classes => BuildClasses(
        BaseClasses,
        VariantClassMap.GetValueOrDefault(Variant, ""),
        Class);
}
