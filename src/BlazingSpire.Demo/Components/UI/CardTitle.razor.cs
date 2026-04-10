using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>Title element within a CardHeader.</summary>
public partial class CardTitle : ChildOf<Card>
{
    protected override string BaseClasses => "text-2xl font-semibold leading-none tracking-tight";
}
