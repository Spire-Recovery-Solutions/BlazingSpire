using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ToggleTests : BlazingSpireTestBase
{
    // ── ARIA / data-state defaults ────────────────────────────────────────────

    [Fact]
    public void Default_AriaPressed_Is_False()
    {
        var cut = Render<Toggle>();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Default_DataState_Is_Off()
    {
        var cut = Render<Toggle>();
        AssertDataState(cut.Find("button"), "off");
    }

    // ── Toggle interaction ────────────────────────────────────────────────────

    [Fact]
    public void Click_Sets_AriaPressed_True_And_DataState_On()
    {
        var cut = Render<Toggle>();
        cut.Find("button").Click();
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
        AssertDataState(cut.Find("button"), "on");
    }

    [Fact]
    public void Second_Click_Returns_To_AriaPressed_False_And_DataState_Off()
    {
        var cut = Render<Toggle>();
        cut.Find("button").Click();
        cut.Find("button").Click();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
        AssertDataState(cut.Find("button"), "off");
    }

    [Fact]
    public void PressedChanged_Fires_With_True_On_First_Click()
    {
        bool? received = null;
        var cut = Render<Toggle>(p => p.Add(x => x.PressedChanged, (bool v) => received = v));
        cut.Find("button").Click();
        Assert.True(received);
    }

    [Fact]
    public void PressedChanged_Fires_With_False_On_Second_Click()
    {
        bool? received = null;
        var cut = Render<Toggle>(p => p.Add(x => x.PressedChanged, (bool v) => received = v));
        cut.Find("button").Click();
        cut.Find("button").Click();
        Assert.False(received);
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Has_Disabled_Attribute_On_Button()
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut.Find("button").GetAttribute("disabled"));
    }

    [Fact]
    public void Disabled_Click_Does_Not_Change_State()
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Disabled, true));
        cut.Find("button").Click();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
        AssertDataState(cut.Find("button"), "off");
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ToggleVariant.Default)]
    [InlineData(ToggleVariant.Outline)]
    public void Each_Variant_Renders_Without_Error(ToggleVariant variant)
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Variant, variant));
        Assert.NotNull(cut.Find("button"));
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Toggle>(p => p.AddUnmatched("aria-label", "Format bold"));
        AssertAriaLabel(cut.Find("button"), "Format bold");
    }

    // ── ChildContent ──────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Button()
    {
        var cut = Render<Toggle>(p => p.AddChildContent("<span>B</span>"));
        Assert.NotNull(cut.Find("button span"));
        Assert.Equal("B", cut.Find("button span").TextContent);
    }
}
