using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SliderTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Outer_Div()
    {
        var cut = Render<Slider>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Renders_Input_Type_Range()
    {
        var cut = Render<Slider>();
        Assert.NotNull(cut.Find("input[type='range']"));
    }

    // ── Default value ─────────────────────────────────────────────────────────

    [Fact]
    public void Default_Value_Is_50()
    {
        var cut = Render<Slider>();
        Assert.Equal("50", cut.Find("input[type='range']").GetAttribute("value"));
    }

    // ── Min / Max / Step attributes ───────────────────────────────────────────

    [Fact]
    public void Min_Attribute_Is_Set()
    {
        var cut = Render<Slider>(p => p.Add(x => x.Min, 10));
        Assert.Equal("10", cut.Find("input[type='range']").GetAttribute("min"));
    }

    [Fact]
    public void Max_Attribute_Is_Set()
    {
        var cut = Render<Slider>(p => p.Add(x => x.Max, 200));
        Assert.Equal("200", cut.Find("input[type='range']").GetAttribute("max"));
    }

    [Fact]
    public void Step_Attribute_Is_Set()
    {
        var cut = Render<Slider>(p => p.Add(x => x.Step, 5));
        Assert.Equal("5", cut.Find("input[type='range']").GetAttribute("step"));
    }

    // ── Disabled ──────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Input_Has_Disabled_Attribute()
    {
        var cut = Render<Slider>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input[type='range']").HasAttribute("disabled"));
    }

    [Fact]
    public void Enabled_Input_Does_Not_Have_Disabled_Attribute()
    {
        var cut = Render<Slider>();
        Assert.False(cut.Find("input[type='range']").HasAttribute("disabled"));
    }

    // ── Fill width reflects percentage ────────────────────────────────────────

    [Theory]
    [InlineData(0, 0, 100, "0%")]
    [InlineData(50, 0, 100, "50%")]
    [InlineData(100, 0, 100, "100%")]
    [InlineData(500, 0, 1000, "50%")]
    public void Fill_Width_Reflects_Percentage(double value, double min, double max, string expectedWidth)
    {
        var cut = Render<Slider>(p => p
            .Add(x => x.Value, value)
            .Add(x => x.Min, min)
            .Add(x => x.Max, max));

        // The fill div is the inner div inside the track div
        var trackDiv = cut.Find("div > div");
        var fillDiv = trackDiv.QuerySelector("div");
        Assert.NotNull(fillDiv);
        Assert.Contains(expectedWidth, fillDiv!.GetAttribute("style") ?? "");
    }

    // ── Base classes ──────────────────────────────────────────────────────────

    [Fact]
    public void Always_Has_Base_Classes()
    {
        var cut = Render<Slider>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("relative", classes);
        Assert.Contains("flex", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("items-center", classes);
    }

    // ── Class parameter ───────────────────────────────────────────────────────

    [Fact]
    public void Custom_Class_Is_Appended()
    {
        var cut = Render<Slider>(p => p.Add(x => x.Class, "my-custom-class"));
        Assert.Contains("my-custom-class", cut.Find("div").ClassName);
    }

    // ── AdditionalAttributes ──────────────────────────────────────────────────

    [Fact]
    public void AriaLabel_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Slider>(p =>
            p.AddUnmatched("aria-label", "Volume"));

        AssertAriaLabel(cut.Find("div"), "Volume");
    }

    [Fact]
    public void DataTestId_PassesThrough_Via_AdditionalAttributes()
    {
        var cut = Render<Slider>(p =>
            p.AddUnmatched("data-testid", "volume-slider"));

        Assert.Equal("volume-slider", cut.Find("div").GetAttribute("data-testid"));
    }

    // ── ValueChanged callback ─────────────────────────────────────────────────

    [Fact]
    public async Task ValueChanged_Fires_When_Input_Changes()
    {
        double? received = null;
        var cut = Render<Slider>(p =>
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, v => received = v)));

        await cut.Find("input[type='range']").TriggerEventAsync("oninput",
            new ChangeEventArgs { Value = "75" });

        Assert.Equal(75.0, received);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void Slider_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Slider).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
