using BlazingSpire.Demo.Components.Shared;
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
    public void Sidebar_Default_Width_Is_Expanded()
    {
        var cut = Render<Sidebar>();
        Assert.Contains("w-64", cut.Find("aside").ClassName);
    }

    [Fact]
    public void Sidebar_Collapsed_Width_Is_w16()
    {
        var cut = Render<Sidebar>(p => p.Add(x => x.IsCollapsed, true));
        Assert.Contains("w-16", cut.Find("aside").ClassName);
    }

    [Fact]
    public void Sidebar_Has_Base_Classes()
    {
        var cut = Render<Sidebar>();
        var cls = cut.Find("aside").ClassName;
        Assert.Contains("flex", cls);
        Assert.Contains("h-full", cls);
        Assert.Contains("flex-col", cls);
        Assert.Contains("border-r", cls);
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
        Assert.Contains("w-64", cut.Find("aside").ClassName);

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.Contains("w-16", cut.Find("aside").ClassName);
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

    [Fact]
    public void Sidebar_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Sidebar).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── SidebarHeader ─────────────────────────────────────────────────────────

    [Fact]
    public void SidebarHeader_Renders_Div()
    {
        var cut = Render<SidebarHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarHeader_Has_Base_Classes()
    {
        var cut = Render<SidebarHeader>();
        var cls = cut.Find("div").ClassName;
        Assert.Contains("flex", cls);
        Assert.Contains("h-14", cls);
        Assert.Contains("items-center", cls);
        Assert.Contains("border-b", cls);
        Assert.Contains("px-4", cls);
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

    [Fact]
    public void SidebarHeader_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(SidebarHeader).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── SidebarContent ────────────────────────────────────────────────────────

    [Fact]
    public void SidebarContent_Renders_Div()
    {
        var cut = Render<SidebarContent>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarContent_Has_Base_Classes()
    {
        var cut = Render<SidebarContent>();
        var cls = cut.Find("div").ClassName;
        Assert.Contains("flex-1", cls);
        Assert.Contains("overflow-auto", cls);
        Assert.Contains("py-2", cls);
    }

    [Fact]
    public void SidebarContent_Custom_Class_Is_Appended()
    {
        var cut = Render<SidebarContent>(p => p.Add(x => x.Class, "custom-content"));
        Assert.Contains("custom-content", cut.Find("div").ClassName);
    }

    [Fact]
    public void SidebarContent_ChildContent_Renders()
    {
        var cut = Render<SidebarContent>(p => p.AddChildContent("<nav>Nav</nav>"));
        Assert.NotNull(cut.Find("nav"));
    }

    [Fact]
    public void SidebarContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(SidebarContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── SidebarFooter ─────────────────────────────────────────────────────────

    [Fact]
    public void SidebarFooter_Renders_Div()
    {
        var cut = Render<SidebarFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SidebarFooter_Has_Base_Classes()
    {
        var cut = Render<SidebarFooter>();
        var cls = cut.Find("div").ClassName;
        Assert.Contains("border-t", cls);
        Assert.Contains("p-4", cls);
    }

    [Fact]
    public void SidebarFooter_Custom_Class_Is_Appended()
    {
        var cut = Render<SidebarFooter>(p => p.Add(x => x.Class, "custom-footer"));
        Assert.Contains("custom-footer", cut.Find("div").ClassName);
    }

    [Fact]
    public void SidebarFooter_ChildContent_Renders()
    {
        var cut = Render<SidebarFooter>(p => p.AddChildContent("<span>User</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void SidebarFooter_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(SidebarFooter).IsAssignableTo(typeof(BlazingSpireComponentBase)));
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

        Assert.Contains("w-64", cut.Find("aside").ClassName);

        await cut.Find("button").ClickAsync(new());

        Assert.Contains("w-16", cut.Find("aside").ClassName);
    }

    [Fact]
    public void SidebarTrigger_Has_Base_Classes()
    {
        var cut = Render<Sidebar>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<SidebarTrigger>(0);
                b.CloseComponent();
            })));

        var cls = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", cls);
        Assert.Contains("h-8", cls);
        Assert.Contains("w-8", cls);
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

    [Fact]
    public void SidebarTrigger_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(SidebarTrigger).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
