using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SkeletonTests : BlazingSpireTestBase
{
    // ── Base class ────────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Skeleton).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Skeleton>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Renders_Single_Root_Element_Without_ChildContent()
    {
        var cut = Render<Skeleton>();
        Assert.Single(cut.FindAll("div"));
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

    [Fact]
    public void AriaHidden_PassesThrough()
    {
        var cut = Render<Skeleton>(p => p.AddUnmatched("aria-hidden", "true"));
        AssertAriaHidden(cut.Find("div"), true);
    }

    [Fact]
    public void Multiple_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Skeleton>(p => p
            .AddUnmatched("aria-label", "Loading content")
            .AddUnmatched("data-testid", "skeleton-block"));
        AssertAriaLabel(cut.Find("div"), "Loading content");
        Assert.Equal("skeleton-block", cut.Find("div").GetAttribute("data-testid"));
    }
}
