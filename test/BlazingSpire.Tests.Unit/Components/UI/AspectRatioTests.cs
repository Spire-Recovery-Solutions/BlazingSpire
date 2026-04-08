using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AspectRatioTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<AspectRatio>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Default ratio ────────────────────────────────────────────────────────

    [Fact]
    public void Default_Ratio_Is_16_By_9_In_Style()
    {
        var cut = Render<AspectRatio>();
        var style = cut.Find("div").GetAttribute("style");
        Assert.Contains("aspect-ratio:", style);
        // 16/9 ≈ 1.7777...
        Assert.Contains("1.777", style);
    }

    // ── Custom ratio ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Ratio_1_Is_Reflected_In_Style()
    {
        var cut = Render<AspectRatio>(p => p.Add(x => x.Ratio, 1.0));
        var style = cut.Find("div").GetAttribute("style");
        Assert.Contains("aspect-ratio: 1", style);
    }

    [Fact]
    public void Custom_Ratio_4_By_3_Is_Reflected_In_Style()
    {
        var cut = Render<AspectRatio>(p => p.Add(x => x.Ratio, 4.0 / 3.0));
        var style = cut.Find("div").GetAttribute("style");
        Assert.Contains("aspect-ratio:", style);
        Assert.Contains("1.333", style);
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Relative_Class()
    {
        var cut = Render<AspectRatio>();
        Assert.Contains("relative", cut.Find("div").ClassName);
    }

    [Fact]
    public void Always_Has_W_Full_Class()
    {
        var cut = Render<AspectRatio>();
        Assert.Contains("w-full", cut.Find("div").ClassName);
    }

    [Fact]
    public void Always_Has_Overflow_Hidden_Class()
    {
        var cut = Render<AspectRatio>();
        Assert.Contains("overflow-hidden", cut.Find("div").ClassName);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<AspectRatio>(p => p.Add(x => x.Class, "rounded-lg"));
        Assert.Contains("rounded-lg", cut.Find("div").ClassName);
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<AspectRatio>(p =>
            p.AddChildContent("<img src='test.jpg' alt='test' />"));

        Assert.NotNull(cut.Find("div img"));
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<AspectRatio>(p =>
            p.AddUnmatched("data-testid", "aspect-ratio-wrapper"));

        Assert.Equal("aspect-ratio-wrapper", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<AspectRatio>(p =>
            p.AddUnmatched("aria-label", "Image container"));

        AssertAriaLabel(cut.Find("div"), "Image container");
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void AspectRatio_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AspectRatio).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
