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
    /// <summary>Earliest selectable date. Days before this date are rendered disabled.</summary>
    [Parameter] public DateOnly? MinDate { get; set; }
    /// <summary>Latest selectable date. Days after this date are rendered disabled.</summary>
    [Parameter] public DateOnly? MaxDate { get; set; }

    protected override string BaseClasses => "p-3";

    private DateOnly FirstDayOfMonth => new(DisplayMonth.Year, DisplayMonth.Month, 1);
    private int DaysInMonth => DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);

    private bool IsPrevMonthDisabled =>
        MinDate.HasValue && FirstDayOfMonth <= MinDate.Value;

    private bool IsNextMonthDisabled =>
        MaxDate.HasValue && new DateOnly(DisplayMonth.Year, DisplayMonth.Month, DaysInMonth) >= MaxDate.Value;

    private void PreviousMonth()
    {
        if (!IsPrevMonthDisabled)
        {
            DisplayMonth = DisplayMonth.AddMonths(-1);
            StateHasChanged();
        }
    }

    private void NextMonth()
    {
        if (!IsNextMonthDisabled)
        {
            DisplayMonth = DisplayMonth.AddMonths(1);
            StateHasChanged();
        }
    }

    private async Task SelectDayAsync(int day)
    {
        if (IsDayDisabled(day)) return;
        var date = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day);
        SelectedDate = SelectedDate == date ? null : date;
        await SelectedDateChanged.InvokeAsync(SelectedDate);
    }

    private bool IsSelected(int day) =>
        SelectedDate == new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day);

    private bool IsToday(int day) =>
        new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day) == DateOnly.FromDateTime(DateTime.Today);

    private bool IsDayDisabled(int day)
    {
        var date = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, day);
        if (MinDate.HasValue && date < MinDate.Value) return true;
        if (MaxDate.HasValue && date > MaxDate.Value) return true;
        return false;
    }

    private string GetDayClasses(bool selected, bool today, bool disabled)
    {
        var baseDay = "inline-flex h-9 w-9 items-center justify-center rounded-md text-sm transition-colors focus:outline-none";
        if (disabled) return $"{baseDay} opacity-50 cursor-not-allowed text-muted-foreground";
        if (selected) return $"{baseDay} bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground";
        if (today) return $"{baseDay} bg-accent text-accent-foreground hover:bg-accent hover:text-accent-foreground";
        return $"{baseDay} hover:bg-accent hover:text-accent-foreground";
    }
}
