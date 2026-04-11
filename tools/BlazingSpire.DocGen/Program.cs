using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

if (args.Length < 3)
{
    Console.Error.WriteLine("Usage: BlazingSpire.DocGen <assemblyPath> <xmlDocPath> <outputDir>");
    return 1;
}

var assemblyPath = Path.GetFullPath(args[0]);
var xmlDocPath = Path.GetFullPath(args[1]);
var outputDir = Path.GetFullPath(args[2]);

if (!File.Exists(assemblyPath))
{
    Console.Error.WriteLine($"Assembly not found: {assemblyPath}");
    return 1;
}

// ── Load assembly ──────────────────────────────────────────────────────────
var assembly = Assembly.LoadFrom(assemblyPath);
var baseType = assembly.GetTypes().FirstOrDefault(t => t.Name == "BlazingSpireComponentBase");
if (baseType is null)
{
    Console.Error.WriteLine("BlazingSpireComponentBase not found in assembly.");
    return 1;
}

// ── Programmatic component discovery ────────────────────────────────────────
//
// All classification is derived from the type system — no naming conventions,
// no exclusion lists, no attributes required:
//
//   • Component:        inherits BlazingSpireComponentBase, concrete, in UI namespace
//   • Infrastructure:   inherits Components.Shared.Infrastructure → excluded
//   • Child:            inherits Components.Shared.ChildOf<TParent> → role=Child, parent=TParent
//   • Root:             everything else → role=Root, gets a playground page
//   • Composite parent: a Root that other components declare ChildOf<thisType>
//
// One inheritance check per fact. Universal, structural, intrinsic.

const string UiNamespace = "BlazingSpire.Demo.Components.UI";
const string InfrastructureBase = "Infrastructure";
const string ChildOfBase = "ChildOf`1";

bool InheritsByName(Type t, string baseName)
{
    var current = t.BaseType;
    while (current is not null && current != typeof(object))
    {
        if (current.Name == baseName) return true;
        current = current.BaseType;
    }
    return false;
}

Type? GetChildOfParent(Type t)
{
    var current = t.BaseType;
    while (current is not null && current != typeof(object))
    {
        if (current.IsGenericType
            && current.GetGenericTypeDefinition().Name == ChildOfBase)
        {
            return current.GetGenericArguments()[0];
        }
        current = current.BaseType;
    }
    return null;
}

// Discover all concrete UI components, excluding Infrastructure markers
var allComponents = assembly.GetTypes()
    .Where(t => t.Namespace == UiNamespace
                && !t.IsAbstract
                && !t.IsGenericTypeDefinition
                && baseType.IsAssignableFrom(t)
                && !InheritsByName(t, InfrastructureBase))
    .OrderBy(t => t.Name)
    .ToList();

Console.WriteLine($"Discovered {allComponents.Count} UI components.");

// Build composition map from ChildOf<TParent> inheritance
var parentChildMap = new Dictionary<string, List<string>>(StringComparer.Ordinal);
var childParent = new Dictionary<string, string>(StringComparer.Ordinal);
var childComponents = new HashSet<string>(StringComparer.Ordinal);

foreach (var type in allComponents)
{
    var parentType = GetChildOfParent(type);
    if (parentType is null) continue;

    childComponents.Add(type.Name);
    childParent[type.Name] = parentType.Name;
    if (!parentChildMap.TryGetValue(parentType.Name, out var kids))
    {
        kids = new List<string>();
        parentChildMap[parentType.Name] = kids;
    }
    if (!kids.Contains(type.Name)) kids.Add(type.Name);
}

Console.WriteLine($"Identified {childComponents.Count} children and {parentChildMap.Count} composite parents.");

var components = allComponents;
var sourceGenRoots = new HashSet<string>(
    components.Where(c => !childComponents.Contains(c.Name)).Select(c => c.Name),
    StringComparer.Ordinal);

Console.WriteLine($"Final: {components.Count} components ({sourceGenRoots.Count} Roots, {childComponents.Count} Children).");

// ── Parse XML docs ─────────────────────────────────────────────────────────
var xmlDocs = new Dictionary<string, string>(StringComparer.Ordinal);
if (File.Exists(xmlDocPath))
{
    var doc = XDocument.Load(xmlDocPath);
    foreach (var member in doc.Descendants("member"))
    {
        var name = member.Attribute("name")?.Value;
        var summary = member.Element("summary")?.Value?.Trim();
        if (name is not null && summary is not null)
            xmlDocs[name] = NormalizeSummary(summary);
    }
    Console.WriteLine($"Parsed {xmlDocs.Count} XML doc entries.");
}
else
{
    Console.WriteLine($"Warning: XML doc file not found: {xmlDocPath}");
}

// ── Extract metadata ───────────────────────────────────────────────────────
var componentModels = new List<ComponentModel>();

foreach (var type in components)
{
    try
    {
        var model = ExtractComponent(type, xmlDocs, baseType, assembly);
        if (model is null) continue;

        // Populate composition from programmatic analysis
        // Role is determined by source generator membership, not just cascading params,
        // so structural children (without [CascadingParameter]) are still classified correctly.
        var isChild = !sourceGenRoots.Contains(type.Name);
        var hasCascadingParent = childComponents.Contains(type.Name);
        var slotInfo = GetRepeatingSlotInfo(type);
        model.Composition = new CompositionModel
        {
            Role = isChild ? "Child" : "Root",
            Parent = hasCascadingParent ? childParent[type.Name] : null,
            Children = parentChildMap.TryGetValue(type.Name, out var kids) ? kids : new List<string>(),
            IsComposite = parentChildMap.ContainsKey(type.Name),
            IsRepeatingSlot = slotInfo.IsSlot ? true : null,
            CountParameterOwner = slotInfo.RootName,
            CountParameterName = slotInfo.CountParam,
            IndexParameterName = slotInfo.IndexParam,
        };

        componentModels.Add(model);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Warning: Skipping {type.Name}: {ex.Message}");
    }
}

Console.WriteLine($"Extracted metadata for {componentModels.Count} components.");

// ── Generate outputs ───────────────────────────────────────────────────────
Directory.CreateDirectory(outputDir);
var examplesDir = Path.Combine(outputDir, "examples");
Directory.CreateDirectory(examplesDir);

// OpenAPI spec
GenerateOpenApiSpec(componentModels, outputDir);

// TONL files
foreach (var comp in componentModels)
    GenerateTonlFile(comp, examplesDir);

// components.json
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};
var json = JsonSerializer.Serialize(componentModels, jsonOptions);

var componentsJsonPath = Path.Combine(outputDir, "components.json");
File.WriteAllText(componentsJsonPath, json);

// Copy to wwwroot if the demo project exists nearby
// Assembly is at e.g. artifacts/bin/BlazingSpire.Demo/debug/ — go up 4 levels to repo root
var assemblyDir = Path.GetDirectoryName(assemblyPath)!;
var wwwrootTarget = Path.GetFullPath(
    Path.Combine(assemblyDir, "..", "..", "..", "..", "src", "BlazingSpire.Demo", "wwwroot", "components.json"));
var wwwrootDir = Path.GetDirectoryName(wwwrootTarget)!;
if (Directory.Exists(wwwrootDir))
{
    File.WriteAllText(wwwrootTarget, json);
    Console.WriteLine($"Copied components.json to {wwwrootTarget}");

    // Copy TONL files to wwwroot/examples/ for runtime access
    var wwwrootExamples = Path.Combine(wwwrootDir, "examples");
    Directory.CreateDirectory(wwwrootExamples);
    foreach (var tonlFile in Directory.GetFiles(examplesDir, "*.tonl"))
    {
        File.Copy(tonlFile, Path.Combine(wwwrootExamples, Path.GetFileName(tonlFile)), overwrite: true);
    }
    Console.WriteLine($"Copied {Directory.GetFiles(wwwrootExamples, "*.tonl").Length} TONL files to wwwroot/examples/");
}
else
{
    Console.WriteLine($"Warning: wwwroot dir not found at {wwwrootDir}, skipping copy.");
}

Console.WriteLine("DocGen complete.");
return 0;

// ═══════════════════════════════════════════════════════════════════════════
// Helpers
// ═══════════════════════════════════════════════════════════════════════════

static string NormalizeSummary(string raw)
{
    // Collapse whitespace, strip <c>/<see> tags
    var text = raw.Replace("\r", "").Replace("\n", " ");
    while (text.Contains("  "))
        text = text.Replace("  ", " ");
    // Strip <c>content</c>
    text = System.Text.RegularExpressions.Regex.Replace(text, @"<c>(.*?)</c>", "$1");
    // Strip <see cref="..."/>
    text = System.Text.RegularExpressions.Regex.Replace(text, @"<see\s+cref=""[^""]*""\s*/?>", "");
    return text.Trim();
}

static string GetTypeXmlDocId(Type type) => $"T:{type.FullName}";
static string GetPropertyXmlDocId(PropertyInfo prop) => $"P:{prop.DeclaringType?.FullName}.{prop.Name}";

static string FindPropertyDoc(PropertyInfo prop, Dictionary<string, string> xmlDocs)
{
    // Try direct declaring type first
    var directId = GetPropertyXmlDocId(prop);
    if (xmlDocs.TryGetValue(directId, out var desc))
        return desc;

    // For inherited properties, walk up the hierarchy and try each declaring type
    var declaringType = prop.DeclaringType;
    if (declaringType is null) return "";

    // Try the generic type definition if it's a constructed generic
    if (declaringType.IsGenericType && !declaringType.IsGenericTypeDefinition)
    {
        var gtd = declaringType.GetGenericTypeDefinition();
        var gtdId = $"P:{gtd.FullName}.{prop.Name}";
        if (xmlDocs.TryGetValue(gtdId, out desc))
            return desc;
    }

    // Walk base types and try each
    var current = declaringType.BaseType;
    while (current is not null && current != typeof(object))
    {
        var baseId = $"P:{current.FullName}.{prop.Name}";
        if (xmlDocs.TryGetValue(baseId, out desc))
            return desc;
        if (current.IsGenericType && !current.IsGenericTypeDefinition)
        {
            var gtd = current.GetGenericTypeDefinition();
            var gtdId = $"P:{gtd.FullName}.{prop.Name}";
            if (xmlDocs.TryGetValue(gtdId, out desc))
                return desc;
        }
        current = current.BaseType;
    }

    return "";
}

static string ClassifyCategory(Type type, Type baseType, Assembly assembly)
{
    // Walk the hierarchy to find the category tier
    var hierarchy = GetTypeHierarchy(type, baseType).ToList();
    var names = hierarchy.Select(t => t.Name).ToHashSet();

    if (names.Any(n => n.StartsWith("MenuBase", StringComparison.Ordinal)))
        return "Navigation";
    if (names.Any(n => n.StartsWith("PopoverBase", StringComparison.Ordinal)))
        return "Overlay";
    if (names.Any(n => n.StartsWith("OverlayBase", StringComparison.Ordinal)))
        return "Overlay";
    if (names.Any(n => n.StartsWith("SelectionBase", StringComparison.Ordinal)))
        return "Form";
    if (names.Any(n => n.StartsWith("NumericInputBase", StringComparison.Ordinal)))
        return "Form";
    if (names.Any(n => n.StartsWith("BooleanInputBase", StringComparison.Ordinal)))
        return "Form";
    if (names.Any(n => n.StartsWith("TextInputBase", StringComparison.Ordinal)))
        return "Form";
    if (names.Any(n => n.StartsWith("FormControlBase", StringComparison.Ordinal)))
        return "Form";
    if (names.Any(n => n.StartsWith("ButtonBase", StringComparison.Ordinal)))
        return "Action";
    if (names.Any(n => n.StartsWith("DisclosureBase", StringComparison.Ordinal)))
        return "Disclosure";
    if (names.Any(n => n.StartsWith("InteractiveBase", StringComparison.Ordinal)))
        return "Interactive";
    if (names.Any(n => n.StartsWith("PresentationalBase", StringComparison.Ordinal)))
        return "Presentational";

    return "General";
}

/// <summary>
/// Detect if a type implements IRepeatingSlot&lt;TRoot&gt; and, if so, extract:
///   - RootName: the TRoot type name (e.g., "InputOTP")
///   - CountParam: the name of the [Parameter] on TRoot that drives GetSampleCount
///   - IndexParam: the value of IndexParameterName (e.g., "Index")
/// Uses reflection to probe GetSampleCount with fresh instances — changes the value
/// of each numeric [Parameter] on TRoot and detects which one shifts the count.
/// </summary>
static (bool IsSlot, string? RootName, string? CountParam, string? IndexParam) GetRepeatingSlotInfo(Type type)
{
    const string interfaceName = "IRepeatingSlot`1";
    var slotInterface = type.GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.Name == interfaceName);

    if (slotInterface is null) return (false, null, null, null);

    var rootType = slotInterface.GetGenericArguments()[0];

    // IndexParameterName: static property on the implementing type
    string? indexParam = null;
    try
    {
        var indexProp = type.GetProperty("IndexParameterName",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (indexProp is not null)
            indexParam = indexProp.GetValue(null) as string;
    }
    catch { /* ignore */ }

    // CountParameterName: probe GetSampleCount(root) with fresh instances,
    // bumping each numeric [Parameter] by +2 to find which one changes the result.
    string? countParam = null;
    try
    {
        var getSampleCount = type.GetMethod("GetSampleCount",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (getSampleCount is not null)
        {
            var baselineRoot = Activator.CreateInstance(rootType);
            if (baselineRoot is not null)
            {
                var baseline = (int)getSampleCount.Invoke(null, [baselineRoot])!;
                var rootParams = rootType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(p => p.GetCustomAttributes().Any(a => a.GetType().Name == "ParameterAttribute")
                             && (p.PropertyType == typeof(int) || p.PropertyType == typeof(long)));

                foreach (var prop in rootParams)
                {
                    var probe = Activator.CreateInstance(rootType);
                    if (probe is null) continue;
                    var orig = (int)(prop.GetValue(baselineRoot) ?? 0);
                    prop.SetValue(probe, orig + 2);
                    var probeResult = (int)getSampleCount.Invoke(null, [probe])!;
                    if (probeResult != baseline)
                    {
                        countParam = prop.Name;
                        break;
                    }
                }
            }
        }
    }
    catch { /* ignore */ }

    return (true, rootType.Name, countParam, indexParam);
}

static string ClassifyBaseTier(Type type, Type baseType)
{
    var hierarchy = GetTypeHierarchy(type, baseType).ToList();
    // Return the most specific base class that isn't the component itself
    if (hierarchy.Count > 1)
    {
        var tier = hierarchy[1]; // Parent of the component
        var name = tier.IsGenericType ? tier.Name[..tier.Name.IndexOf('`')] : tier.Name;
        return name;
    }
    return "BlazingSpireComponentBase";
}

static IEnumerable<Type> GetTypeHierarchy(Type type, Type baseType)
{
    var current = type;
    while (current is not null && current != typeof(object))
    {
        yield return current;
        current = current.BaseType;
    }
}

static ComponentModel? ExtractComponent(Type type, Dictionary<string, string> xmlDocs,
    Type baseType, Assembly assembly)
{
    var description = xmlDocs.GetValueOrDefault(GetTypeXmlDocId(type), "");
    var category = ClassifyCategory(type, baseType, assembly);
    var baseTier = ClassifyBaseTier(type, baseType);

    var parameters = new List<ParameterModel>();

    // Collect [Parameter] properties from the full hierarchy
    var allProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    foreach (var prop in allProps.OrderBy(p => p.Name))
    {
        var paramAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "ParameterAttribute");
        if (paramAttr is null) continue;

        // Skip CaptureUnmatchedValues (AdditionalAttributes)
        var captureUnmatched = paramAttr.GetType().GetProperty("CaptureUnmatchedValues")
            ?.GetValue(paramAttr) as bool?;
        if (captureUnmatched == true) continue;

        // Include ChildContent as a Content parameter with default text
        if (prop.Name is "ChildContent")
        {
            parameters.Add(new ParameterModel
            {
                Name = "ChildContent",
                Type = "RenderFragment",
                Kind = "Content",
                Description = "The content to render inside the component.",
                DefaultValue = $"Sample {type.Name} content",
            });
            continue;
        }

        var paramDesc = FindPropertyDoc(prop, xmlDocs);
        var propType = prop.PropertyType;
        var isNullable = Nullable.GetUnderlyingType(propType) is not null;
        var effectiveType = Nullable.GetUnderlyingType(propType) ?? propType;

        var paramModel = new ParameterModel
        {
            Name = prop.Name,
            Type = GetFriendlyTypeName(effectiveType),
            Description = paramDesc,
            Kind = ClassifyParameterKind(effectiveType),
        };

        // Default value
        paramModel.DefaultValue = GetDefaultValue(type, prop, effectiveType);

        // Enum values
        if (effectiveType.IsEnum)
            paramModel.EnumValues = Enum.GetNames(effectiveType).ToList();

        parameters.Add(paramModel);
    }

    return new ComponentModel
    {
        Name = type.Name,
        Description = description,
        Category = category,
        BaseTier = baseTier,
        Parameters = parameters,
    };
}

static string ClassifyParameterKind(Type type)
{
    if (type.IsEnum) return "Enum";
    if (type == typeof(bool)) return "Boolean";
    if (type == typeof(string)) return "String";
    if (type == typeof(int) || type == typeof(long) || type == typeof(double)
        || type == typeof(float) || type == typeof(decimal)) return "Number";
    if (type.Name == "EventCallback" || type.Name.StartsWith("EventCallback`", StringComparison.Ordinal))
        return "Event";
    if (type.Name == "RenderFragment" || type.Name.StartsWith("RenderFragment`", StringComparison.Ordinal))
        return "Content";
    if (type.Name == "Expression`1") return "Expression";
    return "Object";
}

static string GetFriendlyTypeName(Type type)
{
    if (type == typeof(string)) return "string";
    if (type == typeof(bool)) return "bool";
    if (type == typeof(int)) return "int";
    if (type == typeof(long)) return "long";
    if (type == typeof(double)) return "double";
    if (type == typeof(float)) return "float";
    if (type == typeof(decimal)) return "decimal";
    if (type.IsEnum) return type.Name;
    if (type.IsGenericType)
    {
        var baseName = type.Name[..type.Name.IndexOf('`')];
        var args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
        return $"{baseName}<{args}>";
    }
    return type.Name;
}

static string? GetDefaultValue(Type componentType, PropertyInfo prop, Type effectiveType)
{
    try
    {
        // Try reading the actual default from a fresh instance
        object? instance = null;
        try { instance = Activator.CreateInstance(componentType); } catch { }
        if (instance is not null)
        {
            var val = prop.GetValue(instance);
            if (val is null) return null;
            if (effectiveType == typeof(bool)) return val.ToString()!.ToLowerInvariant();
            if (effectiveType == typeof(string)) return val.ToString();
            if (effectiveType.IsEnum) return val.ToString();
            if (effectiveType == typeof(int)) return val.ToString();
            if (effectiveType == typeof(double)) return val.ToString();
            if (effectiveType == typeof(decimal)) return val.ToString();
            return val.ToString();
        }

        // Fallback: use type defaults
        if (effectiveType.IsEnum)
        {
            var defaultVal = Enum.ToObject(effectiveType, 0);
            return defaultVal.ToString();
        }
        if (effectiveType == typeof(bool)) return "false";
        if (effectiveType == typeof(int)) return "0";
        if (effectiveType == typeof(string)) return null;
    }
    catch { /* ignore */ }
    return null;
}

// ── OpenAPI generation ─────────────────────────────────────────────────────

static void GenerateOpenApiSpec(List<ComponentModel> components, string outputDir)
{
    var document = new OpenApiDocument
    {
        Info = new OpenApiInfo { Title = "BlazingSpire Components", Version = "1.0.0" },
        Paths = new OpenApiPaths(),
        Components = new OpenApiComponents { Schemas = new Dictionary<string, OpenApiSchema>() },
    };

    foreach (var comp in components)
    {
        var schema = new OpenApiSchema
        {
            Type = "object",
            Description = comp.Description,
            Extensions =
            {
                ["x-base-tier"] = new OpenApiString(comp.BaseTier),
                ["x-category"] = new OpenApiString(comp.Category),
                ["x-composition-role"] = new OpenApiString(comp.Composition.Role),
                ["x-composition-is-composite"] = new OpenApiBoolean(comp.Composition.IsComposite),
            },
            Properties = new Dictionary<string, OpenApiSchema>(),
        };

        if (comp.Composition.Parent is not null)
            schema.Extensions["x-composition-parent"] = new OpenApiString(comp.Composition.Parent);

        if (comp.Composition.Children.Count > 0)
        {
            var childrenArray = new OpenApiArray();
            foreach (var child in comp.Composition.Children)
                childrenArray.Add(new OpenApiString(child));
            schema.Extensions["x-composition-children"] = childrenArray;
        }

        foreach (var param in comp.Parameters)
        {
            var propSchema = new OpenApiSchema
            {
                Description = param.Description,
            };

            switch (param.Kind)
            {
                case "String":
                    propSchema.Type = "string";
                    break;
                case "Boolean":
                    propSchema.Type = "boolean";
                    if (param.DefaultValue is not null)
                        propSchema.Default = new OpenApiBoolean(bool.Parse(param.DefaultValue));
                    break;
                case "Number":
                    propSchema.Type = param.Type is "int" or "long" ? "integer" : "number";
                    if (param.DefaultValue is not null && int.TryParse(param.DefaultValue, out var intVal))
                        propSchema.Default = new OpenApiInteger(intVal);
                    break;
                case "Enum":
                    propSchema.Type = "string";
                    if (param.EnumValues is not null)
                        propSchema.Enum = param.EnumValues
                            .Select(v => (IOpenApiAny)new OpenApiString(v)).ToList();
                    if (param.DefaultValue is not null)
                        propSchema.Default = new OpenApiString(param.DefaultValue);
                    break;
                case "Event":
                    propSchema.Type = "string";
                    propSchema.Description = (propSchema.Description ?? "") + " (event callback)";
                    propSchema.ReadOnly = true;
                    break;
                case "Content":
                    propSchema.Type = "string";
                    propSchema.Description = (propSchema.Description ?? "") + " (render fragment)";
                    break;
                default:
                    propSchema.Type = "object";
                    break;
            }

            schema.Properties[param.Name] = propSchema;
        }

        document.Components.Schemas[comp.Name] = schema;
    }

    var path = Path.Combine(outputDir, "openapi.json");
    using var stream = File.Create(path);
    using var writer = new StreamWriter(stream);
    var jsonString = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    writer.Write(jsonString);
    Console.WriteLine($"Wrote {path} ({document.Components.Schemas.Count} schemas)");
}

// ── TONL generation ────────────────────────────────────────────────────────

static void GenerateTonlFile(ComponentModel comp, string examplesDir)
{
    var sb = new StringBuilder();
    sb.AppendLine("#version 1.0");
    sb.AppendLine();

    // Component header
    sb.AppendLine("component{name:str,description:str,category:str,baseTier:str}:");
    sb.AppendLine($"  name: {comp.Name}");
    sb.AppendLine($"  description: {EscapeTonlValue(comp.Description)}");
    sb.AppendLine($"  category: {comp.Category}");
    sb.AppendLine($"  baseTier: {comp.BaseTier}");
    sb.AppendLine();

    // Composition — structural relationships to other components
    sb.AppendLine("composition{role:str,parent:str,isComposite:bool}:");
    sb.AppendLine($"  role: {comp.Composition.Role}");
    sb.AppendLine($"  parent: {comp.Composition.Parent ?? ""}");
    sb.AppendLine($"  isComposite: {(comp.Composition.IsComposite ? "true" : "false")}");
    if (comp.Composition.Children.Count > 0)
    {
        sb.AppendLine($"children[{comp.Composition.Children.Count}]: {string.Join(", ", comp.Composition.Children)}");
    }
    sb.AppendLine();

    // Parameters
    var paramCount = comp.Parameters.Count;
    sb.AppendLine($"parameters[{paramCount}]{{name:str,type:str,kind:str,default:str,description:str}}:");
    foreach (var p in comp.Parameters)
    {
        var defaultVal = p.DefaultValue ?? "";
        var desc = EscapeTonlValue(p.Description);
        sb.AppendLine($"  {p.Name}, {p.Type}, {p.Kind}, {defaultVal}, {desc}");
    }

    // Enum details section for parameters that have enum values
    var enumParams = comp.Parameters.Where(p => p.EnumValues is { Count: > 0 }).ToList();
    if (enumParams.Count > 0)
    {
        sb.AppendLine();
        foreach (var ep in enumParams)
        {
            sb.AppendLine($"enum:{ep.Name}[{ep.EnumValues!.Count}]{{value:str}}:");
            foreach (var v in ep.EnumValues!)
                sb.AppendLine($"  {v}");
        }
    }

    var fileName = comp.Name.ToLowerInvariant() + ".tonl";
    var path = Path.Combine(examplesDir, fileName);
    File.WriteAllText(path, sb.ToString());
}

static string EscapeTonlValue(string value)
{
    // TONL values: escape commas and newlines
    return value.Replace("\n", " ").Replace("\r", "");
}

// ── Models ─────────────────────────────────────────────────────────────────

class ComponentModel
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string BaseTier { get; set; } = "";
    public CompositionModel Composition { get; set; } = new();
    public List<ParameterModel> Parameters { get; set; } = [];
}

class CompositionModel
{
    /// <summary>"Root" (standalone) or "Child" (requires parent via cascading value).</summary>
    public string Role { get; set; } = "Root";

    /// <summary>For Child: the component type that must appear as an ancestor.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Parent { get; set; }

    /// <summary>For Root: the child component types that can be rendered inside this one.</summary>
    public List<string> Children { get; set; } = new();

    /// <summary>True when this component accepts child components via cascading values.</summary>
    public bool IsComposite { get; set; }

    /// <summary>True when this component implements IRepeatingSlot&lt;TRoot&gt;.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsRepeatingSlot { get; set; }

    /// <summary>For IRepeatingSlot: the root component whose parameter drives the slot count.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountParameterOwner { get; set; }

    /// <summary>For IRepeatingSlot: the name of the parameter on CountParameterOwner that controls count.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountParameterName { get; set; }

    /// <summary>For IRepeatingSlot: the name of the [Parameter] that receives the loop index.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IndexParameterName { get; set; }
}

class ParameterModel
{
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public string? DefaultValue { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? EnumValues { get; set; }
}
