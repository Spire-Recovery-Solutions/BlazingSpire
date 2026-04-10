using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.UI;

public partial class DialogProvider : IDisposable
{
    private DialogService _dialogService = default!;

    protected override void OnInitialized()
    {
        _dialogService = (DialogService)DialogService;
        _dialogService.OnDialogsChanged += OnDialogsChanged;
    }

    private void OnDialogsChanged() => InvokeAsync(StateHasChanged);

    // Defer to the closed-generic factory captured at ShowAsync<TDialog> call time.
    // Using OpenComponent(0, Type) here would be trim-unsafe: the trimmer can't see
    // which TDialog types flow through runtime Type lookups, so it would conservatively
    // keep every ComponentBase subclass alive (or, worse, trim the one we need).
    private RenderFragment CreateDynamicComponent(DialogReference dialog) => builder =>
        dialog.RenderFactory?.Invoke(builder);

    public void Dispose() => _dialogService.OnDialogsChanged -= OnDialogsChanged;
}
