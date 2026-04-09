using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SheetTests : BlazingSpireTestBase
{
    // ── Visibility ────────────────────────────────────────────────────────────

    [Fact]
    public void SheetContent_Is_Hidden_When_Closed()
    {
        var cut = Render<Sheet>(p =>
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void SheetContent_Is_Visible_When_DefaultIsOpen_True()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    // ── ARIA ──────────────────────────────────────────────────────────────────

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

        AssertAriaModal(cut.Find("[role=dialog]"), true);
    }

    [Fact]
    public void SheetContent_Has_Data_State_Open()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        AssertDataState(cut.Find("[role=dialog]"), "open");
    }

    [Fact]
    public void SheetTitle_Id_Matches_Aria_LabelledBy()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetTitle>(tp =>
                    tp.AddChildContent("My Title")));
        });

        var dialog = cut.Find("[role=dialog]");
        var labelledBy = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(labelledBy);
        Assert.Equal(labelledBy, cut.Find("h2").GetAttribute("id"));
    }

    [Fact]
    public void SheetDescription_Id_Matches_Aria_DescribedBy()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetDescription>(dp =>
                    dp.AddChildContent("My Description")));
        });

        var dialog = cut.Find("[role=dialog]");
        var describedBy = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(describedBy);
        Assert.Equal(describedBy, cut.Find("p").GetAttribute("id"));
    }

    // ── Correct HTML elements ─────────────────────────────────────────────────

    [Fact]
    public void SheetTitle_Renders_H2()
    {
        var cut = Render<SheetTitle>(p => p.AddChildContent("Title"));
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void SheetDescription_Renders_Paragraph()
    {
        var cut = Render<SheetDescription>(p => p.AddChildContent("Description"));
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void SheetClose_Renders_Button_With_Type_Button()
    {
        var cut = Render<SheetClose>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void SheetClose_Has_AriaLabel_Close()
    {
        var cut = Render<SheetClose>();
        AssertAriaLabel(cut.Find("button"), "Close");
    }

    // ── Trigger / close behavior ──────────────────────────────────────────────

    [Fact]
    public void SheetTrigger_Click_Opens_Sheet()
    {
        var cut = Render<Sheet>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<SheetTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<SheetContent>(2);
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

    // ── Side variants ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(SheetSide.Top)]
    [InlineData(SheetSide.Right)]
    [InlineData(SheetSide.Bottom)]
    [InlineData(SheetSide.Left)]
    public void Each_Side_Renders_Sheet_Without_Error(SheetSide side)
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Side, side);
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    // ── Controlled mode ───────────────────────────────────────────────────────

    [Fact]
    public void Sheet_Controlled_Mode_IsOpen_True_Shows_Content()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => { }));
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void Sheet_Controlled_Mode_IsOpen_False_Hides_Content()
    {
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => { }));
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Sheet_Controlled_Mode_IsOpenChanged_Fires_On_Close_Attempt()
    {
        bool? receivedValue = null;
        var cut = Render<Sheet>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v => receivedValue = v));
            p.AddChildContent<SheetContent>(cp =>
                cp.AddChildContent<SheetClose>());
        });

        cut.Find("button").Click();
        Assert.Equal(false, receivedValue);
    }
}
