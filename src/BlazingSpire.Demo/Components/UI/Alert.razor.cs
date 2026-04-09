using System.Collections.Frozen;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum AlertVariant { Default, Destructive, Success, Warning, Info }

/// <summary>Display callout messages for important information or feedback.</summary>
public partial class Alert : PresentationalBase<AlertVariant>
{
    protected override string BaseClasses =>
        "relative w-full rounded-lg border p-4 [&>svg~*]:pl-7 [&>svg+div]:translate-y-[-3px] [&>svg]:absolute [&>svg]:left-4 [&>svg]:top-4 [&>svg]:text-foreground";

    private static readonly FrozenDictionary<AlertVariant, string> s_variants = new Dictionary<AlertVariant, string>
    {
        [AlertVariant.Default]     = "bg-background text-foreground",
        [AlertVariant.Destructive] = "border-destructive/50 text-destructive dark:border-destructive [&>svg]:text-destructive",
        [AlertVariant.Success]     = "border-success/50 text-success dark:border-success [&>svg]:text-success",
        [AlertVariant.Warning]     = "border-warning/50 text-warning dark:border-warning [&>svg]:text-warning",
        [AlertVariant.Info]        = "border-info/50 text-info dark:border-info [&>svg]:text-info",
    }.ToFrozenDictionary();

    protected override FrozenDictionary<AlertVariant, string> VariantClassMap => s_variants;
}
