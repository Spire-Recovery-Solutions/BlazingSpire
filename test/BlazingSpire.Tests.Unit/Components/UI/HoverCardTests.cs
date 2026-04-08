using System.Reflection;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class HoverCardTests : BlazingSpireTestBase
{
    // ── HoverCard ─────────────────────────────────────────────────────────────

    [Fact]
    public void HoverCard_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(HoverCard).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void HoverCard_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(HoverCard).IsAssignableTo(typeof(OverlayBase)));
    }

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

    // ── Overlay configuration ──────────────────────────────────────────────────

    [Fact]
    public void HoverCard_ShouldCloseOnEscape_Is_True()
    {
        var prop = typeof(HoverCard).GetProperty("ShouldCloseOnEscape", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new HoverCard())!);
    }

    [Fact]
    public void HoverCard_ShouldCloseOnInteractOutside_Is_False()
    {
        var prop = typeof(HoverCard).GetProperty("ShouldCloseOnInteractOutside", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.False((bool)prop!.GetValue(new HoverCard())!);
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
    public void HoverCardContent_Visible_When_Open()
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

        Assert.Equal("open", cut.Find("[data-state]").GetAttribute("data-state"));
    }

    [Fact]
    public void HoverCardContent_Has_Base_Classes()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.AddChildContent("Card content"));
        });

        var classes = cut.Find("[data-state]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("w-64", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("p-4", classes);
    }

    [Fact]
    public void HoverCardContent_Custom_Class_Is_Appended()
    {
        var cut = Render<HoverCard>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<HoverCardContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[data-state]").ClassName);
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

    [Fact]
    public void HoverCardTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<HoverCard>(p =>
            p.AddChildContent<HoverCardTrigger>(tp =>
                tp.Add(x => x.Class, "trigger-class")));

        Assert.Contains("trigger-class", cut.Find("span").ClassName);
    }

    [Fact]
    public void HoverCardTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = Render<HoverCard>(p =>
            p.AddChildContent<HoverCardTrigger>(tp =>
                tp.AddUnmatched("data-testid", "hc-trigger")));

        Assert.Equal("hc-trigger", cut.Find("span").GetAttribute("data-testid"));
    }
}
