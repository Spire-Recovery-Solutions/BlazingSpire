using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class CarouselContent : ChildOf<Carousel>
{
    [CascadingParameter] private Carousel? _carousel { get; set; }

    protected override string BaseClasses =>
        _carousel?.Orientation == CarouselOrientation.Vertical
            ? "flex -mt-4 flex-col transition-transform duration-300 ease-in-out"
            : "flex -ml-4 transition-transform duration-300 ease-in-out";

    internal string TranslateStyle =>
        _carousel?.Orientation == CarouselOrientation.Vertical
            ? $"transform: translateY(-{_carousel.CurrentIndex * 100}%);"
            : $"transform: translateX(-{(_carousel?.CurrentIndex ?? 0) * 100}%);";
}
