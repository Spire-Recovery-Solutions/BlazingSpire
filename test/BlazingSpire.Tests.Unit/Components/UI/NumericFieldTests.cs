using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class NumericFieldTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Number_Input()
    {
        var cut = Render<NumericField<int>>();
        Assert.Equal("number", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void Inherits_From_NumericInputBase()
    {
        Assert.True(typeof(NumericField<int>).IsAssignableTo(typeof(NumericInputBase<int>)));
    }

    [Fact]
    public void Value_Renders_On_Input()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Value, 42));
        Assert.Equal("42", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Placeholder_Renders_On_Input()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Placeholder, "Enter amount"));
        Assert.Equal("Enter amount", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Min_Renders_On_Input()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Min, 0));
        Assert.Equal("0", cut.Find("input").GetAttribute("min"));
    }

    [Fact]
    public void Max_Renders_On_Input()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Max, 100));
        Assert.Equal("100", cut.Find("input").GetAttribute("max"));
    }

    [Fact]
    public void Step_Renders_On_Input()
    {
        var cut = Render<NumericField<decimal>>(p => p.Add(x => x.Step, 0.01m));
        Assert.Equal("0.01", cut.Find("input").GetAttribute("step"));
    }

    [Fact]
    public void Disabled_Renders_On_Input()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.Class, "my-field"));
        Assert.Contains("my-field", cut.Markup);
    }

    [Fact]
    public void ShowButtons_Renders_Increment_Decrement()
    {
        var cut = Render<NumericField<int>>(p => p.Add(x => x.ShowButtons, true));
        Assert.NotNull(cut.Find("[aria-label='Decrease value']"));
        Assert.NotNull(cut.Find("[aria-label='Increase value']"));
    }

    [Fact]
    public void ShowButtons_False_No_Buttons()
    {
        var cut = Render<NumericField<int>>();
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find("[aria-label='Decrease value']"));
    }

    [Fact]
    public void Prefix_Renders_Content()
    {
        var cut = Render<NumericField<decimal>>(p => p
            .Add(x => x.Prefix, (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "$"))));
        Assert.Contains("$", cut.Markup);
    }

    [Fact]
    public void Suffix_Renders_Content()
    {
        var cut = Render<NumericField<int>>(p => p
            .Add(x => x.Suffix, (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "kg"))));
        Assert.Contains("kg", cut.Markup);
    }

    [Fact]
    public void Value_Change_Triggers_ValueChanged()
    {
        int newValue = 0;
        var cut = Render<NumericField<int>>(p => p
            .Add(x => x.Value, 0)
            .Add(x => x.ValueChanged, (int v) => newValue = v));
        cut.Find("input").Change("42");
        Assert.Equal(42, newValue);
    }

    [Fact]
    public void Value_Is_Clamped_To_Min()
    {
        int newValue = 0;
        var cut = Render<NumericField<int>>(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.Min, 0)
            .Add(x => x.ValueChanged, (int v) => newValue = v));
        cut.Find("input").Change("-10");
        Assert.Equal(0, newValue);
    }

    [Fact]
    public void Value_Is_Clamped_To_Max()
    {
        int newValue = 0;
        var cut = Render<NumericField<int>>(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.Max, 100)
            .Add(x => x.ValueChanged, (int v) => newValue = v));
        cut.Find("input").Change("999");
        Assert.Equal(100, newValue);
    }

    [Fact]
    public void Works_With_Decimal()
    {
        var cut = Render<NumericField<decimal>>(p => p.Add(x => x.Value, 3.14m));
        Assert.Equal("3.14", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Works_With_Double()
    {
        var cut = Render<NumericField<double>>(p => p.Add(x => x.Value, 2.718));
        Assert.Equal("2.718", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void AriaLabel_PassesThrough()
    {
        var cut = Render<NumericField<int>>(p => p.AddUnmatched("aria-label", "Quantity"));
        AssertAriaLabel(cut.Find("input"), "Quantity");
    }
}
