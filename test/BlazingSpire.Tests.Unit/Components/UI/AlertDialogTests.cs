using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AlertDialogTests : BlazingSpireTestBase
{
    // ── AlertDialog ──────────────────────────────────────────────────────────

    [Fact]
    public void AlertDialog_Renders_CascadingValue()
    {
        var cut = Render<AlertDialog>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void AlertDialog_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(AlertDialog).IsAssignableTo(typeof(OverlayBase)));
    }

    [Fact]
    public void AlertDialog_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AlertDialog).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void AlertDialog_ShouldCloseOnEscape_Is_False()
    {
        var dialog = new AlertDialog();
        // Use reflection to access the protected property
        var prop = typeof(AlertDialog).GetProperty("ShouldCloseOnEscape",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.False((bool)prop!.GetValue(dialog)!);
    }

    [Fact]
    public void AlertDialog_ShouldCloseOnInteractOutside_Is_False()
    {
        var dialog = new AlertDialog();
        var prop = typeof(AlertDialog).GetProperty("ShouldCloseOnInteractOutside",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.False((bool)prop!.GetValue(dialog)!);
    }

    // ── AlertDialogContent ────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogContent_Hidden_When_Dialog_Closed()
    {
        var cut = Render<AlertDialog>(p =>
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=alertdialog]"));
    }

    [Fact]
    public void AlertDialogContent_Visible_When_Dialog_Open()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=alertdialog]"));
    }

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

        Assert.Equal("true", cut.Find("[role=alertdialog]").GetAttribute("aria-modal"));
    }

    [Fact]
    public void AlertDialogContent_Has_Base_Classes()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=alertdialog]").ClassName;
        Assert.Contains("fixed", classes);
        Assert.Contains("z-50", classes);
        Assert.Contains("max-w-lg", classes);
    }

    [Fact]
    public void AlertDialogContent_Custom_Class_Is_Appended()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=alertdialog]").ClassName);
    }

    [Fact]
    public void AlertDialogContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddUnmatched("data-testid", "alert-panel"));
        });

        Assert.Equal("alert-panel", cut.Find("[role=alertdialog]").GetAttribute("data-testid"));
    }

    [Fact]
    public void AlertDialogContent_Renders_Backdrop_When_Open()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotEmpty(cut.FindAll(".bg-black\\/80"));
    }

    // ── AlertDialogHeader ─────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogHeader_Renders_Div()
    {
        var cut = Render<AlertDialogHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void AlertDialogHeader_Has_Base_Classes()
    {
        var cut = Render<AlertDialogHeader>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("space-y-1.5", classes);
    }

    [Fact]
    public void AlertDialogHeader_Renders_ChildContent()
    {
        var cut = Render<AlertDialogHeader>(p => p.AddChildContent("<span>Header</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── AlertDialogFooter ─────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogFooter_Renders_Div()
    {
        var cut = Render<AlertDialogFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void AlertDialogFooter_Has_Base_Classes()
    {
        var cut = Render<AlertDialogFooter>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("sm:justify-end", classes);
    }

    // ── AlertDialogTitle ──────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogTitle_Renders_H2()
    {
        var cut = Render<AlertDialogTitle>();
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void AlertDialogTitle_Has_Base_Classes()
    {
        var cut = Render<AlertDialogTitle>();
        var classes = cut.Find("h2").ClassName;
        Assert.Contains("text-lg", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("tracking-tight", classes);
    }

    [Fact]
    public void AlertDialogTitle_Id_Matches_ParentDialog_TitleId()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogTitle>(tp =>
                    tp.AddChildContent("Title")));
        });

        var h2 = cut.Find("h2");
        var dialog = cut.Find("[role=alertdialog]");
        var expectedId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, h2.GetAttribute("id"));
    }

    // ── AlertDialogDescription ────────────────────────────────────────────────

    [Fact]
    public void AlertDialogDescription_Renders_P()
    {
        var cut = Render<AlertDialogDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void AlertDialogDescription_Has_Base_Classes()
    {
        var cut = Render<AlertDialogDescription>();
        var classes = cut.Find("p").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void AlertDialogDescription_Id_Matches_ParentDialog_DescriptionId()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogDescription>(dp =>
                    dp.AddChildContent("Description")));
        });

        var p = cut.Find("p");
        var dialog = cut.Find("[role=alertdialog]");
        var expectedId = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, p.GetAttribute("id"));
    }

    // ── AlertDialogAction ─────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogAction_Renders_Button()
    {
        var cut = Render<AlertDialogAction>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void AlertDialogAction_Has_Type_Button()
    {
        var cut = Render<AlertDialogAction>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AlertDialogAction_Has_Base_Classes()
    {
        var cut = Render<AlertDialogAction>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("bg-primary", classes);
        Assert.Contains("text-primary-foreground", classes);
        Assert.Contains("rounded-full", classes);
    }

    [Fact]
    public void AlertDialogAction_Custom_Class_Is_Appended()
    {
        var cut = Render<AlertDialogAction>(p => p.Add(x => x.Class, "extra-class"));
        Assert.Contains("extra-class", cut.Find("button").ClassName);
    }

    [Fact]
    public void AlertDialogAction_Click_Invokes_OnClick_And_Closes_Dialog()
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

    // ── AlertDialogCancel ─────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogCancel_Renders_Button()
    {
        var cut = Render<AlertDialogCancel>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void AlertDialogCancel_Has_Type_Button()
    {
        var cut = Render<AlertDialogCancel>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void AlertDialogCancel_Has_Base_Classes()
    {
        var cut = Render<AlertDialogCancel>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("border", classes);
        Assert.Contains("border-input", classes);
        Assert.Contains("bg-background", classes);
    }

    [Fact]
    public void AlertDialogCancel_Custom_Class_Is_Appended()
    {
        var cut = Render<AlertDialogCancel>(p => p.Add(x => x.Class, "cancel-class"));
        Assert.Contains("cancel-class", cut.Find("button").ClassName);
    }

    [Fact]
    public void AlertDialogCancel_Click_Closes_Dialog()
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

    // ── AlertDialogTrigger ────────────────────────────────────────────────────

    [Fact]
    public void AlertDialogTrigger_Click_Opens_Dialog()
    {
        var cut = Render<AlertDialog>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<AlertDialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<AlertDialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Body")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=alertdialog]"));
        cut.Find("div").Click();
        Assert.NotNull(cut.Find("[role=alertdialog]"));
    }
}
