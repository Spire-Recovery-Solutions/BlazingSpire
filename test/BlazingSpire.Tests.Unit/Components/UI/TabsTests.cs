using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TabsTests : BlazingSpireTestBase
{
    // ── Tabs ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Tabs_Renders_Div()
    {
        var cut = Render<Tabs>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Tabs_Has_Width_Full_Class()
    {
        var cut = Render<Tabs>();
        Assert.Contains("w-full", cut.Find("div").ClassName);
    }

    [Fact]
    public void Tabs_Custom_Class_Is_Appended()
    {
        var cut = Render<Tabs>(p => p.Add(x => x.Class, "my-tabs"));
        Assert.Contains("my-tabs", cut.Find("div").ClassName);
    }

    [Fact]
    public void Tabs_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Tabs>(p => p.AddUnmatched("data-testid", "tabs-root"));
        Assert.Equal("tabs-root", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void Tabs_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Tabs).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── TabsList ──────────────────────────────────────────────────────────────

    [Fact]
    public void TabsList_Has_Role_Tablist()
    {
        var cut = Render<TabsList>();
        AssertRole(cut.Find("div"), "tablist");
    }

    [Fact]
    public void TabsList_Has_Base_Classes()
    {
        var cut = Render<TabsList>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("bg-muted", classes);
    }

    [Fact]
    public void TabsList_Custom_Class_Is_Appended()
    {
        var cut = Render<TabsList>(p => p.Add(x => x.Class, "my-list"));
        Assert.Contains("my-list", cut.Find("div").ClassName);
    }

    [Fact]
    public void TabsList_AdditionalAttributes_PassThrough()
    {
        var cut = Render<TabsList>(p => p.AddUnmatched("data-testid", "tabs-list"));
        Assert.Equal("tabs-list", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── TabsTrigger ───────────────────────────────────────────────────────────

    [Fact]
    public void TabsTrigger_Has_Role_Tab()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        AssertRole(cut.Find("button"), "tab");
    }

    [Fact]
    public void TabsTrigger_Inactive_Has_AriaSelected_False()
    {
        var cut = RenderWithTabs("other", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-selected"));
    }

    [Fact]
    public void TabsTrigger_Active_Has_AriaSelected_True()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-selected"));
    }

    [Fact]
    public void TabsTrigger_Active_Has_DataState_Active()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        Assert.Equal("active", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void TabsTrigger_Inactive_Has_DataState_Inactive()
    {
        var cut = RenderWithTabs("other", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        Assert.Equal("inactive", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void TabsTrigger_Click_Switches_ActiveTab()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsTrigger>(b => b
                .Add(x => x.ItemValue, "password")
                .AddChildContent("Password")));

        cut.Find("button").Click();

        Assert.Equal("password", cut.Instance.ActiveValue);
    }

    [Fact]
    public void TabsTrigger_Disabled_Does_Not_Switch_Tab()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsTrigger>(b => b
                .Add(x => x.ItemValue, "password")
                .Add(x => x.Disabled, true)
                .AddChildContent("Password")));

        cut.Find("button").Click();

        Assert.Equal("account", cut.Instance.ActiveValue);
    }

    [Fact]
    public void TabsTrigger_Has_Base_Classes()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddChildContent("Account"));
        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("rounded-sm", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void TabsTrigger_Custom_Class_Is_Appended()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .Add(x => x.Class, "my-trigger")
            .AddChildContent("Account"));
        Assert.Contains("my-trigger", cut.Find("button").ClassName);
    }

    [Fact]
    public void TabsTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = RenderWithTabs("account", trigger: b => b
            .Add(x => x.ItemValue, "account")
            .AddUnmatched("data-testid", "trigger-account")
            .AddChildContent("Account"));
        Assert.Equal("trigger-account", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── TabsContent ───────────────────────────────────────────────────────────

    [Fact]
    public void TabsContent_Active_Renders_TabPanel()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsContent>(b => b
                .Add(x => x.ItemValue, "account")
                .AddChildContent("Account content")));

        var panel = cut.Find("[role='tabpanel']");
        Assert.NotNull(panel);
        Assert.Equal("active", panel.GetAttribute("data-state"));
    }

    [Fact]
    public void TabsContent_Inactive_Does_Not_Render()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "other")
            .AddChildContent<TabsContent>(b => b
                .Add(x => x.ItemValue, "account")
                .AddChildContent("Account content")));

        Assert.Empty(cut.FindAll("[role='tabpanel']"));
    }

    [Fact]
    public void TabsContent_Has_Base_Classes()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsContent>(b => b
                .Add(x => x.ItemValue, "account")
                .AddChildContent("Content")));

        var classes = cut.Find("[role='tabpanel']").ClassName;
        Assert.Contains("mt-2", classes);
        Assert.Contains("ring-offset-background", classes);
    }

    [Fact]
    public void TabsContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsContent>(b => b
                .Add(x => x.ItemValue, "account")
                .Add(x => x.Class, "my-content")
                .AddChildContent("Content")));

        Assert.Contains("my-content", cut.Find("[role='tabpanel']").ClassName);
    }

    [Fact]
    public void TabsContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Tabs>(p => p
            .Add(x => x.ActiveValue, "account")
            .AddChildContent<TabsContent>(b => b
                .Add(x => x.ItemValue, "account")
                .AddUnmatched("data-testid", "panel-account")
                .AddChildContent("Content")));

        Assert.Equal("panel-account", cut.Find("[role='tabpanel']").GetAttribute("data-testid"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void TabsList_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(TabsList).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void TabsTrigger_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(TabsTrigger).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void TabsContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(TabsContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private IRenderedComponent<Tabs> RenderWithTabs(
        string activeValue,
        Action<ComponentParameterCollectionBuilder<TabsTrigger>> trigger)
    {
        return Render<Tabs>(p => p
            .Add(x => x.ActiveValue, activeValue)
            .AddChildContent<TabsTrigger>(trigger));
    }
}
