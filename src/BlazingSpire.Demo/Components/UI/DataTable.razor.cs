using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public class DataTableContext
{
    public string? SortColumn { get; set; }
    public bool SortAscending { get; set; }
    public Func<string, Task> ToggleSort { get; set; } = _ => Task.CompletedTask;
}

/// <summary>Non-generic marker base for <see cref="DataTable{TItem}"/> so children can declare their parent via the type system without needing to know TItem.</summary>
public abstract class DataTable : BlazingSpireComponentBase { }

public partial class DataTable<TItem> : DataTable
{
    [Parameter, EditorRequired] public IReadOnlyList<TItem> Items { get; set; } = [];
    [Parameter, EditorRequired] public RenderFragment Columns { get; set; } = default!;
    [Parameter] public RenderFragment<TItem>? RowTemplate { get; set; }
    [Parameter] public string? SortColumn { get; set; }
    [Parameter] public bool SortAscending { get; set; } = true;
    [Parameter] public EventCallback<string> OnSort { get; set; }

    private DataTableContext _context = default!;

    protected override string BaseClasses => "";

    protected override void OnInitialized()
    {
        _context = new DataTableContext
        {
            SortColumn = SortColumn,
            SortAscending = SortAscending,
            ToggleSort = ToggleSortAsync
        };
    }

    protected override void OnParametersSet()
    {
        _context.SortColumn = SortColumn;
        _context.SortAscending = SortAscending;
    }

    public async Task ToggleSortAsync(string column)
    {
        if (SortColumn == column)
            SortAscending = !SortAscending;
        else
        {
            SortColumn = column;
            SortAscending = true;
        }
        _context.SortColumn = SortColumn;
        _context.SortAscending = SortAscending;
        await OnSort.InvokeAsync(column);
        StateHasChanged();
    }
}
