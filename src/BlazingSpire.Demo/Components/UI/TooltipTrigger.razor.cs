using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class TooltipTrigger : BlazingSpireComponentBase
{
    [CascadingParameter] public Tooltip? ParentTooltip { get; set; }

    protected override string BaseClasses => "";

    private CancellationTokenSource? _delayCts;

    private async Task OnMouseEnterAsync()
    {
        if (ParentTooltip is null) return;
        _delayCts?.Cancel();
        _delayCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(ParentTooltip.DelayDuration, _delayCts.Token);
            await ParentTooltip.SetIsOpenAsync(true);
        }
        catch (TaskCanceledException) { }
    }

    private async Task OnMouseLeaveAsync()
    {
        _delayCts?.Cancel();
        if (ParentTooltip is not null)
            await ParentTooltip.SetIsOpenAsync(false);
    }
}
