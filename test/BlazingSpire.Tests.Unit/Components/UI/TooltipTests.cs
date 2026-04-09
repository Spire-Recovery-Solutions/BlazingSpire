using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TooltipTests : BlazingSpireTestBase
{
    // ── TooltipProvider ───────────────────────────────────────────────────────

    [Fact]
    public void TooltipProvider_Renders_ChildContent()
    {
        var cut = Render<TooltipProvider>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void TooltipProvider_Default_DelayDuration_Is_200()
    {
        var provider = new TooltipProvider();
        Assert.Equal(200, provider.DelayDuration);
    }

    // ── Tooltip ───────────────────────────────────────────────────────────────

    [Fact]
    public void Tooltip_Renders_ChildContent()
    {
        var cut = Render<Tooltip>(p => p.AddChildContent("<span>inner</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── TooltipContent ────────────────────────────────────────────────────────

    [Fact]
    public void TooltipContent_Hidden_When_Closed()
    {
        var cut = Render<Tooltip>(p =>
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text")));

        Assert.Empty(cut.FindAll("[role=tooltip]"));
    }

    [Fact]
    public void TooltipContent_Visible_When_DefaultIsOpen()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        Assert.NotNull(cut.Find("[role=tooltip]"));
    }

    [Fact]
    public void TooltipContent_Has_Role_Tooltip()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        AssertRole(cut.Find("[role=tooltip]"), "tooltip");
    }

    [Fact]
    public void TooltipContent_Has_Data_State_Open()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        AssertDataState(cut.Find("[role=tooltip]"), "open");
    }

    [Fact]
    public void TooltipContent_Has_DataSide_Attribute()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        Assert.Equal("bottom", cut.Find("[role=tooltip]").GetAttribute("data-side"));
    }

    [Fact]
    public void TooltipContent_Renders_ChildContent()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("<em>Tip text</em>"));
        });

        Assert.NotNull(cut.Find("[role=tooltip] em"));
    }

    [Fact]
    public void TooltipContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddUnmatched("data-testid", "tip-panel"));
        });

        Assert.Equal("tip-panel", cut.Find("[role=tooltip]").GetAttribute("data-testid"));
    }

    // ── TooltipTrigger ────────────────────────────────────────────────────────

    [Fact]
    public void TooltipTrigger_Renders_Span()
    {
        var cut = Render<Tooltip>(p =>
            p.AddChildContent<TooltipTrigger>(tp =>
                tp.AddChildContent("Hover")));

        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void TooltipTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Tooltip>(p =>
            p.AddChildContent<TooltipTrigger>(tp =>
                tp.AddUnmatched("data-testid", "trigger")));

        Assert.Equal("trigger", cut.Find("span").GetAttribute("data-testid"));
    }
}
