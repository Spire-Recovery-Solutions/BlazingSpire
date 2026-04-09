using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class PaginationTests : BlazingSpireTestBase
{
    // ── Pagination nav semantics ──────────────────────────────────────────────

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
            p.Add(x => x.Href, "/page/2");
            p.Add(x => x.IsActive, true);
        });
        Assert.Equal("page", cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void PaginationLink_Inactive_Has_No_AriaCurrent()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.Href, "/page/1"));
        Assert.Null(cut.Find("a").GetAttribute("aria-current"));
    }

    [Fact]
    public void PaginationLink_Href_Is_Set_On_Anchor()
    {
        var cut = Render<PaginationLink>(p => p.Add(x => x.Href, "/page/3"));
        Assert.Equal("/page/3", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void PaginationLink_ChildContent_Renders()
    {
        var cut = Render<PaginationLink>(p => p.AddChildContent("2"));
        Assert.Contains("2", cut.Find("button").TextContent);
    }

    // ── PaginationPrevious ────────────────────────────────────────────────────

    [Fact]
    public void PaginationPrevious_Has_AriaLabel_For_Previous_Page()
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

    // ── PaginationNext ────────────────────────────────────────────────────────

    [Fact]
    public void PaginationNext_Has_AriaLabel_For_Next_Page()
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

    // ── PaginationEllipsis ────────────────────────────────────────────────────

    [Fact]
    public void PaginationEllipsis_Renders_With_AriaHidden()
    {
        var cut = Render<PaginationEllipsis>();
        AssertAriaHidden(cut.Find("span"), true);
    }
}
