using System.Collections.Frozen;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum BadgeVariant { Default, Secondary, Destructive, Outline }

public partial class Badge : PresentationalBase<BadgeVariant>
{
    protected override string BaseClasses =>
        "inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors " +
        "focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2";

    private static readonly FrozenDictionary<BadgeVariant, string> s_variants = new Dictionary<BadgeVariant, string>
    {
        [BadgeVariant.Default]     = "border-transparent bg-primary text-primary-foreground hover:bg-primary/80",
        [BadgeVariant.Secondary]   = "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80",
        [BadgeVariant.Destructive] = "border-transparent bg-destructive text-destructive-foreground hover:bg-destructive/80",
        [BadgeVariant.Outline]     = "text-foreground",
    }.ToFrozenDictionary();

    protected override FrozenDictionary<BadgeVariant, string> VariantClassMap => s_variants;
}
