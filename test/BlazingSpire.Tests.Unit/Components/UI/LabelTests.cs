using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class LabelTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Label_Element()
    {
        var cut = Render<Label>();
        Assert.NotNull(cut.Find("label"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Classes()
    {
        var cut = Render<Label>();
        var classes = cut.Find("label").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-medium", classes);
        Assert.Contains("leading-none", classes);
    }

    // ── For attribute ─────────────────────────────────────────────────────────

    [Fact]
    public void For_Attribute_Renders_On_Label()
    {
        var cut = Render<Label>(p => p.Add(x => x.For, "my-input"));
        Assert.Equal("my-input", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void For_Attribute_Is_Null_By_Default()
    {
        var cut = Render<Label>();
        Assert.Null(cut.Find("label").GetAttribute("for"));
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Label>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("label").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Label>(p =>
            p.AddUnmatched("aria-label", "Field label"));

        AssertAriaLabel(cut.Find("label"), "Field label");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Label>(p =>
            p.AddUnmatched("data-testid", "my-label"));

        Assert.Equal("my-label", cut.Find("label").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Text_Renders_Inside_Label()
    {
        var cut = Render<Label>(p =>
            p.AddChildContent("Email address"));

        Assert.Contains("Email address", cut.Find("label").TextContent);
    }

    [Fact]
    public void ChildContent_Element_Renders_Inside_Label()
    {
        var cut = Render<Label>(p =>
            p.AddChildContent("<span>Required</span>"));

        Assert.NotNull(cut.Find("label span"));
        Assert.Equal("Required", cut.Find("label span").TextContent);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Label_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Label).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
