using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>Result returned when a dialog closes.</summary>
public sealed class DialogResult
{
    public bool Cancelled { get; init; }
    public object? Data { get; init; }

    public static DialogResult Ok(object? data = null) => new() { Cancelled = false, Data = data };
    public static DialogResult Cancel() => new() { Cancelled = true };
}

/// <summary>Parameters passed to a dialog opened via IDialogService.</summary>
public sealed class DialogParameters
{
    private readonly Dictionary<string, object?> _parameters = new();

    public DialogParameters Set(string name, object? value)
    {
        _parameters[name] = value;
        return this;
    }

    public T? Get<T>(string name) =>
        _parameters.TryGetValue(name, out var value) ? (T?)value : default;

    internal Dictionary<string, object?> ToDictionary() => new(_parameters);
}

/// <summary>Reference to an open dialog. Await this to get the result.</summary>
public sealed class DialogReference
{
    internal TaskCompletionSource<DialogResult> Tcs { get; } = new();
    public Type ComponentType { get; init; } = default!;
    public string? Title { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, object?>? Parameters { get; init; }
    internal Guid Id { get; } = Guid.NewGuid();

    public Task<DialogResult> Result => Tcs.Task;

    public void Close(DialogResult result) => Tcs.TrySetResult(result);
}

/// <summary>Service for programmatic dialog invocation.</summary>
public interface IDialogService
{
    event Action? OnDialogsChanged;
    IReadOnlyList<DialogReference> OpenDialogs { get; }

    /// <summary>Open a dialog component by type.</summary>
    Task<DialogResult> ShowAsync<TDialog>(string? title = null, DialogParameters? parameters = null)
        where TDialog : ComponentBase;

    /// <summary>Simple confirmation dialog.</summary>
    Task<DialogResult> ShowMessageBoxAsync(string title, string message, string confirmText = "OK", string cancelText = "Cancel");
}

public sealed class DialogService : IDialogService
{
    private readonly List<DialogReference> _dialogs = [];

    public event Action? OnDialogsChanged;
    public IReadOnlyList<DialogReference> OpenDialogs => _dialogs;

    public Task<DialogResult> ShowAsync<TDialog>(string? title = null, DialogParameters? parameters = null)
        where TDialog : ComponentBase
    {
        var reference = new DialogReference
        {
            ComponentType = typeof(TDialog),
            Title = title,
            Parameters = parameters?.ToDictionary(),
        };
        _dialogs.Add(reference);
        OnDialogsChanged?.Invoke();

        reference.Tcs.Task.ContinueWith(_ =>
        {
            _dialogs.Remove(reference);
            OnDialogsChanged?.Invoke();
        }, TaskScheduler.Current);

        return reference.Result;
    }

    public Task<DialogResult> ShowMessageBoxAsync(string title, string message, string confirmText = "OK", string cancelText = "Cancel")
    {
        var parameters = new DialogParameters()
            .Set("Message", message)
            .Set("ConfirmText", confirmText)
            .Set("CancelText", cancelText);

        var reference = new DialogReference
        {
            ComponentType = typeof(MessageBoxDialog),
            Title = title,
            Message = message,
            Parameters = parameters.ToDictionary(),
        };
        _dialogs.Add(reference);
        OnDialogsChanged?.Invoke();

        reference.Tcs.Task.ContinueWith(_ =>
        {
            _dialogs.Remove(reference);
            OnDialogsChanged?.Invoke();
        }, TaskScheduler.Current);

        return reference.Result;
    }
}
