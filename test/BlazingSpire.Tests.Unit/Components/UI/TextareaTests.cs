using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TextareaTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Textarea_Element()
    {
        var cut = Render<Textarea>();
        Assert.NotNull(cut.Find("textarea"));
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

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Layout_Classes()
    {
        var cut = Render<Textarea>();
        var classes = cut.Find("textarea").ClassName;
        Assert.Contains("min-h-[80px]", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("border-input", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void Always_Has_Focus_Ring_Classes()
    {
        var cut = Render<Textarea>();
        var classes = cut.Find("textarea").ClassName;
        Assert.Contains("focus-visible:ring-2", classes);
        Assert.Contains("focus-visible:ring-ring", classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Textarea>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("textarea").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p =>
            p.AddUnmatched("aria-label", "Message input"));

        AssertAriaLabel(cut.Find("textarea"), "Message input");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Textarea>(p =>
            p.AddUnmatched("data-testid", "message-textarea"));

        Assert.Equal("message-textarea", cut.Find("textarea").GetAttribute("data-testid"));
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Text_Renders_Inside_Textarea()
    {
        var cut = Render<Textarea>(p =>
            p.AddChildContent("Hello World"));

        Assert.Contains("Hello World", cut.Find("textarea").TextContent);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Textarea_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Textarea).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
