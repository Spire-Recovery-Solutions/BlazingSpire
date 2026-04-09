using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AlertTests : BlazingSpireTestBase
{
    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Role_Alert()
    {
        var cut = Render<Alert>();
        AssertRole(cut.Find("div"), "alert");
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(AlertVariant.Default)]
    [InlineData(AlertVariant.Destructive)]
    [InlineData(AlertVariant.Success)]
    [InlineData(AlertVariant.Warning)]
    [InlineData(AlertVariant.Info)]
    public void Each_Variant_Renders_Without_Error(AlertVariant variant)
    {
        var cut = Render<Alert>(p => p.Add(x => x.Variant, variant));
        Assert.NotNull(cut.Find("[role='alert']"));
    }

    // ── Child content ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Alert>(p => p.AddChildContent("<span>Hello</span>"));
        Assert.Equal("Hello", cut.Find("[role='alert'] span").TextContent);
    }

    // ── Custom class ─────────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Alert>(p => p.Add(x => x.Class, "my-alert"));
        Assert.Contains("my-alert", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes passthrough ─────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<Alert>(p => p.AddUnmatched("aria-label", "Warning"));
        AssertAriaLabel(cut.Find("[role='alert']"), "Warning");
    }
}

public class AlertTitleTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_H5_Element()
    {
        var cut = Render<AlertTitle>();
        Assert.NotNull(cut.Find("h5"));
    }

    [Fact]
    public void ChildContent_Renders_Inside_H5()
    {
        var cut = Render<AlertTitle>(p => p.AddChildContent("Warning"));
        Assert.Equal("Warning", cut.Find("h5").TextContent);
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<AlertTitle>(p => p.Add(x => x.Class, "text-lg"));
        Assert.Contains("text-lg", cut.Find("h5").ClassName);
    }
}

public class AlertDescriptionTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Div_Element()
    {
        var cut = Render<AlertDescription>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<AlertDescription>(p => p.AddChildContent("Something went wrong."));
        Assert.Contains("Something went wrong.", cut.Find("div").TextContent);
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<AlertDescription>(p => p.Add(x => x.Class, "mt-2"));
        Assert.Contains("mt-2", cut.Find("div").ClassName);
    }
}
