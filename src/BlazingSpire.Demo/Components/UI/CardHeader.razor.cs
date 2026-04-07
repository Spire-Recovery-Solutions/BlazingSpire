using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// The header section of a <see cref="Card"/>. Contains <see cref="CardTitle"/> and <see cref="CardDescription"/>.
/// </summary>
public partial class CardHeader
{
    /// <summary>The header content, typically <see cref="CardTitle"/> and <see cref="CardDescription"/>.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the header element.</summary>
    [Parameter] public string? Class { get; set; }
}
