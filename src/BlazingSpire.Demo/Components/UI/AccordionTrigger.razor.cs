using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AccordionTrigger : BlazingSpireComponentBase
{
    protected override string BaseClasses =>
        "flex flex-1 cursor-pointer items-center justify-between py-4 font-medium transition-all hover:underline [&>svg]:transition-transform";
}
