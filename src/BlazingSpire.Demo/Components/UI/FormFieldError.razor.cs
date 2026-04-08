using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class FormFieldError : BlazingSpireComponentBase
{
    [CascadingParameter] public FormField? Field { get; set; }
    protected override string BaseClasses => "text-sm font-medium text-destructive";
}
