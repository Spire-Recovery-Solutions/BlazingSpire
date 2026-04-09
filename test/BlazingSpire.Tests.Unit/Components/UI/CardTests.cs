using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CardTests : BlazingSpireTestBase
{
    // ── Card ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Card_Renders_Div_Element()
    {
        var cut = Render<Card>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Card_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Card>(p => p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void Card_Custom_Class_Is_Included()
    {
        var cut = Render<Card>(p => p.Add(x => x.Class, "my-card"));
        Assert.Contains("my-card", cut.Find("div").ClassName);
    }

    // ── CardHeader ────────────────────────────────────────────────────────────

    [Fact]
    public void CardHeader_Renders_Div_Element()
    {
        var cut = Render<CardHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardHeader_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardHeader>(p => p.AddChildContent("<h3>Title</h3>"));
        Assert.NotNull(cut.Find("div h3"));
    }

    [Fact]
    public void CardHeader_Custom_Class_Is_Included()
    {
        var cut = Render<CardHeader>(p => p.Add(x => x.Class, "my-header"));
        Assert.Contains("my-header", cut.Find("div").ClassName);
    }

    // ── CardTitle ─────────────────────────────────────────────────────────────

    [Fact]
    public void CardTitle_Renders_H3_Element()
    {
        var cut = Render<CardTitle>();
        Assert.NotNull(cut.Find("h3"));
    }

    [Fact]
    public void CardTitle_ChildContent_Renders_Inside_H3()
    {
        var cut = Render<CardTitle>(p => p.AddChildContent("My Title"));
        Assert.Contains("My Title", cut.Find("h3").TextContent);
    }

    [Fact]
    public void CardTitle_Custom_Class_Is_Included()
    {
        var cut = Render<CardTitle>(p => p.Add(x => x.Class, "my-title"));
        Assert.Contains("my-title", cut.Find("h3").ClassName);
    }

    // ── CardDescription ───────────────────────────────────────────────────────

    [Fact]
    public void CardDescription_Renders_P_Element()
    {
        var cut = Render<CardDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void CardDescription_ChildContent_Renders_Inside_P()
    {
        var cut = Render<CardDescription>(p => p.AddChildContent("Some description text"));
        Assert.Contains("Some description text", cut.Find("p").TextContent);
    }

    [Fact]
    public void CardDescription_Custom_Class_Is_Included()
    {
        var cut = Render<CardDescription>(p => p.Add(x => x.Class, "my-desc"));
        Assert.Contains("my-desc", cut.Find("p").ClassName);
    }

    // ── CardContent ───────────────────────────────────────────────────────────

    [Fact]
    public void CardContent_Renders_Div_Element()
    {
        var cut = Render<CardContent>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardContent_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardContent>(p => p.AddChildContent("<p>Body text</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void CardContent_Custom_Class_Is_Included()
    {
        var cut = Render<CardContent>(p => p.Add(x => x.Class, "my-content"));
        Assert.Contains("my-content", cut.Find("div").ClassName);
    }

    // ── CardFooter ────────────────────────────────────────────────────────────

    [Fact]
    public void CardFooter_Renders_Div_Element()
    {
        var cut = Render<CardFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardFooter_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardFooter>(p => p.AddChildContent("<button>Action</button>"));
        Assert.NotNull(cut.Find("div button"));
    }

    [Fact]
    public void CardFooter_Custom_Class_Is_Included()
    {
        var cut = Render<CardFooter>(p => p.Add(x => x.Class, "my-footer"));
        Assert.Contains("my-footer", cut.Find("div").ClassName);
    }
}
