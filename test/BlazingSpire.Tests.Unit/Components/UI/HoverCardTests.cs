using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class HoverCardTests : BlazingSpireTestBase
{
    // ── HoverCard ─────────────────────────────────────────────────────────────

    [Fact]
    public void HoverCard_Default_OpenDelay_Is_300()
    {
        var card = new HoverCard();
        Assert.Equal(300, card.OpenDelay);
    }

    [Fact]
    public void HoverCard_Default_CloseDelay_Is_200()
    {
        var card = new HoverCard();
        Assert.Equal(200, card.CloseDelay);
    }

    [Fact]
    public void HoverCard_Renders_ChildContent()
    {
        var cut = Render<HoverCard>(p => p.AddChildContent("<span>child</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── HoverCardContent ──────────────────────────────────────────────────────

    [Fact]
    public void HoverCardContent_Hidden_When_Closed()
    {
        var cut = Render<HoverCard>(p =>
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddChildContent("Card content")));

        Assert.Empty(cut.FindAll("[data-state]"));
    }

    [Fact]
    public void HoverCardContent_Visible_When_DefaultIsOpen()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddChildContent("Card content"));
        });

        Assert.NotNull(cut.Find("[data-state='open']"));
    }

    [Fact]
    public void HoverCardContent_Has_Data_State_Open()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddChildContent("Card content"));
        });

        AssertDataState(cut.Find("[data-state]"), "open");
    }

    [Fact]
    public void HoverCardContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddUnmatched("data-testid", "hovercard-panel"));
        });

        Assert.Equal("hovercard-panel", cut.Find("[data-state]").GetAttribute("data-testid"));
    }

    [Fact]
    public void HoverCardContent_Renders_ChildContent()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddChildContent("<strong>Rich content</strong>"));
        });

        Assert.NotNull(cut.Find("[data-state] strong"));
    }

    // ── HoverCardTrigger ──────────────────────────────────────────────────────

    [Fact]
    public void HoverCardTrigger_Renders_Span_By_Default()
    {
        var cut = Render<HoverCard>(p =>
            p.AddChildContent<HoverCardTrigger>(tp =>
                tp.AddChildContent("Hover me")));

        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void HoverCardTrigger_Renders_Anchor_When_Href_Provided()
    {
        var cut = Render<HoverCard>(p =>
            p.AddChildContent<HoverCardTrigger>(tp =>
            {
                tp.AddChildContent("@username");
                tp.AddUnmatched("href", "https://example.com");
            }));

        Assert.NotNull(cut.Find("a[href]"));
    }
}
