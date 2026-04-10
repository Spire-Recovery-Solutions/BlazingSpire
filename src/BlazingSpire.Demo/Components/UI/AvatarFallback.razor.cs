using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AvatarFallback : ChildOf<Avatar>
{
    protected override string BaseClasses =>
        "flex h-full w-full items-center justify-center rounded-full bg-muted";
}
