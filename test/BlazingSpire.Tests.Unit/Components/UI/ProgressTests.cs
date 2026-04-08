using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ProgressTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Role_Progressbar()
    {
        var cut = Render<Progress>();
        AssertRole(cut.Find("div[role]"), "progressbar");
    }

    [Fact]
    public void Renders_Indicator_Div()
    {
        var cut = Render<Progress>();
        var outer = cut.Find("[role='progressbar']");
        Assert.NotNull(outer.QuerySelector("div"));
    }

    // ── ARIA attributes ───────────────────────────────────────────────────────

    [Fact]
    public void AriaValueMin_Is_Zero()
    {
        var cut = Render<Progress>();
        Assert.Equal("0", cut.Find("[role='progressbar']").GetAttribute("aria-valuemin"));
    }

    [Fact]
    public void AriaValueMax_Is_OneHundred()
    {
        var cut = Render<Progress>();
        Assert.Equal("100", cut.Find("[role='progressbar']").GetAttribute("aria-valuemax"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void AriaValueNow_Matches_Value(int value)
    {
        var cut = Render<Progress>(p => p.Add(x => x.Value, value));
        Assert.Equal(value.ToString(), cut.Find("[role='progressbar']").GetAttribute("aria-valuenow"));
    }

    // ── Indicator style ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(0, "translateX(-100%)")]
    [InlineData(50, "translateX(-50%)")]
    [InlineData(100, "translateX(-0%)")]
    public void Indicator_TranslateX_Reflects_Value(int value, string expectedTransform)
    {
        var cut = Render<Progress>(p => p.Add(x => x.Value, value));
        var indicator = cut.Find("[role='progressbar'] div");
        var style = indicator.GetAttribute("style");
        Assert.Contains(expectedTransform, style);
    }

    [Fact]
    public void Indicator_Has_BgPrimary_Class()
    {
        var cut = Render<Progress>();
        var indicator = cut.Find("[role='progressbar'] div");
        Assert.Contains("bg-primary", indicator.ClassName);
    }

    // ── Base classes ──────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Classes()
    {
        var cut = Render<Progress>();
        var classes = cut.Find("[role='progressbar']").ClassName;
        Assert.Contains("relative", classes);
        Assert.Contains("h-4", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("overflow-hidden", classes);
        Assert.Contains("rounded-full", classes);
        Assert.Contains("bg-secondary", classes);
    }

    // ── Class parameter ───────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Progress>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("[role='progressbar']").ClassName);
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Progress>(p =>
            p.AddUnmatched("aria-label", "Loading progress"));

        AssertAriaLabel(cut.Find("[role='progressbar']"), "Loading progress");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Progress>(p =>
            p.AddUnmatched("data-testid", "upload-progress"));

        Assert.Equal("upload-progress", cut.Find("[role='progressbar']").GetAttribute("data-testid"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Progress_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Progress).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
