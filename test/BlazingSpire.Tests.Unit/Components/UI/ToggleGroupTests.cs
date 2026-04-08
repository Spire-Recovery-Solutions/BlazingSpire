using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ToggleGroupTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Group_Role()
    {
        var cut = Render<ToggleGroup>();
        var div = cut.Find("div");
        AssertRole(div, "group");
    }

    [Fact]
    public void Has_Base_Flex_Classes()
    {
        var cut = Render<ToggleGroup>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("gap-1", classes);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<ToggleGroup>(p => p.Add(x => x.Class, "my-class"));
        Assert.Contains("my-class", cut.Find("div").ClassName);
    }

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent("<span id=\"child\">content</span>"));
        Assert.NotNull(cut.Find("span#child"));
    }

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddUnmatched("data-testid", "my-group"));
        Assert.Equal("my-group", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void ToggleGroup_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(ToggleGroup).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}

public class ToggleGroupItemTests : BlazingSpireTestBase
{
    private IRenderedComponent<ToggleGroup> RenderSingleGroup(
        string? value = null,
        Action<ComponentParameterCollectionBuilder<ToggleGroup>>? configure = null)
    {
        return Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Type, ToggleGroupType.Single);
            p.Add(x => x.Value, value);
            configure?.Invoke(p);
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "bold"));
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "italic"));
            p.AddChildContent<ToggleGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "underline");
                ip.Add(x => x.Disabled, true);
            });
        });
    }

    private IRenderedComponent<ToggleGroup> RenderMultipleGroup(
        HashSet<string>? values = null)
    {
        return Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Type, ToggleGroupType.Multiple);
            p.Add(x => x.Values, values ?? []);
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "bold"));
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "italic"));
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "underline"));
        });
    }

    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_With_AriaPressed()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        var btn = cut.Find("button");
        Assert.NotNull(btn.GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Default_AriaPressed_Is_False()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Selected_Item_Has_AriaPressed_True()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Selected_Item_Has_DataState_On()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("on", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Unselected_Item_Has_DataState_Off()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Value, "b");
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("off", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void Has_Base_Classes()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("h-10", classes);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "a");
                ip.Add(x => x.Class, "extra-class");
            }));
        Assert.Contains("extra-class", cut.Find("button").ClassName);
    }

    // ── Single Mode ───────────────────────────────────────────────────────────

    [Fact]
    public void Single_Clicking_Item_Selects_It()
    {
        var cut = RenderSingleGroup();
        cut.FindAll("button")[0].Click();

        Assert.Equal("true", cut.FindAll("button")[0].GetAttribute("aria-pressed"));
        Assert.Equal("false", cut.FindAll("button")[1].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Single_Clicking_Selected_Item_Deselects_It()
    {
        var cut = RenderSingleGroup("bold");
        cut.FindAll("button")[0].Click();

        Assert.Equal("false", cut.FindAll("button")[0].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Single_Only_One_Item_Selected_At_A_Time()
    {
        var cut = RenderSingleGroup("bold");
        cut.FindAll("button")[1].Click();

        Assert.Equal("false", cut.FindAll("button")[0].GetAttribute("aria-pressed"));
        Assert.Equal("true", cut.FindAll("button")[1].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Single_ValueChanged_Invoked_On_Toggle()
    {
        string? received = null;
        var cut = RenderSingleGroup(configure: p =>
            p.Add(x => x.ValueChanged, (string? v) => received = v));

        cut.FindAll("button")[0].Click();

        Assert.Equal("bold", received);
    }

    // ── Multiple Mode ─────────────────────────────────────────────────────────

    [Fact]
    public void Multiple_Multiple_Items_Can_Be_Active()
    {
        var cut = RenderMultipleGroup();
        var buttons = cut.FindAll("button");

        buttons[0].Click();
        buttons[1].Click();

        Assert.Equal("true", cut.FindAll("button")[0].GetAttribute("aria-pressed"));
        Assert.Equal("true", cut.FindAll("button")[1].GetAttribute("aria-pressed"));
        Assert.Equal("false", cut.FindAll("button")[2].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Multiple_Clicking_Active_Item_Deactivates_It()
    {
        var cut = RenderMultipleGroup(["bold", "italic"]);
        cut.FindAll("button")[0].Click();

        Assert.Equal("false", cut.FindAll("button")[0].GetAttribute("aria-pressed"));
        Assert.Equal("true", cut.FindAll("button")[1].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Multiple_ValuesChanged_Invoked_On_Toggle()
    {
        HashSet<string>? received = null;
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Type, ToggleGroupType.Multiple);
            p.Add(x => x.Values, []);
            p.Add(x => x.ValuesChanged, (HashSet<string> v) => received = v);
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "bold"));
        });

        cut.Find("button").Click();

        Assert.NotNull(received);
        Assert.Contains("bold", received);
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Item_Has_Disabled_Attribute()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip =>
            {
                ip.Add(x => x.ItemValue, "a");
                ip.Add(x => x.Disabled, true);
            }));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Clicking_Disabled_Item_Does_Not_Select()
    {
        var cut = RenderSingleGroup();
        var buttons = cut.FindAll("button");

        // Third button is disabled
        buttons[2].Click();

        Assert.Equal("false", cut.FindAll("button")[2].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Disabled_Group_Disables_All_Items()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "b"));
        });

        var buttons = cut.FindAll("button");
        Assert.True(buttons[0].HasAttribute("disabled"));
        Assert.True(buttons[1].HasAttribute("disabled"));
    }

    [Fact]
    public void Clicking_Item_In_Disabled_Group_Does_Not_Select()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });

        cut.Find("button").Click();

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void ToggleGroupItem_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(ToggleGroupItem).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
