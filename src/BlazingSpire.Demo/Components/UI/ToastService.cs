namespace BlazingSpire.Demo.Components.UI;

public enum ToastVariant { Default, Destructive }

public record ToastMessage(
    string? Title,
    string? Description,
    ToastVariant Variant = ToastVariant.Default,
    int DurationMs = 5000,
    string? ActionLabel = null,
    Action? OnAction = null);

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    void Show(ToastMessage message);
    void Show(string title, string? description = null, ToastVariant variant = ToastVariant.Default);
}

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Show(ToastMessage message) => OnShow?.Invoke(message);

    public void Show(string title, string? description = null, ToastVariant variant = ToastVariant.Default)
        => Show(new ToastMessage(title, description, variant));
}
