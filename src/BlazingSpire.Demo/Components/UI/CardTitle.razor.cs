using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// The title element within a <see cref="CardHeader"/>. Renders as an <c>&lt;h3&gt;</c>.
/// </summary>
public partial class CardTitle
{
    /// <summary>The title text or markup.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the title element.</summary>
    [Parameter] public string? Class { get; set; }
}
