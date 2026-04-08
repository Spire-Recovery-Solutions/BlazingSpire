using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum ButtonVariant { Default, Destructive, Outline, Secondary, Ghost, Link }
public enum ButtonSize { Default, Sm, Lg, Icon }

public partial class Button : ButtonBase<ButtonVariant, ButtonSize>
{
    [Parameter] public string Type { get; set; } = "button";

    protected override string BaseClasses =>
        "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-full text-sm font-medium cursor-pointer " +
        "ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 " +
        "focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0";

    private static readonly FrozenDictionary<ButtonVariant, string> s_variants = new Dictionary<ButtonVariant, string>
    {
        [ButtonVariant.Default]     = "bg-primary text-primary-foreground hover:bg-primary/90 shadow-sm",
        [ButtonVariant.Destructive] = "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        [ButtonVariant.Outline]     = "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground",
        [ButtonVariant.Secondary]   = "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80",
        [ButtonVariant.Ghost]       = "hover:bg-accent hover:text-accent-foreground",
        [ButtonVariant.Link]        = "text-primary underline-offset-4 hover:underline",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ButtonSize, string> s_sizes = new Dictionary<ButtonSize, string>
    {
        [ButtonSize.Default] = "h-10 px-4 py-2",
        [ButtonSize.Sm]      = "h-9 px-3",
        [ButtonSize.Lg]      = "h-11 px-8",
        [ButtonSize.Icon]    = "h-10 w-10",
    }.ToFrozenDictionary();

    protected override FrozenDictionary<ButtonVariant, string> VariantClassMap => s_variants;
    protected override FrozenDictionary<ButtonSize, string> SizeClassMap => s_sizes;
}
