using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// The main body section of a <see cref="Card"/>. Provides padding with no top padding (assumes <see cref="CardHeader"/> above).
/// </summary>
public partial class CardContent
{
    /// <summary>The body content of the card.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the content element.</summary>
    [Parameter] public string? Class { get; set; }
}
