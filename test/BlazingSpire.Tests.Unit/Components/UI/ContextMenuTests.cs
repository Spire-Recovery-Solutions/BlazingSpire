using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ContextMenuTests : BlazingSpireTestBase
{
    // ── ContextMenu ──────────────────────────────────────────────────────────

    [Fact]
    public void ContextMenu_Renders_CascadingValue()
    {
        var cut = Render<ContextMenu>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void ContextMenu_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(ContextMenu).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void ContextMenu_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(ContextMenu).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void ContextMenu_ShouldCloseOnEscape_Is_True()
    {
        var menu = new ContextMenu();
        var prop = typeof(ContextMenu).GetProperty("ShouldCloseOnEscape",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(menu)!);
    }

    [Fact]
    public void ContextMenu_ShouldCloseOnInteractOutside_Is_True()
    {
        var menu = new ContextMenu();
        var prop = typeof(ContextMenu).GetProperty("ShouldCloseOnInteractOutside",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(menu)!);
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
    public void ContextMenuContent_Visible_When_Open()
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

        Assert.Equal("menu", cut.Find("[role=menu]").GetAttribute("role"));
    }

    [Fact]
    public void ContextMenuContent_Has_Base_Classes()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        var classes = cut.Find("[role=menu]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-md", classes);
    }

    [Fact]
    public void ContextMenuContent_Custom_Class_Is_Appended()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=menu]").ClassName);
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
    public void ContextMenuItem_Has_Base_Classes()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                    ip.AddChildContent("Back")));
        });

        var classes = cut.Find("[role=menuitem]").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("rounded-sm", classes);
    }

    [Fact]
    public void ContextMenuItem_Custom_Class_Is_Appended()
    {
        var cut = Render<ContextMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ContextMenuContent>(cp =>
                cp.AddChildContent<ContextMenuItem>(ip =>
                    ip.Add(x => x.Class, "custom-item")));
        });

        Assert.Contains("custom-item", cut.Find("[role=menuitem]").ClassName);
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
    public void ContextMenuItem_Click_Invokes_OnClick_And_Closes_Menu()
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
        Assert.Equal("separator", cut.Find("[role=separator]").GetAttribute("role"));
    }

    [Fact]
    public void ContextMenuSeparator_Has_Base_Classes()
    {
        var cut = Render<ContextMenuSeparator>();
        var classes = cut.Find("[role=separator]").ClassName;
        Assert.Contains("h-px", classes);
        Assert.Contains("bg-muted", classes);
        Assert.Contains("my-1", classes);
    }

    // ── ContextMenuLabel ─────────────────────────────────────────────────────

    [Fact]
    public void ContextMenuLabel_Renders_Div()
    {
        var cut = Render<ContextMenuLabel>(p => p.AddChildContent("Navigation"));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void ContextMenuLabel_Has_Base_Classes()
    {
        var cut = Render<ContextMenuLabel>(p => p.AddChildContent("Navigation"));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("px-2", classes);
    }

    [Fact]
    public void ContextMenuLabel_Inset_Adds_Pl8()
    {
        var cut = Render<ContextMenuLabel>(p =>
        {
            p.Add(x => x.Inset, true);
            p.AddChildContent("Inset Label");
        });

        Assert.Contains("pl-8", cut.Find("div").ClassName);
    }

    [Fact]
    public void ContextMenuLabel_No_Inset_Has_No_Pl8()
    {
        var cut = Render<ContextMenuLabel>(p => p.AddChildContent("Label"));
        Assert.DoesNotContain("pl-8", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void ContextMenuLabel_Renders_ChildContent()
    {
        var cut = Render<ContextMenuLabel>(p => p.AddChildContent("Navigation"));
        Assert.Contains("Navigation", cut.Find("div").TextContent);
    }
}
