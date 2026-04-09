using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ToggleGroupTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Div_With_Group_Role()
    {
        var cut = Render<ToggleGroup>();
        AssertRole(cut.Find("div"), "group");
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

    // ── ARIA state ────────────────────────────────────────────────────────────

    [Fact]
    public void Default_AriaPressed_Is_False()
    {
        var cut = Render<ToggleGroup>(p =>
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a")));
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Selected_Item_Has_AriaPressed_True_And_DataState_On()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Value, "a");
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
        AssertDataState(cut.Find("button"), "on");
    }

    [Fact]
    public void Unselected_Item_Has_DataState_Off()
    {
        var cut = Render<ToggleGroup>(p =>
        {
            p.Add(x => x.Value, "b");
            p.AddChildContent<ToggleGroupItem>(ip => ip.Add(x => x.ItemValue, "a"));
        });
        AssertDataState(cut.Find("button"), "off");
    }

    // ── Single mode ───────────────────────────────────────────────────────────

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
    public void Single_ValueChanged_Invoked_With_Correct_Value()
    {
        string? received = null;
        var cut = RenderSingleGroup(configure: p =>
            p.Add(x => x.ValueChanged, (string? v) => received = v));

        cut.FindAll("button")[0].Click();

        Assert.Equal("bold", received);
    }

    // ── Multiple mode ─────────────────────────────────────────────────────────

    [Fact]
    public void Multiple_Multiple_Items_Can_Be_Active_Simultaneously()
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
    public void Multiple_ValuesChanged_Invoked_With_New_Set()
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
    public void Clicking_Disabled_Item_Does_Not_Select_It()
    {
        var cut = RenderSingleGroup();
        // Third button is disabled
        cut.FindAll("button")[2].Click();
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
}
