using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AvatarImage : ChildOf<Avatar>
{
    [Parameter] public string? Src { get; set; }
    [Parameter] public string? Alt { get; set; }

    protected override string BaseClasses => "aspect-square h-full w-full";
}
