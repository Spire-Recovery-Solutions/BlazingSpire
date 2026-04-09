using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DialogTests : BlazingSpireTestBase
{
    // ── Visibility ────────────────────────────────────────────────────────────

    [Fact]
    public void DialogContent_Is_Hidden_When_Closed()
    {
        var cut = Render<Dialog>(p =>
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void DialogContent_Is_Visible_When_DefaultIsOpen_True()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    // ── ARIA ──────────────────────────────────────────────────────────────────

    [Fact]
    public void DialogContent_Has_Role_Dialog()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("dialog", cut.Find("[role=dialog]").GetAttribute("role"));
    }

    [Fact]
    public void DialogContent_Has_Aria_Modal_True()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        AssertAriaModal(cut.Find("[role=dialog]"), true);
    }

    [Fact]
    public void DialogContent_Has_Data_State_Open()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        AssertDataState(cut.Find("[role=dialog]"), "open");
    }

    [Fact]
    public void DialogTitle_Id_Matches_Aria_LabelledBy()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogTitle>(tp =>
                    tp.AddChildContent("My Title")));
        });

        var dialog = cut.Find("[role=dialog]");
        var labelledBy = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(labelledBy);
        Assert.Equal(labelledBy, cut.Find("h2").GetAttribute("id"));
    }

    [Fact]
    public void DialogDescription_Id_Matches_Aria_DescribedBy()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogDescription>(dp =>
                    dp.AddChildContent("My Description")));
        });

        var dialog = cut.Find("[role=dialog]");
        var describedBy = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Equal(describedBy, cut.Find("p").GetAttribute("id"));
    }

    // ── Correct HTML elements ─────────────────────────────────────────────────

    [Fact]
    public void DialogTitle_Renders_H2()
    {
        var cut = Render<DialogTitle>(p => p.AddChildContent("Title"));
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void DialogDescription_Renders_Paragraph()
    {
        var cut = Render<DialogDescription>(p => p.AddChildContent("Description"));
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void DialogClose_Renders_Button_With_Type_Button()
    {
        var cut = Render<DialogClose>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void DialogClose_Has_AriaLabel_Close()
    {
        var cut = Render<DialogClose>();
        AssertAriaLabel(cut.Find("button"), "Close");
    }

    // ── Trigger / close behavior ──────────────────────────────────────────────

    [Fact]
    public void DialogTrigger_Click_Opens_Dialog()
    {
        var cut = Render<Dialog>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Body")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=dialog]"));
        cut.Find("div").Click();
        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void DialogClose_Click_Closes_Dialog()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogClose>());
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
        cut.Find("button").Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    // ── Controlled mode ───────────────────────────────────────────────────────

    [Fact]
    public void Dialog_Controlled_Mode_IsOpen_True_Shows_Content()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => { }));
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void Dialog_Controlled_Mode_IsOpen_False_Hides_Content()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => { }));
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Dialog_Controlled_Mode_IsOpenChanged_Fires_On_Close_Attempt()
    {
        bool? receivedValue = null;
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => receivedValue = v));
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogClose>());
        });

        cut.Find("button").Click();
        Assert.Equal(false, receivedValue);
    }

    [Fact]
    public void Dialog_Controlled_Mode_IsOpenChanged_Fires_On_Trigger_Click()
    {
        bool? receivedValue = null;
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => receivedValue = v));
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();
            });
        });

        cut.Find("div").Click();
        Assert.Equal(true, receivedValue);
    }
}
