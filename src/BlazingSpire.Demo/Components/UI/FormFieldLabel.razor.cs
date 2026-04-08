using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class FormFieldLabel : BlazingSpireComponentBase
{
    [CascadingParameter] public FormField? Field { get; set; }

    protected override string BaseClasses =>
        "text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70";
}
