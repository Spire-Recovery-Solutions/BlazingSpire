using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class TimePickerTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Input_Element()
    {
        var cut = Render<TimePicker>();
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(TimePicker).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Default_Placeholder_Is_Pick_A_Time()
    {
        var cut = Render<TimePicker>();
        Assert.Equal("Pick a time", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Custom_Placeholder_Renders()
    {
        var cut = Render<TimePicker>(p => p.Add(x => x.Placeholder, "Select time"));
        Assert.Equal("Select time", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void SelectedTime_Displays_Formatted_Value()
    {
        var time = new TimeOnly(14, 30);
        var cut = Render<TimePicker>(p => p.Add(x => x.SelectedTime, time));
        Assert.Equal("2:30 PM", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void No_Time_Shows_Empty_Value()
    {
        var cut = Render<TimePicker>();
        Assert.Equal("", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Input_Is_ReadOnly()
    {
        var cut = Render<TimePicker>();
        Assert.True(cut.Find("input").HasAttribute("readonly"));
    }

    [Fact]
    public void Disabled_Renders_On_Input()
    {
        var cut = Render<TimePicker>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Clock_Icon_Is_Rendered()
    {
        var cut = Render<TimePicker>();
        Assert.NotNull(cut.Find("svg"));
    }

    [Fact]
    public void Morning_Time_Shows_AM()
    {
        var time = new TimeOnly(9, 0);
        var cut = Render<TimePicker>(p => p.Add(x => x.SelectedTime, time));
        Assert.Equal("9:00 AM", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Midnight_Shows_12_AM()
    {
        var time = new TimeOnly(0, 0);
        var cut = Render<TimePicker>(p => p.Add(x => x.SelectedTime, time));
        Assert.Equal("12:00 AM", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Noon_Shows_12_PM()
    {
        var time = new TimeOnly(12, 0);
        var cut = Render<TimePicker>(p => p.Add(x => x.SelectedTime, time));
        Assert.Equal("12:00 PM", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<TimePicker>(p => p.Add(x => x.Class, "my-picker"));
        Assert.Contains("my-picker", cut.Markup);
    }
}
