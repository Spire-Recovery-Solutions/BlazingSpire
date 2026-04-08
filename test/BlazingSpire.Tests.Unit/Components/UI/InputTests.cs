using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class InputTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Input_Element()
    {
        var cut = Render<Input>();
        Assert.NotNull(cut.Find("input"));
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

    // ── Base classes ──────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Layout_Classes()
    {
        var cut = Render<Input>();
        var classes = cut.Find("input").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("border", classes);
        Assert.Contains("px-3", classes);
        Assert.Contains("py-2", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void Always_Has_Focus_Ring_Classes()
    {
        var cut = Render<Input>();
        var classes = cut.Find("input").ClassName;
        Assert.Contains("focus-visible:ring-2", classes);
        Assert.Contains("focus-visible:ring-ring", classes);
    }

    [Fact]
    public void Always_Has_Disabled_State_Classes()
    {
        var cut = Render<Input>();
        var classes = cut.Find("input").ClassName;
        Assert.Contains("disabled:cursor-not-allowed", classes);
        Assert.Contains("disabled:opacity-50", classes);
    }

    // ── Class parameter ───────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Input>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("input").ClassName);
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p =>
            p.AddUnmatched("aria-label", "Email address"));

        AssertAriaLabel(cut.Find("input"), "Email address");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p =>
            p.AddUnmatched("data-testid", "email-input"));

        Assert.Equal("email-input", cut.Find("input").GetAttribute("data-testid"));
    }

    [Fact]
    public void Name_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Input>(p =>
            p.AddUnmatched("name", "email"));

        Assert.Equal("email", cut.Find("input").GetAttribute("name"));
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Input_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Input).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
