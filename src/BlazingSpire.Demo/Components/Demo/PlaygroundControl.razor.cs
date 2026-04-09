using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Demo;

public partial class PlaygroundControl
{
    [Parameter, EditorRequired] public ParameterMeta Meta { get; set; } = default!;
    [Parameter] public object? Value { get; set; }
    [Parameter] public EventCallback<object?> ValueChanged { get; set; }
}
