using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ButtonTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_Element()
    {
        var cut = Render<Button>();
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void Default_Type_Is_Button()
    {
        var cut = Render<Button>();
        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Fact]
    public void Default_Variant_Has_Primary_Classes()
    {
        var cut = Render<Button>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("bg-primary", classes);
        Assert.Contains("text-primary-foreground", classes);
    }

    [Theory]
    [InlineData(ButtonVariant.Default, "bg-primary", "text-primary-foreground")]
    [InlineData(ButtonVariant.Destructive, "bg-destructive", "text-destructive-foreground")]
    [InlineData(ButtonVariant.Outline, "border", "bg-background")]
    [InlineData(ButtonVariant.Secondary, "bg-secondary", "text-secondary-foreground")]
    [InlineData(ButtonVariant.Ghost, "hover:bg-accent", "hover:text-accent-foreground")]
    [InlineData(ButtonVariant.Link, "text-primary", "underline-offset-4")]
    public void Variant_Produces_Correct_Classes(ButtonVariant variant, string class1, string class2)
    {
        var cut = Render<Button>(p => p.Add(x => x.Variant, variant));
        var classes = cut.Find("button").ClassName;
        Assert.Contains(class1, classes);
        Assert.Contains(class2, classes);
    }

    // ── Sizes ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ButtonSize.Default, "h-10", "px-4")]
    [InlineData(ButtonSize.Sm, "h-9", "px-3")]
    [InlineData(ButtonSize.Lg, "h-11", "px-8")]
    [InlineData(ButtonSize.Icon, "h-10", "w-10")]
    public void Size_Produces_Correct_Classes(ButtonSize size, string class1, string class2)
    {
        var cut = Render<Button>(p => p.Add(x => x.Size, size));
        var classes = cut.Find("button").ClassName;
        Assert.Contains(class1, classes);
        Assert.Contains(class2, classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Button>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("button").ClassName);
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_True_Renders_Disabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_False_Does_Not_Render_Disabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Disabled, false));
        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }

    // ── OnClick ──────────────────────────────────────────────────────────────

    [Fact]
    public void OnClick_Fires_When_Clicked()
    {
        var clicked = false;
        var cut = Render<Button>(p =>
            p.Add(x => x.OnClick, (MouseEventArgs _) => { clicked = true; }));

        cut.Find("button").Click();

        Assert.True(clicked);
    }

    [Fact]
    public void OnClick_Not_Fired_Without_Click()
    {
        var clicked = false;
        Render<Button>(p =>
            p.Add(x => x.OnClick, (MouseEventArgs _) => { clicked = true; }));

        Assert.False(clicked);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Button>(p =>
            p.AddUnmatched("aria-label", "Close dialog"));

        AssertAriaLabel(cut.Find("button"), "Close dialog");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Button>(p =>
            p.AddUnmatched("data-testid", "submit-btn"));

        Assert.Equal("submit-btn", cut.Find("button").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Button()
    {
        var cut = Render<Button>(p =>
            p.AddChildContent("<span>Click me</span>"));

        Assert.NotNull(cut.Find("button span"));
        Assert.Equal("Click me", cut.Find("button span").TextContent);
    }

    [Fact]
    public void ChildContent_Text_Renders_Inside_Button()
    {
        var cut = Render<Button>(p =>
            p.AddChildContent("Save"));

        Assert.Contains("Save", cut.Find("button").TextContent);
    }

    // ── Type parameter ───────────────────────────────────────────────────────

    [Theory]
    [InlineData("submit")]
    [InlineData("reset")]
    [InlineData("button")]
    public void Type_Parameter_Sets_Type_Attribute(string type)
    {
        var cut = Render<Button>(p => p.Add(x => x.Type, type));
        Assert.Equal(type, cut.Find("button").GetAttribute("type"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Focus_Visible_Classes()
    {
        var cut = Render<Button>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("focus-visible:ring-2", classes);
        Assert.Contains("focus-visible:ring-ring", classes);
    }

    [Fact]
    public void Always_Has_Disabled_Pointer_Events_None_Class()
    {
        var cut = Render<Button>();
        Assert.Contains("data-[disabled]:pointer-events-none", cut.Find("button").ClassName);
    }

    // ── Loading state ─────────────────────────────────────────────────────────

    [Fact]
    public void Loading_True_Renders_Svg_Spinner()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.NotNull(cut.Find("button svg.animate-spin"));
    }

    [Fact]
    public void Loading_False_Does_Not_Render_Spinner()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, false));
        Assert.Empty(cut.FindAll("button svg.animate-spin"));
    }

    [Fact]
    public void Loading_True_Disables_Button()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Loading_True_Sets_AriaBusy()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-busy"));
    }

    [Fact]
    public void Loading_True_Sets_DataDisabled()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.True(cut.Find("button").HasAttribute("data-disabled"));
    }

    // ── IsEffectivelyDisabled ─────────────────────────────────────────────────

    [Fact]
    public void Loading_Without_Disabled_Still_Disables_Button()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Loading_And_Disabled_Both_True_Disables_Button()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Loading, true);
            p.Add(x => x.Disabled, true);
        });
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    // ── Link rendering (Href) ─────────────────────────────────────────────────

    [Fact]
    public void Href_Set_Renders_Anchor_Element()
    {
        var cut = Render<Button>(p => p.Add(x => x.Href, "/about"));
        Assert.NotNull(cut.Find("a"));
    }

    [Fact]
    public void Without_Href_Renders_Button_Element()
    {
        var cut = Render<Button>();
        Assert.NotNull(cut.Find("button"));
        Assert.Empty(cut.FindAll("a"));
    }

    [Fact]
    public void Href_Renders_Correct_Href_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Href, "/about"));
        Assert.Equal("/about", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Target_PassesThrough_On_Anchor()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Target, "_blank");
        });
        Assert.Equal("_blank", cut.Find("a").GetAttribute("target"));
    }

    [Fact]
    public void Rel_PassesThrough_On_Anchor()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Rel, "noopener noreferrer");
        });
        Assert.Equal("noopener noreferrer", cut.Find("a").GetAttribute("rel"));
    }

    [Fact]
    public void Link_Mode_Does_Not_Render_Type_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Href, "/about"));
        Assert.False(cut.Find("a").HasAttribute("type"));
    }

    [Fact]
    public void Disabled_Link_Has_AriaDisabled_True()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Disabled, true);
        });
        Assert.Equal("true", cut.Find("a").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Disabled_Link_Has_TabIndex_Minus_One()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Disabled, true);
        });
        Assert.Equal("-1", cut.Find("a").GetAttribute("tabindex"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Button_Is_Assignable_To_ButtonBase()
    {
        Assert.True(typeof(Button).IsAssignableTo(typeof(ButtonBase<ButtonVariant, ButtonSize>)));
    }

    [Fact]
    public void Button_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Button).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
