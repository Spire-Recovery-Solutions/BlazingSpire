using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class RadioGroupTests : BlazingSpireTestBase
{
    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Radiogroup_Role()
    {
        var cut = Render<RadioGroup>();
        AssertRole(cut.Find("div"), "radiogroup");
    }

    // ── Selection ─────────────────────────────────────────────────────────────

    [Fact]
    public void SelectAsync_Updates_Value_And_Invokes_Callback()
    {
        string? received = null;
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.Add(x => x.ValueChanged, (string v) => received = v);
        });

        cut.InvokeAsync(() => cut.Instance.SelectAsync("b"));

        Assert.Equal("b", cut.Instance.Value);
        Assert.Equal("b", received);
    }
}

public class RadioGroupItemTests : BlazingSpireTestBase
{
    private IRenderedComponent<RadioGroup> RenderGroup(string? value = null, Action<ComponentParameterCollectionBuilder<RadioGroup>>? configure = null)
    {
        return Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, value);
            configure?.Invoke(p);
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "option-a"));
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "option-b"));
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "option-c");
                ip.Add(x => x.Disabled, true);
            });
        });
    }

    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Radio_Role()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        AssertRole(cut.Find("button"), "radio");
    }

    // ── ARIA checked state ────────────────────────────────────────────────────

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        AssertAriaChecked(cut.Find("button"), false);
    }

    [Fact]
    public void Selected_Item_Has_AriaChecked_True()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        AssertAriaChecked(cut.Find("button"), true);
    }

    // ── data-state ────────────────────────────────────────────────────────────

    [Fact]
    public void Selected_Item_Has_DataState_Checked()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        AssertDataState(cut.Find("button"), "checked");
    }

    [Fact]
    public void Unselected_Item_Has_DataState_Unchecked()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "b");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        AssertDataState(cut.Find("button"), "unchecked");
    }

    // ── Interaction ───────────────────────────────────────────────────────────

    [Fact]
    public void Clicking_Item_Selects_It()
    {
        var cut = RenderGroup();
        cut.FindAll("button")[0].Click();

        AssertAriaChecked(cut.FindAll("button")[0], true);
        AssertAriaChecked(cut.FindAll("button")[1], false);
    }

    [Fact]
    public void Clicking_Different_Item_Deselects_Previous()
    {
        var cut = RenderGroup("option-a");
        cut.FindAll("button")[1].Click();

        AssertAriaChecked(cut.FindAll("button")[0], false);
        AssertAriaChecked(cut.FindAll("button")[1], true);
    }

    [Fact]
    public void Clicking_Disabled_Item_Does_Not_Select()
    {
        var cut = RenderGroup();
        cut.FindAll("button")[2].Click();

        AssertAriaChecked(cut.FindAll("button")[2], false);
        AssertDataState(cut.FindAll("button")[2], "unchecked");
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Item_Has_Disabled_Attribute()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "a");
                ip.Add(x => x.Disabled, true);
            }));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }
}
