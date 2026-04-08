using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ScrollAreaTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Outer_Div_Element()
    {
        var cut = Render<ScrollArea>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Renders_Inner_Div_With_Overflow_Auto()
    {
        var cut = Render<ScrollArea>();
        var inner = cut.Find("div > div");
        Assert.Contains("overflow-auto", inner.ClassName);
    }

    [Fact]
    public void Inner_Div_Has_Full_Width_And_Height()
    {
        var cut = Render<ScrollArea>();
        var inner = cut.Find("div > div");
        Assert.Contains("h-full", inner.ClassName);
        Assert.Contains("w-full", inner.ClassName);
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Outer_Div_Has_Relative_Class()
    {
        var cut = Render<ScrollArea>();
        Assert.Contains("relative", cut.Find("div").ClassName);
    }

    [Fact]
    public void Outer_Div_Has_Overflow_Hidden_Class()
    {
        var cut = Render<ScrollArea>();
        Assert.Contains("overflow-hidden", cut.Find("div").ClassName);
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Inner_Div()
    {
        var cut = Render<ScrollArea>(p =>
            p.AddChildContent("<p>Scrollable content</p>"));

        Assert.NotNull(cut.Find("div > div > p"));
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended_To_Outer_Div()
    {
        var cut = Render<ScrollArea>(p => p.Add(x => x.Class, "h-72 w-48"));
        var outer = cut.Find("div");
        Assert.Contains("h-72", outer.ClassName);
        Assert.Contains("w-48", outer.ClassName);
    }

    [Fact]
    public void Custom_Class_Does_Not_Affect_Inner_Div()
    {
        var cut = Render<ScrollArea>(p => p.Add(x => x.Class, "h-72"));
        var inner = cut.Find("div > div");
        Assert.DoesNotContain("h-72", inner.ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

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

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void ScrollArea_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(ScrollArea).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
