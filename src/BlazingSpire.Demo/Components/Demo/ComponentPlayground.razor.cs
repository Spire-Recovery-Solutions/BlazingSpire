using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Demo.Components.Demo;

public partial class ComponentPlayground
{
    [Inject] private HttpClient Http { get; set; } = default!;

    [Parameter, EditorRequired] public string ComponentName { get; set; } = "";

    private ComponentMeta? _meta;
    private bool _loading = true;
    private Dictionary<string, object?> _paramValues = [];
    private RenderFragment? _renderFragment;
    private int _renderKey;
    private string _snippet = "";
    private string _tonl = "";
    private IReadOnlyList<ParameterMeta> _editableParams = [];

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        _meta = await MetaService.GetAsync(ComponentName);
        if (_meta is not null)
        {
            // Filter to editable params (skip events, expressions, objects, and AdditionalAttributes)
            _editableParams = _meta.Parameters
                .Where(p => p.Kind is not ("Event" or "Expression" or "Object")
                    && p.Name is not "AdditionalAttributes" and not "ChildContent")
                .ToList();

            // Add ChildContent separately if present (so it appears last)
            var childContent = _meta.Parameters.FirstOrDefault(p => p.Name == "ChildContent");
            if (childContent is not null)
                _editableParams = [.._editableParams, childContent];

            // Initialize defaults
            _paramValues = [];
            foreach (var p in _editableParams)
            {
                _paramValues[p.Name] = ParseDefault(p);
            }

            UpdateRender();

            // Load TONL doc
            try
            {
                _tonl = await Http.GetStringAsync($"examples/{ComponentName.ToLowerInvariant()}.tonl");
            }
            catch
            {
                _tonl = "";
            }
        }
        _loading = false;
    }

    private void OnParameterChanged(string name, object? value)
    {
        _paramValues[name] = value;
        UpdateRender();
        StateHasChanged();
    }

    private void UpdateRender()
    {
        if (_meta is null) return;

        // Resolve string values to their typed equivalents before passing to the render factory
        var resolved = new Dictionary<string, object?>(_paramValues.Count);
        foreach (var (key, value) in _paramValues)
        {
            var paramMeta = _editableParams.FirstOrDefault(p => p.Name == key);
            resolved[key] = CoerceValue(paramMeta, value);
        }

        // Get render factory from source generator
        if (PlaygroundFactories.All.TryGetValue(_meta.Name, out var factory))
        {
            _renderFragment = factory(resolved);
            _renderKey++;
        }

        // Build code snippet
        _snippet = BuildSnippet();
    }

    private static object? CoerceValue(ParameterMeta? meta, object? value)
    {
        if (meta is null || value is null) return value;

        if (meta.Kind == "Enum" && value is string enumStr)
            return ResolveEnum(meta.Type, enumStr);

        if (meta.Kind == "Boolean" && value is string bs)
            return bool.TryParse(bs, out var b) && b;

        // For numeric kinds, use the declared parameter Type to pick the right parser
        if (meta.Kind == "Number" && value is string ns)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return meta.Type switch
            {
                "double" => double.TryParse(ns, System.Globalization.NumberStyles.Any, culture, out var d) ? d : 0.0,
                "float"  => float.TryParse(ns, System.Globalization.NumberStyles.Any, culture, out var f) ? f : 0f,
                "decimal" => decimal.TryParse(ns, System.Globalization.NumberStyles.Any, culture, out var dec) ? dec : 0m,
                "long" => long.TryParse(ns, System.Globalization.NumberStyles.Any, culture, out var l) ? l : 0L,
                _ => int.TryParse(ns, System.Globalization.NumberStyles.Any, culture, out var i) ? i : 0,
            };
        }

        return value;
    }

    private static readonly string[] s_enumNamespaces =
    [
        "BlazingSpire.Demo.Components.UI",
        "BlazingSpire.Demo.Components.Shared",
        "BlazingSpire.Demo.Components.Layout",
        "BlazingSpire.Demo.Components.Demo",
    ];

    private static object? ResolveEnum(string enumTypeName, string valueName)
    {
        var assembly = typeof(PlaygroundFactories).Assembly;
        foreach (var ns in s_enumNamespaces)
        {
            var enumType = assembly.GetType($"{ns}.{enumTypeName}");
            if (enumType is not null && enumType.IsEnum)
                return Enum.Parse(enumType, valueName);
        }
        return valueName;
    }

    private string BuildSnippet()
    {
        if (_meta is null) return "";
        var sb = new System.Text.StringBuilder();
        sb.Append('<').Append(_meta.Name);

        string? childContent = null;

        foreach (var param in _editableParams)
        {
            var value = _paramValues.GetValueOrDefault(param.Name);
            var defaultStr = param.DefaultValue;

            if (param.Kind == "Content")
            {
                childContent = value?.ToString();
                continue;
            }

            // Skip if at default
            var valueStr = FormatValue(param, value);
            if (valueStr == defaultStr || (string.IsNullOrEmpty(valueStr) && string.IsNullOrEmpty(defaultStr)))
                continue;

            if (param.Kind == "Boolean")
            {
                if (value is true)
                    sb.Append($" {param.Name}=\"true\"");
            }
            else if (param.Kind == "Enum")
            {
                sb.Append($" {param.Name}=\"{param.Type}.{value}\"");
            }
            else
            {
                sb.Append($" {param.Name}=\"{valueStr}\"");
            }
        }

        if (string.IsNullOrEmpty(childContent))
        {
            sb.Append(" />");
        }
        else
        {
            sb.Append('>').Append(childContent).Append("</").Append(_meta.Name).Append('>');
        }

        return sb.ToString();
    }

    private static string? FormatValue(ParameterMeta param, object? value)
    {
        return value?.ToString();
    }

    private static object? ParseDefault(ParameterMeta param)
    {
        if (string.IsNullOrEmpty(param.DefaultValue)) return null;
        // Delegate to CoerceValue so numeric, boolean, and enum parsing all
        // honor the actual parameter Type rather than assuming int.
        return CoerceValue(param, param.DefaultValue);
    }
}
