using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// A styled button component with multiple visual variants and sizes.
/// Inspired by shadcn/ui Button. Renders a native <c>&lt;button&gt;</c> element.
/// </summary>
public partial class Button
{
    /// <summary>The button content (text, icons, or other markup).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>The visual style variant. Default is <see cref="ButtonVariant.Default"/>.</summary>
    [Parameter] public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>The size of the button. Default is <see cref="ButtonSize.Default"/>.</summary>
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>The HTML button type attribute. Default is <c>"button"</c>.</summary>
    [Parameter] public string Type { get; set; } = "button";

    /// <summary>Whether the button is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Additional CSS classes to append to the button element.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Callback invoked when the button is clicked.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Captures any additional HTML attributes not explicitly defined as parameters.</summary>
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes =>
        $"inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium " +
        $"ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        $"disabled:pointer-events-none disabled:opacity-50 " +
        $"{VariantClass} {SizeClass} {Class}";

    private string VariantClass => Variant switch
    {
        ButtonVariant.Default => "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm",
        ButtonVariant.Destructive => "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        ButtonVariant.Outline => "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Secondary => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ButtonVariant.Ghost => "hover:bg-accent hover:text-accent-foreground",
        ButtonVariant.Link => "text-primary underline-offset-4 hover:underline",
        _ => ""
    };

    private string SizeClass => Size switch
    {
        ButtonSize.Default => "h-10 px-4 py-2",
        ButtonSize.Sm => "h-9 px-3",
        ButtonSize.Lg => "h-11 px-8",
        ButtonSize.Icon => "h-10 w-10",
        _ => ""
    };

    /// <summary>Visual style variants for the Button component.</summary>
    public enum ButtonVariant
    {
        /// <summary>Primary filled button with brand color.</summary>
        Default,
        /// <summary>Red/danger filled button for destructive actions.</summary>
        Destructive,
        /// <summary>Bordered button with transparent background.</summary>
        Outline,
        /// <summary>Muted filled button for secondary actions.</summary>
        Secondary,
        /// <summary>Transparent button that shows background on hover.</summary>
        Ghost,
        /// <summary>Styled as a text link with underline on hover.</summary>
        Link
    }

    /// <summary>Size options for the Button component.</summary>
    public enum ButtonSize
    {
        /// <summary>Standard button size (h-10).</summary>
        Default,
        /// <summary>Small button size (h-9).</summary>
        Sm,
        /// <summary>Large button size (h-11).</summary>
        Lg,
        /// <summary>Square icon-only button (h-10 w-10).</summary>
        Icon
    }
}
