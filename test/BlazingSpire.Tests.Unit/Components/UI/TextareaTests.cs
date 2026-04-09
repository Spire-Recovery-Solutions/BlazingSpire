using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TextareaTests : BlazingSpireTestBase
{
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
}
