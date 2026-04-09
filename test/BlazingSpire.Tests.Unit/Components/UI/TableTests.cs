using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TableTests : BlazingSpireTestBase
{
    // ── Table ────────────────────────────────────────────────────────────────

    [Fact]
    public void Table_Renders_Table_Element()
    {
        var cut = Render<Table>();
        Assert.NotNull(cut.Find("table"));
    }

    [Fact]
    public void Table_Is_Wrapped_In_Scrollable_Div()
    {
        var cut = Render<Table>();
        Assert.Contains("overflow-auto", cut.Find("div").ClassName);
    }

    [Fact]
    public void Table_ChildContent_Renders()
    {
        var cut = Render<Table>(p => p.AddChildContent("<tbody></tbody>"));
        Assert.NotNull(cut.Find("tbody"));
    }

    [Fact]
    public void Table_Custom_Class_Is_Included()
    {
        var cut = Render<Table>(p => p.Add(x => x.Class, "my-table"));
        Assert.Contains("my-table", cut.Find("table").ClassName);
    }

    [Fact]
    public void Table_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Table>(p => p.AddUnmatched("data-testid", "my-table"));
        Assert.Equal("my-table", cut.Find("table").GetAttribute("data-testid"));
    }

    // ── TableHeader ──────────────────────────────────────────────────────────

    [Fact]
    public void TableHeader_Renders_Thead_Element()
    {
        var cut = Render<TableHeader>();
        Assert.NotNull(cut.Find("thead"));
    }

    [Fact]
    public void TableHeader_ChildContent_Renders()
    {
        var cut = Render<TableHeader>(p => p.AddChildContent("<tr></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }

    [Fact]
    public void TableHeader_Custom_Class_Is_Included()
    {
        var cut = Render<TableHeader>(p => p.Add(x => x.Class, "my-header"));
        Assert.Contains("my-header", cut.Find("thead").ClassName);
    }

    // ── TableBody ────────────────────────────────────────────────────────────

    [Fact]
    public void TableBody_Renders_Tbody_Element()
    {
        var cut = Render<TableBody>();
        Assert.NotNull(cut.Find("tbody"));
    }

    [Fact]
    public void TableBody_ChildContent_Renders()
    {
        var cut = Render<TableBody>(p => p.AddChildContent("<tr><td>Cell</td></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }

    [Fact]
    public void TableBody_Custom_Class_Is_Included()
    {
        var cut = Render<TableBody>(p => p.Add(x => x.Class, "my-body"));
        Assert.Contains("my-body", cut.Find("tbody").ClassName);
    }

    // ── TableRow ─────────────────────────────────────────────────────────────

    [Fact]
    public void TableRow_Renders_Tr_Element()
    {
        var cut = Render<TableRow>();
        Assert.NotNull(cut.Find("tr"));
    }

    [Fact]
    public void TableRow_ChildContent_Renders()
    {
        var cut = Render<TableRow>(p => p.AddChildContent("<td>Data</td>"));
        Assert.Equal("Data", cut.Find("td").TextContent);
    }

    [Fact]
    public void TableRow_Custom_Class_Is_Included()
    {
        var cut = Render<TableRow>(p => p.Add(x => x.Class, "my-row"));
        Assert.Contains("my-row", cut.Find("tr").ClassName);
    }

    // ── TableHead ────────────────────────────────────────────────────────────

    [Fact]
    public void TableHead_Renders_Th_Element()
    {
        var cut = Render<TableHead>();
        Assert.NotNull(cut.Find("th"));
    }

    [Fact]
    public void TableHead_ChildContent_Renders()
    {
        var cut = Render<TableHead>(p => p.AddChildContent("Invoice"));
        Assert.Equal("Invoice", cut.Find("th").TextContent);
    }

    [Fact]
    public void TableHead_Custom_Class_Is_Included()
    {
        var cut = Render<TableHead>(p => p.Add(x => x.Class, "my-head"));
        Assert.Contains("my-head", cut.Find("th").ClassName);
    }

    // ── TableCell ────────────────────────────────────────────────────────────

    [Fact]
    public void TableCell_Renders_Td_Element()
    {
        var cut = Render<TableCell>();
        Assert.NotNull(cut.Find("td"));
    }

    [Fact]
    public void TableCell_ChildContent_Renders()
    {
        var cut = Render<TableCell>(p => p.AddChildContent("$250.00"));
        Assert.Equal("$250.00", cut.Find("td").TextContent);
    }

    [Fact]
    public void TableCell_Custom_Class_Is_Included()
    {
        var cut = Render<TableCell>(p => p.Add(x => x.Class, "my-cell"));
        Assert.Contains("my-cell", cut.Find("td").ClassName);
    }

    // ── TableCaption ─────────────────────────────────────────────────────────

    [Fact]
    public void TableCaption_Renders_Caption_Element()
    {
        var cut = Render<TableCaption>();
        Assert.NotNull(cut.Find("caption"));
    }

    [Fact]
    public void TableCaption_ChildContent_Renders()
    {
        var cut = Render<TableCaption>(p => p.AddChildContent("A list of invoices."));
        Assert.Equal("A list of invoices.", cut.Find("caption").TextContent);
    }

    [Fact]
    public void TableCaption_Custom_Class_Is_Included()
    {
        var cut = Render<TableCaption>(p => p.Add(x => x.Class, "my-caption"));
        Assert.Contains("my-caption", cut.Find("caption").ClassName);
    }

    // ── TableFooter ──────────────────────────────────────────────────────────

    [Fact]
    public void TableFooter_Renders_Tfoot_Element()
    {
        var cut = Render<TableFooter>();
        Assert.NotNull(cut.Find("tfoot"));
    }

    [Fact]
    public void TableFooter_ChildContent_Renders()
    {
        var cut = Render<TableFooter>(p => p.AddChildContent("<tr><td>Total</td></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }

    [Fact]
    public void TableFooter_Custom_Class_Is_Included()
    {
        var cut = Render<TableFooter>(p => p.Add(x => x.Class, "my-footer"));
        Assert.Contains("my-footer", cut.Find("tfoot").ClassName);
    }
}
