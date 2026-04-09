using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TextareaTests : BlazingSpireTestBase
{
    // ── Element type ──────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Textarea_Element()
    {
        var cut = Render<Textarea>();
        Assert.NotNull(cut.Find("textarea"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Textarea).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Placeholder ──────────────────────────────────────────────────────────

    [Fact]
    public void Placeholder_Renders_On_Textarea()
    {
        var cut = Render<Textarea>(p => p.Add(x => x.Placeholder, "Enter text..."));
        Assert.Equal("Enter text...", cut.Find("textarea").GetAttribute("placeholder"));
    }

    [Fact]
    public void No_Placeholder_By_Default()
    {
        var cut = Render<Textarea>();
        Assert.Null(cut.Find("textarea").GetAttribute("placeholder"));
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Attribute_Renders_When_True()
    {
        var cut = Render<Textarea>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("textarea").HasAttribute("disabled"));
    }

    [Fact]
    public void Not_Disabled_By_Default()
    {
        var cut = Render<Textarea>();
        Assert.False(cut.Find("textarea").HasAttribute("disabled"));
    }

    // ── ARIA ─────────────────────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p => p.AddUnmatched("aria-label", "Message input"));
        AssertAriaLabel(cut.Find("textarea"), "Message input");
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void Rows_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p => p.AddUnmatched("rows", "5"));
        Assert.Equal("5", cut.Find("textarea").GetAttribute("rows"));
    }

    [Fact]
    public void MaxLength_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p => p.AddUnmatched("maxlength", "100"));
        Assert.Equal("100", cut.Find("textarea").GetAttribute("maxlength"));
    }

    [Fact]
    public void Name_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p => p.AddUnmatched("name", "message"));
        Assert.Equal("message", cut.Find("textarea").GetAttribute("name"));
    }

    [Fact]
    public void Required_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p => p.AddUnmatched("required", "required"));
        Assert.True(cut.Find("textarea").HasAttribute("required"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Textarea()
    {
        var cut = Render<Textarea>(p => p.AddChildContent("Hello World"));
        Assert.Contains("Hello World", cut.Find("textarea").TextContent);
    }
}
