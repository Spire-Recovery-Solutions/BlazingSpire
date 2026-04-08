using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

internal record Payment(string Status, string Email, decimal Amount);

public class DataTableTests : BlazingSpireTestBase
{
    private static readonly IReadOnlyList<Payment> SampleData =
    [
        new("Paid", "alice@example.com", 100m),
        new("Pending", "bob@example.com", 250m),
        new("Failed", "carol@example.com", 75m),
    ];

    [Fact]
    public void DataTable_Renders_Table_Structure()
    {
        var cut = Render<DataTable<Payment>>(p => p
            .Add(x => x.Items, SampleData)
            .Add(x => x.Columns, builder => { })
            .Add(x => x.RowTemplate, item => builder =>
            {
                builder.OpenComponent<TableRow>(0);
                builder.CloseComponent();
            }));

        Assert.NotNull(cut.Find("table"));
        Assert.NotNull(cut.Find("thead"));
        Assert.NotNull(cut.Find("tbody"));
    }

    [Fact]
    public void DataTable_Shows_NoResults_When_Empty()
    {
        var cut = Render<DataTable<Payment>>(p => p
            .Add(x => x.Items, Array.Empty<Payment>())
            .Add(x => x.Columns, builder => { }));

        Assert.Contains("No results.", cut.Markup);
    }

    [Fact]
    public void DataTable_Renders_Rows_For_Items()
    {
        var cut = Render<DataTable<Payment>>(p => p
            .Add(x => x.Items, SampleData)
            .Add(x => x.Columns, builder => { })
            .Add(x => x.RowTemplate, item => builder =>
            {
                builder.OpenComponent<TableRow>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<TableCell>(2);
                    b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddContent(4, item.Email)));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        var rows = cut.FindAll("tbody tr");
        Assert.Equal(3, rows.Count);
        Assert.Contains("alice@example.com", cut.Markup);
        Assert.Contains("bob@example.com", cut.Markup);
        Assert.Contains("carol@example.com", cut.Markup);
    }

    [Fact]
    public void DataTable_Renders_Column_Headers()
    {
        var cut = Render<DataTable<Payment>>(p => p
            .Add(x => x.Items, SampleData)
            .Add(x => x.Columns, builder =>
            {
                builder.OpenComponent<DataTableColumn>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(2, "Status")));
                builder.CloseComponent();
                builder.OpenComponent<DataTableColumn>(3);
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(b => b.AddContent(5, "Email")));
                builder.CloseComponent();
            })
            .Add(x => x.RowTemplate, item => builder => { }));

        var headers = cut.FindAll("th");
        Assert.Equal(2, headers.Count);
        Assert.Equal("Status", headers[0].TextContent.Trim());
        Assert.Equal("Email", headers[1].TextContent.Trim());
    }

    [Fact]
    public void DataTable_IsWrappedIn_BorderDiv()
    {
        var cut = Render<DataTable<Payment>>(p => p
            .Add(x => x.Items, Array.Empty<Payment>())
            .Add(x => x.Columns, builder => { }));

        var outer = cut.Find("div");
        Assert.Contains("rounded-md", outer.ClassName);
        Assert.Contains("border", outer.ClassName);
    }

    [Fact]
    public void DataTable_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(DataTable<Payment>).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void DataTableColumn_Renders_TableHead()
    {
        var cut = Render<DataTableColumn>(p => p
            .AddChildContent("Amount"));

        Assert.NotNull(cut.Find("th"));
        Assert.Equal("Amount", cut.Find("th").TextContent.Trim());
    }

    [Fact]
    public void DataTableColumn_Sortable_Renders_Button()
    {
        var cut = Render<DataTableColumn>(p => p
            .Add(x => x.Sortable, true)
            .Add(x => x.SortKey, "amount")
            .AddChildContent("Amount"));

        Assert.NotNull(cut.Find("button"));
        Assert.Contains("Amount", cut.Find("button").TextContent);
    }

    [Fact]
    public void DataTableColumn_NotSortable_NoButton()
    {
        var cut = Render<DataTableColumn>(p => p
            .Add(x => x.Sortable, false)
            .AddChildContent("Status"));

        Assert.Empty(cut.FindAll("button"));
    }
}
