using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Toast : PresentationalBase<ToastVariant>
{
    [Parameter] public ToastMessage? Message { get; set; }
    [Parameter] public EventCallback OnDismiss { get; set; }

    protected override string BaseClasses =>
        "group pointer-events-auto relative flex w-full items-center justify-between space-x-4 overflow-hidden rounded-md border p-6 pr-8 shadow-lg transition-all mb-2";

    private static readonly FrozenDictionary<ToastVariant, string> s_variants = new Dictionary<ToastVariant, string>
    {
        [ToastVariant.Default]     = "border bg-background text-foreground",
        [ToastVariant.Destructive] = "destructive border-destructive bg-destructive text-destructive-foreground",
    }.ToFrozenDictionary();

    protected override FrozenDictionary<ToastVariant, string> VariantClassMap => s_variants;
}
