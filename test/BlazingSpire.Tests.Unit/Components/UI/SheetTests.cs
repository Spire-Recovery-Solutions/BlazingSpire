using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SheetTests : BlazingSpireTestBase
{
    // ── Sheet ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Sheet_Renders_CascadingValue()
    {
        var cut = Render<Sheet>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Sheet_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(Sheet).IsAssignableTo(typeof(OverlayBase)));
    }

    [Fact]
    public void Sheet_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Sheet).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Sheet_Default_Side_Is_Right()
    {
        var cut = Render<Sheet>(p => p.AddChildContent("<span>x</span>"));
        // Default side is Right — verified via SheetContent classes
        Assert.Equal(SheetSide.Right, cut.Instance.Side);
    }

    // ── SheetContent ──────────────────────────────────────────────────────────

    [Fact]
    public void SheetContent_Hidden_When_Sheet_Closed()
    {
        var cut = Render<Sheet>(p =>
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void SheetContent_Visible_When_Sheet_Open()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void SheetContent_Has_Role_Dialog()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("dialog", cut.Find("[role=dialog]").GetAttribute("role"));
    }

    [Fact]
    public void SheetContent_Has_Aria_Modal_True()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("true", cut.Find("[role=dialog]").GetAttribute("aria-modal"));
    }

    [Fact]
    public void SheetContent_Has_Data_State_Open_When_Open()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("open", cut.Find("[role=dialog]").GetAttribute("data-state"));
    }

    [Fact]
    public void SheetContent_Renders_Backdrop_When_Open()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotEmpty(cut.FindAll(".bg-black\\/80"));
    }

    [Fact]
    public void SheetContent_Has_Base_Classes()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("fixed", classes);
        Assert.Contains("z-50", classes);
        Assert.Contains("bg-background", classes);
    }

    [Fact]
    public void SheetContent_Right_Side_Classes()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Side, SheetSide.Right);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("right-0", classes);
        Assert.Contains("border-l", classes);
    }

    [Fact]
    public void SheetContent_Left_Side_Classes()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Side, SheetSide.Left);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("left-0", classes);
        Assert.Contains("border-r", classes);
    }

    [Fact]
    public void SheetContent_Top_Side_Classes()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Side, SheetSide.Top);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("top-0", classes);
        Assert.Contains("border-b", classes);
    }

    [Fact]
    public void SheetContent_Bottom_Side_Classes()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Side, SheetSide.Bottom);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("bottom-0", classes);
        Assert.Contains("border-t", classes);
    }

    [Fact]
    public void SheetContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=dialog]").ClassName);
    }

    [Fact]
    public void SheetContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddUnmatched("data-testid", "sheet-panel"));
        });

        Assert.Equal("sheet-panel", cut.Find("[role=dialog]").GetAttribute("data-testid"));
    }

    // ── SheetHeader ───────────────────────────────────────────────────────────

    [Fact]
    public void SheetHeader_Renders_Div()
    {
        var cut = Render<SheetHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SheetHeader_Has_Base_Classes()
    {
        var cut = Render<SheetHeader>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("space-y-2", classes);
    }

    [Fact]
    public void SheetHeader_Custom_Class_Is_Appended()
    {
        var cut = Render<SheetHeader>(p => p.Add(x => x.Class, "extra-class"));
        Assert.Contains("extra-class", cut.Find("div").ClassName);
    }

    [Fact]
    public void SheetHeader_Renders_ChildContent()
    {
        var cut = Render<SheetHeader>(p => p.AddChildContent("<span>Header</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── SheetFooter ───────────────────────────────────────────────────────────

    [Fact]
    public void SheetFooter_Renders_Div()
    {
        var cut = Render<SheetFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SheetFooter_Has_Base_Classes()
    {
        var cut = Render<SheetFooter>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("sm:justify-end", classes);
    }

    // ── SheetTitle ────────────────────────────────────────────────────────────

    [Fact]
    public void SheetTitle_Renders_H2()
    {
        var cut = Render<SheetTitle>();
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void SheetTitle_Has_Base_Classes()
    {
        var cut = Render<SheetTitle>();
        var classes = cut.Find("h2").ClassName;
        Assert.Contains("text-lg", classes);
        Assert.Contains("font-semibold", classes);
    }

    [Fact]
    public void SheetTitle_Id_Matches_ParentSheet_TitleId()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetTitle>(tp =>
                    tp.AddChildContent("Title")));
        });

        var h2 = cut.Find("h2");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, h2.GetAttribute("id"));
    }

    // ── SheetDescription ──────────────────────────────────────────────────────

    [Fact]
    public void SheetDescription_Renders_P()
    {
        var cut = Render<SheetDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void SheetDescription_Has_Base_Classes()
    {
        var cut = Render<SheetDescription>();
        var classes = cut.Find("p").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void SheetDescription_Id_Matches_ParentSheet_DescriptionId()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetDescription>(dp =>
                    dp.AddChildContent("Description")));
        });

        var p = cut.Find("p");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, p.GetAttribute("id"));
    }

    // ── SheetClose ────────────────────────────────────────────────────────────

    [Fact]
    public void SheetClose_Renders_Button()
    {
        var cut = Render<SheetClose>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void SheetClose_Has_Type_Button()
    {
        var cut = Render<SheetClose>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void SheetClose_Has_AriaLabel_Close()
    {
        var cut = Render<SheetClose>();
        Assert.Equal("Close", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void SheetClose_Has_Base_Classes()
    {
        var cut = Render<SheetClose>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("absolute", classes);
        Assert.Contains("right-4", classes);
        Assert.Contains("top-4", classes);
    }

    [Fact]
    public void SheetClose_Renders_X_Svg()
    {
        var cut = Render<SheetClose>();
        Assert.NotNull(cut.Find("button svg"));
    }

    [Fact]
    public void SheetClose_Click_Closes_Sheet()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetClose>());
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
        cut.Find("button").Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    // ── SheetTrigger ──────────────────────────────────────────────────────────

    [Fact]
    public void SheetTrigger_Click_Opens_Sheet()
    {
        var cut = Render<Sheet>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<SheetTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<SheetContent>(2);
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
