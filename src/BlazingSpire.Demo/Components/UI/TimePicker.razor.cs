using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A time picker with hour, minute, and AM/PM selection.</summary>
public partial class TimePicker : BlazingSpireComponentBase
{
    /// <summary>The currently selected time.</summary>
    [Parameter] public TimeOnly? SelectedTime { get; set; }
    /// <summary>Callback invoked when the selected time changes.</summary>
    [Parameter] public EventCallback<TimeOnly?> SelectedTimeChanged { get; set; }
    /// <summary>Earliest selectable time.</summary>
    [Parameter] public TimeOnly? MinTime { get; set; }
    /// <summary>Latest selectable time.</summary>
    [Parameter] public TimeOnly? MaxTime { get; set; }
    /// <summary>Placeholder text when no time is selected.</summary>
    [Parameter] public string? Placeholder { get; set; } = "Pick a time";
    /// <summary>Whether the time picker is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }
    /// <summary>Minute increment step size.</summary>
    [Parameter] public int MinuteStep { get; set; } = 1;

    protected override string BaseClasses => "";

    private bool _isOpen;
    private int _selectedHour = 12;
    private int _selectedMinute;
    private bool _isPM;

    private string DisplayValue => SelectedTime?.ToString("h:mm tt") ?? "";

    protected override void OnParametersSet()
    {
        if (SelectedTime.HasValue)
        {
            var h = SelectedTime.Value.Hour;
            _isPM = h >= 12;
            _selectedHour = h % 12;
            if (_selectedHour == 0) _selectedHour = 12;
            _selectedMinute = SelectedTime.Value.Minute;
        }
    }

    private int To24Hour() => _isPM
        ? (_selectedHour == 12 ? 12 : _selectedHour + 12)
        : (_selectedHour == 12 ? 0 : _selectedHour);

    private async Task SetHourAsync(int hour)
    {
        _selectedHour = hour;
        await UpdateTimeAsync();
    }

    private async Task SetMinuteAsync(int minute)
    {
        _selectedMinute = minute;
        await UpdateTimeAsync();
    }

    private async Task TogglePeriodAsync()
    {
        _isPM = !_isPM;
        await UpdateTimeAsync();
    }

    private async Task UpdateTimeAsync()
    {
        var time = new TimeOnly(To24Hour(), _selectedMinute);

        if (MinTime.HasValue && time < MinTime.Value) time = MinTime.Value;
        if (MaxTime.HasValue && time > MaxTime.Value) time = MaxTime.Value;

        SelectedTime = time;
        await SelectedTimeChanged.InvokeAsync(time);
    }

    private async Task ConfirmAsync()
    {
        _isOpen = false;
        await Task.CompletedTask;
    }

    private string HourButtonClasses(int hour) =>
        hour == _selectedHour
            ? "inline-flex h-8 w-8 items-center justify-center rounded-md bg-primary text-primary-foreground text-sm font-medium"
            : "inline-flex h-8 w-8 items-center justify-center rounded-md text-sm hover:bg-accent hover:text-accent-foreground";

    private string MinuteButtonClasses(int minute) =>
        minute == _selectedMinute
            ? "inline-flex h-8 w-8 items-center justify-center rounded-md bg-primary text-primary-foreground text-sm font-medium"
            : "inline-flex h-8 w-8 items-center justify-center rounded-md text-sm hover:bg-accent hover:text-accent-foreground";

    private string PeriodButtonClasses(bool isPM) =>
        isPM == _isPM
            ? "inline-flex h-8 items-center justify-center rounded-md bg-primary text-primary-foreground px-3 text-sm font-medium"
            : "inline-flex h-8 items-center justify-center rounded-md px-3 text-sm hover:bg-accent hover:text-accent-foreground";
}
