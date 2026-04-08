using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Shared;

/// <summary>
/// Base for text-based form controls (Input, Textarea, InputOTP).
/// </summary>
public abstract class TextInputBase : FormControlBase<string>
{
    [Parameter] public int? MaxLength { get; set; }
    [Parameter] public string? Pattern { get; set; }
    [Parameter] public string? AutoComplete { get; set; }
}
