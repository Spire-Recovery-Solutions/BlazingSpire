using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for components that exist as children of a parent component.
/// Inheriting from <c>ChildOf&lt;TParent&gt;</c> intrinsically declares this component's
/// role and parent through the type system — detected programmatically with no
/// attributes, naming conventions, or external markers required.
/// </summary>
/// <typeparam name="TParent">The parent component this component must be rendered inside.</typeparam>
public abstract class ChildOf<TParent> : BlazingSpireComponentBase
    where TParent : BlazingSpireComponentBase
{
    /// <summary>The parent component, automatically supplied via cascading value.</summary>
    [CascadingParameter] public TParent? Parent { get; set; }
}
