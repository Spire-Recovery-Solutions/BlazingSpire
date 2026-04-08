using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class NavigationMenuTests : BlazingSpireTestBase
{
    // ── NavigationMenu ───────────────────────────────────────────────────────

    [Fact]
    public void NavigationMenu_Renders_Nav_Element()
    {
        var cut = Render<NavigationMenu>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("nav"));
    }

    [Fact]
    public void NavigationMenu_Has_Base_Classes()
    {
        var cut = Render<NavigationMenu>(p => p.AddChildContent("<span>x</span>"));
        var classes = cut.Find("nav").ClassName;
        Assert.Contains("relative", classes);
        Assert.Contains("z-10", classes);
        Assert.Contains("flex", classes);
    }

    [Fact]
    public void NavigationMenu_Custom_Class_Is_Appended()
    {
        var cut = Render<NavigationMenu>(p => p.Add(x => x.Class, "my-nav-class"));
        Assert.Contains("my-nav-class", cut.Find("nav").ClassName);
    }

    [Fact]
    public void NavigationMenu_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(NavigationMenu).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── NavigationMenuList ───────────────────────────────────────────────────

    [Fact]
    public void NavigationMenuList_Renders_Ul_Element()
    {
        var cut = Render<NavigationMenuList>(p => p.AddChildContent("<li>item</li>"));
        Assert.NotNull(cut.Find("ul"));
    }

    [Fact]
    public void NavigationMenuList_Has_Base_Classes()
    {
        var cut = Render<NavigationMenuList>(p => p.AddChildContent("<li>item</li>"));
        var classes = cut.Find("ul").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("list-none", classes);
        Assert.Contains("items-center", classes);
    }

    [Fact]
    public void NavigationMenuList_Custom_Class_Is_Appended()
    {
        var cut = Render<NavigationMenuList>(p => p.Add(x => x.Class, "custom-list"));
        Assert.Contains("custom-list", cut.Find("ul").ClassName);
    }

    // ── NavigationMenuItem ───────────────────────────────────────────────────

    [Fact]
    public void NavigationMenuItem_Renders_Li_Element()
    {
        var cut = Render<NavigationMenuItem>(p => p.AddChildContent("<span>item</span>"));
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void NavigationMenuItem_IsOpen_Defaults_To_False()
    {
        var cut = Render<NavigationMenuItem>(p => p.AddChildContent("<span>item</span>"));
        var item = cut.Instance;
        Assert.False(item.IsOpen);
    }

    [Fact]
    public async Task NavigationMenuItem_ToggleAsync_Opens_And_Closes()
    {
        var cut = Render<NavigationMenuItem>(p => p.AddChildContent("<span>item</span>"));
        var item = cut.Instance;

        await cut.InvokeAsync(() => item.ToggleAsync());
        Assert.True(item.IsOpen);

        await cut.InvokeAsync(() => item.ToggleAsync());
        Assert.False(item.IsOpen);
    }

    // ── NavigationMenuTrigger ────────────────────────────────────────────────

    [Fact]
    public void NavigationMenuTrigger_Renders_Button()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.AddChildContent("Getting Started")));

        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void NavigationMenuTrigger_Has_Base_Classes()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.AddChildContent("Getting Started")));

        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void NavigationMenuTrigger_Has_DataState_Closed_When_Item_Closed()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.AddChildContent("Trigger")));

        Assert.Equal("closed", cut.Find("button").GetAttribute("data-state"));
    }

    [Fact]
    public void NavigationMenuTrigger_Click_Toggles_Content_Visibility()
    {
        var cut = Render<NavigationMenuItem>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<NavigationMenuTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Getting Started")));
                builder.CloseComponent();

                builder.OpenComponent<NavigationMenuContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "dropdown content")));
                builder.CloseComponent();
            });
        });

        // Content hidden initially
        Assert.Empty(cut.FindAll("div[class*=absolute]"));

        // Click trigger
        cut.Find("button").Click();

        // Content visible
        Assert.NotEmpty(cut.FindAll("div[class*=absolute]"));
    }

    [Fact]
    public void NavigationMenuTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.Add(x => x.Class, "my-trigger-class")));

        Assert.Contains("my-trigger-class", cut.Find("button").ClassName);
    }

    // ── NavigationMenuContent ────────────────────────────────────────────────

    [Fact]
    public void NavigationMenuContent_Hidden_When_Item_Closed()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuContent>(cp =>
                cp.AddChildContent("<span>content</span>")));

        Assert.Empty(cut.FindAll("div[class*=absolute]"));
    }

    [Fact]
    public async Task NavigationMenuContent_Visible_When_Item_Open()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuContent>(cp =>
                cp.AddChildContent("<span data-testid='inner'>content</span>")));

        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());

        Assert.NotNull(cut.Find("[data-testid=inner]"));
    }

    [Fact]
    public async Task NavigationMenuContent_Has_Base_Classes_When_Open()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuContent>(cp =>
                cp.AddChildContent("<span>x</span>")));

        cut.Find("li"); // render
        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());

        var classes = cut.Find("div").ClassName;
        Assert.Contains("absolute", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-lg", classes);
    }

    // ── NavigationMenuLink ───────────────────────────────────────────────────

    [Fact]
    public void NavigationMenuLink_Renders_Anchor()
    {
        var cut = Render<NavigationMenuLink>(p =>
        {
            p.Add(x => x.Href, "/docs");
            p.AddChildContent("Documentation");
        });

        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void NavigationMenuLink_Href_Is_Set()
    {
        var cut = Render<NavigationMenuLink>(p =>
        {
            p.Add(x => x.Href, "/docs");
            p.AddChildContent("Documentation");
        });

        Assert.Equal("/docs", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void NavigationMenuLink_Has_Base_Classes()
    {
        var cut = Render<NavigationMenuLink>(p =>
        {
            p.Add(x => x.Href, "/docs");
            p.AddChildContent("Documentation");
        });

        var classes = cut.Find("a").ClassName;
        Assert.Contains("block", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("no-underline", classes);
    }

    [Fact]
    public void NavigationMenuLink_Renders_ChildContent()
    {
        var cut = Render<NavigationMenuLink>(p =>
        {
            p.Add(x => x.Href, "/docs");
            p.AddChildContent("Documentation");
        });

        Assert.Contains("Documentation", cut.Find("a").TextContent);
    }

    [Fact]
    public void NavigationMenuLink_Custom_Class_Is_Appended()
    {
        var cut = Render<NavigationMenuLink>(p =>
            p.Add(x => x.Class, "custom-link"));

        Assert.Contains("custom-link", cut.Find("a").ClassName);
    }
}
