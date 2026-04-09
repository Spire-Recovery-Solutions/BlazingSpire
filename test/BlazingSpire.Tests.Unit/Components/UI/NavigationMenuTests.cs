using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class NavigationMenuTests : BlazingSpireTestBase
{
    // ── NavigationMenuItem toggle ─────────────────────────────────────────────

    [Fact]
    public void NavigationMenuItem_IsOpen_Defaults_To_False()
    {
        var cut = Render<NavigationMenuItem>(p => p.AddChildContent("<span>item</span>"));
        Assert.False(cut.Instance.IsOpen);
    }

    [Fact]
    public async Task NavigationMenuItem_ToggleAsync_Opens_Then_Closes()
    {
        var cut = Render<NavigationMenuItem>(p => p.AddChildContent("<span>item</span>"));

        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());
        Assert.True(cut.Instance.IsOpen);

        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());
        Assert.False(cut.Instance.IsOpen);
    }

    // ── NavigationMenuTrigger data-state ──────────────────────────────────────

    [Fact]
    public void Trigger_Has_DataState_Closed_When_Item_Closed()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.AddChildContent("Getting Started")));

        AssertDataState(cut.Find("button"), "closed");
    }

    [Fact]
    public void Trigger_Click_Sets_DataState_Open()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuTrigger>(tp =>
                tp.AddChildContent("Getting Started")));

        cut.Find("button").Click();
        AssertDataState(cut.Find("button"), "open");
    }

    // ── NavigationMenuContent visibility ──────────────────────────────────────

    [Fact]
    public void Content_Is_Hidden_When_Item_Closed()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuContent>(cp =>
                cp.AddChildContent("<span data-testid='inner'>content</span>")));

        Assert.Empty(cut.FindAll("[data-testid=inner]"));
    }

    [Fact]
    public async Task Content_Is_Visible_When_Item_Open()
    {
        var cut = Render<NavigationMenuItem>(p =>
            p.AddChildContent<NavigationMenuContent>(cp =>
                cp.AddChildContent("<span data-testid='inner'>content</span>")));

        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());

        Assert.NotNull(cut.Find("[data-testid=inner]"));
    }

    [Fact]
    public void Trigger_Click_Shows_Content()
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

        Assert.Empty(cut.FindAll("li div"));
        cut.Find("button").Click();
        AssertDataState(cut.Find("button"), "open");
        Assert.NotEmpty(cut.FindAll("li div"));
    }

    // ── NavigationMenuLink ────────────────────────────────────────────────────

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
    public void NavigationMenuLink_Renders_ChildContent()
    {
        var cut = Render<NavigationMenuLink>(p =>
        {
            p.Add(x => x.Href, "/docs");
            p.AddChildContent("Documentation");
        });

        Assert.Contains("Documentation", cut.Find("a").TextContent);
    }

    // ── NavigationMenuList semantic ───────────────────────────────────────────

    [Fact]
    public void NavigationMenuList_Renders_Ul_Element()
    {
        var cut = Render<NavigationMenuList>(p => p.AddChildContent("<li>item</li>"));
        Assert.NotNull(cut.Find("ul"));
    }
}
