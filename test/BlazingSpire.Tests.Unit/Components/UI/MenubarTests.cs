using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class MenubarTests : BlazingSpireTestBase
{
    // ── Menubar ──────────────────────────────────────────────────────────────

    [Fact]
    public void Menubar_Has_Role_Menubar()
    {
        var cut = Render<Menubar>(p => p.AddChildContent("<span>menu</span>"));
        Assert.Equal("menubar", cut.Find("[role=menubar]").GetAttribute("role"));
    }

    [Fact]
    public void Menubar_Has_Base_Classes()
    {
        var cut = Render<Menubar>(p => p.AddChildContent("<span>menu</span>"));
        var classes = cut.Find("[role=menubar]").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("border", classes);
    }

    [Fact]
    public void Menubar_Custom_Class_Is_Appended()
    {
        var cut = Render<Menubar>(p =>
        {
            p.Add(x => x.Class, "my-menubar");
            p.AddChildContent("<span>menu</span>");
        });
        Assert.Contains("my-menubar", cut.Find("[role=menubar]").ClassName);
    }

    // ── MenubarMenu ──────────────────────────────────────────────────────────

    [Fact]
    public void MenubarMenu_Renders_Div()
    {
        var cut = Render<MenubarMenu>(p => p.AddChildContent("<span>item</span>"));
        Assert.NotNull(cut.Find("div"));
    }

    // ── MenubarTrigger ───────────────────────────────────────────────────────

    [Fact]
    public void MenubarTrigger_Has_Role_Menuitem()
    {
        var cut = Render<MenubarMenu>(p =>
            p.AddChildContent<MenubarTrigger>(tp => tp.AddChildContent("File")));

        Assert.Equal("menuitem", cut.Find("[role=menuitem]").GetAttribute("role"));
    }

    [Fact]
    public void MenubarTrigger_Toggle_Opens_Content()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=menu]"));
        cut.Find("[role=menuitem]").Click();
        Assert.NotNull(cut.Find("[role=menu]"));
    }

    [Fact]
    public void MenubarTrigger_Toggle_Closes_Content()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click(); // open
        cut.Find("[role=menuitem]").Click(); // close
        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void MenubarTrigger_Has_Base_Classes()
    {
        var cut = Render<MenubarMenu>(p =>
            p.AddChildContent<MenubarTrigger>(tp => tp.AddChildContent("File")));

        var classes = cut.Find("[role=menuitem]").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-medium", classes);
        Assert.Contains("rounded-sm", classes);
    }

    // ── MenubarContent ───────────────────────────────────────────────────────

    [Fact]
    public void MenubarContent_Hidden_When_Closed()
    {
        var cut = Render<MenubarMenu>(p =>
            p.AddChildContent<MenubarContent>(cp => cp.AddChildContent("items")));

        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void MenubarContent_Has_Role_Menu()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click();
        Assert.Equal("menu", cut.Find("[role=menu]").GetAttribute("role"));
    }

    [Fact]
    public void MenubarContent_Has_Base_Classes()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click();
        var classes = cut.Find("[role=menu]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-md", classes);
    }

    [Fact]
    public void MenubarContent_Custom_Class_Is_Appended()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "Class", "custom-content");
                builder.AddAttribute(4, "ChildContent", (RenderFragment)(b => b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click();
        Assert.Contains("custom-content", cut.Find("[role=menu]").ClassName);
    }

    // ── MenubarItem ──────────────────────────────────────────────────────────

    [Fact]
    public void MenubarItem_Has_Role_Menuitem()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<MenubarItem>(0);
                    b.AddAttribute(1, "ChildContent", (RenderFragment)(ib => ib.AddContent(0, "New")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click(); // open menu
        var items = cut.FindAll("[role=menuitem]");
        Assert.Equal(2, items.Count); // trigger + item
    }

    [Fact]
    public void MenubarItem_Click_Invokes_OnClick_And_Closes_Menu()
    {
        var clicked = false;

        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<MenubarItem>(0);
                    b.AddAttribute(1, "OnClick", EventCallback.Factory.Create(this, () => clicked = true));
                    b.AddAttribute(2, "ChildContent", (RenderFragment)(ib => ib.AddContent(0, "New")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click(); // open
        Assert.NotNull(cut.Find("[role=menu]"));

        var items = cut.FindAll("[role=menuitem]");
        items[1].Click(); // click the item

        Assert.True(clicked);
        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void MenubarItem_Disabled_Has_Data_Disabled()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<MenubarItem>(0);
                    b.AddAttribute(1, "Disabled", true);
                    b.AddAttribute(2, "ChildContent", (RenderFragment)(ib => ib.AddContent(0, "Exit")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click(); // open
        var items = cut.FindAll("[role=menuitem]");
        Assert.Equal("true", items[1].GetAttribute("data-disabled"));
    }

    [Fact]
    public void MenubarItem_Has_Base_Classes()
    {
        var cut = Render<MenubarMenu>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<MenubarTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => b.AddContent(0, "File")));
                builder.CloseComponent();

                builder.OpenComponent<MenubarContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<MenubarItem>(0);
                    b.AddAttribute(1, "ChildContent", (RenderFragment)(ib => ib.AddContent(0, "New")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });

        cut.Find("[role=menuitem]").Click();
        var items = cut.FindAll("[role=menuitem]");
        var classes = items[1].ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("rounded-sm", classes);
    }

    // ── MenubarSeparator ─────────────────────────────────────────────────────

    [Fact]
    public void MenubarSeparator_Has_Role_Separator()
    {
        var cut = Render<MenubarSeparator>();
        Assert.Equal("separator", cut.Find("[role=separator]").GetAttribute("role"));
    }

    [Fact]
    public void MenubarSeparator_Has_Base_Classes()
    {
        var cut = Render<MenubarSeparator>();
        var classes = cut.Find("[role=separator]").ClassName;
        Assert.Contains("h-px", classes);
        Assert.Contains("bg-muted", classes);
        Assert.Contains("my-1", classes);
    }

    // ── MenubarLabel ─────────────────────────────────────────────────────────

    [Fact]
    public void MenubarLabel_Renders_Div()
    {
        var cut = Render<MenubarLabel>(p => p.AddChildContent("File Options"));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void MenubarLabel_Has_Base_Classes()
    {
        var cut = Render<MenubarLabel>(p => p.AddChildContent("File Options"));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("px-2", classes);
    }

    [Fact]
    public void MenubarLabel_Renders_ChildContent()
    {
        var cut = Render<MenubarLabel>(p => p.AddChildContent("File Options"));
        Assert.Contains("File Options", cut.Find("div").TextContent);
    }
}
