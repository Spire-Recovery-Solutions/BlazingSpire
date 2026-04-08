using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class BadgeTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Badge>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Fact]
    public void Default_Variant_Has_Primary_Classes()
    {
        var cut = Render<Badge>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("bg-primary", classes);
        Assert.Contains("text-primary-foreground", classes);
    }

    [Theory]
    [InlineData(BadgeVariant.Default, "bg-primary", "text-primary-foreground")]
    [InlineData(BadgeVariant.Secondary, "bg-secondary", "text-secondary-foreground")]
    [InlineData(BadgeVariant.Destructive, "bg-destructive", "text-destructive-foreground")]
    [InlineData(BadgeVariant.Outline, "text-foreground", "border")]
    public void Variant_Produces_Correct_Classes(BadgeVariant variant, string class1, string class2)
    {
        var cut = Render<Badge>(p => p.Add(x => x.Variant, variant));
        var classes = cut.Find("div").ClassName;
        Assert.Contains(class1, classes);
        Assert.Contains(class2, classes);
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Layout_Classes()
    {
        var cut = Render<Badge>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("rounded-full", classes);
        Assert.Contains("px-2.5", classes);
        Assert.Contains("text-xs", classes);
        Assert.Contains("font-semibold", classes);
    }

    [Fact]
    public void Always_Has_Focus_Ring_Classes()
    {
        var cut = Render<Badge>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("focus:ring-2", classes);
        Assert.Contains("focus:ring-ring", classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Badge>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Badge>(p =>
            p.AddUnmatched("aria-label", "Status badge"));

        AssertAriaLabel(cut.Find("div"), "Status badge");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Badge>(p =>
            p.AddUnmatched("data-testid", "status-badge"));

        Assert.Equal("status-badge", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Text_Renders_Inside_Div()
    {
        var cut = Render<Badge>(p =>
            p.AddChildContent("New"));

        Assert.Contains("New", cut.Find("div").TextContent);
    }

    [Fact]
    public void ChildContent_Element_Renders_Inside_Div()
    {
        var cut = Render<Badge>(p =>
            p.AddChildContent("<span>Active</span>"));

        Assert.NotNull(cut.Find("div span"));
        Assert.Equal("Active", cut.Find("div span").TextContent);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Badge_Is_Assignable_To_PresentationalBase()
    {
        Assert.True(typeof(Badge).IsAssignableTo(typeof(PresentationalBase<BadgeVariant>)));
    }

    [Fact]
    public void Badge_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Badge).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
