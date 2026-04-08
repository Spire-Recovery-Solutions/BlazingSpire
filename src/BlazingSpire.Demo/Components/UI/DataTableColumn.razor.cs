using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class DataTableColumn : BlazingSpireComponentBase
{
    [CascadingParameter] private DataTableContext? Context { get; set; }
    [Parameter] public string? SortKey { get; set; }
    [Parameter] public bool Sortable { get; set; }

    protected override string BaseClasses => "";

    private bool IsSorted => Context?.SortColumn == SortKey && SortKey is not null;
    private bool IsSortedAscending => IsSorted && (Context?.SortAscending ?? true);

    private async Task HandleSortClick()
    {
        if (Context is not null && SortKey is not null)
            await Context.ToggleSort(SortKey);
    }
}
