using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// A muted description paragraph within a <see cref="CardHeader"/>. Renders as a <c>&lt;p&gt;</c>.
/// </summary>
public partial class CardDescription
{
    /// <summary>The description text or markup.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Additional CSS classes to append to the description element.</summary>
    [Parameter] public string? Class { get; set; }
}
