using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class CommandItem : ChildOf<Command>
{
    public Command? ParentCommand => Parent;

    [Parameter] public string? FilterText { get; set; }
    [Parameter] public EventCallback OnSelect { get; set; }
    [Parameter] public bool Disabled { get; set; }

    public bool IsVisible => string.IsNullOrEmpty(ParentCommand?.SearchText) ||
        (FilterText?.Contains(ParentCommand.SearchText, StringComparison.OrdinalIgnoreCase) == true);

    protected override string BaseClasses =>
        "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none " +
        "aria-selected:bg-accent aria-selected:text-accent-foreground " +
        "data-[disabled]:pointer-events-none data-[disabled]:opacity-50";
}
