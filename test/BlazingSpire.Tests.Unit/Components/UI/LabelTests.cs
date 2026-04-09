using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class LabelTests : BlazingSpireTestBase
{
    // ── Semantic element ─────────────────────────────────────────────────────

    [Fact]
    public void Renders_Label_Element()
    {
        var cut = Render<Label>();
        Assert.NotNull(cut.Find("label"));
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

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Label()
    {
        var cut = Render<Label>(p => p.AddChildContent("Email address"));
        Assert.Contains("Email address", cut.Find("label").TextContent);
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Label>(p => p.Add(x => x.Class, "my-label"));
        Assert.Contains("my-label", cut.Find("label").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Label>(p => p.AddUnmatched("aria-label", "Field label"));
        AssertAriaLabel(cut.Find("label"), "Field label");
    }
}
