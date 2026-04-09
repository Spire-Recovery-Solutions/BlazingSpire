using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A date picker combining an input field with a calendar popover.</summary>
public partial class DatePicker : BlazingSpireComponentBase
{
    /// <summary>The currently selected date.</summary>
    [Parameter] public DateOnly? SelectedDate { get; set; }
    /// <summary>Callback invoked when the selected date changes.</summary>
    [Parameter] public EventCallback<DateOnly?> SelectedDateChanged { get; set; }
    /// <summary>Earliest selectable date.</summary>
    [Parameter] public DateOnly? MinDate { get; set; }
    /// <summary>Latest selectable date.</summary>
    [Parameter] public DateOnly? MaxDate { get; set; }
    /// <summary>Date display format string.</summary>
    [Parameter] public string DateFormat { get; set; } = "MM/dd/yyyy";
    /// <summary>Placeholder text when no date is selected.</summary>
    [Parameter] public string? Placeholder { get; set; } = "Pick a date";
    /// <summary>Whether the date picker is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses => "";

    private bool _isOpen;
    private DateOnly _displayMonth = DateOnly.FromDateTime(DateTime.Today);

    private string DisplayValue => SelectedDate?.ToString(DateFormat) ?? "";

    protected override void OnParametersSet()
    {
        if (SelectedDate.HasValue)
            _displayMonth = new DateOnly(SelectedDate.Value.Year, SelectedDate.Value.Month, 1);
    }

    private async Task OnDateSelectedAsync(DateOnly? date)
    {
        SelectedDate = date;
        await SelectedDateChanged.InvokeAsync(date);
        _isOpen = false;
    }
}
