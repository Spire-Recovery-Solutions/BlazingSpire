using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// A small status label component with multiple visual variants.
/// Inspired by shadcn/ui Badge. Renders as a rounded pill.
/// </summary>
public partial class Badge
{
    /// <summary>The badge content (text or other markup).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>The visual style variant. Default is <see cref="BadgeVariant.Default"/>.</summary>
    [Parameter] public BadgeVariant Variant { get; set; } = BadgeVariant.Default;

    /// <summary>Additional CSS classes to append to the badge element.</summary>
    [Parameter] public string? Class { get; set; }

    private string Classes =>
        $"inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors " +
        $"focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 " +
        $"{VariantClass} {Class}";

    private string VariantClass => Variant switch
    {
        BadgeVariant.Default => "border-transparent bg-primary text-primary-foreground hover:bg-primary/80",
        BadgeVariant.Secondary => "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80",
        BadgeVariant.Destructive => "border-transparent bg-destructive text-destructive-foreground hover:bg-destructive/80",
        BadgeVariant.Outline => "text-foreground",
        _ => ""
    };

    /// <summary>Visual style variants for the Badge component.</summary>
    public enum BadgeVariant
    {
        /// <summary>Primary filled badge with brand color.</summary>
        Default,
        /// <summary>Muted filled badge for secondary information.</summary>
        Secondary,
        /// <summary>Red/danger filled badge for warnings or errors.</summary>
        Destructive,
        /// <summary>Bordered badge with transparent background.</summary>
        Outline
    }
}
