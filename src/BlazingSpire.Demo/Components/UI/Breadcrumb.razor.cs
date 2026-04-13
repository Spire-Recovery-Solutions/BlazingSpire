using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class Breadcrumb : BlazingSpireComponentBase
{
    protected override string BaseClasses => "flex flex-wrap items-center";

    protected override string Classes => BuildClasses(BaseClasses, Class);
}
