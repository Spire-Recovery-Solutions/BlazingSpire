using System.Text.Json.Serialization;

namespace BlazingSpire.Demo.Components.Demo;

public sealed class ComponentMeta
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("category")] public string Category { get; set; } = "";
    [JsonPropertyName("baseTier")] public string BaseTier { get; set; } = "";
    [JsonPropertyName("parameters")] public List<ParameterMeta> Parameters { get; set; } = [];
}

public sealed class ParameterMeta
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("kind")] public string Kind { get; set; } = ""; // String, Bool, Int, Enum, etc.
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("defaultValue")] public string? DefaultValue { get; set; }
    [JsonPropertyName("enumValues")] public List<string>? EnumValues { get; set; }
    [JsonPropertyName("minimum")] public double? Minimum { get; set; }
    [JsonPropertyName("maximum")] public double? Maximum { get; set; }
}
