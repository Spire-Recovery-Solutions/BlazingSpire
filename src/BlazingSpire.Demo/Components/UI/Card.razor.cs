using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// A container component with rounded corners, border, and shadow.
/// Inspired by shadcn/ui Card. Use with <see cref="CardHeader"/>, <see cref="CardContent"/>, and <see cref="CardFooter"/>.
/// </summary>
public partial class Card
{
    /// <summary>The card content, typically <see cref="CardHeader"/>, <see cref="CardContent"/>, and <see cref="CardFooter"/> components.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the card element.</summary>
    [Parameter] public string? Class { get; set; }
}
