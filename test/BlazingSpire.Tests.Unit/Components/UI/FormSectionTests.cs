using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class FormSectionTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Fieldset_Element()
    {
        var cut = Render<FormSection>();
        Assert.NotNull(cut.Find("fieldset"));
    }

    // ── Legend ───────────────────────────────────────────────────────────────

    [Fact]
    public void Legend_Renders_When_Provided()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Legend, "Personal Information"));
        var legend = cut.Find("legend");
        Assert.NotNull(legend);
        Assert.Equal("Personal Information", legend.TextContent);
    }

    [Fact]
    public void Legend_Not_Rendered_When_Null()
    {
        var cut = Render<FormSection>();
        Assert.Empty(cut.FindAll("legend"));
    }

    [Fact]
    public void Legend_Not_Rendered_When_Empty()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Legend, ""));
        Assert.Empty(cut.FindAll("legend"));
    }

    // ── Description ──────────────────────────────────────────────────────────

    [Fact]
    public void Description_Renders_When_Provided()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Description, "Fill in your details."));
        var p = cut.Find("p");
        Assert.NotNull(p);
        Assert.Equal("Fill in your details.", p.TextContent);
    }

    [Fact]
    public void Description_Not_Rendered_When_Null()
    {
        var cut = Render<FormSection>();
        Assert.Empty(cut.FindAll("p"));
    }

    [Fact]
    public void Description_Not_Rendered_When_Empty()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Description, ""));
        Assert.Empty(cut.FindAll("p"));
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_True_Sets_Disabled_Attribute()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("fieldset").HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_False_Does_Not_Set_Disabled_Attribute()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Disabled, false));
        Assert.False(cut.Find("fieldset").HasAttribute("disabled"));
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Class, "my-custom"));
        Assert.Contains("my-custom", cut.Find("fieldset").ClassName);
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Fieldset()
    {
        var cut = Render<FormSection>(p =>
            p.AddChildContent("<input type=\"text\" />"));
        Assert.NotNull(cut.Find("fieldset input"));
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<FormSection>(p =>
            p.AddUnmatched("data-testid", "my-form-section"));
        Assert.Equal("my-form-section", cut.Find("fieldset").GetAttribute("data-testid"));
    }
}
