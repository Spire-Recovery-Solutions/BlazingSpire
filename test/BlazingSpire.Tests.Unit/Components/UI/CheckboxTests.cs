using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CheckboxTests : BlazingSpireTestBase
{
    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Role_Checkbox()
    {
        var cut = Render<Checkbox>();
        AssertRole(cut.Find("button"), "checkbox");
    }

    // ── Default state ─────────────────────────────────────────────────────────

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<Checkbox>();
        AssertAriaChecked(cut.Find("button"), false);
    }

    [Fact]
    public void Default_DataState_Is_Unchecked()
    {
        var cut = Render<Checkbox>();
        AssertDataState(cut.Find("button"), "unchecked");
    }

    // ── Checked state ─────────────────────────────────────────────────────────

    [Fact]
    public void Checked_True_Sets_AriaChecked_True()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        AssertAriaChecked(cut.Find("button"), true);
    }

    [Fact]
    public void Checked_True_Sets_DataState_Checked()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        AssertDataState(cut.Find("button"), "checked");
    }

    // ── Toggle ────────────────────────────────────────────────────────────────

    [Fact]
    public void Click_Toggles_To_Checked()
    {
        var cut = Render<Checkbox>();
        cut.Find("button").Click();

        AssertAriaChecked(cut.Find("button"), true);
        AssertDataState(cut.Find("button"), "checked");
    }

    [Fact]
    public void Click_Toggles_Back_To_Unchecked()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        cut.Find("button").Click();

        AssertAriaChecked(cut.Find("button"), false);
        AssertDataState(cut.Find("button"), "unchecked");
    }

    [Fact]
    public void Click_Invokes_CheckedChanged_With_True()
    {
        bool? raised = null;
        var cut = Render<Checkbox>(p => p.Add(x => x.CheckedChanged, (bool v) => raised = v));
        cut.Find("button").Click();
        Assert.True(raised);
    }

    [Fact]
    public void Click_Invokes_CheckedChanged_With_False_When_Checked()
    {
        bool? raised = null;
        var cut = Render<Checkbox>(p =>
        {
            p.Add(x => x.Checked, true);
            p.Add(x => x.CheckedChanged, (bool v) => raised = v);
        });
        cut.Find("button").Click();
        Assert.False(raised);
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_True_Renders_Disabled_Attribute()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_False_Does_Not_Render_Disabled_Attribute()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Disabled, false));
        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_Prevents_Toggle()
    {
        bool? raised = null;
        var cut = Render<Checkbox>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.CheckedChanged, (bool v) => raised = v);
        });
        cut.Find("button").Click();
        Assert.Null(raised);
        AssertAriaChecked(cut.Find("button"), false);
    }

    // ── ARIA ──────────────────────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Checkbox>(p => p.AddUnmatched("aria-label", "Accept terms"));
        AssertAriaLabel(cut.Find("button"), "Accept terms");
    }
}
