using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DatePickerTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Input_Element()
    {
        var cut = Render<DatePicker>();
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(DatePicker).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Default_Placeholder_Is_Pick_A_Date()
    {
        var cut = Render<DatePicker>();
        Assert.Equal("Pick a date", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Custom_Placeholder_Renders()
    {
        var cut = Render<DatePicker>(p => p.Add(x => x.Placeholder, "Select date"));
        Assert.Equal("Select date", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void SelectedDate_Displays_Formatted_Value()
    {
        var date = new DateOnly(2026, 3, 15);
        var cut = Render<DatePicker>(p => p.Add(x => x.SelectedDate, date));
        Assert.Equal("03/15/2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Custom_DateFormat_Formats_Value()
    {
        var date = new DateOnly(2026, 3, 15);
        var cut = Render<DatePicker>(p => p
            .Add(x => x.SelectedDate, date)
            .Add(x => x.DateFormat, "yyyy-MM-dd"));
        Assert.Equal("2026-03-15", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void No_Date_Shows_Empty_Value()
    {
        var cut = Render<DatePicker>();
        Assert.Equal("", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Input_Is_ReadOnly()
    {
        var cut = Render<DatePicker>();
        Assert.True(cut.Find("input").HasAttribute("readonly"));
    }

    [Fact]
    public void Disabled_Renders_On_Input()
    {
        var cut = Render<DatePicker>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void Calendar_Icon_Is_Rendered()
    {
        var cut = Render<DatePicker>();
        Assert.NotNull(cut.Find("svg"));
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<DatePicker>(p => p.Add(x => x.Class, "my-picker"));
        Assert.Contains("my-picker", cut.Markup);
    }
}
