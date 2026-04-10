using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TabsList : ChildOf<Tabs>
{
    protected override string BaseClasses =>
        "inline-flex h-10 items-center justify-center rounded-md bg-muted p-1 text-muted-foreground";
}
