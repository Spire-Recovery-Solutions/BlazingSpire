using BlazingSpire.Demo.Components.Shared;
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
    public void Card_Has_Base_Classes()
    {
        var cut = Render<Card>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("rounded-xl", classes);
        Assert.Contains("border", classes);
        Assert.Contains("bg-card", classes);
        Assert.Contains("text-card-foreground", classes);
    }

    [Fact]
    public void Card_Custom_Class_Is_Appended()
    {
        var cut = Render<Card>(p => p.Add(x => x.Class, "my-card"));
        Assert.Contains("my-card", cut.Find("div").ClassName);
    }

    [Fact]
    public void Card_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Card>(p =>
            p.AddUnmatched("data-testid", "my-card"));
        Assert.Equal("my-card", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void Card_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Card>(p =>
            p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void Card_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Card).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CardHeader ────────────────────────────────────────────────────────────

    [Fact]
    public void CardHeader_Renders_Div_Element()
    {
        var cut = Render<CardHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardHeader_Has_Base_Classes()
    {
        var cut = Render<CardHeader>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("p-6", classes);
    }

    [Fact]
    public void CardHeader_Custom_Class_Is_Appended()
    {
        var cut = Render<CardHeader>(p => p.Add(x => x.Class, "my-header"));
        Assert.Contains("my-header", cut.Find("div").ClassName);
    }

    [Fact]
    public void CardHeader_AdditionalAttributes_PassThrough()
    {
        var cut = Render<CardHeader>(p =>
            p.AddUnmatched("data-testid", "card-header"));
        Assert.Equal("card-header", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void CardHeader_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardHeader>(p =>
            p.AddChildContent("<h3>Title</h3>"));
        Assert.NotNull(cut.Find("div h3"));
    }

    [Fact]
    public void CardHeader_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CardHeader).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CardTitle ─────────────────────────────────────────────────────────────

    [Fact]
    public void CardTitle_Renders_H3_Element()
    {
        var cut = Render<CardTitle>();
        Assert.NotNull(cut.Find("h3"));
    }

    [Fact]
    public void CardTitle_Has_Base_Classes()
    {
        var cut = Render<CardTitle>();
        var classes = cut.Find("h3").ClassName;
        Assert.Contains("text-2xl", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("leading-none", classes);
        Assert.Contains("tracking-tight", classes);
    }

    [Fact]
    public void CardTitle_Custom_Class_Is_Appended()
    {
        var cut = Render<CardTitle>(p => p.Add(x => x.Class, "my-title"));
        Assert.Contains("my-title", cut.Find("h3").ClassName);
    }

    [Fact]
    public void CardTitle_AdditionalAttributes_PassThrough()
    {
        var cut = Render<CardTitle>(p =>
            p.AddUnmatched("data-testid", "card-title"));
        Assert.Equal("card-title", cut.Find("h3").GetAttribute("data-testid"));
    }

    [Fact]
    public void CardTitle_ChildContent_Renders_Inside_H3()
    {
        var cut = Render<CardTitle>(p =>
            p.AddChildContent("My Title"));
        Assert.Contains("My Title", cut.Find("h3").TextContent);
    }

    [Fact]
    public void CardTitle_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CardTitle).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CardDescription ───────────────────────────────────────────────────────

    [Fact]
    public void CardDescription_Renders_P_Element()
    {
        var cut = Render<CardDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void CardDescription_Has_Base_Classes()
    {
        var cut = Render<CardDescription>();
        var classes = cut.Find("p").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void CardDescription_Custom_Class_Is_Appended()
    {
        var cut = Render<CardDescription>(p => p.Add(x => x.Class, "my-desc"));
        Assert.Contains("my-desc", cut.Find("p").ClassName);
    }

    [Fact]
    public void CardDescription_AdditionalAttributes_PassThrough()
    {
        var cut = Render<CardDescription>(p =>
            p.AddUnmatched("data-testid", "card-desc"));
        Assert.Equal("card-desc", cut.Find("p").GetAttribute("data-testid"));
    }

    [Fact]
    public void CardDescription_ChildContent_Renders_Inside_P()
    {
        var cut = Render<CardDescription>(p =>
            p.AddChildContent("Some description text"));
        Assert.Contains("Some description text", cut.Find("p").TextContent);
    }

    [Fact]
    public void CardDescription_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CardDescription).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CardContent ───────────────────────────────────────────────────────────

    [Fact]
    public void CardContent_Renders_Div_Element()
    {
        var cut = Render<CardContent>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardContent_Has_Base_Classes()
    {
        var cut = Render<CardContent>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("p-6", classes);
        Assert.Contains("pt-0", classes);
    }

    [Fact]
    public void CardContent_Custom_Class_Is_Appended()
    {
        var cut = Render<CardContent>(p => p.Add(x => x.Class, "my-content"));
        Assert.Contains("my-content", cut.Find("div").ClassName);
    }

    [Fact]
    public void CardContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<CardContent>(p =>
            p.AddUnmatched("data-testid", "card-content"));
        Assert.Equal("card-content", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void CardContent_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardContent>(p =>
            p.AddChildContent("<p>Body text</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void CardContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CardContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CardFooter ────────────────────────────────────────────────────────────

    [Fact]
    public void CardFooter_Renders_Div_Element()
    {
        var cut = Render<CardFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CardFooter_Has_Base_Classes()
    {
        var cut = Render<CardFooter>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("p-6", classes);
        Assert.Contains("pt-0", classes);
    }

    [Fact]
    public void CardFooter_Custom_Class_Is_Appended()
    {
        var cut = Render<CardFooter>(p => p.Add(x => x.Class, "my-footer"));
        Assert.Contains("my-footer", cut.Find("div").ClassName);
    }

    [Fact]
    public void CardFooter_AdditionalAttributes_PassThrough()
    {
        var cut = Render<CardFooter>(p =>
            p.AddUnmatched("data-testid", "card-footer"));
        Assert.Equal("card-footer", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void CardFooter_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<CardFooter>(p =>
            p.AddChildContent("<button>Action</button>"));
        Assert.NotNull(cut.Find("div button"));
    }

    [Fact]
    public void CardFooter_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CardFooter).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Composition ───────────────────────────────────────────────────────────

    [Fact]
    public void Full_Card_Composition_Renders_Correct_Structure()
    {
        var cut = Render<Card>(p => p.AddChildContent(@"
            <CardHeader>
                <CardTitle>Hello</CardTitle>
                <CardDescription>World</CardDescription>
            </CardHeader>
            <CardContent>Body</CardContent>
            <CardFooter>Footer</CardFooter>"));

        Assert.NotNull(cut.Find("div"));
    }
}
