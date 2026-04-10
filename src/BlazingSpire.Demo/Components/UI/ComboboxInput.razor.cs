using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ComboboxInput : ChildOf<Combobox>
{
    public Combobox? ParentCombobox => Parent;

    protected override string BaseClasses =>
        "flex h-10 w-full rounded-md bg-transparent py-3 text-sm outline-none " +
        "placeholder:text-muted-foreground disabled:cursor-not-allowed disabled:opacity-50";

    private void OnInput(ChangeEventArgs e)
    {
        ParentCombobox?.UpdateSearch(e.Value?.ToString() ?? "");
    }
}
