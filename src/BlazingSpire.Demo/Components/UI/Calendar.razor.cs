using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A month calendar for date selection.</summary>
public partial class Calendar : BlazingSpireComponentBase
{
    /// <summary>The currently selected date.</summary>
    [Parameter] public DateOnly? SelectedDate { get; set; }
    /// <summary>Callback invoked when the selected date changes.</summary>
    [Parameter] public EventCallback<DateOnly?> SelectedDateChanged { get; set; }
    /// <summary>The month currently displayed.</summary>
    [Parameter] public DateOnly DisplayMonth { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    protected override string BaseClasses => "p-3";

    private DateOnly FirstDayOfMonth => new(DisplayMonth.Year, DisplayMonth.Month, 1);
    private int DaysInMonth => DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);

    private void PreviousMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(-1);
        StateHasChanged();
    }

    private void NextMonth()
    {
        DisplayMonth = DisplayMonth.AddMonths(1);
        StateHasChanged();
    }

    private async Task SelectDayAsync(int day)
    {
        var date = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day);
        SelectedDate = SelectedDate == date ? null : date;
        await SelectedDateChanged.InvokeAsync(SelectedDate);
    }

    private bool IsSelected(int day) =>
        SelectedDate == new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day);

    private bool IsToday(int day) =>
        new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day) == DateOnly.FromDateTime(DateTime.Today);

    private string GetDayClasses(bool selected, bool today)
    {
        var baseDay = "inline-flex h-9 w-9 items-center justify-center rounded-md text-sm transition-colors hover:bg-accent hover:text-accent-foreground focus:outline-none";
        if (selected) return $"{baseDay} bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground";
        if (today) return $"{baseDay} bg-accent text-accent-foreground";
        return baseDay;
    }
}
