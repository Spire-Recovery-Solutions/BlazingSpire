using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SeparatorTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<Separator>();
        Assert.NotNull(cut.Find("div"));
    }

    // ── Orientation ──────────────────────────────────────────────────────────

    [Fact]
    public void Default_Orientation_Is_Horizontal()
    {
        var cut = Render<Separator>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("h-[1px]", classes);
        Assert.Contains("w-full", classes);
    }

    [Fact]
    public void Vertical_Orientation_Produces_Correct_Classes()
    {
        var cut = Render<Separator>(p => p.Add(x => x.Orientation, SeparatorOrientation.Vertical));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("h-full", classes);
        Assert.Contains("w-[1px]", classes);
    }

    [Theory]
    [InlineData(SeparatorOrientation.Horizontal, "h-[1px]", "w-full")]
    [InlineData(SeparatorOrientation.Vertical, "h-full", "w-[1px]")]
    public void Orientation_Produces_Correct_Classes(SeparatorOrientation orientation, string class1, string class2)
    {
        var cut = Render<Separator>(p => p.Add(x => x.Orientation, orientation));
        var classes = cut.Find("div").ClassName;
        Assert.Contains(class1, classes);
        Assert.Contains(class2, classes);
    }

    // ── ARIA / Decorative ────────────────────────────────────────────────────

    [Fact]
    public void Default_Is_Decorative_With_Role_None()
    {
        var cut = Render<Separator>();
        AssertRole(cut.Find("div"), "none");
    }

    [Fact]
    public void Decorative_Does_Not_Have_Aria_Orientation()
    {
        var cut = Render<Separator>();
        Assert.Null(cut.Find("div").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void NonDecorative_Has_Role_Separator()
    {
        var cut = Render<Separator>(p => p.Add(x => x.Decorative, false));
        AssertRole(cut.Find("div"), "separator");
    }

    [Fact]
    public void NonDecorative_Horizontal_Has_Aria_Orientation_Horizontal()
    {
        var cut = Render<Separator>(p => p.Add(x => x.Decorative, false));
        Assert.Equal("horizontal", cut.Find("div").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void NonDecorative_Vertical_Has_Aria_Orientation_Vertical()
    {
        var cut = Render<Separator>(p =>
        {
            p.Add(x => x.Decorative, false);
            p.Add(x => x.Orientation, SeparatorOrientation.Vertical);
        });
        Assert.Equal("vertical", cut.Find("div").GetAttribute("aria-orientation"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Classes()
    {
        var cut = Render<Separator>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("shrink-0", classes);
        Assert.Contains("bg-border", classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Separator>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Separator>(p =>
            p.AddUnmatched("data-testid", "divider"));

        Assert.Equal("divider", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Separator_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Separator).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
