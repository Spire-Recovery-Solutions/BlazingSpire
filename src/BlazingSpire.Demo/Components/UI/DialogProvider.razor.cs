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

    private RenderFragment CreateDynamicComponent(DialogReference dialog) => builder =>
    {
        builder.OpenComponent(0, dialog.ComponentType);
        if (dialog.Parameters is not null)
        {
            var seq = 1;
            foreach (var (key, value) in dialog.Parameters)
            {
                builder.AddAttribute(seq++, key, value);
            }
        }
        builder.CloseComponent();
    };

    public void Dispose() => _dialogService.OnDialogsChanged -= OnDialogsChanged;
}
