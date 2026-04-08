using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Card : BlazingSpireComponentBase
{
    protected override string BaseClasses =>
        "rounded-xl border bg-card text-card-foreground shadow-[0_0_2px_rgba(0,0,0,0.12),0_2px_4px_rgba(0,0,0,0.24)]";
}
