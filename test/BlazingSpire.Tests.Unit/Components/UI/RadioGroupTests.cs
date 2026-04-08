using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class RadioGroupTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Radiogroup_Role()
    {
        var cut = Render<RadioGroup>();
        var div = cut.Find("div");
        AssertRole(div, "radiogroup");
    }

    [Fact]
    public void Has_Base_Grid_Classes()
    {
        var cut = Render<RadioGroup>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("grid", classes);
        Assert.Contains("gap-2", classes);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<RadioGroup>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("div").ClassName);
    }

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent("<span id=\"child\">content</span>"));
        Assert.NotNull(cut.Find("span#child"));
    }

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddUnmatched("data-testid", "my-group"));
        Assert.Equal("my-group", cut.Find("div").GetAttribute("data-testid"));
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

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void RadioGroup_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(RadioGroup).IsAssignableTo(typeof(BlazingSpireComponentBase)));
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
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "option-a");
            });
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "option-b");
            });
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "option-c");
                ip.Add(x => x.Disabled, true);
            });
        });
    }

    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_Radio_Role()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        var btn = cut.Find("button");
        AssertRole(btn, "radio");
    }

    [Fact]
    public void Default_AriaChecked_Is_False()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Selected_Item_Has_AriaChecked_True()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Selected_Item_Shows_Circle_SVG()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.NotNull(cut.Find("button svg"));
    }

    [Fact]
    public void Unselected_Item_Has_No_Circle_SVG()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "b");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Empty(cut.FindAll("button svg"));
    }

    // ── Interaction ───────────────────────────────────────────────────────────

    [Fact]
    public void Clicking_Item_Selects_It()
    {
        var cut = RenderGroup();
        var buttons = cut.FindAll("button");

        buttons[0].Click();

        Assert.Equal("true", cut.FindAll("button")[0].GetAttribute("aria-checked"));
        Assert.Equal("false", cut.FindAll("button")[1].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Clicking_Different_Item_Deselects_Previous()
    {
        var cut = RenderGroup("option-a");
        var buttons = cut.FindAll("button");

        buttons[1].Click();

        Assert.Equal("false", cut.FindAll("button")[0].GetAttribute("aria-checked"));
        Assert.Equal("true", cut.FindAll("button")[1].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Clicking_Disabled_Item_Does_Not_Select()
    {
        var cut = RenderGroup();
        var buttons = cut.FindAll("button");

        // Third button is disabled
        buttons[2].Click();

        Assert.Equal("false", cut.FindAll("button")[2].GetAttribute("aria-checked"));
        Assert.Equal("unchecked", cut.FindAll("button")[2].GetAttribute("data-state"));
    }

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

    // ── DataState ─────────────────────────────────────────────────────────────

    [Fact]
    public void Selected_Item_Has_DataState_Checked()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("checked", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Unselected_Item_Has_DataState_Unchecked()
    {
        var cut = Render<RadioGroup>(p =>
        {
            p.Add(x => x.Value, "b");
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("unchecked", cut.Find("button").GetAttribute("data-state"));
    }

    // ── Base classes ──────────────────────────────────────────────────────────

    [Fact]
    public void Has_Base_Layout_Classes()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        var classes = cut.Find("button").ClassName;
        Assert.Contains("aspect-square", classes);
        Assert.Contains("h-4", classes);
        Assert.Contains("w-4", classes);
        Assert.Contains("rounded-full", classes);
        Assert.Contains("border-primary", classes);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<RadioGroup>(p =>
            p.AddChildContent<RadioGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "a");
                ip.Add(x => x.Class, "extra-class");
            }));
        Assert.Contains("extra-class", cut.Find("button").ClassName);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void RadioGroupItem_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(RadioGroupItem).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
