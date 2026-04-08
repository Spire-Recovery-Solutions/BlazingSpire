using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class HoverCard : PopoverBase
{
    [Parameter] public int OpenDelay { get; set; } = 300;
    [Parameter] public int CloseDelay { get; set; } = 200;

    protected override bool ShouldCloseOnEscape => true;
    protected override bool ShouldCloseOnInteractOutside => false;
    protected override string BaseClasses => "";

    private CancellationTokenSource? _openCts;
    private CancellationTokenSource? _closeCts;

    public async Task HandleMouseEnterAsync()
    {
        _closeCts?.Cancel();
        _openCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(OpenDelay, _openCts.Token);
            await SetIsOpenAsync(true);
        }
        catch (TaskCanceledException) { }
    }

    public async Task HandleMouseLeaveAsync()
    {
        _openCts?.Cancel();
        _closeCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(CloseDelay, _closeCts.Token);
            await SetIsOpenAsync(false);
        }
        catch (TaskCanceledException) { }
    }
}
