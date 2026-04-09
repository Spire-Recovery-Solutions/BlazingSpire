using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ProgressTests : BlazingSpireTestBase
{
    // ── Base class ────────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Progress).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── ARIA role and attributes ──────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Role_Progressbar()
    {
        var cut = Render<Progress>();
        AssertRole(cut.Find("div[role]"), "progressbar");
    }

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

    // ── Structure ─────────────────────────────────────────────────────────────

    [Fact]
    public void Indicator_Div_Exists_Inside_Progressbar()
    {
        var cut = Render<Progress>();
        Assert.NotNull(cut.Find("[role='progressbar'] div"));
    }

    // ── Indicator transform reflects value ────────────────────────────────────

    [Theory]
    [InlineData(0, "translateX(-100%)")]
    [InlineData(50, "translateX(-50%)")]
    [InlineData(100, "translateX(-0%)")]
    public void Indicator_TranslateX_Reflects_Value(int value, string expectedTransform)
    {
        var cut = Render<Progress>(p => p.Add(x => x.Value, value));
        var indicator = cut.Find("[role='progressbar'] div");
        Assert.Contains(expectedTransform, indicator.GetAttribute("style"));
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Progress>(p => p.Add(x => x.Class, "my-progress"));
        Assert.Contains("my-progress", cut.Find("[role='progressbar']").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Progress>(p => p.AddUnmatched("aria-label", "Loading progress"));
        AssertAriaLabel(cut.Find("[role='progressbar']"), "Loading progress");
    }

    [Fact]
    public void DataTestId_PassesThrough()
    {
        var cut = Render<Progress>(p => p.AddUnmatched("data-testid", "upload-progress"));
        Assert.Equal("upload-progress", cut.Find("[role='progressbar']").GetAttribute("data-testid"));
    }

    [Fact]
    public void AriaDescribedBy_PassesThrough()
    {
        var cut = Render<Progress>(p => p.AddUnmatched("aria-describedby", "progress-desc"));
        Assert.Equal("progress-desc", cut.Find("[role='progressbar']").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Multiple_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Progress>(p => p
            .AddUnmatched("aria-label", "File upload")
            .AddUnmatched("data-testid", "file-progress"));
        AssertAriaLabel(cut.Find("[role='progressbar']"), "File upload");
        Assert.Equal("file-progress", cut.Find("[role='progressbar']").GetAttribute("data-testid"));
    }
}
