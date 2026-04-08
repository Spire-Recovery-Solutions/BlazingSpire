using BlazingSpire.Demo.Components.Shared;
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

    [Fact]
    public void Legend_Has_Correct_Classes()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Legend, "Title"));
        var legend = cut.Find("legend");
        Assert.Contains("text-lg", legend.ClassName);
        Assert.Contains("font-semibold", legend.ClassName);
        Assert.Contains("leading-none", legend.ClassName);
        Assert.Contains("tracking-tight", legend.ClassName);
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

    [Fact]
    public void Description_Has_Correct_Classes()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Description, "Desc"));
        var p = cut.Find("p");
        Assert.Contains("text-sm", p.ClassName);
        Assert.Contains("text-muted-foreground", p.ClassName);
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_True_Sets_Disabled_Attribute()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Disabled, true));
        var fieldset = cut.Find("fieldset");
        Assert.True(fieldset.HasAttribute("disabled"));
    }

    [Fact]
    public void Disabled_False_Does_Not_Set_Disabled_Attribute()
    {
        var cut = Render<FormSection>(p => p.Add(x => x.Disabled, false));
        var fieldset = cut.Find("fieldset");
        Assert.False(fieldset.HasAttribute("disabled"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Classes()
    {
        var cut = Render<FormSection>();
        Assert.Contains("space-y-6", cut.Find("fieldset").ClassName);
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

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void FormSection_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(FormSection).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
