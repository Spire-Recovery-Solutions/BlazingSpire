using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DrawerTests : BlazingSpireTestBase
{
    // ── Drawer ────────────────────────────────────────────────────────────────

    [Fact]
    public void Drawer_Renders_CascadingValue()
    {
        var cut = Render<Drawer>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Drawer_Is_Assignable_To_OverlayBase()
    {
        Assert.True(typeof(Drawer).IsAssignableTo(typeof(OverlayBase)));
    }

    [Fact]
    public void Drawer_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Drawer).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── DrawerContent ─────────────────────────────────────────────────────────

    [Fact]
    public void DrawerContent_Hidden_When_Drawer_Closed()
    {
        var cut = Render<Drawer>(p =>
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void DrawerContent_Visible_When_Drawer_Open()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void DrawerContent_Has_Role_Dialog()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("dialog", cut.Find("[role=dialog]").GetAttribute("role"));
    }

    [Fact]
    public void DrawerContent_Has_Aria_Modal_True()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("true", cut.Find("[role=dialog]").GetAttribute("aria-modal"));
    }

    [Fact]
    public void DrawerContent_Has_Data_State_Open_When_Open()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.Equal("open", cut.Find("[role=dialog]").GetAttribute("data-state"));
    }

    [Fact]
    public void DrawerContent_Renders_Backdrop_When_Open()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        Assert.NotEmpty(cut.FindAll(".bg-black\\/80"));
    }

    [Fact]
    public void DrawerContent_Has_Drag_Handle()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var handle = cut.Find(".h-2.w-\\[100px\\].rounded-full.bg-muted");
        Assert.NotNull(handle);
    }

    [Fact]
    public void DrawerContent_Has_Base_Classes()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var classes = cut.Find("[role=dialog]").ClassName;
        Assert.Contains("fixed", classes);
        Assert.Contains("inset-x-0", classes);
        Assert.Contains("bottom-0", classes);
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-t-[10px]", classes);
    }

    [Fact]
    public void DrawerContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=dialog]").ClassName);
    }

    // ── DrawerHeader ──────────────────────────────────────────────────────────

    [Fact]
    public void DrawerHeader_Renders_Div()
    {
        var cut = Render<DrawerHeader>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void DrawerHeader_Has_Base_Classes()
    {
        var cut = Render<DrawerHeader>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("grid", classes);
        Assert.Contains("gap-1.5", classes);
        Assert.Contains("p-4", classes);
    }

    [Fact]
    public void DrawerHeader_Renders_ChildContent()
    {
        var cut = Render<DrawerHeader>(p => p.AddChildContent("<span>Header</span>"));
        Assert.NotNull(cut.Find("span"));
    }

    // ── DrawerFooter ──────────────────────────────────────────────────────────

    [Fact]
    public void DrawerFooter_Renders_Div()
    {
        var cut = Render<DrawerFooter>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void DrawerFooter_Has_Base_Classes()
    {
        var cut = Render<DrawerFooter>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("p-4", classes);
    }

    // ── DrawerTitle ───────────────────────────────────────────────────────────

    [Fact]
    public void DrawerTitle_Renders_H2()
    {
        var cut = Render<DrawerTitle>();
        Assert.NotNull(cut.Find("h2"));
    }

    [Fact]
    public void DrawerTitle_Has_Base_Classes()
    {
        var cut = Render<DrawerTitle>();
        var classes = cut.Find("h2").ClassName;
        Assert.Contains("text-lg", classes);
        Assert.Contains("font-semibold", classes);
        Assert.Contains("tracking-tight", classes);
    }

    [Fact]
    public void DrawerTitle_Id_Matches_ParentDrawer_TitleId()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent<DrawerTitle>(tp =>
                    tp.AddChildContent("Title")));
        });

        var h2 = cut.Find("h2");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, h2.GetAttribute("id"));
    }

    // ── DrawerDescription ─────────────────────────────────────────────────────

    [Fact]
    public void DrawerDescription_Renders_P()
    {
        var cut = Render<DrawerDescription>();
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void DrawerDescription_Has_Base_Classes()
    {
        var cut = Render<DrawerDescription>();
        var classes = cut.Find("p").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("text-muted-foreground", classes);
    }

    [Fact]
    public void DrawerDescription_Id_Matches_ParentDrawer_DescriptionId()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent<DrawerDescription>(dp =>
                    dp.AddChildContent("Description")));
        });

        var p = cut.Find("p");
        var dialog = cut.Find("[role=dialog]");
        var expectedId = dialog.GetAttribute("aria-describedby");
        Assert.NotNull(expectedId);
        Assert.Equal(expectedId, p.GetAttribute("id"));
    }

    // ── DrawerClose ───────────────────────────────────────────────────────────

    [Fact]
    public void DrawerClose_Renders_Button()
    {
        var cut = Render<DrawerClose>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void DrawerClose_Has_Type_Button()
    {
        var cut = Render<DrawerClose>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void DrawerClose_Has_AriaLabel_Close()
    {
        var cut = Render<DrawerClose>();
        Assert.Equal("Close", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void DrawerClose_Has_Base_Classes()
    {
        var cut = Render<DrawerClose>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("absolute", classes);
        Assert.Contains("right-4", classes);
        Assert.Contains("top-4", classes);
    }

    [Fact]
    public void DrawerClose_Renders_X_Svg()
    {
        var cut = Render<DrawerClose>();
        Assert.NotNull(cut.Find("button svg"));
    }

    [Fact]
    public void DrawerClose_Click_Closes_Drawer()
    {
        var cut = Render<Drawer>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DrawerContent>(cp =>
                cp.AddChildContent<DrawerClose>());
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
        cut.Find("button").Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    // ── DrawerTrigger ─────────────────────────────────────────────────────────

    [Fact]
    public void DrawerTrigger_Click_Opens_Drawer()
    {
        var cut = Render<Drawer>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DrawerTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    b.AddContent(0, "Open")));
                builder.CloseComponent();

                builder.OpenComponent<DrawerContent>(2);
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
