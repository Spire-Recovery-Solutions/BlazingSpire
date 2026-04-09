using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SkeletonTests : BlazingSpireTestBase
{
    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Skeleton>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Skeleton>(p => p.AddChildContent("<span>placeholder</span>"));
        Assert.NotNull(cut.Find("div span"));
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Skeleton>(p => p.Add(x => x.Class, "h-4 w-32"));
        Assert.Contains("h-4", cut.Find("div").ClassName);
        Assert.Contains("w-32", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Skeleton>(p => p.AddUnmatched("aria-label", "Loading"));
        AssertAriaLabel(cut.Find("div"), "Loading");
    }

    [Fact]
    public void DataTestId_PassesThrough()
    {
        var cut = Render<Skeleton>(p => p.AddUnmatched("data-testid", "skeleton-line"));
        Assert.Equal("skeleton-line", cut.Find("div").GetAttribute("data-testid"));
    }
}
