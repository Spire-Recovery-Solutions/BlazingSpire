using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ScrollAreaTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Outer_Div_Element()
    {
        var cut = Render<ScrollArea>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void ChildContent_Renders_Inside_Inner_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddChildContent("<p>Scrollable content</p>"));

        Assert.NotNull(cut.Find("div > div > p"));
    }

    [Fact]
    public void Custom_Class_Is_Appended_To_Outer_Div()
    {
        var cut = Render<ScrollArea>(p => p.Add(x => x.Class, "h-72 w-48"));
        var outer = cut.Find("div");
        Assert.Contains("h-72", outer.ClassName);
    }

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
}
