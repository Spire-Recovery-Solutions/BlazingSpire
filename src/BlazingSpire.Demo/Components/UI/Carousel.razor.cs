using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public enum CarouselOrientation { Horizontal, Vertical }

public partial class Carousel : BlazingSpireComponentBase
{
    [Parameter] public CarouselOrientation Orientation { get; set; } = CarouselOrientation.Horizontal;
    [Parameter] public int CurrentIndex { get; set; }
    [Parameter] public EventCallback<int> CurrentIndexChanged { get; set; }

    public int ItemCount { get; set; }

    protected override string BaseClasses => "relative";

    internal void RegisterItem()
    {
        ItemCount++;
        StateHasChanged();
    }

    internal void UnregisterItem()
    {
        ItemCount--;
        StateHasChanged();
    }

    public async Task PreviousAsync()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
            await CurrentIndexChanged.InvokeAsync(CurrentIndex);
            StateHasChanged();
        }
    }

    public async Task NextAsync()
    {
        if (CurrentIndex < ItemCount - 1)
        {
            CurrentIndex++;
            await CurrentIndexChanged.InvokeAsync(CurrentIndex);
            StateHasChanged();
        }
    }
}
