using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Breadcrumb : BlazingSpireComponentBase
{
    protected override string BaseClasses => "";

    protected override string Classes => BuildClasses(Class);
}
