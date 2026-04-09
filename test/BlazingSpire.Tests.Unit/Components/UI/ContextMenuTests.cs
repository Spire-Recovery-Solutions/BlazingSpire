using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ContextMenuTests : BlazingSpireTestBase
{
    // ── ContextMenu ──────────────────────────────────────────────────────────

    [Fact]
    public void ContextMenu_Renders_ChildContent()
    {
        var cut = Render<ContextMenu>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── ContextMenuContent ───────────────────────────────────────────────────

    [Fact]
    public void ContextMenuContent_Hidden_When_Closed()
    {
        var cut = Render<ContextMenu>(p =>
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>")));

        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void ContextMenuContent_Visible_When_DefaultIsOpen()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        Assert.NotNull(cut.Find("[role=menu]"));
    }

    [Fact]
    public void ContextMenuContent_Has_Role_Menu()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        AssertRole(cut.Find("[role=menu]"), "menu");
    }

    [Fact]
    public void ContextMenuContent_Renders_ChildContent()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span data-testid='inner'>item</span>"));
        });

        Assert.NotNull(cut.Find("[data-testid=inner]"));
    }

    // ── ContextMenuItem ──────────────────────────────────────────────────────

    [Fact]
    public void ContextMenuItem_Has_Role_Menuitem()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                    ip.AddChildContent("Back")));
        });

        Assert.NotNull(cut.Find("[role=menuitem]"));
    }

    [Fact]
    public void ContextMenuItem_Disabled_Has_Data_Disabled()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                {
                    ip.Add(x => x.Disabled, true);
                    ip.AddChildContent("Disabled");
                }));
        });

        Assert.Equal("true", cut.Find("[role=menuitem]").GetAttribute("data-disabled"));
    }

    [Fact]
    public void ContextMenuItem_Not_Disabled_Has_No_Data_Disabled()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                    ip.AddChildContent("Back")));
        });

        Assert.Null(cut.Find("[role=menuitem]").GetAttribute("data-disabled"));
    }

    [Fact]
    public void ContextMenuItem_Click_Fires_OnClick_And_Closes_Menu()
    {
        var clicked = false;
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                {
                    ip.Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true));
                    ip.AddChildContent("Back");
                }));
        });

        Assert.NotNull(cut.Find("[role=menu]"));
        cut.Find("[role=menuitem]").Click();
        Assert.True(clicked);
        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    // ── ContextMenuSeparator ─────────────────────────────────────────────────

    [Fact]
    public void ContextMenuSeparator_Has_Role_Separator()
    {
        var cut = Render<ContextMenuSeparator>();
        AssertRole(cut.Find("[role=separator]"), "separator");
    }

    // ── ContextMenuLabel ─────────────────────────────────────────────────────

    [Fact]
    public void ContextMenuLabel_Renders_ChildContent()
    {
        var cut = Render<ContextMenuLabel>(p => p.AddChildContent("Navigation"));
        Assert.Contains("Navigation", cut.Find("div").TextContent);
    }

    [Fact]
    public void ContextMenuLabel_AdditionalAttributes_PassThrough()
    {
        var cut = Render<ContextMenuLabel>(p =>
        {
            p.AddChildContent("Label");
            p.AddUnmatched("data-testid", "ctx-label");
        });
        Assert.Equal("ctx-label", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── ContextMenuTrigger ───────────────────────────────────────────────────

    [Fact]
    public void ContextMenuTrigger_Renders_ChildContent()
    {
        var cut = Render<ContextMenu>(p =>
            p.AddChildContent<ContextMenuTrigger>(tp =>
                tp.AddChildContent("<span data-testid='trigger-child'>right-click me</span>")));

        Assert.NotNull(cut.Find("[data-testid=trigger-child]"));
    }

    // ── ContextMenuItem additional ───────────────────────────────────────────

    [Fact]
    public void ContextMenuItem_Renders_ChildContent_Text()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                    ip.AddChildContent("Copy")));
        });

        Assert.Contains("Copy", cut.Find("[role=menuitem]").TextContent);
    }

    [Fact]
    public void ContextMenuContent_Has_Data_State_Open()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        AssertDataState(cut.Find("[role=menu]"), "open");
    }

    [Fact]
    public void ContextMenuSeparator_AdditionalAttributes_PassThrough()
    {
        var cut = Render<ContextMenuSeparator>(p =>
            p.AddUnmatched("data-testid", "ctx-sep"));

        Assert.Equal("ctx-sep", cut.Find("[role=separator]").GetAttribute("data-testid"));
    }
}
