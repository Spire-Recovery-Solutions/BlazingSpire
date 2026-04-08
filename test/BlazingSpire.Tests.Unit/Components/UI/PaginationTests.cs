using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class PaginationTests : BlazingSpireTestBase
{
    // ── Pagination ────────────────────────────────────────────────────────────

    [Fact]
    public void Pagination_Renders_Nav_Element()
    {
        var cut = Render<Pagination>();
        Assert.NotNull(cut.Find("nav"));
    }

    [Fact]
    public void Pagination_Has_Role_Navigation()
    {
        var cut = Render<Pagination>();
        Assert.Equal("navigation", cut.Find("nav").GetAttribute("role"));
    }

    [Fact]
    public void Pagination_Has_AriaLabel_Pagination()
    {
        var cut = Render<Pagination>();
        Assert.Equal("pagination", cut.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void Pagination_Has_Base_Classes()
    {
        var cut = Render<Pagination>();
        var classes = cut.Find("nav").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("justify-center", classes);
    }

    [Fact]
    public void Pagination_Custom_Class_Is_Applied()
    {
        var cut = Render<Pagination>(p => p.Add(x => x.Class, "my-pagination"));
        Assert.Contains("my-pagination", cut.Find("nav").ClassName);
    }

    [Fact]
    public void Pagination_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Pagination>(p => p.AddUnmatched("data-testid", "pg"));
        Assert.Equal("pg", cut.Find("nav").GetAttribute("data-testid"));
    }

    [Fact]
    public void Pagination_ChildContent_Renders_Inside_Nav()
    {
        var cut = Render<Pagination>(p => p.AddChildContent("<ul></ul>"));
        Assert.NotNull(cut.Find("nav ul"));
    }

    [Fact]
    public void Pagination_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Pagination).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationContent ─────────────────────────────────────────────────────

    [Fact]
    public void PaginationContent_Renders_Ul_Element()
    {
        var cut = Render<PaginationContent>();
        Assert.NotNull(cut.Find("ul"));
    }

    [Fact]
    public void PaginationContent_Has_Base_Classes()
    {
        var cut = Render<PaginationContent>();
        var classes = cut.Find("ul").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-row", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("gap-1", classes);
    }

    [Fact]
    public void PaginationContent_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationContent>(p => p.Add(x => x.Class, "my-content"));
        Assert.Contains("my-content", cut.Find("ul").ClassName);
    }

    [Fact]
    public void PaginationContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<PaginationContent>(p => p.AddUnmatched("data-testid", "pg-content"));
        Assert.Equal("pg-content", cut.Find("ul").GetAttribute("data-testid"));
    }

    [Fact]
    public void PaginationContent_ChildContent_Renders_Inside_Ul()
    {
        var cut = Render<PaginationContent>(p => p.AddChildContent("<li>1</li>"));
        Assert.NotNull(cut.Find("ul li"));
    }

    [Fact]
    public void PaginationContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationItem ────────────────────────────────────────────────────────

    [Fact]
    public void PaginationItem_Renders_Li_Element()
    {
        var cut = Render<PaginationItem>();
        Assert.NotNull(cut.Find("li"));
    }

    [Fact]
    public void PaginationItem_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationItem>(p => p.Add(x => x.Class, "my-item"));
        Assert.Contains("my-item", cut.Find("li").ClassName);
    }

    [Fact]
    public void PaginationItem_AdditionalAttributes_PassThrough()
    {
        var cut = Render<PaginationItem>(p => p.AddUnmatched("data-testid", "pg-item"));
        Assert.Equal("pg-item", cut.Find("li").GetAttribute("data-testid"));
    }

    [Fact]
    public void PaginationItem_ChildContent_Renders_Inside_Li()
    {
        var cut = Render<PaginationItem>(p => p.AddChildContent("<a href=\"#\">1</a>"));
        Assert.NotNull(cut.Find("li a"));
    }

    [Fact]
    public void PaginationItem_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationItem).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationLink ────────────────────────────────────────────────────────

    [Fact]
    public void PaginationLink_With_Href_Renders_Anchor()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.Href, "/page/1"));
        Assert.NotNull(cut.Find("a"));
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find("button"));
    }

    [Fact]
    public void PaginationLink_Without_Href_Renders_Button()
    {
        var cut = Render<PaginationLink>();
        Assert.NotNull(cut.Find("button"));
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find("a"));
    }

    [Fact]
    public void PaginationLink_Active_Has_AriaCurrent_Page()
    {
        var cut = Render<PaginationLink>(p =>
        {
            p.Add(x => x.Href, "#");
            p.Add(x => x.IsActive, true);
        });
        Assert.Equal("page", cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void PaginationLink_Inactive_Has_No_AriaCurrent()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.Href, "#"));
        Assert.Null(cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void PaginationLink_Active_Has_Active_Classes()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.IsActive, true));
        var classes = cut.Find("button").ClassName;
        Assert.Contains("bg-primary", classes);
        Assert.Contains("text-primary-foreground", classes);
    }

    [Fact]
    public void PaginationLink_Inactive_Has_Inactive_Classes()
    {
        var cut = Render<PaginationLink>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("border", classes);
        Assert.Contains("border-input", classes);
    }

    [Fact]
    public void PaginationLink_Has_Base_Classes()
    {
        var cut = Render<PaginationLink>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("justify-center", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("w-10", classes);
    }

    [Fact]
    public void PaginationLink_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.Class, "my-link"));
        Assert.Contains("my-link", cut.Find("button").ClassName);
    }

    [Fact]
    public void PaginationLink_ChildContent_Renders()
    {
        var cut = Render<PaginationLink>(p => p.AddChildContent("2"));
        Assert.Contains("2", cut.Find("button").TextContent);
    }

    [Fact]
    public void PaginationLink_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationLink).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationPrevious ────────────────────────────────────────────────────

    [Fact]
    public void PaginationPrevious_Renders_Anchor()
    {
        var cut = Render<PaginationPrevious>();
        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void PaginationPrevious_Has_AriaLabel()
    {
        var cut = Render<PaginationPrevious>();
        Assert.Equal("Go to previous page", cut.Find("a").GetAttribute("aria-label"));
    }

    [Fact]
    public void PaginationPrevious_Sets_Href()
    {
        var cut = Render<PaginationPrevious>(p => p.Add(x => x.Href, "/page/1"));
        Assert.Equal("/page/1", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void PaginationPrevious_Contains_Previous_Text()
    {
        var cut = Render<PaginationPrevious>();
        Assert.Contains("Previous", cut.Find("a").TextContent);
    }

    [Fact]
    public void PaginationPrevious_Contains_Svg_Icon()
    {
        var cut = Render<PaginationPrevious>();
        Assert.NotNull(cut.Find("a svg"));
    }

    [Fact]
    public void PaginationPrevious_Has_Base_Classes()
    {
        var cut = Render<PaginationPrevious>();
        var classes = cut.Find("a").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("gap-1", classes);
        Assert.Contains("pl-2.5", classes);
    }

    [Fact]
    public void PaginationPrevious_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationPrevious>(p => p.Add(x => x.Class, "my-prev"));
        Assert.Contains("my-prev", cut.Find("a").ClassName);
    }

    [Fact]
    public void PaginationPrevious_AdditionalAttributes_PassThrough()
    {
        var cut = Render<PaginationPrevious>(p => p.AddUnmatched("data-testid", "pg-prev"));
        Assert.Equal("pg-prev", cut.Find("a").GetAttribute("data-testid"));
    }

    [Fact]
    public void PaginationPrevious_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationPrevious).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationNext ────────────────────────────────────────────────────────

    [Fact]
    public void PaginationNext_Renders_Anchor()
    {
        var cut = Render<PaginationNext>();
        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void PaginationNext_Has_AriaLabel()
    {
        var cut = Render<PaginationNext>();
        Assert.Equal("Go to next page", cut.Find("a").GetAttribute("aria-label"));
    }

    [Fact]
    public void PaginationNext_Sets_Href()
    {
        var cut = Render<PaginationNext>(p => p.Add(x => x.Href, "/page/3"));
        Assert.Equal("/page/3", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void PaginationNext_Contains_Next_Text()
    {
        var cut = Render<PaginationNext>();
        Assert.Contains("Next", cut.Find("a").TextContent);
    }

    [Fact]
    public void PaginationNext_Contains_Svg_Icon()
    {
        var cut = Render<PaginationNext>();
        Assert.NotNull(cut.Find("a svg"));
    }

    [Fact]
    public void PaginationNext_Has_Base_Classes()
    {
        var cut = Render<PaginationNext>();
        var classes = cut.Find("a").ClassName;
        Assert.Contains("inline-flex", classes);
        Assert.Contains("gap-1", classes);
        Assert.Contains("pr-2.5", classes);
    }

    [Fact]
    public void PaginationNext_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationNext>(p => p.Add(x => x.Class, "my-next"));
        Assert.Contains("my-next", cut.Find("a").ClassName);
    }

    [Fact]
    public void PaginationNext_AdditionalAttributes_PassThrough()
    {
        var cut = Render<PaginationNext>(p => p.AddUnmatched("data-testid", "pg-next"));
        Assert.Equal("pg-next", cut.Find("a").GetAttribute("data-testid"));
    }

    [Fact]
    public void PaginationNext_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationNext).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── PaginationEllipsis ────────────────────────────────────────────────────

    [Fact]
    public void PaginationEllipsis_Renders_Span_With_AriaHidden()
    {
        var cut = Render<PaginationEllipsis>();
        var span = cut.Find("span");
        Assert.Equal("true", span.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void PaginationEllipsis_Contains_Svg_Icon()
    {
        var cut = Render<PaginationEllipsis>();
        Assert.NotNull(cut.Find("span svg"));
    }

    [Fact]
    public void PaginationEllipsis_Has_Base_Classes()
    {
        var cut = Render<PaginationEllipsis>();
        var classes = cut.Find("span").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("h-9", classes);
        Assert.Contains("w-9", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("justify-center", classes);
    }

    [Fact]
    public void PaginationEllipsis_Custom_Class_Is_Applied()
    {
        var cut = Render<PaginationEllipsis>(p => p.Add(x => x.Class, "my-ellipsis"));
        Assert.Contains("my-ellipsis", cut.Find("span").ClassName);
    }

    [Fact]
    public void PaginationEllipsis_AdditionalAttributes_PassThrough()
    {
        var cut = Render<PaginationEllipsis>(p => p.AddUnmatched("data-testid", "pg-ellipsis"));
        Assert.Equal("pg-ellipsis", cut.Find("span").GetAttribute("data-testid"));
    }

    [Fact]
    public void PaginationEllipsis_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(PaginationEllipsis).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
