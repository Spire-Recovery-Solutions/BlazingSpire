using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SpinnerTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Svg_Element()
    {
        var cut = Render<Spinner>();
        Assert.NotNull(cut.Find("svg"));
    }

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Spinner).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Has_Role_Status()
    {
        var cut = Render<Spinner>();
        AssertRole(cut.Find("svg"), "status");
    }

    [Fact]
    public void Has_AriaLabel_Loading()
    {
        var cut = Render<Spinner>();
        AssertAriaLabel(cut.Find("svg"), "Loading");
    }

    [Fact]
    public void Default_Size_Applies_H6_W6()
    {
        var cut = Render<Spinner>();
        var svg = cut.Find("svg");
        Assert.Contains("h-6", svg.ClassName);
        Assert.Contains("w-6", svg.ClassName);
    }

    [Theory]
    [InlineData(SpinnerSize.Sm)]
    [InlineData(SpinnerSize.Default)]
    [InlineData(SpinnerSize.Lg)]
    public void Each_Size_Renders_Without_Error(SpinnerSize size)
    {
        var cut = Render<Spinner>(p => p.Add(x => x.Size, size));
        Assert.NotNull(cut.Find("svg"));
    }

    [Fact]
    public void Sm_Size_Applies_H4_W4()
    {
        var cut = Render<Spinner>(p => p.Add(x => x.Size, SpinnerSize.Sm));
        var svg = cut.Find("svg");
        Assert.Contains("h-4", svg.ClassName);
        Assert.Contains("w-4", svg.ClassName);
    }

    [Fact]
    public void Lg_Size_Applies_H10_W10()
    {
        var cut = Render<Spinner>(p => p.Add(x => x.Size, SpinnerSize.Lg));
        var svg = cut.Find("svg");
        Assert.Contains("h-10", svg.ClassName);
        Assert.Contains("w-10", svg.ClassName);
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Spinner>(p => p.Add(x => x.Class, "text-red-500"));
        Assert.Contains("text-red-500", cut.Find("svg").ClassName);
    }

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Spinner>(p => p.AddUnmatched("aria-label", "Processing"));
        AssertAriaLabel(cut.Find("svg"), "Processing");
    }

    [Fact]
    public void Has_Animate_Spin_Class()
    {
        var cut = Render<Spinner>();
        Assert.Contains("animate-spin", cut.Find("svg").ClassName);
    }
}
