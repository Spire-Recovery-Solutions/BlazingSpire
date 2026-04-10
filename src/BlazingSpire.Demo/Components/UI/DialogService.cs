using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
    /// <summary>The component type backing this dialog. Used by DialogProvider to branch on the built-in MessageBox path; custom dialogs render via <see cref="RenderFactory"/>.</summary>
    public Type ComponentType { get; init; } = default!;
    /// <summary>Dialog title (MessageBox only).</summary>
    public string? Title { get; init; }
    /// <summary>Dialog message (MessageBox only).</summary>
    public string? Message { get; init; }
    /// <summary>Parameters that will be applied to the rendered component.</summary>
    public Dictionary<string, object?>? Parameters { get; init; }

    /// <summary>
    /// Closure that renders the dialog's component into a RenderTreeBuilder. Captured at
    /// <see cref="DialogService.ShowAsync{TDialog}"/> call time with a closed generic
    /// (<c>builder.OpenComponent&lt;TDialog&gt;()</c>) so the trimmer can statically see
    /// every component type that ever flows through <see cref="IDialogService"/> — no
    /// reflection, no runtime Type lookup. Null for the built-in MessageBox branch,
    /// which the provider renders inline without going through a component type.
    /// </summary>
    internal Action<RenderTreeBuilder>? RenderFactory { get; init; }

    internal Guid Id { get; } = Guid.NewGuid();

    /// <summary>Task completed when the dialog closes.</summary>
    public Task<DialogResult> Result => Tcs.Task;

    /// <summary>Close the dialog and complete <see cref="Result"/> with the given outcome.</summary>
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
        var paramDict = parameters?.ToDictionary();

        // Closed generic: the trimmer statically sees every TDialog that ever flows
        // through ShowAsync<T>. No runtime Type lookup, no reflection.
        var factory = new Action<RenderTreeBuilder>(builder =>
        {
            builder.OpenComponent<TDialog>(0);
            if (paramDict is not null)
            {
                var seq = 1;
                foreach (var (key, value) in paramDict)
                {
                    builder.AddAttribute(seq++, key, value);
                }
            }
            builder.CloseComponent();
        });

        var reference = new DialogReference
        {
            ComponentType = typeof(TDialog),
            Title = title,
            Parameters = paramDict,
            RenderFactory = factory,
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
