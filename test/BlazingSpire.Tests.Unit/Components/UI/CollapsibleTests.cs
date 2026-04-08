using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CollapsibleTests : BlazingSpireTestBase
{
    // ── Collapsible ───────────────────────────────────────────────────────────

    [Fact]
    public void Collapsible_Renders_Div_With_DataState_Closed()
    {
        var cut = Render<Collapsible>();
        Assert.Equal("closed", cut.Find("div").GetAttribute("data-state"));
    }

    [Fact]
    public void Collapsible_Renders_Div_With_DataState_Open()
    {
        var cut = Render<Collapsible>(p => p.Add(x => x.IsOpen, true));
        Assert.Equal("open", cut.Find("div").GetAttribute("data-state"));
    }

    [Fact]
    public void Collapsible_Has_Base_Classes()
    {
        var cut = Render<Collapsible>();
        Assert.Contains("space-y-2", cut.Find("div").ClassName);
    }

    [Fact]
    public void Collapsible_Custom_Class_Is_Appended()
    {
        var cut = Render<Collapsible>(p => p.Add(x => x.Class, "my-collapsible"));
        Assert.Contains("my-collapsible", cut.Find("div").ClassName);
    }

    [Fact]
    public void Collapsible_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Collapsible>(p => p.AddUnmatched("data-testid", "collapsible"));
        Assert.Equal("collapsible", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void Collapsible_ChildContent_Renders()
    {
        var cut = Render<Collapsible>(p => p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public async Task Collapsible_ToggleAsync_Changes_IsOpen_State()
    {
        var cut = Render<Collapsible>();
        Assert.Equal("closed", cut.Find("div").GetAttribute("data-state"));

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.Equal("open", cut.Find("div").GetAttribute("data-state"));
    }

    [Fact]
    public async Task Collapsible_ToggleAsync_Invokes_IsOpenChanged()
    {
        bool? received = null;
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.Equal(true, received);
    }

    [Fact]
    public void Collapsible_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Collapsible).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CollapsibleContent ────────────────────────────────────────────────────

    [Fact]
    public void CollapsibleContent_Hidden_When_Parent_Closed()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleContent>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "p");
                    b2.AddContent(1, "Hidden");
                    b2.CloseElement();
                }));
                b.CloseComponent();
            })));

        Assert.Empty(cut.FindAll("p"));
    }

    [Fact]
    public void CollapsibleContent_Visible_When_Parent_Open()
    {
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleContent>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "p");
                    b2.AddContent(1, "Visible");
                    b2.CloseElement();
                }));
                b.CloseComponent();
            }));
        });

        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void CollapsibleContent_Has_DataState_Open_When_Visible()
    {
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleContent>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "x")));
                b.CloseComponent();
            }));
        });

        // Both Collapsible root and CollapsibleContent have data-state="open" when open
        var openEls = cut.FindAll("[data-state='open']");
        Assert.Equal(2, openEls.Count);
    }

    [Fact]
    public void CollapsibleContent_Has_Base_Classes()
    {
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleContent>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "x")));
                b.CloseComponent();
            }));
        });

        // Last [data-state='open'] is the CollapsibleContent div (root is first)
        var contentEl = cut.FindAll("[data-state='open']").Last();
        Assert.Contains("space-y-2", contentEl.ClassName);
    }

    [Fact]
    public void CollapsibleContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleContent>(0);
                b.AddAttribute(1, nameof(CollapsibleContent.Class), "extra");
                b.AddAttribute(2, nameof(CollapsibleContent.ChildContent), (RenderFragment)(b2 => b2.AddContent(0, "x")));
                b.CloseComponent();
            }));
        });

        // Last [data-state='open'] is the CollapsibleContent div (root is first)
        var contentEl = cut.FindAll("[data-state='open']").Last();
        Assert.Contains("extra", contentEl.ClassName);
    }

    [Fact]
    public void CollapsibleContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CollapsibleContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── CollapsibleTrigger ────────────────────────────────────────────────────

    [Fact]
    public async Task CollapsibleTrigger_Click_Toggles_Parent()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "button");
                    b2.AddContent(1, "Toggle");
                    b2.CloseElement();
                }));
                b.CloseComponent();

                b.OpenComponent<CollapsibleContent>(2);
                b.AddAttribute(3, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "p");
                    b2.AddContent(1, "Body");
                    b2.CloseElement();
                }));
                b.CloseComponent();
            })));

        // closed initially, content hidden
        Assert.Empty(cut.FindAll("p"));

        // click the trigger div
        await cut.Find("button").ClickAsync(new());

        // now open, content visible
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void CollapsibleTrigger_ChildContent_Renders()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "button");
                    b2.AddContent(1, "Click");
                    b2.CloseElement();
                }));
                b.CloseComponent();
            })));

        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void CollapsibleTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "data-testid", "trigger");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "x")));
                b.CloseComponent();
            })));

        Assert.Equal("trigger", cut.Find("[data-testid]").GetAttribute("data-testid"));
    }

    [Fact]
    public void CollapsibleTrigger_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CollapsibleTrigger).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
