using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum InputSize { Sm, Default, Lg }

/// <summary>A form input field with two-way binding, sizes, and adornment slots.</summary>
public partial class Input : TextInputBase
{
    /// <summary>HTML input type (text, email, password, etc.).</summary>
    [Parameter] public string Type { get; set; } = "text";
    /// <summary>The size of the input field.</summary>
    [Parameter] public InputSize Size { get; set; } = InputSize.Default;
    /// <summary>Update value on every keystroke instead of on blur.</summary>
    [Parameter] public bool Immediate { get; set; }
    /// <summary>Hint for virtual keyboard type (numeric, tel, etc.).</summary>
    [Parameter] public string? InputMode { get; set; }
    /// <summary>Content displayed before the input (e.g., currency symbol).</summary>
    [Parameter] public RenderFragment? Prefix { get; set; }
    /// <summary>Content displayed after the input (e.g., unit label).</summary>
    [Parameter] public RenderFragment? Suffix { get; set; }
    /// <summary>Callback invoked on key down events.</summary>
    [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    private bool HasAdornments => Prefix is not null || Suffix is not null;

    protected override string BaseClasses =>
        "flex w-full rounded-md border border-input bg-background text-sm ring-offset-background " +
        "file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground " +
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 " +
        "disabled:cursor-not-allowed disabled:opacity-50";

    private static readonly FrozenDictionary<InputSize, string> s_sizes = new Dictionary<InputSize, string>
    {
        [InputSize.Sm]      = "h-8 px-2 text-xs",
        [InputSize.Default] = "h-10 px-3 py-2",
        [InputSize.Lg]      = "h-12 px-4 text-base",
    }.ToFrozenDictionary();

    protected override string Classes => BuildClasses(
        BaseClasses,
        s_sizes.GetValueOrDefault(Size, "h-10 px-3 py-2"),
        HasAdornments ? "px-0" : null,
        IsInvalid ? "border-destructive ring-destructive" : null,
        Class);

    private string WrapperClasses => BuildClasses(
        "flex items-center rounded-md border border-input bg-background ring-offset-background " +
        "has-[input:focus-visible]:ring-2 has-[input:focus-visible]:ring-ring has-[input:focus-visible]:ring-offset-2",
        s_sizes.GetValueOrDefault(Size, "h-10"),
        IsEffectivelyDisabled ? "cursor-not-allowed opacity-50" : null,
        IsInvalid ? "border-destructive ring-destructive" : null,
        Class);

    private string InnerInputClasses => BuildClasses(
        "flex-1 min-w-0 bg-transparent text-sm placeholder:text-muted-foreground " +
        "focus-visible:outline-none disabled:cursor-not-allowed",
        Size == InputSize.Sm ? "px-2 text-xs" : Size == InputSize.Lg ? "px-4 text-base" : "px-3 py-2");

    private Task OnInputAsync(ChangeEventArgs e) =>
        Immediate ? SetValueAsync(e.Value?.ToString()) : Task.CompletedTask;

    private Task OnChangeAsync(ChangeEventArgs e) => SetValueAsync(e.Value?.ToString());
}
