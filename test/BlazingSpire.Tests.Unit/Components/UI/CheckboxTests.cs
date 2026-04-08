using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CheckboxTests : BlazingSpireTestBase
{
    // ── Rendering ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Role_Checkbox()
    {
        var cut = Render<Checkbox>();
        var btn = cut.Find("button");
        Assert.Equal("checkbox", btn.GetAttribute("role"));
    }

    // ── Default state ─────────────────────────────────────────────────────────

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<Checkbox>();
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Default_DataState_Is_Unchecked()
    {
        var cut = Render<Checkbox>();
        Assert.Equal("unchecked", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Default_No_SVG_Rendered()
    {
        var cut = Render<Checkbox>();
        Assert.Empty(cut.FindAll("svg"));
    }

    // ── Checked state ─────────────────────────────────────────────────────────

    [Fact]
    public void Checked_True_Sets_AriaChecked_True()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Checked_True_Sets_DataState_Checked()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        Assert.Equal("checked", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Checked_True_Renders_SVG()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        Assert.Single(cut.FindAll("svg"));
    }

    // ── Toggle ────────────────────────────────────────────────────────────────

    [Fact]
    public void Click_Toggles_To_Checked()
    {
        var cut = Render<Checkbox>();
        cut.Find("button").Click();

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-checked"));
        Assert.Equal("checked", cut.Find("button").GetAttribute("data-state"));
        Assert.Single(cut.FindAll("svg"));
    }

    [Fact]
    public void Click_Toggles_Back_To_Unchecked()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Checked, true));
        cut.Find("button").Click();

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
        Assert.Equal("unchecked", cut.Find("button").GetAttribute("data-state"));
        Assert.Empty(cut.FindAll("svg"));
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
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
    }

    // ── Base classes ──────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Size_Classes()
    {
        var cut = Render<Checkbox>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("h-4", classes);
        Assert.Contains("w-4", classes);
        Assert.Contains("shrink-0", classes);
    }

    [Fact]
    public void Always_Has_Border_Classes()
    {
        var cut = Render<Checkbox>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("border", classes);
        Assert.Contains("border-primary", classes);
        Assert.Contains("rounded-sm", classes);
    }

    [Fact]
    public void Always_Has_Focus_Ring_Classes()
    {
        var cut = Render<Checkbox>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("focus-visible:ring-2", classes);
        Assert.Contains("focus-visible:ring-ring", classes);
    }

    [Fact]
    public void Always_Has_Disabled_State_Classes()
    {
        var cut = Render<Checkbox>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("disabled:cursor-not-allowed", classes);
        Assert.Contains("disabled:opacity-50", classes);
    }

    // ── Class parameter ───────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("button").ClassName);
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Checkbox>(p => p.AddUnmatched("aria-label", "Accept terms"));
        AssertAriaLabel(cut.Find("button"), "Accept terms");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Checkbox>(p => p.AddUnmatched("data-testid", "terms-checkbox"));
        Assert.Equal("terms-checkbox", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Checkbox_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Checkbox).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
