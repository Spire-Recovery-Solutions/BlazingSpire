using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class AccordionContent : ChildOf<AccordionItem>
{
    protected override string BaseClasses => "overflow-hidden text-sm pb-4 pt-0";
}
