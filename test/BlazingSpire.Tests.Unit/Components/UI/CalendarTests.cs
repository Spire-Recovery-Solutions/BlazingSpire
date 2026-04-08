using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CalendarTests : BlazingSpireTestBase
{
    [Fact]
    public void Calendar_Renders_Div_With_P3()
    {
        var cut = Render<Calendar>();
        Assert.Contains("p-3", cut.Find("div").ClassName);
    }

    [Fact]
    public void Calendar_Has_Table()
    {
        var cut = Render<Calendar>();
        Assert.NotNull(cut.Find("table"));
    }

    [Fact]
    public void Calendar_Has_Seven_Day_Headers()
    {
        var cut = Render<Calendar>();
        var headers = cut.FindAll("thead th");
        Assert.Equal(7, headers.Count);
    }

    [Fact]
    public void Calendar_Day_Headers_Are_Correct()
    {
        var cut = Render<Calendar>();
        var headers = cut.FindAll("thead th").Select(h => h.TextContent.Trim()).ToArray();
        Assert.Equal(new[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" }, headers);
    }

    [Fact]
    public void Calendar_Renders_Correct_Number_Of_Days_For_February_2024()
    {
        // Feb 2024 has 29 days (leap year)
        var cut = Render<Calendar>(p => p.Add(x => x.DisplayMonth, new DateOnly(2024, 2, 1)));
        var dayButtons = cut.FindAll("tbody button");
        Assert.Equal(29, dayButtons.Count);
    }

    [Fact]
    public void Calendar_Renders_Correct_Number_Of_Days_For_January_2025()
    {
        // Jan 2025 has 31 days
        var cut = Render<Calendar>(p => p.Add(x => x.DisplayMonth, new DateOnly(2025, 1, 1)));
        var dayButtons = cut.FindAll("tbody button");
        Assert.Equal(31, dayButtons.Count);
    }

    [Fact]
    public void Calendar_Clicking_A_Day_Selects_It()
    {
        DateOnly? selected = null;
        var cut = Render<Calendar>(p =>
        {
            p.Add(x => x.DisplayMonth, new DateOnly(2025, 1, 1));
            p.Add(x => x.SelectedDateChanged, (DateOnly? d) => selected = d);
        });

        cut.FindAll("tbody button").First().Click();

        Assert.NotNull(selected);
    }

    [Fact]
    public void Calendar_Clicking_Selected_Day_Deselects_It()
    {
        DateOnly? selected = new DateOnly(2025, 1, 1);
        var cut = Render<Calendar>(p =>
        {
            p.Add(x => x.DisplayMonth, new DateOnly(2025, 1, 1));
            p.Add(x => x.SelectedDate, selected);
            p.Add(x => x.SelectedDateChanged, (DateOnly? d) => selected = d);
        });

        // Click the first day button (day 1 = already selected)
        cut.FindAll("tbody button").First().Click();

        Assert.Null(selected);
    }

    [Fact]
    public void Calendar_Previous_Month_Button_Changes_Display()
    {
        var cut = Render<Calendar>(p => p.Add(x => x.DisplayMonth, new DateOnly(2025, 3, 1)));

        cut.Find("button[aria-label='Go to previous month']").Click();

        Assert.Contains("February 2025", cut.Find("div div").TextContent);
    }

    [Fact]
    public void Calendar_Next_Month_Button_Changes_Display()
    {
        var cut = Render<Calendar>(p => p.Add(x => x.DisplayMonth, new DateOnly(2025, 3, 1)));

        cut.Find("button[aria-label='Go to next month']").Click();

        Assert.Contains("April 2025", cut.Find("div div").TextContent);
    }

    [Fact]
    public void Calendar_Custom_Class_Is_Appended()
    {
        var cut = Render<Calendar>(p => p.Add(x => x.Class, "my-calendar"));
        Assert.Contains("my-calendar", cut.Find("div").ClassName);
    }

    [Fact]
    public void Calendar_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Calendar>(p => p.AddUnmatched("data-testid", "calendar"));
        Assert.Equal("calendar", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void Calendar_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Calendar).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
