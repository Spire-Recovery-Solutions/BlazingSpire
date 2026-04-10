namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for infrastructure components that should NOT appear in the playground:
/// service providers (DialogProvider, ToastProvider), marker types (MessageBoxDialog),
/// and other components that exist for technical reasons rather than as user-facing UI.
///
/// Detected programmatically via the type system — playground discovery skips any
/// component inheriting this base.
/// </summary>
public abstract class Infrastructure : BlazingSpireComponentBase
{
}
