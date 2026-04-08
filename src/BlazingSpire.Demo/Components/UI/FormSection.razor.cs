using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class FormSection : BlazingSpireComponentBase
{
    [Parameter] public string? Legend { get; set; }
    [Parameter] public string? Description { get; set; }
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses => "space-y-6";
}
