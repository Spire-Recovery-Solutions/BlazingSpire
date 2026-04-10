using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AlertDescription : ChildOf<Alert>
{
    protected override string BaseClasses =>
        "text-sm [&_p]:leading-relaxed";
}
