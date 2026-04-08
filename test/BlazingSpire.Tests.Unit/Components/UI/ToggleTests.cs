using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ToggleTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_Element()
    {
        var cut = Render<Toggle>();
        Assert.NotNull(cut.Find("button"));
    }

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
        Assert.Equal("off", cut.Find("button").GetAttribute("data-state"));
    }

    // ── Toggle interaction ────────────────────────────────────────────────────

    [Fact]
    public void Clicking_Toggles_Pressed_To_True()
    {
        var cut = Render<Toggle>();
        cut.Find("button").Click();
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
        Assert.Equal("on", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Clicking_Twice_Returns_To_Off()
    {
        var cut = Render<Toggle>();
        cut.Find("button").Click();
        cut.Find("button").Click();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
        Assert.Equal("off", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void PressedChanged_Fires_With_New_Value()
    {
        bool? received = null;
        var cut = Render<Toggle>(p => p.Add(x => x.PressedChanged, (bool v) => received = v));
        cut.Find("button").Click();
        Assert.True(received);
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Toggle_Does_Not_Change_State()
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Disabled, true));
        cut.Find("button").Click();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
        Assert.Equal("off", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Disabled_Attribute_Is_Set_On_Button()
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut.Find("button").GetAttribute("disabled"));
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ToggleVariant.Default, "bg-transparent")]
    [InlineData(ToggleVariant.Outline, "border-input")]
    public void Variant_Produces_Correct_Classes(ToggleVariant variant, string expectedClass)
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Variant, variant));
        Assert.Contains(expectedClass, cut.Find("button").ClassName);
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Layout_Classes()
    {
        var cut = Render<Toggle>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-medium", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("px-3", classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Toggle>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("button").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Toggle>(p => p.AddUnmatched("aria-label", "Format bold"));
        AssertAriaLabel(cut.Find("button"), "Format bold");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Toggle>(p => p.AddUnmatched("data-testid", "bold-toggle"));
        Assert.Equal("bold-toggle", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Button()
    {
        var cut = Render<Toggle>(p => p.AddChildContent("<span>B</span>"));
        Assert.NotNull(cut.Find("button span"));
        Assert.Equal("B", cut.Find("button span").TextContent);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Toggle_Is_Assignable_To_PresentationalBase()
    {
        Assert.True(typeof(Toggle).IsAssignableTo(typeof(PresentationalBase<ToggleVariant>)));
    }

    [Fact]
    public void Toggle_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Toggle).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
