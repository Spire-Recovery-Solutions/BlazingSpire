using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DropdownMenuTests : BlazingSpireTestBase
{
    // ── DropdownMenu ─────────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenu_Renders_ChildContent()
    {
        var cut = Render<DropdownMenu>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── DropdownMenuContent ──────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuContent_Hidden_When_Closed()
    {
        var cut = Render<DropdownMenu>(p =>
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>")));

        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void DropdownMenuContent_Visible_When_DefaultIsOpen()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        Assert.NotNull(cut.Find("[role=menu]"));
    }

    [Fact]
    public void DropdownMenuContent_Has_Role_Menu()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        AssertRole(cut.Find("[role=menu]"), "menu");
    }

    [Fact]
    public void DropdownMenuContent_Renders_ChildContent()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent("<span data-testid='inner'>test</span>"));
        });

        Assert.NotNull(cut.Find("[data-testid=inner]"));
    }

    // ── DropdownMenuTrigger ──────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuTrigger_Click_Opens_Menu()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DropdownMenuTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open Menu")));
                builder.CloseComponent();

                builder.OpenComponent<DropdownMenuContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=menu]"));
        cut.Find("div").Click();
        Assert.NotNull(cut.Find("[role=menu]"));
    }

    // ── DropdownMenuItem ─────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuItem_Has_Role_Menuitem()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                    ip.AddChildContent("Profile")));
        });

        Assert.NotNull(cut.Find("[role=menuitem]"));
    }

    [Fact]
    public void DropdownMenuItem_Disabled_Has_Data_Disabled()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                {
                    ip.Add(x => x.Disabled, true);
                    ip.AddChildContent("Disabled");
                }));
        });

        Assert.Equal("true", cut.Find("[role=menuitem]").GetAttribute("data-disabled"));
    }

    [Fact]
    public void DropdownMenuItem_Not_Disabled_Has_No_Data_Disabled()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                    ip.AddChildContent("Profile")));
        });

        Assert.Null(cut.Find("[role=menuitem]").GetAttribute("data-disabled"));
    }

    [Fact]
    public void DropdownMenuItem_Click_Fires_OnClick_And_Closes_Menu()
    {
        var clicked = false;
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                {
                    ip.Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true));
                    ip.AddChildContent("Profile");
                }));
        });

        Assert.NotNull(cut.Find("[role=menu]"));
        cut.Find("[role=menuitem]").Click();
        Assert.True(clicked);
        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    // ── DropdownMenuSeparator ────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuSeparator_Has_Role_Separator()
    {
        var cut = Render<DropdownMenuSeparator>();
        AssertRole(cut.Find("[role=separator]"), "separator");
    }

    // ── DropdownMenuLabel ────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuLabel_Renders_ChildContent()
    {
        var cut = Render<DropdownMenuLabel>(p => p.AddChildContent("My Account"));
        Assert.Contains("My Account", cut.Find("div").TextContent);
    }

    // ── DropdownMenuGroup ────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuGroup_Has_Role_Group()
    {
        var cut = Render<DropdownMenuGroup>(p => p.AddChildContent("<span>item</span>"));
        AssertRole(cut.Find("[role=group]"), "group");
    }

    [Fact]
    public void DropdownMenuGroup_Renders_ChildContent()
    {
        var cut = Render<DropdownMenuGroup>(p =>
            p.AddChildContent("<span data-testid='child'>child</span>"));
        Assert.NotNull(cut.Find("[data-testid=child]"));
    }
}
