using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ComboboxContent : ChildOf<Combobox>
{
    public Combobox? ParentCombobox => Parent;

    protected override string BaseClasses =>
        "z-50 min-w-[8rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-md";
}
