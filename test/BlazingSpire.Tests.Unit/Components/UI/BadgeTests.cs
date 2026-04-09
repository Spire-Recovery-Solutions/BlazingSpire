using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class BadgeTests : BlazingSpireTestBase
{
    // ── Base class ────────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_PresentationalBase()
    {
        Assert.True(typeof(Badge).IsAssignableTo(typeof(PresentationalBase<BadgeVariant>)));
    }

    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Badge>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Renders_Single_Root_Element()
    {
        var cut = Render<Badge>();
        Assert.Single(cut.FindAll("div"));
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

    [Fact]
    public void ChildContent_With_Html_Element_Renders_Inside_Div()
    {
        var cut = Render<Badge>(p => p.AddChildContent("<span>Beta</span>"));
        Assert.NotNull(cut.Find("div span"));
        Assert.Contains("Beta", cut.Find("div span").TextContent);
    }

    [Fact]
    public void Empty_ChildContent_Renders_Without_Error()
    {
        var cut = Render<Badge>();
        Assert.Equal(string.Empty, cut.Find("div").TextContent);
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

    [Fact]
    public void DataTestId_PassesThrough()
    {
        var cut = Render<Badge>(p => p.AddUnmatched("data-testid", "badge-new"));
        Assert.Equal("badge-new", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void AriaDescribedBy_PassesThrough()
    {
        var cut = Render<Badge>(p => p.AddUnmatched("aria-describedby", "tooltip-1"));
        Assert.Equal("tooltip-1", cut.Find("div").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Multiple_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Badge>(p => p
            .AddUnmatched("aria-label", "Status")
            .AddUnmatched("data-testid", "status-badge"));
        AssertAriaLabel(cut.Find("div"), "Status");
        Assert.Equal("status-badge", cut.Find("div").GetAttribute("data-testid"));
    }
}
