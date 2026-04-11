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
    /// Root components that have their own playground page (not Child components
    /// like AlertTitle, DialogContent, etc. which only render inside a parent).
    /// Derived programmatically from composition.role in components.json.
    /// </summary>
    public static IEnumerable<ComponentMeta> TopLevel =>
        All.Where(c => c.Composition.Role == "Root");

    /// <summary>Components with at least one Enum parameter.</summary>
    public static IEnumerable<(ComponentMeta Component, ParameterMeta Parameter, string Value)> EnumPermutations =>
        from c in TopLevel
        from p in c.Parameters.Where(p => p.Kind == "Enum" && p.EnumValues is { Count: > 0 })
        from v in p.EnumValues!
        select (c, p, v);

    /// <summary>Components with at least one Bool parameter.</summary>
    public static IEnumerable<(ComponentMeta Component, ParameterMeta Parameter, bool Value)> BoolPermutations =>
        from c in TopLevel
        from p in c.Parameters.Where(p => p.Kind == "Boolean")
        from v in new[] { true, false }
        select (c, p, v);

    /// <summary>
    /// Recursively collects every descendant of a root component by walking the
    /// composition tree. After the hierarchical ChildOf refactor, direct children
    /// are only the immediate visual containers; true descendants (Title, Description,
    /// Action, Cancel, etc.) live inside nested containers and need a tree walk.
    /// </summary>
    public static IEnumerable<string> AllDescendantNames(ComponentMeta root)
    {
        var byName = All.ToDictionary(c => c.Name, c => c);
        var seen = new HashSet<string>();
        var stack = new Stack<ComponentMeta>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            foreach (var childName in cur.Composition.Children)
            {
                if (!seen.Add(childName)) continue;
                yield return childName;
                if (byName.TryGetValue(childName, out var child))
                    stack.Push(child);
            }
        }
    }

    /// <summary>
    /// Overlay-style composites: have an IsOpen parameter, a Trigger descendant, and
    /// a Description descendant. Walks the full composition tree (Description may
    /// live several levels deep, e.g. Dialog → Content → Header → Description).
    /// </summary>
    public static IEnumerable<ComponentMeta> OverlayComposites =>
        TopLevel.Where(c =>
        {
            if (!c.Parameters.Any(p => p.Name == "IsOpen")) return false;
            var descendants = AllDescendantNames(c).ToHashSet();
            return descendants.Contains($"{c.Name}Trigger")
                && descendants.Contains($"{c.Name}Description");
        });

    /// <summary>
    /// Every top-level composite (root with at least one declared child). Used by
    /// "non-empty playground" assertions to catch bugs where the composite factory
    /// produces nothing visible. Overlay composites intentionally show only their
    /// trigger until clicked — the non-empty test accounts for this by accepting
    /// any count >= 1.
    /// </summary>
    public static IEnumerable<ComponentMeta> CompositesWithMultipleChildren =>
        TopLevel.Where(c => c.Composition.Children.Count >= 1);

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
    [JsonPropertyName("composition")] public CompositionMeta Composition { get; set; } = new();
    [JsonPropertyName("parameters")] public List<ParameterMeta> Parameters { get; set; } = new();
}

internal sealed class CompositionMeta
{
    [JsonPropertyName("role")] public string Role { get; set; } = "Root";
    [JsonPropertyName("parent")] public string? Parent { get; set; }
    [JsonPropertyName("children")] public List<string> Children { get; set; } = new();
    [JsonPropertyName("isComposite")] public bool IsComposite { get; set; }
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
