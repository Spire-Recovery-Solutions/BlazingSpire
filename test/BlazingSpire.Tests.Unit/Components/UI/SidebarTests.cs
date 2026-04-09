using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SidebarTests : BlazingSpireTestBase
{
    // ── Sidebar ───────────────────────────────────────────────────────────────

    [Fact]
    public void Sidebar_Renders_Aside_Element()
    {
        var cut = Render<Sidebar>();
        Assert.NotNull(cut.Find("aside"));
    }

    [Fact]
    public void Sidebar_Custom_Class_Is_Appended()
    {
        var cut = Render<Sidebar>(p => p.Add(x => x.Class, "my-sidebar"));
        Assert.Contains("my-sidebar", cut.Find("aside").ClassName);
    }

    [Fact]
    public void Sidebar_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Sidebar>(p => p.AddUnmatched("data-testid", "sidebar"));
        Assert.Equal("sidebar", cut.Find("aside").GetAttribute("data-testid"));
    }

    [Fact]
    public void Sidebar_ChildContent_Renders()
    {
        var cut = Render<Sidebar>(p => p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public async Task Sidebar_ToggleAsync_Changes_IsCollapsed()
    {
        var cut = Render<Sidebar>();
        Assert.False(cut.Instance.IsCollapsed);

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.True(cut.Instance.IsCollapsed);
    }

    [Fact]
    public async Task Sidebar_ToggleAsync_Invokes_IsCollapsedChanged()
    {
        bool? received = null;
        var cut = Render<Sidebar>(p =>
            p.Add(x => x.IsCollapsedChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.Equal(true, received);
    }

    // ── SidebarHeader ─────────────────────────────────────────────────────────

    [Fact]
    public void SidebarHeader_Renders_Div()
    {
        var cut = Render<SidebarHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarHeader_Custom_Class_Is_Appended()
    {
        var cut = Render<SidebarHeader>(p => p.Add(x => x.Class, "custom-header"));
        Assert.Contains("custom-header", cut.Find("div").ClassName);
    }

    [Fact]
    public void SidebarHeader_ChildContent_Renders()
    {
        var cut = Render<SidebarHeader>(p => p.AddChildContent("<span>Logo</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── SidebarContent ────────────────────────────────────────────────────────

    [Fact]
    public void SidebarContent_Renders_Div()
    {
        var cut = Render<SidebarContent>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarContent_ChildContent_Renders()
    {
        var cut = Render<SidebarContent>(p => p.AddChildContent("<nav>Nav</nav>"));
        Assert.NotNull(cut.Find("nav"));
    }

    // ── SidebarFooter ─────────────────────────────────────────────────────────

    [Fact]
    public void SidebarFooter_Renders_Div()
    {
        var cut = Render<SidebarFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarFooter_ChildContent_Renders()
    {
        var cut = Render<SidebarFooter>(p => p.AddChildContent("<span>User</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── SidebarTrigger ────────────────────────────────────────────────────────

    [Fact]
    public void SidebarTrigger_Renders_Button()
    {
        var cut = Render<Sidebar>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SidebarTrigger>(0);
                b.CloseComponent();
            })));

        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public async Task SidebarTrigger_Click_Toggles_Parent()
    {
        var cut = Render<Sidebar>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SidebarTrigger>(0);
                b.CloseComponent();
            })));

        Assert.False(cut.Instance.IsCollapsed);

        await cut.Find("button").ClickAsync(new());

        Assert.True(cut.Instance.IsCollapsed);
    }

    [Fact]
    public void SidebarTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Sidebar>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SidebarTrigger>(0);
                b.AddAttribute(1, "data-testid", "trigger");
                b.CloseComponent();
            })));

        Assert.Equal("trigger", cut.Find("button").GetAttribute("data-testid"));
    }
}
