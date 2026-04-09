using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AlertDialogTests : BlazingSpireTestBase
{
    // ── Visibility ────────────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogContent_Is_Hidden_When_Closed()
    {
        var cut = Render<AlertDialog>(p =>
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void AlertDialogContent_Is_Visible_When_DefaultIsOpen_True()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=alertdialog]"));
    }

    // ── ARIA ──────────────────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogContent_Has_Role_Alertdialog()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("alertdialog", cut.Find("[role=alertdialog]").GetAttribute("role"));
    }

    [Fact]
    public void AlertDialogContent_Has_Aria_Modal_True()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        AssertAriaModal(cut.Find("[role=alertdialog]"), true);
    }

    [Fact]
    public void AlertDialogContent_Has_Data_State_Open()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        AssertDataState(cut.Find("[role=alertdialog]"), "open");
    }

    [Fact]
    public void AlertDialogTitle_Id_Matches_Aria_LabelledBy()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogTitle>(tp =>
                    tp.AddChildContent("My Title")));
        });

        var dialog = cut.Find("[role=alertdialog]");
        var labelledBy = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(labelledBy);
        Assert.Equal(labelledBy, cut.Find("h2").GetAttribute("id"));
    }

    [Fact]
    public void AlertDialogDescription_Id_Matches_Aria_DescribedBy()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogDescription>(dp =>
                    dp.AddChildContent("My Description")));
        });

        var dialog = cut.Find("[role=alertdialog]");
        var describedBy = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Equal(describedBy, cut.Find("p").GetAttribute("id"));
    }

    // ── Correct HTML elements ─────────────────────────────────────────────────

    [Fact]
    public void AlertDialogTitle_Renders_H2()
    {
        var cut = Render<AlertDialogTitle>(p => p.AddChildContent("Title"));
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void AlertDialogDescription_Renders_Paragraph()
    {
        var cut = Render<AlertDialogDescription>(p => p.AddChildContent("Description"));
        Assert.NotNull(cut.Find("p"));
    }

    // ── Action and Cancel buttons ─────────────────────────────────────────────

    [Fact]
    public void AlertDialogAction_Renders_Button_With_Type_Button()
    {
        var cut = Render<AlertDialogAction>(p => p.AddChildContent("Continue"));
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AlertDialogAction_Click_Invokes_OnClick_And_Closes()
    {
        var onClickCalled = false;
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogAction>(ap =>
                {
                    ap.AddChildContent("Continue");
                    ap.Add(x => x.OnClick, EventCallback.Factory.Create(this, () => onClickCalled = true));
                }));
        });

        Assert.NotNull(cut.Find("[role=alertdialog]"));
        cut.Find("button").Click();
        Assert.True(onClickCalled);
        Assert.Empty(cut.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void AlertDialogCancel_Renders_Button_With_Type_Button()
    {
        var cut = Render<AlertDialogCancel>(p => p.AddChildContent("Cancel"));
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AlertDialogCancel_Click_Closes()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogCancel>(ap =>
                    ap.AddChildContent("Cancel")));
        });

        Assert.NotNull(cut.Find("[role=alertdialog]"));
        cut.Find("button").Click();
        Assert.Empty(cut.FindAll("[role=alertdialog]"));
    }

    // ── Trigger behavior ──────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogTrigger_Click_Opens_Dialog()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<AlertDialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<AlertDialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Body")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=alertdialog]"));
        cut.Find("div").Click();
        Assert.NotNull(cut.Find("[role=alertdialog]"));
    }

    // ── Controlled mode ───────────────────────────────────────────────────────

    [Fact]
    public void AlertDialog_Controlled_Mode_IsOpen_True_Shows_Content()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => { }));
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=alertdialog]"));
    }

    [Fact]
    public void AlertDialog_Controlled_Mode_IsOpenChanged_Fires_On_Action_Click()
    {
        bool? receivedValue = null;
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => receivedValue = v));
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogAction>(ap =>
                    ap.AddChildContent("Continue")));
        });

        cut.Find("button").Click();
        Assert.Equal(false, receivedValue);
    }
}
