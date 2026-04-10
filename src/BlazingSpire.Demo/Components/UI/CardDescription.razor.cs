using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>Description text within a CardHeader.</summary>
public partial class CardDescription : ChildOf<Card>
{
    protected override string BaseClasses => "text-sm text-muted-foreground";
}
