using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class InputTests : BlazingSpireTestBase
{
    // ── Element type ──────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Input_Element()
    {
        var cut = Render<Input>();
        Assert.NotNull(cut.Find("input"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Input).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Type parameter ────────────────────────────────────────────────────────

    [Fact]
    public void Default_Type_Is_Text()
    {
        var cut = Render<Input>();
        Assert.Equal("text", cut.Find("input").GetAttribute("type"));
    }

    [Theory]
    [InlineData("email")]
    [InlineData("password")]
    [InlineData("file")]
    [InlineData("number")]
    [InlineData("search")]
    public void Type_Parameter_Sets_Input_Type(string type)
    {
        var cut = Render<Input>(p => p.Add(x => x.Type, type));
        Assert.Equal(type, cut.Find("input").GetAttribute("type"));
    }

    // ── Placeholder ───────────────────────────────────────────────────────────

    [Fact]
    public void Placeholder_Renders_On_Input()
    {
        var cut = Render<Input>(p => p.Add(x => x.Placeholder, "Enter your email"));
        Assert.Equal("Enter your email", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void No_Placeholder_Attribute_When_Not_Set()
    {
        var cut = Render<Input>();
        Assert.Null(cut.Find("input").GetAttribute("placeholder"));
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_True_Renders_Disabled_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_False_Does_Not_Render_Disabled_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.Disabled, false));
        Assert.False(cut.Find("input").HasAttribute("disabled"));
    }

    // ── ARIA ──────────────────────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("aria-label", "Email address"));
        AssertAriaLabel(cut.Find("input"), "Email address");
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void Name_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("name", "email"));
        Assert.Equal("email", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void Required_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("required", "required"));
        Assert.True(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void ReadOnly_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("readonly", "readonly"));
        Assert.True(cut.Find("input").HasAttribute("readonly"));
    }

    [Fact]
    public void MaxLength_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("maxlength", "50"));
        Assert.Equal("50", cut.Find("input").GetAttribute("maxlength"));
    }

    [Fact]
    public void AutoComplete_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p => p.AddUnmatched("autocomplete", "email"));
        Assert.Equal("email", cut.Find("input").GetAttribute("autocomplete"));
    }
}
