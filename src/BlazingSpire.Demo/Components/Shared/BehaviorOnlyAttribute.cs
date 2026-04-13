namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Marks a [Parameter] as behavioral-only.
/// Its value changes event timing, interaction mode, or selection semantics —
/// not the initial rendered DOM. The involution test suite routes parameters
/// with this attribute to a behavior-aware test path instead of the snapshot
/// liveness check.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BehaviorOnlyAttribute : Attribute { }
