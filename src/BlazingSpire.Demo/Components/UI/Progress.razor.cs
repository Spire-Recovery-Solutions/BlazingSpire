using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>A horizontal progress bar indicating completion.</summary>
public partial class Progress : BlazingSpireComponentBase
{
    /// <summary>Current progress value (0-100).</summary>
    [Parameter] public int Value { get; set; }

    protected override string BaseClasses =>
        "relative h-4 w-full overflow-hidden rounded-full bg-secondary";
}
