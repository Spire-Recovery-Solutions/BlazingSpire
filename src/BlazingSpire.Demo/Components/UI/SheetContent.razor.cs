using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class SheetContent : ChildOf<Sheet>
{
    // Backwards-compat alias for the old property name (to avoid changing .razor files)
    public Sheet? ParentSheet => Parent;

    private static readonly FrozenDictionary<SheetSide, string> s_sideClasses = new Dictionary<SheetSide, string>
    {
        [SheetSide.Top]    = "inset-x-0 top-0 border-b",
        [SheetSide.Right]  = "inset-y-0 right-0 h-full w-3/4 border-l sm:max-w-sm",
        [SheetSide.Bottom] = "inset-x-0 bottom-0 border-t",
        [SheetSide.Left]   = "inset-y-0 left-0 h-full w-3/4 border-r sm:max-w-sm",
    }.ToFrozenDictionary();

    protected override string BaseClasses =>
        "fixed z-50 gap-4 bg-background p-6 shadow-lg transition ease-in-out";

    protected override string Classes => BuildClasses(
        BaseClasses,
        s_sideClasses.GetValueOrDefault(ParentSheet?.Side ?? SheetSide.Right, ""),
        Class);
}
