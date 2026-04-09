using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for text-based form controls (Input, Textarea, InputOTP).
/// </summary>
public abstract class TextInputBase : FormControlBase<string>
{
    /// <summary>Maximum number of characters allowed.</summary>
    [Parameter] public int? MaxLength { get; set; }
    /// <summary>Regex pattern for input validation.</summary>
    [Parameter] public string? Pattern { get; set; }
    /// <summary>Browser autocomplete hint.</summary>
    [Parameter] public string? AutoComplete { get; set; }
}
