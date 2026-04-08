using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SkeletonTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Skeleton>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Animate_Pulse_Class()
    {
        var cut = Render<Skeleton>();
        Assert.Contains("animate-pulse", cut.Find("div").ClassName);
    }

    [Fact]
    public void Always_Has_Rounded_Class()
    {
        var cut = Render<Skeleton>();
        Assert.Contains("rounded-md", cut.Find("div").ClassName);
    }

    [Fact]
    public void Always_Has_Background_Muted_Class()
    {
        var cut = Render<Skeleton>();
        Assert.Contains("bg-muted", cut.Find("div").ClassName);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended_For_Width()
    {
        var cut = Render<Skeleton>(p => p.Add(x => x.Class, "w-[250px]"));
        Assert.Contains("w-[250px]", cut.Find("div").ClassName);
    }

    [Fact]
    public void Custom_Class_Is_Appended_For_Height()
    {
        var cut = Render<Skeleton>(p => p.Add(x => x.Class, "h-4"));
        Assert.Contains("h-4", cut.Find("div").ClassName);
    }

    [Fact]
    public void Custom_Class_Overrides_Rounded_For_Circle()
    {
        var cut = Render<Skeleton>(p => p.Add(x => x.Class, "h-12 w-12 rounded-full"));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("rounded-full", classes);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Skeleton>(p =>
            p.AddUnmatched("aria-label", "Loading"));

        AssertAriaLabel(cut.Find("div"), "Loading");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Skeleton>(p =>
            p.AddUnmatched("data-testid", "skeleton-line"));

        Assert.Equal("skeleton-line", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Skeleton>(p =>
            p.AddChildContent("<span>placeholder</span>"));

        Assert.NotNull(cut.Find("div span"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Skeleton_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Skeleton).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
