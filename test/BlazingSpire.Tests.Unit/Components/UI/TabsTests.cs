using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TabsTests : BlazingSpireTestBase
{
    // Note: Tabs uses ActiveValue/ActiveValueChanged (not DefaultValue/Value/ValueChanged).

    private IRenderedComponent<Tabs> RenderTabs(string activeValue = "tab1")
    {
        return Render<Tabs>(p =>
        {
            p.Add(x => x.ActiveValue, activeValue);
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<TabsTrigger>(0);
                    b.AddAttribute(1, "ItemValue", "tab1");
                    b.AddAttribute(2, "ChildContent",
                        (RenderFragment)(c => c.AddContent(0, "Tab 1")));
                    b.CloseComponent();

                    b.OpenComponent<TabsTrigger>(3);
                    b.AddAttribute(4, "ItemValue", "tab2");
                    b.AddAttribute(5, "ChildContent",
                        (RenderFragment)(c => c.AddContent(0, "Tab 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(2);
                builder.AddAttribute(3, "ItemValue", "tab1");
                builder.AddAttribute(4, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Panel 1")));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(5);
                builder.AddAttribute(6, "ItemValue", "tab2");
                builder.AddAttribute(7, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Panel 2")));
                builder.CloseComponent();
            });
        });
    }

    // ── ARIA roles ────────────────────────────────────────────────────────────

    [Fact]
    public void TabsList_Has_Role_Tablist()
    {
        var cut = RenderTabs();
        AssertRole(cut.Find("[role=tablist]"), "tablist");
    }

    [Fact]
    public void Triggers_Have_Role_Tab()
    {
        var cut = RenderTabs();
        var triggers = cut.FindAll("[role=tab]");
        Assert.Equal(2, triggers.Count);
    }

    // ── Active state ──────────────────────────────────────────────────────────

    [Fact]
    public void Active_Trigger_Has_AriaSelected_True()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(triggers[0], true);
    }

    [Fact]
    public void Inactive_Trigger_Has_AriaSelected_False()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(triggers[1], false);
    }

    [Fact]
    public void Active_Trigger_Has_DataState_Active()
    {
        var cut = RenderTabs("tab1");
        AssertDataState(cut.FindAll("[role=tab]")[0], "active");
    }

    [Fact]
    public void Inactive_Trigger_Has_DataState_Inactive()
    {
        var cut = RenderTabs("tab1");
        AssertDataState(cut.FindAll("[role=tab]")[1], "inactive");
    }

    [Fact]
    public void Active_Panel_Has_Role_Tabpanel_And_DataState_Active()
    {
        var cut = RenderTabs("tab1");
        var panel = cut.Find("[role=tabpanel]");
        AssertDataState(panel, "active");
    }

    [Fact]
    public void Inactive_Panel_Is_Not_Rendered()
    {
        var cut = Render<Tabs>(p =>
        {
            p.Add(x => x.ActiveValue, "other");
            p.AddChildContent<TabsContent>(b =>
            {
                b.Add(x => x.ItemValue, "tab1");
                b.AddChildContent("Panel 1");
            });
        });

        Assert.Empty(cut.FindAll("[role=tabpanel]"));
    }

    // ── Click behavior ────────────────────────────────────────────────────────

    [Fact]
    public void Click_Trigger_Switches_Active_Tab()
    {
        var cut = RenderTabs("tab1");
        cut.FindAll("[role=tab]")[1].Click();

        var triggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(triggers[0], false);
        AssertAriaSelected(triggers[1], true);
    }

    [Fact]
    public void Click_Trigger_Shows_Correct_Panel()
    {
        var cut = RenderTabs("tab1");
        cut.FindAll("[role=tab]")[1].Click();

        Assert.Contains("Panel 2", cut.Find("[role=tabpanel]").TextContent);
    }

    [Fact]
    public void Disabled_Trigger_Does_Not_Switch_Active_Tab()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "tab1")
            .AddChildContent<TabsTrigger>(b => b
                .Add(x => x.ItemValue, "tab2")
                .Add(x => x.Disabled, true)
                .AddChildContent("Tab 2")));

        cut.Find("[role=tab]").Click();

        Assert.Equal("tab1", cut.Instance.ActiveValue);
    }

    // ── ActiveValueChanged ────────────────────────────────────────────────────

    [Fact]
    public void ActiveValueChanged_Fires_With_New_Value_On_Tab_Click()
    {
        string? received = null;
        var cut = Render<Tabs>(p =>
        {
            p.Add(x => x.ActiveValue, "tab1");
            p.Add(x => x.ActiveValueChanged,
                EventCallback.Factory.Create<string>(this, v => received = v));
            p.AddChildContent<TabsList>(tl =>
                tl.AddChildContent<TabsTrigger>(t =>
                {
                    t.Add(x => x.ItemValue, "tab2");
                    t.AddChildContent("Tab 2");
                }));
        });

        cut.Find("[role=tab]").Click();
        Assert.Equal("tab2", received);
    }

    // ── Roving tabindex ───────────────────────────────────────────────────────

    [Fact]
    public void Active_Trigger_Has_Tabindex_Zero()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");
        Assert.Equal("0", triggers[0].GetAttribute("tabindex"));
    }

    [Fact]
    public void Inactive_Trigger_Has_Tabindex_Minus_One()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");
        Assert.Equal("-1", triggers[1].GetAttribute("tabindex"));
    }

    [Fact]
    public void ArrowRight_On_First_Tab_Activates_Second_Tab()
    {
        var cut = RenderTabs("tab1");
        cut.FindAll("[role=tab]")[0].TriggerEvent("onkeydown",
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });

        var triggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(triggers[0], false);
        AssertAriaSelected(triggers[1], true);
    }

    // ── TabsTrigger role ──────────────────────────────────────────────────────

    [Fact]
    public void TabsTrigger_Renders_Button_With_Tab_Role()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsTrigger>(b => b
                .Add(x => x.ItemValue, "account")
                .AddChildContent("Account")));
        AssertRole(cut.Find("button"), "tab");
    }
}
