using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class CarouselPrevious : ChildOf<Carousel>
{
    [CascadingParameter] private Carousel? _carousel { get; set; }

    private static readonly string HorizontalClasses =
        "absolute left-4 top-1/2 -translate-y-1/2 z-10 h-8 w-8 inline-flex items-center justify-center " +
        "rounded-full border bg-background shadow-sm cursor-pointer " +
        "hover:bg-accent hover:text-accent-foreground " +
        "disabled:opacity-50 disabled:pointer-events-none";

    private static readonly string VerticalClasses =
        "absolute top-4 left-1/2 -translate-x-1/2 z-10 h-8 w-8 inline-flex items-center justify-center " +
        "rounded-full border bg-background shadow-sm cursor-pointer " +
        "hover:bg-accent hover:text-accent-foreground " +
        "disabled:opacity-50 disabled:pointer-events-none";

    protected override string BaseClasses =>
        _carousel?.Orientation == CarouselOrientation.Vertical ? VerticalClasses : HorizontalClasses;

    internal bool IsDisabled => _carousel is null || _carousel.CurrentIndex <= 0;

    internal async Task HandleClick()
    {
        if (_carousel is not null)
            await _carousel.PreviousAsync();
    }
}
