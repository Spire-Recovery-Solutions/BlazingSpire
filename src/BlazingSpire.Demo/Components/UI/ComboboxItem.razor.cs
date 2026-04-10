using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ComboboxItem : ChildOf<ComboboxContent>
{
    [CascadingParameter] private Combobox? ComboboxRoot { get; set; }

    public Combobox? ParentCombobox => ComboboxRoot;

    [Parameter, EditorRequired] public string ItemValue { get; set; } = "";
    [Parameter] public string? FilterText { get; set; }
    [Parameter] public bool Disabled { get; set; }

    private bool IsSelected => ParentCombobox?.Value == ItemValue;
    private bool IsVisible => string.IsNullOrEmpty(ParentCombobox?.SearchText) ||
        (FilterText?.Contains(ParentCombobox.SearchText, StringComparison.OrdinalIgnoreCase) == true);

    protected override string BaseClasses =>
        "relative flex w-full cursor-pointer select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm " +
        "outline-none hover:bg-accent hover:text-accent-foreground " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50";

    private async Task OnClickAsync()
    {
        if (Disabled || ParentCombobox is null) return;
        await ParentCombobox.SelectItemAsync(ItemValue, FilterText ?? ItemValue);
    }
}
