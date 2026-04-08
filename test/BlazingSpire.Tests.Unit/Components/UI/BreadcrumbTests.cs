using BlazingSpire.Demo.Components.Shared;
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
    public void Breadcrumb_Custom_Class_Is_Applied()
    {
        var cut = Render<Breadcrumb>(p => p.Add(x => x.Class, "my-breadcrumb"));
        Assert.Contains("my-breadcrumb", cut.Find("nav").ClassName);
    }

    [Fact]
    public void Breadcrumb_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Breadcrumb>(p => p.AddUnmatched("data-testid", "bc"));
        Assert.Equal("bc", cut.Find("nav").GetAttribute("data-testid"));
    }

    [Fact]
    public void Breadcrumb_ChildContent_Renders_Inside_Nav()
    {
        var cut = Render<Breadcrumb>(p => p.AddChildContent("<ol></ol>"));
        Assert.NotNull(cut.Find("nav ol"));
    }

    [Fact]
    public void Breadcrumb_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Breadcrumb).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── BreadcrumbList ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbList_Renders_Ol_Element()
    {
        var cut = Render<BreadcrumbList>();
        Assert.NotNull(cut.Find("ol"));
    }

    [Fact]
    public void BreadcrumbList_Has_Base_Classes()
    {
        var cut = Render<BreadcrumbList>();
        var classes = cut.Find("ol").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-wrap", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void BreadcrumbList_Custom_Class_Is_Appended()
    {
        var cut = Render<BreadcrumbList>(p => p.Add(x => x.Class, "my-list"));
        Assert.Contains("my-list", cut.Find("ol").ClassName);
    }

    [Fact]
    public void BreadcrumbList_AdditionalAttributes_PassThrough()
    {
        var cut = Render<BreadcrumbList>(p => p.AddUnmatched("data-testid", "bc-list"));
        Assert.Equal("bc-list", cut.Find("ol").GetAttribute("data-testid"));
    }

    [Fact]
    public void BreadcrumbList_ChildContent_Renders_Inside_Ol()
    {
        var cut = Render<BreadcrumbList>(p => p.AddChildContent("<li>Item</li>"));
        Assert.NotNull(cut.Find("ol li"));
    }

    [Fact]
    public void BreadcrumbList_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(BreadcrumbList).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── BreadcrumbItem ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbItem_Renders_Li_Element()
    {
        var cut = Render<BreadcrumbItem>();
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void BreadcrumbItem_Has_Base_Classes()
    {
        var cut = Render<BreadcrumbItem>();
        var classes = cut.Find("li").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("gap-1.5", classes);
    }

    [Fact]
    public void BreadcrumbItem_Custom_Class_Is_Appended()
    {
        var cut = Render<BreadcrumbItem>(p => p.Add(x => x.Class, "my-item"));
        Assert.Contains("my-item", cut.Find("li").ClassName);
    }

    [Fact]
    public void BreadcrumbItem_AdditionalAttributes_PassThrough()
    {
        var cut = Render<BreadcrumbItem>(p => p.AddUnmatched("data-testid", "bc-item"));
        Assert.Equal("bc-item", cut.Find("li").GetAttribute("data-testid"));
    }

    [Fact]
    public void BreadcrumbItem_ChildContent_Renders_Inside_Li()
    {
        var cut = Render<BreadcrumbItem>(p => p.AddChildContent("<a href=\"#\">Home</a>"));
        Assert.NotNull(cut.Find("li a"));
    }

    [Fact]
    public void BreadcrumbItem_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(BreadcrumbItem).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── BreadcrumbLink ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbLink_Renders_Anchor_Element()
    {
        var cut = Render<BreadcrumbLink>();
        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void BreadcrumbLink_Has_Base_Classes()
    {
        var cut = Render<BreadcrumbLink>();
        var classes = cut.Find("a").ClassName;
        Assert.Contains("transition-colors", classes);
        Assert.Contains("hover:text-foreground", classes);
    }

    [Fact]
    public void BreadcrumbLink_Href_Is_Set()
    {
        var cut = Render<BreadcrumbLink>(p => p.Add(x => x.Href, "/home"));
        Assert.Equal("/home", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void BreadcrumbLink_Custom_Class_Is_Appended()
    {
        var cut = Render<BreadcrumbLink>(p => p.Add(x => x.Class, "my-link"));
        Assert.Contains("my-link", cut.Find("a").ClassName);
    }

    [Fact]
    public void BreadcrumbLink_AdditionalAttributes_PassThrough()
    {
        var cut = Render<BreadcrumbLink>(p => p.AddUnmatched("data-testid", "bc-link"));
        Assert.Equal("bc-link", cut.Find("a").GetAttribute("data-testid"));
    }

    [Fact]
    public void BreadcrumbLink_ChildContent_Renders_Inside_Anchor()
    {
        var cut = Render<BreadcrumbLink>(p => p.AddChildContent("Home"));
        Assert.Contains("Home", cut.Find("a").TextContent);
    }

    [Fact]
    public void BreadcrumbLink_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(BreadcrumbLink).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── BreadcrumbPage ────────────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbPage_Renders_Span_Element()
    {
        var cut = Render<BreadcrumbPage>();
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void BreadcrumbPage_Has_Correct_ARIA_Attributes()
    {
        var cut = Render<BreadcrumbPage>();
        var span = cut.Find("span");
        Assert.Equal("link", span.GetAttribute("role"));
        Assert.Equal("true", span.GetAttribute("aria-disabled"));
        Assert.Equal("page", span.GetAttribute("aria-current"));
    }

    [Fact]
    public void BreadcrumbPage_Has_Base_Classes()
    {
        var cut = Render<BreadcrumbPage>();
        var classes = cut.Find("span").ClassName;
        Assert.Contains("font-normal", classes);
        Assert.Contains("text-foreground", classes);
    }

    [Fact]
    public void BreadcrumbPage_Custom_Class_Is_Appended()
    {
        var cut = Render<BreadcrumbPage>(p => p.Add(x => x.Class, "my-page"));
        Assert.Contains("my-page", cut.Find("span").ClassName);
    }

    [Fact]
    public void BreadcrumbPage_AdditionalAttributes_PassThrough()
    {
        var cut = Render<BreadcrumbPage>(p => p.AddUnmatched("data-testid", "bc-page"));
        Assert.Equal("bc-page", cut.Find("span").GetAttribute("data-testid"));
    }

    [Fact]
    public void BreadcrumbPage_ChildContent_Renders_Inside_Span()
    {
        var cut = Render<BreadcrumbPage>(p => p.AddChildContent("Breadcrumb"));
        Assert.Contains("Breadcrumb", cut.Find("span").TextContent);
    }

    [Fact]
    public void BreadcrumbPage_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(BreadcrumbPage).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── BreadcrumbSeparator ───────────────────────────────────────────────────

    [Fact]
    public void BreadcrumbSeparator_Renders_Li_Element()
    {
        var cut = Render<BreadcrumbSeparator>();
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void BreadcrumbSeparator_Has_Correct_ARIA_Attributes()
    {
        var cut = Render<BreadcrumbSeparator>();
        var li = cut.Find("li");
        Assert.Equal("presentation", li.GetAttribute("role"));
        Assert.Equal("true", li.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void BreadcrumbSeparator_Has_Base_Classes()
    {
        var cut = Render<BreadcrumbSeparator>();
        var classes = cut.Find("li").ClassName;
        Assert.Contains("[&>svg]:h-3.5", classes);
        Assert.Contains("[&>svg]:w-3.5", classes);
    }

    [Fact]
    public void BreadcrumbSeparator_Renders_Default_Svg_Icon()
    {
        var cut = Render<BreadcrumbSeparator>();
        Assert.NotNull(cut.Find("li svg"));
    }

    [Fact]
    public void BreadcrumbSeparator_Custom_ChildContent_Replaces_Icon()
    {
        var cut = Render<BreadcrumbSeparator>(p => p.AddChildContent("<span>/</span>"));
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find("li svg"));
        Assert.NotNull(cut.Find("li span"));
    }

    [Fact]
    public void BreadcrumbSeparator_Custom_Class_Is_Appended()
    {
        var cut = Render<BreadcrumbSeparator>(p => p.Add(x => x.Class, "my-sep"));
        Assert.Contains("my-sep", cut.Find("li").ClassName);
    }

    [Fact]
    public void BreadcrumbSeparator_AdditionalAttributes_PassThrough()
    {
        var cut = Render<BreadcrumbSeparator>(p => p.AddUnmatched("data-testid", "bc-sep"));
        Assert.Equal("bc-sep", cut.Find("li").GetAttribute("data-testid"));
    }

    [Fact]
    public void BreadcrumbSeparator_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(BreadcrumbSeparator).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
