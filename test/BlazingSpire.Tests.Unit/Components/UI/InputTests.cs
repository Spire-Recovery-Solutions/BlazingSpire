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

    [Fact]
    public void Inherits_From_TextInputBase()
    {
        Assert.True(typeof(Input).IsAssignableTo(typeof(TextInputBase)));
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
    public void Name_Parameter_Sets_Name_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.Name, "email"));
        Assert.Equal("email", cut.Find("input").GetAttribute("name"));
    }

    [Fact]
    public void Required_True_Renders_Required_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.Required, true));
        Assert.True(cut.Find("input").HasAttribute("required"));
    }

    [Fact]
    public void ReadOnly_True_Renders_ReadOnly_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.ReadOnly, true));
        Assert.True(cut.Find("input").HasAttribute("readonly"));
    }

    [Fact]
    public void MaxLength_Renders_On_Input()
    {
        var cut = Render<Input>(p => p.Add(x => x.MaxLength, 50));
        Assert.Equal("50", cut.Find("input").GetAttribute("maxlength"));
    }

    [Fact]
    public void AutoComplete_Renders_On_Input()
    {
        var cut = Render<Input>(p => p.Add(x => x.AutoComplete, "email"));
        Assert.Equal("email", cut.Find("input").GetAttribute("autocomplete"));
    }

    // ── Value binding ─────────────────────────────────────────────────────────

    [Fact]
    public void Value_Renders_On_Input()
    {
        var cut = Render<Input>(p => p.Add(x => x.Value, "hello"));
        Assert.Equal("hello", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Value_Change_Triggers_ValueChanged()
    {
        string? newValue = null;
        var cut = Render<Input>(p => p
            .Add(x => x.Value, "")
            .Add(x => x.ValueChanged, (string? v) => newValue = v));
        cut.Find("input").Change("updated");
        Assert.Equal("updated", newValue);
    }

    // ── Sizes ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(InputSize.Sm)]
    [InlineData(InputSize.Default)]
    [InlineData(InputSize.Lg)]
    public void Each_Size_Renders_Without_Error(InputSize size)
    {
        var cut = Render<Input>(p => p.Add(x => x.Size, size));
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void Sm_Size_Has_H8_Class()
    {
        var cut = Render<Input>(p => p.Add(x => x.Size, InputSize.Sm));
        Assert.Contains("h-8", cut.Find("input").ClassName);
    }

    [Fact]
    public void Lg_Size_Has_H12_Class()
    {
        var cut = Render<Input>(p => p.Add(x => x.Size, InputSize.Lg));
        Assert.Contains("h-12", cut.Find("input").ClassName);
    }

    // ── InputMode ─────────────────────────────────────────────────────────────

    [Fact]
    public void InputMode_Renders_On_Input()
    {
        var cut = Render<Input>(p => p.Add(x => x.InputMode, "numeric"));
        Assert.Equal("numeric", cut.Find("input").GetAttribute("inputmode"));
    }

    // ── Adornments ────────────────────────────────────────────────────────────

    [Fact]
    public void Prefix_Renders_Wrapper_With_Prefix_Content()
    {
        var cut = Render<Input>(p => p
            .Add(x => x.Prefix, (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "$"))));
        Assert.Contains("$", cut.Markup);
        // Should use wrapper div pattern
        Assert.NotNull(cut.Find("div input"));
    }

    [Fact]
    public void Suffix_Renders_Wrapper_With_Suffix_Content()
    {
        var cut = Render<Input>(p => p
            .Add(x => x.Suffix, (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "kg"))));
        Assert.Contains("kg", cut.Markup);
        Assert.NotNull(cut.Find("div input"));
    }

    [Fact]
    public void No_Adornments_Renders_Plain_Input_Without_Wrapper()
    {
        var cut = Render<Input>();
        // No wrapper div — the markup should not contain a div wrapping the input
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find("div input"));
    }

    // ── Custom class ──────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Input>(p => p.Add(x => x.Class, "my-input"));
        Assert.Contains("my-input", cut.Markup);
    }
}
