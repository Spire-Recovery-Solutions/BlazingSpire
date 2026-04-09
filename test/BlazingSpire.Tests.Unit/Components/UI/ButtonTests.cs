using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ButtonTests : BlazingSpireTestBase
{
    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Button_Element_With_Type_Button()
    {
        var cut = Render<Button>(p => p.Add(x => x.ChildContent, "Save"));
        var btn = cut.Find("button");
        Assert.Equal("button", btn.GetAttribute("type"));
        Assert.Contains("Save", btn.TextContent);
    }

    [Theory]
    [InlineData("submit")]
    [InlineData("reset")]
    [InlineData("button")]
    public void Type_Parameter_Sets_Type_Attribute(string type)
    {
        var cut = Render<Button>(p => p.Add(x => x.Type, type));
        Assert.Equal(type, cut.Find("button").GetAttribute("type"));
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ButtonVariant.Default)]
    [InlineData(ButtonVariant.Destructive)]
    [InlineData(ButtonVariant.Outline)]
    [InlineData(ButtonVariant.Secondary)]
    [InlineData(ButtonVariant.Ghost)]
    [InlineData(ButtonVariant.Link)]
    public void Each_Variant_Renders_Without_Error(ButtonVariant variant)
    {
        var cut = Render<Button>(p => p.Add(x => x.Variant, variant));
        Assert.NotNull(cut.Find("button"));
    }

    // ── Sizes ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ButtonSize.Default)]
    [InlineData(ButtonSize.Sm)]
    [InlineData(ButtonSize.Lg)]
    [InlineData(ButtonSize.Icon)]
    public void Each_Size_Renders_Without_Error(ButtonSize size)
    {
        var cut = Render<Button>(p => p.Add(x => x.Size, size));
        Assert.NotNull(cut.Find("button"));
    }

    // ── Disabled state ───────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Button_Has_Disabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Enabled_Button_Does_Not_Have_Disabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Disabled, false));
        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }

    // ── Loading state ─────────────────────────────────────────────────────────

    [Fact]
    public void Loading_Button_Is_Effectively_Disabled()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Loading_Button_Has_AriaBusy_True()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-busy"));
    }

    [Fact]
    public void Loading_Button_Has_DataDisabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.True(cut.Find("button").HasAttribute("data-disabled"));
    }

    // ── EventCallback ────────────────────────────────────────────────────────

    [Fact]
    public void Click_Fires_OnClick()
    {
        bool clicked = false;
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
            p.Add(x => x.ChildContent, "Go");
        });
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    // ── Link mode ─────────────────────────────────────────────────────────────

    [Fact]
    public void With_Href_Renders_Anchor_Not_Button()
    {
        var cut = Render<Button>(p => p.Add(x => x.Href, "/about"));
        Assert.NotNull(cut.Find("a"));
        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void Anchor_Has_Correct_Href()
    {
        var cut = Render<Button>(p => p.Add(x => x.Href, "/about"));
        Assert.Equal("/about", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Anchor_Renders_Target_Attribute()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Target, "_blank");
        });
        Assert.Equal("_blank", cut.Find("a").GetAttribute("target"));
    }

    [Fact]
    public void Anchor_Renders_Rel_Attribute()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.Rel, "noopener noreferrer");
        });
        Assert.Equal("noopener noreferrer", cut.Find("a").GetAttribute("rel"));
    }

    [Fact]
    public void Anchor_Does_Not_Have_Type_Attribute()
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

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Button>(p => p.Add(x => x.Class, "my-btn"));
        Assert.Contains("my-btn", cut.Find("button").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Button>(p => p.AddUnmatched("aria-label", "Close dialog"));
        AssertAriaLabel(cut.Find("button"), "Close dialog");
    }
}
