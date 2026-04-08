using System.Reflection;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DialogTests : BlazingSpireTestBase
{
    // ── Dialog ───────────────────────────────────────────────────────────────

    [Fact]
    public void Dialog_Renders_CascadingValue()
    {
        var cut = Render<Dialog>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Dialog_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(Dialog).IsAssignableTo(typeof(OverlayBase)));
    }

    [Fact]
    public void Dialog_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Dialog).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── DialogContent ─────────────────────────────────────────────────────────

    [Fact]
    public void DialogContent_Hidden_When_Dialog_Closed()
    {
        var cut = Render<Dialog>(p =>
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void DialogContent_Visible_When_Dialog_Open()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

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

        Assert.Equal("true", cut.Find("[role=dialog]").GetAttribute("aria-modal"));
    }

    [Fact]
    public void DialogContent_Has_Data_State_Open_When_Open()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("open", cut.Find("[role=dialog]").GetAttribute("data-state"));
    }

    [Fact]
    public void DialogContent_Renders_Backdrop_When_Open()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotEmpty(cut.FindAll(".bg-black\\/80"));
    }

    [Fact]
    public void DialogContent_Has_Base_Classes()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("fixed", classes);
        Assert.Contains("z-50", classes);
        Assert.Contains("max-w-lg", classes);
    }

    [Fact]
    public void DialogContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=dialog]").ClassName);
    }

    [Fact]
    public void DialogContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddUnmatched("data-testid", "dialog-panel"));
        });

        Assert.Equal("dialog-panel", cut.Find("[role=dialog]").GetAttribute("data-testid"));
    }

    // ── DialogHeader ─────────────────────────────────────────────────────────

    [Fact]
    public void DialogHeader_Renders_Div()
    {
        var cut = Render<DialogHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void DialogHeader_Has_Base_Classes()
    {
        var cut = Render<DialogHeader>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("space-y-1.5", classes);
    }

    [Fact]
    public void DialogHeader_Custom_Class_Is_Appended()
    {
        var cut = Render<DialogHeader>(p => p.Add(x => x.Class, "extra-class"));
        Assert.Contains("extra-class", cut.Find("div").ClassName);
    }

    [Fact]
    public void DialogHeader_Renders_ChildContent()
    {
        var cut = Render<DialogHeader>(p => p.AddChildContent("<span>Header</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── DialogFooter ─────────────────────────────────────────────────────────

    [Fact]
    public void DialogFooter_Renders_Div()
    {
        var cut = Render<DialogFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void DialogFooter_Has_Base_Classes()
    {
        var cut = Render<DialogFooter>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("sm:justify-end", classes);
    }

    // ── DialogTitle ───────────────────────────────────────────────────────────

    [Fact]
    public void DialogTitle_Renders_H2()
    {
        var cut = Render<DialogTitle>();
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void DialogTitle_Has_Base_Classes()
    {
        var cut = Render<DialogTitle>();
        var classes = cut.Find("h2").ClassName;
        Assert.Contains("text-lg", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("tracking-tight", classes);
    }

    [Fact]
    public void DialogTitle_Id_Matches_ParentDialog_TitleId()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogTitle>(tp =>
                    tp.AddChildContent("Title")));
        });

        var h2 = cut.Find("h2");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, h2.GetAttribute("id"));
    }

    // ── DialogDescription ────────────────────────────────────────────────────

    [Fact]
    public void DialogDescription_Renders_P()
    {
        var cut = Render<DialogDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void DialogDescription_Has_Base_Classes()
    {
        var cut = Render<DialogDescription>();
        var classes = cut.Find("p").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void DialogDescription_Id_Matches_ParentDialog_DescriptionId()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogDescription>(dp =>
                    dp.AddChildContent("Description")));
        });

        var p = cut.Find("p");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, p.GetAttribute("id"));
    }

    // ── DialogClose ──────────────────────────────────────────────────────────

    [Fact]
    public void DialogClose_Renders_Button()
    {
        var cut = Render<DialogClose>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void DialogClose_Has_Type_Button()
    {
        var cut = Render<DialogClose>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void DialogClose_Has_AriaLabel_Close()
    {
        var cut = Render<DialogClose>();
        Assert.Equal("Close", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void DialogClose_Has_Base_Classes()
    {
        var cut = Render<DialogClose>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("absolute", classes);
        Assert.Contains("right-4", classes);
        Assert.Contains("top-4", classes);
    }

    [Fact]
    public void DialogClose_Renders_X_Svg()
    {
        var cut = Render<DialogClose>();
        Assert.NotNull(cut.Find("button svg"));
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

    // ── DialogTrigger ─────────────────────────────────────────────────────────

    // ── Controlled mode ───────────────────────────────────────────────────────

    [Fact]
    public void Dialog_Controlled_Mode_Opens_When_IsOpen_True()
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
    public void Dialog_Controlled_Mode_IsOpenChanged_Fires()
    {
        bool? receivedValue = null;
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => receivedValue = v));
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();
            });
        });

        cut.Find("div").Click();
        Assert.True(receivedValue);
    }

    // ── Overlay configuration ──────────────────────────────────────────────────

    [Fact]
    public void Dialog_ShouldCloseOnEscape_Is_True()
    {
        var prop = typeof(OverlayBase).GetProperty("ShouldCloseOnEscape", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new Dialog())!);
    }

    [Fact]
    public void Dialog_ShouldTrapFocus_Is_True()
    {
        var prop = typeof(OverlayBase).GetProperty("ShouldTrapFocus", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new Dialog())!);
    }

    [Fact]
    public void Dialog_ShouldLockScroll_Is_True()
    {
        var prop = typeof(OverlayBase).GetProperty("ShouldLockScroll", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new Dialog())!);
    }

    [Fact]
    public void Dialog_ShouldCloseOnInteractOutside_Is_True()
    {
        var prop = typeof(OverlayBase).GetProperty("ShouldCloseOnInteractOutside", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new Dialog())!);
    }

    [Fact]
    public void Dialog_IsModal_Is_True()
    {
        var prop = typeof(OverlayBase).GetProperty("IsModal", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.True((bool)prop!.GetValue(new Dialog())!);
    }

    // ── DialogTrigger ─────────────────────────────────────────────────────────

    [Fact]
    public void DialogTrigger_Click_Opens_Dialog()
    {
        var cut = Render<Dialog>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Body")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=dialog]"));
        cut.Find("div").Click();
        Assert.NotNull(cut.Find("[role=dialog]"));
    }
}
