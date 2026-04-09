using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ScrollAreaTests : BlazingSpireTestBase
{
    // ── Base class ────────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(ScrollArea).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Structure ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Outer_Div_Element()
    {
        var cut = Render<ScrollArea>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Renders_Two_Nested_Divs()
    {
        var cut = Render<ScrollArea>();
        Assert.NotNull(cut.Find("div > div"));
    }

    [Fact]
    public void Inner_Div_Is_Direct_Child_Of_Outer_Div()
    {
        var cut = Render<ScrollArea>();
        var outer = cut.Find("div");
        Assert.Single(outer.Children);
        Assert.Equal("DIV", outer.Children[0].TagName);
    }

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Inner_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddChildContent("<p>Scrollable content</p>"));

        Assert.NotNull(cut.Find("div > div > p"));
    }

    [Fact]
    public void Multiple_Children_Render_Inside_Inner_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddChildContent("<p>Item 1</p><p>Item 2</p>"));

        Assert.Equal(2, cut.FindAll("div > div > p").Count);
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended_To_Outer_Div()
    {
        var cut = Render<ScrollArea>(p => p.Add(x => x.Class, "h-72 w-48"));
        var outer = cut.Find("div");
        Assert.Contains("h-72", outer.ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void DataTestId_PassesThrough_On_Outer_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddUnmatched("data-testid", "scroll-area"));

        Assert.Equal("scroll-area", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void AriaLabel_PassesThrough_On_Outer_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddUnmatched("aria-label", "Scrollable list"));

        AssertAriaLabel(cut.Find("div"), "Scrollable list");
    }

    [Fact]
    public void Role_PassesThrough_On_Outer_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddUnmatched("role", "region"));

        AssertRole(cut.Find("div"), "region");
    }

    [Fact]
    public void AriaDescribedBy_PassesThrough_On_Outer_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddUnmatched("aria-describedby", "scroll-desc"));

        Assert.Equal("scroll-desc", cut.Find("div").GetAttribute("aria-describedby"));
    }
}
