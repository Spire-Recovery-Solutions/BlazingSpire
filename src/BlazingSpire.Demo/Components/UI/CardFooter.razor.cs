using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// The footer section of a <see cref="Card"/>. Displays children in a horizontal flex row.
/// </summary>
public partial class CardFooter
{
    /// <summary>The footer content, typically action buttons.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the footer element.</summary>
    [Parameter] public string? Class { get; set; }
}
