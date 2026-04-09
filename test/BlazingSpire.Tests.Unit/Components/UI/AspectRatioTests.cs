using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AspectRatioTests : BlazingSpireTestBase
{
    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<AspectRatio>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Ratio in style attribute ──────────────────────────────────────────────

    [Fact]
    public void Default_Ratio_Is_16_By_9_In_Style()
    {
        var cut = Render<AspectRatio>();
        var style = cut.Find("div").GetAttribute("style");
        Assert.Contains("aspect-ratio:", style);
        Assert.Contains("1.777", style);
    }

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

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<AspectRatio>(p => p.AddChildContent("<img src='test.jpg' alt='test' />"));
        Assert.NotNull(cut.Find("div img"));
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<AspectRatio>(p => p.Add(x => x.Class, "rounded-lg"));
        Assert.Contains("rounded-lg", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<AspectRatio>(p => p.AddUnmatched("aria-label", "Image container"));
        AssertAriaLabel(cut.Find("div"), "Image container");
    }
}
