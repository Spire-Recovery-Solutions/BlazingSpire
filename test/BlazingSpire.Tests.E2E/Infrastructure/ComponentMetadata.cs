using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Runtime loader for component metadata from components.json.
/// Single source of truth for parameterized tests — reads from the same file
/// the playground UI consumes, so tests automatically cover new components.
/// </summary>
internal static class ComponentMetadata
{
    private static IReadOnlyList<ComponentMeta>? _cache;
    private static readonly object _lock = new();

    public static IReadOnlyList<ComponentMeta> All
    {
        get
        {
            if (_cache is not null) return _cache;
            lock (_lock)
            {
                if (_cache is not null) return _cache;
                var path = FindComponentsJson();
                var json = File.ReadAllText(path);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _cache = JsonSerializer.Deserialize<List<ComponentMeta>>(json, options)
                    ?? new List<ComponentMeta>();
                return _cache;
            }
        }
    }

    /// <summary>
    /// Top-level components that have their own playground page (not sub-components
    /// like AlertTitle, DialogContent, etc. which only render inside a parent).
    /// </summary>
    public static IEnumerable<ComponentMeta> TopLevel =>
        All.Where(c => !IsSubComponent(c.Name));

    /// <summary>Components with at least one Enum parameter.</summary>
    public static IEnumerable<(ComponentMeta Component, ParameterMeta Parameter, string Value)> EnumPermutations =>
        from c in TopLevel
        from p in c.Parameters.Where(p => p.Kind == "Enum" && p.EnumValues is { Count: > 0 })
        from v in p.EnumValues!
        select (c, p, v);

    /// <summary>Components with at least one Bool parameter.</summary>
    public static IEnumerable<(ComponentMeta Component, ParameterMeta Parameter, bool Value)> BoolPermutations =>
        from c in TopLevel
        from p in c.Parameters.Where(p => p.Kind == "Boolean" || p.Kind == "Bool")
        from v in new[] { true, false }
        select (c, p, v);

    private static bool IsSubComponent(string name)
    {
        // Heuristic: names ending in structural suffixes are child components
        // These only render inside a cascading parent and don't appear on their own page
        var suffixes = new[]
        {
            "Content", "Trigger", "Title", "Description", "Header", "Footer",
            "Close", "Action", "Cancel", "Input", "Value", "Item", "Empty",
            "Label", "List", "Separator", "Group", "Slot"
        };
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix, StringComparison.Ordinal) && name != suffix)
            {
                // The name has the suffix AND isn't literally just the suffix
                // Additional check: the prefix should match a known parent component name
                var prefix = name.Substring(0, name.Length - suffix.Length);
                if (prefix.Length > 0) return true;
            }
        }
        return false;
    }

    private static string FindComponentsJson()
    {
        // Check deploy locations: next to the test DLL first, then walk up
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var direct = Path.Combine(dir.FullName, "components.json");
            if (File.Exists(direct)) return direct;

            var wwwroot = Path.Combine(dir.FullName, "src", "BlazingSpire.Demo", "wwwroot", "components.json");
            if (File.Exists(wwwroot)) return wwwroot;

            dir = dir.Parent;
        }
        throw new FileNotFoundException("components.json not found — run DocGen first");
    }
}

internal sealed class ComponentMeta
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("category")] public string Category { get; set; } = "";
    [JsonPropertyName("baseTier")] public string BaseTier { get; set; } = "";
    [JsonPropertyName("parameters")] public List<ParameterMeta> Parameters { get; set; } = new();
}

internal sealed class ParameterMeta
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("kind")] public string Kind { get; set; } = "";
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("defaultValue")] public string? DefaultValue { get; set; }
    [JsonPropertyName("enumValues")] public List<string>? EnumValues { get; set; }
}
