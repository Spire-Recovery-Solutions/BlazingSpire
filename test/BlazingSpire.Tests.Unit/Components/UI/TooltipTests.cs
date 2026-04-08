using BlazingSpire.Demo.Components.Shared;
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
    public void Tooltip_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(Tooltip).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void Tooltip_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(Tooltip).IsAssignableTo(typeof(OverlayBase)));
    }

    [Fact]
    public void Tooltip_Renders_CascadingValue()
    {
        var cut = Render<Tooltip>(p => p.AddChildContent("<span>inner</span>"));
        Assert.NotNull(cut);
    }

    // ── TooltipContent ────────────────────────────────────────────────────────

    [Fact]
    public void TooltipContent_Hidden_When_Tooltip_Closed()
    {
        var cut = Render<Tooltip>(p =>
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text")));

        Assert.Empty(cut.FindAll("[role=tooltip]"));
    }

    [Fact]
    public void TooltipContent_Visible_When_Tooltip_Open()
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

        Assert.Equal("tooltip", cut.Find("[role=tooltip]").GetAttribute("role"));
    }

    [Fact]
    public void TooltipContent_Has_Data_State_Open_When_Open()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        Assert.Equal("open", cut.Find("[role=tooltip]").GetAttribute("data-state"));
    }

    [Fact]
    public void TooltipContent_Has_Base_Classes()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.AddChildContent("Tip text"));
        });

        var classes = cut.Find("[role=tooltip]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void TooltipContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Tooltip>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<TooltipContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=tooltip]").ClassName);
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
    public void TooltipTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<Tooltip>(p =>
            p.AddChildContent<TooltipTrigger>(tp =>
                tp.Add(x => x.Class, "trigger-class")));

        Assert.Contains("trigger-class", cut.Find("span").ClassName);
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
