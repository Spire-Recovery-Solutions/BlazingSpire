using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SeparatorTests : BlazingSpireTestBase
{
    // ── ARIA role: decorative ─────────────────────────────────────────────────

    [Fact]
    public void Default_Decorative_Has_Role_None()
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

    // ── ARIA role: non-decorative ─────────────────────────────────────────────

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

    // ── Orientation renders without error ─────────────────────────────────────

    [Theory]
    [InlineData(SeparatorOrientation.Horizontal)]
    [InlineData(SeparatorOrientation.Vertical)]
    public void Each_Orientation_Renders_Without_Error(SeparatorOrientation orientation)
    {
        var cut = Render<Separator>(p => p.Add(x => x.Orientation, orientation));
        Assert.NotNull(cut.Find("div"));
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Separator>(p => p.Add(x => x.Class, "my-separator"));
        Assert.Contains("my-separator", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void DataTestId_PassesThrough()
    {
        var cut = Render<Separator>(p => p.AddUnmatched("data-testid", "divider"));
        Assert.Equal("divider", cut.Find("div").GetAttribute("data-testid"));
    }
}
