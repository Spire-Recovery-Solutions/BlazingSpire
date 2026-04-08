using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for all form controls. Provides two-way Value binding,
/// EditContext integration, validation state, and ARIA attributes.
/// </summary>
public abstract class FormControlBase<TValue> : InteractiveBase, IDisposable
{
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }

    [Parameter] public string? Name { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }

    [CascadingParameter] private EditContext? CascadedEditContext { get; set; }

    protected EditContext? EditContext => CascadedEditContext;
    protected FieldIdentifier FieldId { get; private set; }
    private bool _hasFieldId;
    private bool _previousValidationState;

    protected bool IsInvalid =>
        _hasFieldId && EditContext?.GetValidationMessages(FieldId).Any() == true;

    protected string? AriaInvalid => IsInvalid ? "true" : null;
    protected string? AriaDescribedBy => IsInvalid ? $"{Name ?? FieldId.FieldName}-error" : null;

    protected override string Classes => BuildClasses(
        BaseClasses,
        IsInvalid ? "border-destructive ring-destructive" : null,
        Class);

    protected override void OnParametersSet()
    {
        if (ValueExpression is not null && !_hasFieldId)
        {
            FieldId = FieldIdentifier.Create(ValueExpression);
            _hasFieldId = true;
        }
    }

    protected override void OnInitialized()
    {
        if (CascadedEditContext is not null)
            CascadedEditContext.OnValidationStateChanged += OnValidationStateChanged;
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        var currentState = IsInvalid;
        if (currentState != _previousValidationState)
        {
            _previousValidationState = currentState;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Call from concrete components to update the bound value.
    /// Handles two-way binding notification and EditContext field change.
    /// </summary>
    protected async Task SetValueAsync(TValue? value)
    {
        if (EqualityComparer<TValue?>.Default.Equals(Value, value)) return;
        Value = value;
        await ValueChanged.InvokeAsync(value);
        if (_hasFieldId)
            EditContext?.NotifyFieldChanged(FieldId);
    }

    public virtual void Dispose()
    {
        if (CascadedEditContext is not null)
            CascadedEditContext.OnValidationStateChanged -= OnValidationStateChanged;
    }
}
