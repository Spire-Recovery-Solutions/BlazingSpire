using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class BreadcrumbTests : BlazingSpireTestBase
{
    // ── Breadcrumb ────────────────────────────────────────────────────────────

    [Fact]
    public void Breadcrumb_Renders_Nav_Element()
    {
        var cut = Render<Breadcrumb>();
        Assert.NotNull(cut.Find("nav"));
    }

    [Fact]
    public void Breadcrumb_Has_AriaLabel_Breadcrumb()
    {
        var cut = Render<Breadcrumb>();
        Assert.Equal("breadcrumb", cut.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void Breadcrumb_ChildContent_Renders_Inside_Nav()
    {
        var cut = Render<Breadcrumb>(p => p.AddChildContent("<ol></ol>"));
        Assert.NotNull(cut.Find("nav ol"));
    }

    [Fact]
    public void Breadcrumb_Custom_Class_Is_Included()
    {
        var cut = Render<Breadcrumb>(p => p.Add(x => x.Class, "my-breadcrumb"));
        Assert.Contains("my-breadcrumb", cut.Find("nav").ClassName);
    }

    // ── BreadcrumbList ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbList_Renders_Ol_Element()
    {
        var cut = Render<BreadcrumbList>();
        Assert.NotNull(cut.Find("ol"));
    }

    [Fact]
    public void BreadcrumbList_ChildContent_Renders_Inside_Ol()
    {
        var cut = Render<BreadcrumbList>(p => p.AddChildContent("<li>Item</li>"));
        Assert.NotNull(cut.Find("ol li"));
    }

    [Fact]
    public void BreadcrumbList_Custom_Class_Is_Included()
    {
        var cut = Render<BreadcrumbList>(p => p.Add(x => x.Class, "my-list"));
        Assert.Contains("my-list", cut.Find("ol").ClassName);
    }

    // ── BreadcrumbItem ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbItem_Renders_Li_Element()
    {
        var cut = Render<BreadcrumbItem>();
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void BreadcrumbItem_ChildContent_Renders_Inside_Li()
    {
        var cut = Render<BreadcrumbItem>(p => p.AddChildContent("<a href=\"/home\">Home</a>"));
        Assert.NotNull(cut.Find("li a"));
    }

    [Fact]
    public void BreadcrumbItem_Custom_Class_Is_Included()
    {
        var cut = Render<BreadcrumbItem>(p => p.Add(x => x.Class, "my-item"));
        Assert.Contains("my-item", cut.Find("li").ClassName);
    }

    // ── BreadcrumbLink ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbLink_Renders_Anchor_Element()
    {
        var cut = Render<BreadcrumbLink>();
        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void BreadcrumbLink_Href_Is_Set()
    {
        var cut = Render<BreadcrumbLink>(p => p.Add(x => x.Href, "/home"));
        Assert.Equal("/home", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void BreadcrumbLink_ChildContent_Renders_Inside_Anchor()
    {
        var cut = Render<BreadcrumbLink>(p => p.AddChildContent("Home"));
        Assert.Contains("Home", cut.Find("a").TextContent);
    }

    [Fact]
    public void BreadcrumbLink_Custom_Class_Is_Included()
    {
        var cut = Render<BreadcrumbLink>(p => p.Add(x => x.Class, "my-link"));
        Assert.Contains("my-link", cut.Find("a").ClassName);
    }

    // ── BreadcrumbPage ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbPage_Renders_Span_Element()
    {
        var cut = Render<BreadcrumbPage>();
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void BreadcrumbPage_Has_Role_Link()
    {
        var cut = Render<BreadcrumbPage>();
        AssertRole(cut.Find("span"), "link");
    }

    [Fact]
    public void BreadcrumbPage_Has_AriaDisabled_True()
    {
        var cut = Render<BreadcrumbPage>();
        Assert.Equal("true", cut.Find("span").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void BreadcrumbPage_Has_AriaCurrent_Page()
    {
        var cut = Render<BreadcrumbPage>();
        Assert.Equal("page", cut.Find("span").GetAttribute("aria-current"));
    }

    [Fact]
    public void BreadcrumbPage_ChildContent_Renders()
    {
        var cut = Render<BreadcrumbPage>(p => p.AddChildContent("Settings"));
        Assert.Contains("Settings", cut.Find("span").TextContent);
    }

    [Fact]
    public void BreadcrumbPage_Custom_Class_Is_Included()
    {
        var cut = Render<BreadcrumbPage>(p => p.Add(x => x.Class, "my-page"));
        Assert.Contains("my-page", cut.Find("span").ClassName);
    }

    // ── BreadcrumbSeparator ───────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbSeparator_Renders_Li_Element()
    {
        var cut = Render<BreadcrumbSeparator>();
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void BreadcrumbSeparator_Has_Role_Presentation()
    {
        var cut = Render<BreadcrumbSeparator>();
        AssertRole(cut.Find("li"), "presentation");
    }

    [Fact]
    public void BreadcrumbSeparator_Has_AriaHidden_True()
    {
        var cut = Render<BreadcrumbSeparator>();
        AssertAriaHidden(cut.Find("li"), true);
    }

    [Fact]
    public void BreadcrumbSeparator_Custom_ChildContent_Replaces_Default_Icon()
    {
        var cut = Render<BreadcrumbSeparator>(p => p.AddChildContent("<span>/</span>"));
        Assert.Empty(cut.FindAll("li svg"));
        Assert.NotNull(cut.Find("li span"));
    }

    [Fact]
    public void BreadcrumbSeparator_Custom_Class_Is_Included()
    {
        var cut = Render<BreadcrumbSeparator>(p => p.Add(x => x.Class, "my-sep"));
        Assert.Contains("my-sep", cut.Find("li").ClassName);
    }
}
