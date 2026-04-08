using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SwitchTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Role_Switch()
    {
        var cut = Render<Switch>();
        var btn = cut.Find("button");
        Assert.Equal("switch", btn.GetAttribute("role"));
    }

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<Switch>();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Default_DataState_Is_Unchecked()
    {
        var cut = Render<Switch>();
        Assert.Equal("unchecked", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Thumb_Span_Exists()
    {
        var cut = Render<Switch>();
        var span = cut.Find("button span");
        Assert.NotNull(span);
    }

    [Fact]
    public void Thumb_DataState_Matches_Unchecked_Default()
    {
        var cut = Render<Switch>();
        var span = cut.Find("button span");
        Assert.Equal("unchecked", span.GetAttribute("data-state"));
    }

    // ── Toggle ───────────────────────────────────────────────────────────────

    [Fact]
    public void Clicking_Toggles_To_Checked()
    {
        var cut = Render<Switch>();
        cut.Find("button").Click();

        var btn = cut.Find("button");
        Assert.Equal("true", btn.GetAttribute("aria-checked"));
        Assert.Equal("checked", btn.GetAttribute("data-state"));
        Assert.Equal("checked", cut.Find("button span").GetAttribute("data-state"));
    }

    [Fact]
    public void Clicking_Twice_Toggles_Back_To_Unchecked()
    {
        var cut = Render<Switch>();
        cut.Find("button").Click();
        cut.Find("button").Click();

        var btn = cut.Find("button");
        Assert.Equal("false", btn.GetAttribute("aria-checked"));
        Assert.Equal("unchecked", btn.GetAttribute("data-state"));
    }

    [Fact]
    public async Task Toggle_Invokes_CheckedChanged()
    {
        bool? received = null;
        var cut = Render<Switch>(p =>
            p.Add(x => x.CheckedChanged, (bool v) => { received = v; }));

        cut.Find("button").Click();

        Assert.True(received);
    }

    // ── Checked Parameter ────────────────────────────────────────────────────

    [Fact]
    public void Checked_True_Sets_AriaChecked_True()
    {
        var cut = Render<Switch>(p => p.Add(x => x.Checked, true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-checked"));
        Assert.Equal("checked", cut.Find("button").GetAttribute("data-state"));
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Renders_Disabled_Attribute()
    {
        var cut = Render<Switch>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_Prevents_Toggle()
    {
        var cut = Render<Switch>(p => p.Add(x => x.Disabled, true));
        cut.Find("button").Click();

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
    }

    // ── CSS ──────────────────────────────────────────────────────────────────

    [Fact]
    public void BaseClasses_Contain_Expected_Tokens()
    {
        var cut = Render<Switch>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("rounded-full", classes);
        Assert.Contains("data-[state=checked]:bg-primary", classes);
        Assert.Contains("data-[state=unchecked]:bg-input", classes);
    }

    [Fact]
    public void Custom_Class_Is_Applied()
    {
        var cut = Render<Switch>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("button").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AdditionalAttributes_Are_Spread_Onto_Button()
    {
        var cut = Render<Switch>(p => p.AddUnmatched("data-testid", "my-switch"));
        Assert.Equal("my-switch", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── Inheritance ──────────────────────────────────────────────────────────

    [Fact]
    public void Switch_Inherits_BlazingSpireComponentBase()
    {
        var cut = Render<Switch>();
        Assert.IsAssignableFrom<BlazingSpireComponentBase>(cut.Instance);
    }
}
