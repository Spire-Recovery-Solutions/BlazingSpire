using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ToastProvider : BlazingSpireComponentBase, IDisposable
{
    [Inject] private IToastService ToastService { get; set; } = default!;

    private readonly List<ToastMessage> _toasts = [];

    protected override void OnInitialized()
    {
        ToastService.OnShow += OnToastShow;
    }

    private void OnToastShow(ToastMessage message)
    {
        InvokeAsync(() =>
        {
            _toasts.Add(message);
            StateHasChanged();
            _ = DismissAfterDelay(message);
        });
    }

    private async Task DismissAfterDelay(ToastMessage message)
    {
        await Task.Delay(message.DurationMs);
        _toasts.Remove(message);
        await InvokeAsync(StateHasChanged);
    }

    public void Dismiss(ToastMessage message)
    {
        _toasts.Remove(message);
        StateHasChanged();
    }

    protected override string BaseClasses =>
        "fixed bottom-0 right-0 z-[100] flex max-h-screen w-full flex-col-reverse p-4 sm:bottom-0 sm:right-0 sm:top-auto sm:flex-col-reverse md:max-w-[420px]";

    public void Dispose()
    {
        ToastService.OnShow -= OnToastShow;
    }
}
