using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class PopoverTests : BlazingSpireTestBase
{
    // ── Popover ───────────────────────────────────────────────────────────────

    [Fact]
    public void Popover_Renders_ChildContent()
    {
        var cut = Render<Popover>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("span"));
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
    public void PopoverContent_Visible_When_DefaultIsOpen()
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
    public void PopoverContent_Has_DataState_Open()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<PopoverContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        AssertDataState(cut.Find("[data-state]"), "open");
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

    [Fact]
    public void PopoverContent_Renders_ChildContent()
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

    // ── PopoverTrigger ────────────────────────────────────────────────────────

    [Fact]
    public void PopoverTrigger_Renders_ChildContent()
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

    [Fact]
    public void PopoverTrigger_Click_Closes_Open_Popover()
    {
        var cut = Render<Popover>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
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

        Assert.NotEmpty(cut.FindAll("[data-state=open]"));
        cut.Find("div").Click();
        Assert.Empty(cut.FindAll("[data-state=open]"));
    }
}
