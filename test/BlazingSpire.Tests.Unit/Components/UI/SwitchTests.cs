using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SwitchTests : BlazingSpireTestBase
{
    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Role_Switch()
    {
        var cut = Render<Switch>();
        AssertRole(cut.Find("button"), "switch");
    }

    // ── Default state ─────────────────────────────────────────────────────────

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<Switch>();
        AssertAriaChecked(cut.Find("button"), false);
    }

    [Fact]
    public void Default_DataState_Is_Unchecked()
    {
        var cut = Render<Switch>();
        AssertDataState(cut.Find("button"), "unchecked");
    }

    [Fact]
    public void Thumb_DataState_Matches_Unchecked_Default()
    {
        var cut = Render<Switch>();
        AssertDataState(cut.Find("button span"), "unchecked");
    }

    // ── Toggle ───────────────────────────────────────────────────────────────

    [Fact]
    public void Clicking_Toggles_To_Checked()
    {
        var cut = Render<Switch>();
        cut.Find("button").Click();

        AssertAriaChecked(cut.Find("button"), true);
        AssertDataState(cut.Find("button"), "checked");
        AssertDataState(cut.Find("button span"), "checked");
    }

    [Fact]
    public void Clicking_Twice_Toggles_Back_To_Unchecked()
    {
        var cut = Render<Switch>();
        cut.Find("button").Click();
        cut.Find("button").Click();

        AssertAriaChecked(cut.Find("button"), false);
        AssertDataState(cut.Find("button"), "unchecked");
    }

    [Fact]
    public void Toggle_Invokes_CheckedChanged()
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
        AssertAriaChecked(cut.Find("button"), true);
        AssertDataState(cut.Find("button"), "checked");
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

        AssertAriaChecked(cut.Find("button"), false);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Switch).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<Switch>(p => p.AddUnmatched("data-testid", "my-switch"));
        Assert.Equal("my-switch", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── CheckedChanged value on toggle off ────────────────────────────────────

    [Fact]
    public void Toggle_Off_Invokes_CheckedChanged_With_False()
    {
        bool? received = null;
        var cut = Render<Switch>(p => p
            .Add(x => x.Checked, true)
            .Add(x => x.CheckedChanged, (bool v) => { received = v; }));

        cut.Find("button").Click();

        Assert.False(received);
    }

    // ── Disabled does not fire callback ───────────────────────────────────────

    [Fact]
    public void Disabled_Does_Not_Invoke_CheckedChanged()
    {
        bool invoked = false;
        var cut = Render<Switch>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.CheckedChanged, (bool _) => { invoked = true; }));

        cut.Find("button").Click();

        Assert.False(invoked);
    }
}
