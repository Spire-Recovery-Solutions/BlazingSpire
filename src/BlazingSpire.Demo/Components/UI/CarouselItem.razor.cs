using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class CarouselItem : ChildOf<CarouselContent>, IDisposable
{
    [CascadingParameter] private Carousel? _carousel { get; set; }

    protected override string BaseClasses => "min-w-0 shrink-0 grow-0 basis-full pl-4";

    protected override void OnInitialized()
    {
        _carousel?.RegisterItem();
    }

    public void Dispose()
    {
        _carousel?.UnregisterItem();
    }
}
