using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Marks a component as a repeating slot inside a composite. When a composite's
/// playground factory walks the <see cref="ChildOf{TParent}"/> type graph and
/// encounters a slot implementing this interface, it emits a <c>for</c>-loop over
/// <see cref="GetSampleCount"/> at render time, setting <see cref="IndexParameterName"/>
/// on each iteration.
///
/// <para>Purely a type-system signal — no attributes, no naming conventions. Mirrors
/// the <see cref="ChildOf{TParent}"/> pattern: inheritance/interface implementation
/// <i>is</i> the declaration.</para>
/// </summary>
/// <typeparam name="TRoot">The outermost composite root that owns the parameter
/// driving the slot count (e.g. <c>InputOTP.MaxLength</c> for <c>InputOTPSlot</c>).</typeparam>
public interface IRepeatingSlot<TRoot> where TRoot : ComponentBase
{
    /// <summary>
    /// How many instances of this slot to emit in the default playground, given the
    /// current root component instance. Called at render time so toggling parameters
    /// in the playground (e.g. MaxLength) updates the slot count live.
    /// </summary>
    static abstract int GetSampleCount(TRoot root);

    /// <summary>
    /// Name of the <c>[Parameter]</c> on the slot that should receive the loop index
    /// (0-based). Validated via <c>nameof</c> at the call site so renames stay safe.
    /// </summary>
    static abstract string IndexParameterName { get; }

    /// <summary>
    /// Name of the <c>[Parameter]</c> on <typeparamref name="TRoot"/> that drives the
    /// slot count. The playground generator reads this parameter value from the live
    /// playground parameter dictionary rather than relying on a reference capture
    /// (which only fires on initial component creation, not on re-renders).
    /// </summary>
    static abstract string CountParameterName { get; }
}
