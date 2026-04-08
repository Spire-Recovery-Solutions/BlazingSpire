using BlazingSpire.Demo.Components.Shared;
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
    public void Table_Has_Base_Classes()
    {
        var cut = Render<Table>();
        var classes = cut.Find("table").ClassName;
        Assert.Contains("w-full", classes);
        Assert.Contains("caption-bottom", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void Table_Custom_Class_Is_Appended()
    {
        var cut = Render<Table>(p => p.Add(x => x.Class, "my-table"));
        Assert.Contains("my-table", cut.Find("table").ClassName);
    }

    [Fact]
    public void Table_ChildContent_Renders()
    {
        var cut = Render<Table>(p => p.AddChildContent("<tbody></tbody>"));
        Assert.NotNull(cut.Find("tbody"));
    }

    [Fact]
    public void Table_IsWrappedIn_ScrollDiv()
    {
        var cut = Render<Table>();
        var wrapper = cut.Find("div");
        Assert.Contains("overflow-auto", wrapper.ClassName);
    }

    [Fact]
    public void Table_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Table).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── TableHeader ──────────────────────────────────────────────────────────

    [Fact]
    public void TableHeader_Renders_Thead_Element()
    {
        var cut = Render<TableHeader>();
        Assert.NotNull(cut.Find("thead"));
    }

    [Fact]
    public void TableHeader_Has_Base_Classes()
    {
        var cut = Render<TableHeader>();
        Assert.Contains("[&_tr]:border-b", cut.Find("thead").ClassName);
    }

    [Fact]
    public void TableHeader_Custom_Class_Is_Appended()
    {
        var cut = Render<TableHeader>(p => p.Add(x => x.Class, "custom-header"));
        Assert.Contains("custom-header", cut.Find("thead").ClassName);
    }

    [Fact]
    public void TableHeader_ChildContent_Renders()
    {
        var cut = Render<TableHeader>(p => p.AddChildContent("<tr></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }

    // ── TableBody ────────────────────────────────────────────────────────────

    [Fact]
    public void TableBody_Renders_Tbody_Element()
    {
        var cut = Render<TableBody>();
        Assert.NotNull(cut.Find("tbody"));
    }

    [Fact]
    public void TableBody_Has_Base_Classes()
    {
        var cut = Render<TableBody>();
        Assert.Contains("[&_tr:last-child]:border-0", cut.Find("tbody").ClassName);
    }

    [Fact]
    public void TableBody_Custom_Class_Is_Appended()
    {
        var cut = Render<TableBody>(p => p.Add(x => x.Class, "custom-body"));
        Assert.Contains("custom-body", cut.Find("tbody").ClassName);
    }

    [Fact]
    public void TableBody_ChildContent_Renders()
    {
        var cut = Render<TableBody>(p => p.AddChildContent("<tr><td>Cell</td></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }

    // ── TableRow ─────────────────────────────────────────────────────────────

    [Fact]
    public void TableRow_Renders_Tr_Element()
    {
        var cut = Render<TableRow>();
        Assert.NotNull(cut.Find("tr"));
    }

    [Fact]
    public void TableRow_Has_Base_Classes()
    {
        var cut = Render<TableRow>();
        var classes = cut.Find("tr").ClassName;
        Assert.Contains("border-b", classes);
        Assert.Contains("transition-colors", classes);
        Assert.Contains("hover:bg-muted/50", classes);
    }

    [Fact]
    public void TableRow_Custom_Class_Is_Appended()
    {
        var cut = Render<TableRow>(p => p.Add(x => x.Class, "custom-row"));
        Assert.Contains("custom-row", cut.Find("tr").ClassName);
    }

    [Fact]
    public void TableRow_ChildContent_Renders()
    {
        var cut = Render<TableRow>(p => p.AddChildContent("<td>Data</td>"));
        Assert.Equal("Data", cut.Find("td").TextContent);
    }

    // ── TableHead ────────────────────────────────────────────────────────────

    [Fact]
    public void TableHead_Renders_Th_Element()
    {
        var cut = Render<TableHead>();
        Assert.NotNull(cut.Find("th"));
    }

    [Fact]
    public void TableHead_Has_Base_Classes()
    {
        var cut = Render<TableHead>();
        var classes = cut.Find("th").ClassName;
        Assert.Contains("h-12", classes);
        Assert.Contains("px-4", classes);
        Assert.Contains("font-medium", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void TableHead_Custom_Class_Is_Appended()
    {
        var cut = Render<TableHead>(p => p.Add(x => x.Class, "custom-head"));
        Assert.Contains("custom-head", cut.Find("th").ClassName);
    }

    [Fact]
    public void TableHead_ChildContent_Renders()
    {
        var cut = Render<TableHead>(p => p.AddChildContent("Invoice"));
        Assert.Equal("Invoice", cut.Find("th").TextContent);
    }

    // ── TableCell ────────────────────────────────────────────────────────────

    [Fact]
    public void TableCell_Renders_Td_Element()
    {
        var cut = Render<TableCell>();
        Assert.NotNull(cut.Find("td"));
    }

    [Fact]
    public void TableCell_Has_Base_Classes()
    {
        var cut = Render<TableCell>();
        var classes = cut.Find("td").ClassName;
        Assert.Contains("p-4", classes);
        Assert.Contains("align-middle", classes);
    }

    [Fact]
    public void TableCell_Custom_Class_Is_Appended()
    {
        var cut = Render<TableCell>(p => p.Add(x => x.Class, "custom-cell"));
        Assert.Contains("custom-cell", cut.Find("td").ClassName);
    }

    [Fact]
    public void TableCell_ChildContent_Renders()
    {
        var cut = Render<TableCell>(p => p.AddChildContent("$250.00"));
        Assert.Equal("$250.00", cut.Find("td").TextContent);
    }

    // ── TableCaption ─────────────────────────────────────────────────────────

    [Fact]
    public void TableCaption_Renders_Caption_Element()
    {
        var cut = Render<TableCaption>();
        Assert.NotNull(cut.Find("caption"));
    }

    [Fact]
    public void TableCaption_Has_Base_Classes()
    {
        var cut = Render<TableCaption>();
        var classes = cut.Find("caption").ClassName;
        Assert.Contains("mt-4", classes);
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void TableCaption_Custom_Class_Is_Appended()
    {
        var cut = Render<TableCaption>(p => p.Add(x => x.Class, "custom-caption"));
        Assert.Contains("custom-caption", cut.Find("caption").ClassName);
    }

    [Fact]
    public void TableCaption_ChildContent_Renders()
    {
        var cut = Render<TableCaption>(p => p.AddChildContent("A list of invoices."));
        Assert.Equal("A list of invoices.", cut.Find("caption").TextContent);
    }

    // ── TableFooter ──────────────────────────────────────────────────────────

    [Fact]
    public void TableFooter_Renders_Tfoot_Element()
    {
        var cut = Render<TableFooter>();
        Assert.NotNull(cut.Find("tfoot"));
    }

    [Fact]
    public void TableFooter_Has_Base_Classes()
    {
        var cut = Render<TableFooter>();
        var classes = cut.Find("tfoot").ClassName;
        Assert.Contains("border-t", classes);
        Assert.Contains("bg-muted/50", classes);
        Assert.Contains("font-medium", classes);
    }

    [Fact]
    public void TableFooter_Custom_Class_Is_Appended()
    {
        var cut = Render<TableFooter>(p => p.Add(x => x.Class, "custom-footer"));
        Assert.Contains("custom-footer", cut.Find("tfoot").ClassName);
    }

    [Fact]
    public void TableFooter_ChildContent_Renders()
    {
        var cut = Render<TableFooter>(p => p.AddChildContent("<tr><td>Total</td></tr>"));
        Assert.NotNull(cut.Find("tr"));
    }
}
