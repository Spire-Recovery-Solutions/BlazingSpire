using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class MenubarTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public MenubarMenu? ParentMenu { get; set; }

    protected override string BaseClasses =>
        "flex cursor-default select-none items-center rounded-sm px-3 py-1.5 text-sm font-medium outline-none " +
        "focus:bg-accent focus:text-accent-foreground " +
        "data-[state=open]:bg-accent data-[state=open]:text-accent-foreground";

    private void OnClick() => ParentMenu?.Toggle();
}
