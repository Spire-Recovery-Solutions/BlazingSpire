using System.Threading;
using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class FormField : BlazingSpireComponentBase
{
    [Parameter] public string? Name { get; set; }

    private static int s_counter;
    private readonly string _id = $"bs-form-{Interlocked.Increment(ref s_counter)}";

    public string ItemId => Name is not null ? $"{Name}-form-item" : _id;
    public string DescriptionId => $"{ItemId}-description";
    public string MessageId => $"{ItemId}-message";

    [Parameter] public string? ErrorMessage { get; set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    protected override string BaseClasses => "space-y-2";
}
