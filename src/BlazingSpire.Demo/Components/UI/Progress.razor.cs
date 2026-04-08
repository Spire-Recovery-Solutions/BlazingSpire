using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Progress : BlazingSpireComponentBase
{
    [Parameter] public int Value { get; set; }

    protected override string BaseClasses =>
        "relative h-4 w-full overflow-hidden rounded-full bg-secondary";
}
