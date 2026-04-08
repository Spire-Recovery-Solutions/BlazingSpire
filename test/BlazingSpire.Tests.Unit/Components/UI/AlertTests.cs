using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AlertTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Role_Alert()
    {
        var cut = Render<Alert>();
        var el = cut.Find("div");
        Assert.NotNull(el);
        AssertRole(el, "alert");
    }

    // ── Variants ─────────────────────────────────────────────────────────────

    [Fact]
    public void Default_Variant_Has_Background_Classes()
    {
        var cut = Render<Alert>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("bg-background", classes);
        Assert.Contains("text-foreground", classes);
    }

    [Theory]
    [InlineData(AlertVariant.Default, "bg-background", "text-foreground")]
    [InlineData(AlertVariant.Destructive, "border-destructive/50", "text-destructive")]
    public void Variant_Produces_Correct_Classes(AlertVariant variant, string class1, string class2)
    {
        var cut = Render<Alert>(p => p.Add(x => x.Variant, variant));
        var classes = cut.Find("div").ClassName;
        Assert.Contains(class1, classes);
        Assert.Contains(class2, classes);
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Layout_Classes()
    {
        var cut = Render<Alert>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("relative", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("rounded-lg", classes);
        Assert.Contains("border", classes);
        Assert.Contains("p-4", classes);
    }

    // ── Class parameter ──────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Alert>(p => p.Add(x => x.Class, "my-custom"));
        Assert.Contains("my-custom", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes ─────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Alert>(p =>
            p.AddUnmatched("aria-label", "Warning"));
        AssertAriaLabel(cut.Find("div"), "Warning");
    }

    // ── ChildContent ─────────────────────────────────────────────────────────

    [Fact]
    public void ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Alert>(p =>
            p.AddChildContent("<span>Hello</span>"));
        Assert.NotNull(cut.Find("div span"));
        Assert.Equal("Hello", cut.Find("div span").TextContent);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Alert_Is_Assignable_To_PresentationalBase()
    {
        Assert.True(typeof(Alert).IsAssignableTo(typeof(PresentationalBase<AlertVariant>)));
    }

    [Fact]
    public void Alert_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Alert).IsAssignableTo(typeof(BlazingSpireComponentBase)));
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
    public void Has_Base_Classes()
    {
        var cut = Render<AlertTitle>();
        var classes = cut.Find("h5").ClassName;
        Assert.Contains("font-medium", classes);
        Assert.Contains("leading-none", classes);
        Assert.Contains("tracking-tight", classes);
    }

    [Fact]
    public void ChildContent_Renders()
    {
        var cut = Render<AlertTitle>(p => p.AddChildContent("Warning"));
        Assert.Equal("Warning", cut.Find("h5").TextContent);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
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
    public void Has_Base_Classes()
    {
        var cut = Render<AlertDescription>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void ChildContent_Renders()
    {
        var cut = Render<AlertDescription>(p =>
            p.AddChildContent("Something went wrong."));
        Assert.Contains("Something went wrong.", cut.Find("div").TextContent);
    }

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<AlertDescription>(p => p.Add(x => x.Class, "mt-2"));
        Assert.Contains("mt-2", cut.Find("div").ClassName);
    }
}
