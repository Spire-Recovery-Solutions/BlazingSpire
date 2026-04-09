using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class BadgeTests : BlazingSpireTestBase
{
    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Badge>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(BadgeVariant.Default)]
    [InlineData(BadgeVariant.Secondary)]
    [InlineData(BadgeVariant.Destructive)]
    [InlineData(BadgeVariant.Outline)]
    public void Each_Variant_Renders_Without_Error(BadgeVariant variant)
    {
        var cut = Render<Badge>(p => p.Add(x => x.Variant, variant));
        Assert.NotNull(cut.Find("div"));
    }

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Text_Renders_Inside_Div()
    {
        var cut = Render<Badge>(p => p.AddChildContent("New"));
        Assert.Contains("New", cut.Find("div").TextContent);
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Badge>(p => p.Add(x => x.Class, "my-badge"));
        Assert.Contains("my-badge", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Badge>(p => p.AddUnmatched("aria-label", "Status badge"));
        AssertAriaLabel(cut.Find("div"), "Status badge");
    }
}
