using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class InputOTP : BlazingSpireComponentBase
{
    [Parameter] public string Value { get; set; } = "";
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public int MaxLength { get; set; } = 6;
    [Parameter] public bool Disabled { get; set; }

    protected override string BaseClasses => "flex items-center gap-2 has-[:disabled]:opacity-50";

    private ElementReference _inputRef;
    private bool _focused;

    public bool IsFocused => _focused;
    public char? GetChar(int index) => index < Value.Length ? Value[index] : null;

    private async Task FocusInputAsync() => await _inputRef.FocusAsync();

    public async Task UpdateValueAsync(string newValue)
    {
        if (newValue.Length <= MaxLength)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
            StateHasChanged();
        }
    }
}
