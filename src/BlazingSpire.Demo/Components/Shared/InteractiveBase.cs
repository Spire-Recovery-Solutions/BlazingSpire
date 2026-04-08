using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Branch point for all controls that can be disabled.
/// </summary>
public abstract class InteractiveBase : BlazingSpireComponentBase
{
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Whether the component should behave as disabled. Override to combine
    /// with additional state (e.g., <c>Disabled || Loading</c> in ButtonBase).
    /// </summary>
    protected virtual bool IsEffectivelyDisabled => Disabled;
}
