using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class PopoverTests : BlazingSpireTestBase
{
    // ── Popover ───────────────────────────────────────────────────────────────

    [Fact]
    public void Popover_Renders_CascadingValue()
    {
        var cut = Render<Popover>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Popover_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(Popover).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void Popover_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(Popover).IsAssignableTo(typeof(OverlayBase)));
    }

    // ── PopoverContent ────────────────────────────────────────────────────────

    [Fact]
    public void PopoverContent_Hidden_When_Closed()
    {
        var cut = Render<Popover>(p =>
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>")));

        Assert.Empty(cut.FindAll("[data-state=open]"));
    }

    [Fact]
    public void PopoverContent_Visible_When_Open()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        Assert.NotEmpty(cut.FindAll("[data-state=open]"));
    }

    [Fact]
    public void PopoverContent_Has_Base_Classes()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        var classes = cut.Find("[data-state]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-md", classes);
    }

    [Fact]
    public void PopoverContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[data-state]").ClassName);
    }

    [Fact]
    public void PopoverContent_ChildContent_Renders()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<span id=\"inner\">hello</span>"));
        });

        Assert.NotNull(cut.Find("#inner"));
    }

    [Fact]
    public void PopoverContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddUnmatched("data-testid", "pop-panel"));
        });

        Assert.Equal("pop-panel", cut.Find("[data-state]").GetAttribute("data-testid"));
    }

    [Fact]
    public void PopoverContent_Has_DataState_Open_When_Open()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        Assert.Equal("open", cut.Find("[data-state]").GetAttribute("data-state"));
    }

    [Fact]
    public void PopoverContent_Has_DataSide_Attribute()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        Assert.Equal("bottom", cut.Find("[data-state]").GetAttribute("data-side"));
    }

    // ── PopoverTrigger ────────────────────────────────────────────────────────

    [Fact]
    public void PopoverTrigger_Renders_Div()
    {
        var cut = Render<Popover>(p =>
            p.AddChildContent<PopoverTrigger>(tp =>
                tp.AddChildContent("<span>Open</span>")));

        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void PopoverTrigger_ChildContent_Renders()
    {
        var cut = Render<Popover>(p =>
            p.AddChildContent<PopoverTrigger>(tp =>
                tp.AddChildContent("<span id=\"trigger-inner\">Open</span>")));

        Assert.NotNull(cut.Find("#trigger-inner"));
    }

    [Fact]
    public void PopoverTrigger_Click_Opens_Popover()
    {
        var cut = Render<Popover>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<PopoverTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<PopoverContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Body")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[data-state=open]"));
        cut.Find("div").Click();
        Assert.NotEmpty(cut.FindAll("[data-state=open]"));
    }
}
