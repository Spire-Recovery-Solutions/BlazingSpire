using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

/// <summary>
/// Renders a syntax-highlighted code block using Prism.js.
/// Sets textContent via JS interop to keep Prism's DOM changes outside Blazor's render tree.
/// </summary>
public partial class CodeBlock : BlazingSpireComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Raw code to display. Pass plain text — HTML encoding is handled internally.</summary>
    [Parameter, EditorRequired] public string Code { get; set; } = "";

    /// <summary>Prism language class (e.g., "xml", "csharp"). Default: "xml".</summary>
    [Parameter] public string Language { get; set; } = "xml";

    protected override string BaseClasses => "";

    private ElementReference _codeRef;
    private string _lastHighlighted = "";

    private string LanguageClass => $"language-{Language}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var trimmed = Code.Trim();
        if (trimmed != _lastHighlighted)
        {
            _lastHighlighted = trimmed;
            await JS.InvokeVoidAsync("BlazingSpire.setTextAndHighlight", _codeRef, trimmed);
        }
    }
}
