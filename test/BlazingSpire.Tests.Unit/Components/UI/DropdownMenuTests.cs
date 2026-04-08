using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DropdownMenuTests : BlazingSpireTestBase
{
    // ── DropdownMenu ─────────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenu_Renders_CascadingValue()
    {
        var cut = Render<DropdownMenu>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void DropdownMenu_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(DropdownMenu).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void DropdownMenu_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(DropdownMenu).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void DropdownMenu_ShouldCloseOnEscape_Is_True()
    {
        var menu = new DropdownMenu();
        var prop = typeof(DropdownMenu).GetProperty("ShouldCloseOnEscape",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(menu)!);
    }

    [Fact]
    public void DropdownMenu_ShouldCloseOnInteractOutside_Is_True()
    {
        var menu = new DropdownMenu();
        var prop = typeof(DropdownMenu).GetProperty("ShouldCloseOnInteractOutside",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(menu)!);
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
    public void DropdownMenuContent_Visible_When_Open()
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

        Assert.Equal("menu", cut.Find("[role=menu]").GetAttribute("role"));
    }

    [Fact]
    public void DropdownMenuContent_Has_Base_Classes()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        var classes = cut.Find("[role=menu]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-md", classes);
    }

    [Fact]
    public void DropdownMenuContent_Custom_Class_Is_Appended()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=menu]").ClassName);
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
    public void DropdownMenuItem_Has_Base_Classes()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                    ip.AddChildContent("Profile")));
        });

        var classes = cut.Find("[role=menuitem]").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("rounded-sm", classes);
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
    public void DropdownMenuItem_Click_Invokes_OnClick_And_Closes_Menu()
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

    [Fact]
    public void DropdownMenuItem_Custom_Class_Is_Appended()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                    ip.Add(x => x.Class, "custom-item")));
        });

        Assert.Contains("custom-item", cut.Find("[role=menuitem]").ClassName);
    }

    // ── DropdownMenuSeparator ────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuSeparator_Has_Role_Separator()
    {
        var cut = Render<DropdownMenuSeparator>();
        Assert.Equal("separator", cut.Find("[role=separator]").GetAttribute("role"));
    }

    [Fact]
    public void DropdownMenuSeparator_Has_Base_Classes()
    {
        var cut = Render<DropdownMenuSeparator>();
        var classes = cut.Find("[role=separator]").ClassName;
        Assert.Contains("h-px", classes);
        Assert.Contains("bg-muted", classes);
        Assert.Contains("my-1", classes);
    }

    // ── DropdownMenuLabel ────────────────────────────────────────────────────

    [Fact]
    public void DropdownMenuLabel_Renders_Div()
    {
        var cut = Render<DropdownMenuLabel>(p => p.AddChildContent("My Account"));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void DropdownMenuLabel_Has_Base_Classes()
    {
        var cut = Render<DropdownMenuLabel>(p => p.AddChildContent("My Account"));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("px-2", classes);
    }

    [Fact]
    public void DropdownMenuLabel_Inset_Adds_Pl8()
    {
        var cut = Render<DropdownMenuLabel>(p =>
        {
            p.Add(x => x.Inset, true);
            p.AddChildContent("Inset Label");
        });

        Assert.Contains("pl-8", cut.Find("div").ClassName);
    }

    [Fact]
    public void DropdownMenuLabel_No_Inset_Has_No_Pl8()
    {
        var cut = Render<DropdownMenuLabel>(p => p.AddChildContent("Label"));
        Assert.DoesNotContain("pl-8", cut.Find("div").ClassName ?? "");
    }

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
        Assert.Equal("group", cut.Find("[role=group]").GetAttribute("role"));
    }

    [Fact]
    public void DropdownMenuGroup_Renders_ChildContent()
    {
        var cut = Render<DropdownMenuGroup>(p =>
            p.AddChildContent("<span data-testid='child'>child</span>"));
        Assert.NotNull(cut.Find("[data-testid=child]"));
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
}
