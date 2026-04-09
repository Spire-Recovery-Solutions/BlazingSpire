using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for all BlazingSpire styled components. Provides the universal
/// parameter triple: ChildContent, Class, AdditionalAttributes.
/// </summary>
public abstract class BlazingSpireComponentBase : ComponentBase
{
    /// <summary>The content to render inside the component.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>Additional CSS classes to apply to the root element.</summary>
    [Parameter] public string? Class { get; set; }
    /// <summary>Additional HTML attributes to apply to the root element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Constant CSS classes for the component's root element.</summary>
    protected abstract string BaseClasses { get; }

    /// <summary>
    /// Template method: final composed class string applied to the root element.
    /// Each tier overrides to insert its own parts (variant, size, validation, etc.).
    /// Consumer's <c>Class</c> is always last — TailwindMerge gives it priority.
    /// </summary>
    protected virtual string Classes => BuildClasses(BaseClasses, Class);

    /// <summary>
    /// Joins non-empty CSS class parts with spaces.
    /// Uses <c>params ReadOnlySpan</c> — compiler stack-allocates
    /// the span at call sites, zero heap allocation for the argument list.
    /// </summary>
    protected static string BuildClasses(params ReadOnlySpan<string?> parts)
    {
        var sb = new StringBuilder(256);
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(part);
        }
        return sb.ToString();
    }
}
