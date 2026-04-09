using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CollapsibleTests : BlazingSpireTestBase
{
    // ── Collapsible root ──────────────────────────────────────────────────────

    [Fact]
    public void DataState_Is_Closed_By_Default()
    {
        var cut = Render<Collapsible>();
        AssertDataState(cut.Find("div"), "closed");
    }

    [Fact]
    public void IsOpen_True_Sets_DataState_Open()
    {
        var cut = Render<Collapsible>(p => p.Add(x => x.IsOpen, true));
        AssertDataState(cut.Find("div"), "open");
    }

    [Fact]
    public async Task ToggleAsync_Switches_DataState_To_Open()
    {
        var cut = Render<Collapsible>();
        await cut.InvokeAsync(cut.Instance.ToggleAsync);
        AssertDataState(cut.Find("div"), "open");
    }

    [Fact]
    public async Task ToggleAsync_Invokes_IsOpenChanged_With_True()
    {
        bool? received = null;
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => received = v)));

        await cut.InvokeAsync(cut.Instance.ToggleAsync);

        Assert.True(received);
    }

    // ── CollapsibleContent visibility ─────────────────────────────────────────

    [Fact]
    public void Content_Is_Hidden_When_Parent_Closed()
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
    public void Content_Is_Visible_When_Parent_Open()
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
    public void Content_Has_DataState_Open_When_Visible()
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

        var openEls = cut.FindAll("[data-state='open']");
        Assert.Equal(2, openEls.Count); // root div + content div
    }

    // ── CollapsibleTrigger interaction ────────────────────────────────────────

    [Fact]
    public void Trigger_Click_Opens_Content()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "data-testid", "trigger");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Toggle")));
                b.CloseComponent();

                b.OpenComponent<CollapsibleContent>(3);
                b.AddAttribute(4, "ChildContent", (RenderFragment)(b2 =>
                {
                    b2.OpenElement(0, "p");
                    b2.AddContent(1, "Body");
                    b2.CloseElement();
                }));
                b.CloseComponent();
            })));

        Assert.Empty(cut.FindAll("p"));
        cut.Find("[data-testid=trigger]").Click();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void Trigger_Click_Updates_DataState_To_Open()
    {
        var cut = Render<Collapsible>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "data-testid", "trigger");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Toggle")));
                b.CloseComponent();
            })));

        AssertDataState(cut.Find("div[data-state]"), "closed");
        cut.Find("[data-testid=trigger]").Click();
        AssertDataState(cut.Find("div[data-state]"), "open");
    }

    [Fact]
    public void Controlled_Mode_Fires_IsOpenChanged()
    {
        bool? received = null;
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged,
                EventCallback.Factory.Create<bool>(this, v => received = v));
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CollapsibleTrigger>(0);
                b.AddAttribute(1, "data-testid", "trigger");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Toggle")));
                b.CloseComponent();
            }));
        });

        cut.Find("[data-testid=trigger]").Click();
        Assert.True(received);
    }
}
