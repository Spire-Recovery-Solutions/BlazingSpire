using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazingSpire.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class PlaygroundGenerator : IIncrementalGenerator
{
    private const string BaseClassName = "BlazingSpireComponentBase";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, GenerateSource);
    }

    private static void GenerateSource(SourceProductionContext context, Compilation compilation)
    {
        // ── Phase 1: Discover all concrete component types ──────────────────
        var allComponents = new Dictionary<string, ComponentInfo>();

        foreach (var tree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var classDecl in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol symbol)
                    continue;
                if (symbol.IsAbstract || !symbol.CanBeReferencedByName || symbol.IsGenericType)
                    continue;
                if (!InheritsFrom(symbol, BaseClassName))
                    continue;
                if (HasBrowsableFalse(symbol))
                    continue;

                var fullName = symbol.ToDisplayString();
                if (!allComponents.ContainsKey(fullName))
                    allComponents[fullName] = new ComponentInfo(symbol.Name, symbol.ContainingNamespace.ToDisplayString(), fullName, symbol);
            }
        }

        if (allComponents.Count == 0) return;

        // ── Phase 2: Find [CascadingParameter] relationships via syntax ─────
        // Scan all syntax trees for properties with [CascadingParameter] attribute
        var parentToChildren = new Dictionary<string, List<ChildInfo>>();
        var childSet = new HashSet<string>();

        foreach (var tree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var propDecl in tree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
            {
                // Check for [CascadingParameter] attribute in syntax
                var attrNames = propDecl.AttributeLists
                    .SelectMany(al => al.Attributes)
                    .Select(a => a.Name.ToString())
                    .ToList();

                var hasCascading = attrNames.Any(n => n.Contains("CascadingParameter"));
                if (!hasCascading) continue;

                // Get the containing type
                var containingClass = propDecl.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (containingClass is null) continue;

                var containingSymbol = model.GetDeclaredSymbol(containingClass) as INamedTypeSymbol;
                if (containingSymbol is null) continue;

                var childFullName = containingSymbol.ToDisplayString();
                if (!allComponents.ContainsKey(childFullName)) continue;

                // Get the property type symbol
                var propSymbol = model.GetDeclaredSymbol(propDecl) as IPropertySymbol;
                if (propSymbol is null) continue;

                var propType = propSymbol.Type;
                // Unwrap Nullable<T> for value types
                if (propType is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    propType = namedType.TypeArguments[0];

                if (propType is not INamedTypeSymbol parentType) continue;
                if (!InheritsFrom(parentType, BaseClassName)) continue;

                // Strip trailing ? from nullable reference type display string
                var parentFullName = parentType.ToDisplayString().TrimEnd('?');
                if (!allComponents.ContainsKey(parentFullName)) continue;

                // Determine child's role from name suffix
                var parentName = parentType.Name;
                var childName = containingSymbol.Name;
                var suffix = childName.StartsWith(parentName, StringComparison.Ordinal)
                    ? childName.Substring(parentName.Length)
                    : childName;

                if (string.IsNullOrEmpty(suffix)) continue;

                if (!parentToChildren.TryGetValue(parentFullName, out var children))
                {
                    children = new List<ChildInfo>();
                    parentToChildren[parentFullName] = children;
                }

                if (!children.Any(c => c.Name == childName))
                {
                    children.Add(new ChildInfo(childName, suffix, childFullName));
                    childSet.Add(childFullName);
                }
            }
        }

        // ── Phase 3: Top-level = not a child ────────────────────────────────
        var topLevel = allComponents.Values
            .Where(c => !childSet.Contains(c.FullName))
            .OrderBy(c => c.Name)
            .ToList();

        // ── Phase 4: Generate code ──────────────────────────────────────────
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine($"// Stats: {allComponents.Count} total, {childSet.Count} children, {parentToChildren.Count} composites, {topLevel.Count} top-level");
        foreach (var kvp in parentToChildren.Take(5))
            sb.AppendLine($"// Composite: {kvp.Key} -> [{string.Join(", ", kvp.Value.Select(c => c.Name))}]");
        // Debug: count syntax trees and check for CascadingParameter
        var treeCount = compilation.SyntaxTrees.Count();
        var cascadingCount = 0;
        foreach (var tree in compilation.SyntaxTrees)
        {
            foreach (var attr in tree.GetRoot().DescendantNodes().OfType<AttributeSyntax>())
            {
                if (attr.Name.ToString().Contains("Cascading"))
                    cascadingCount++;
            }
        }
        sb.AppendLine($"// Debug: {treeCount} syntax trees, {cascadingCount} [Cascading*] attributes found");
        // Sample tree paths
        foreach (var tree in compilation.SyntaxTrees.Take(3))
            sb.AppendLine($"// Tree: {tree.FilePath}");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.AspNetCore.Components;");
        sb.AppendLine("using Microsoft.AspNetCore.Components.Rendering;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();

        var allNamespaces = new HashSet<string>();
        foreach (var c in allComponents.Values) allNamespaces.Add(c.Namespace);
        foreach (var ns in allNamespaces.OrderBy(n => n))
            sb.AppendLine($"using {ns};");

        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Demo.Components.Demo;");
        sb.AppendLine();
        sb.AppendLine("public static class PlaygroundFactories");
        sb.AppendLine("{");

        // Registry: only top-level
        sb.AppendLine("    public static readonly IReadOnlyDictionary<string, System.Func<IReadOnlyDictionary<string, object?>, RenderFragment>> All = new Dictionary<string, System.Func<IReadOnlyDictionary<string, object?>, RenderFragment>>");
        sb.AppendLine("    {");
        foreach (var c in topLevel)
            sb.AppendLine($"        [\"{c.Name}\"] = Render{c.Name},");
        sb.AppendLine("    };");
        sb.AppendLine();

        // Factories
        foreach (var c in topLevel)
        {
            if (parentToChildren.TryGetValue(c.FullName, out var children))
                EmitCompositeFactory(sb, c, children);
            else
                EmitSimpleFactory(sb, c);
        }

        // Debug info
        // Debug: trace the Phase 2 logic for PopoverContent specifically
        var pcDebug = new List<string>();
        foreach (var tree in compilation.SyntaxTrees)
        {
            var treeModel = compilation.GetSemanticModel(tree);
            foreach (var propDecl in tree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
            {
                var propAttrNames = propDecl.AttributeLists
                    .SelectMany(al => al.Attributes)
                    .Select(a => a.Name.ToString())
                    .ToList();
                if (!propAttrNames.Any(n => n.Contains("CascadingParameter"))) continue;

                var containingClass = propDecl.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                var containingSymbol = containingClass is not null ? treeModel.GetDeclaredSymbol(containingClass) as INamedTypeSymbol : null;
                var propSymbol = treeModel.GetDeclaredSymbol(propDecl) as IPropertySymbol;
                var propTypeName = propSymbol?.Type?.ToDisplayString() ?? "null";
                var containingName = containingSymbol?.ToDisplayString() ?? "null";
                var isInAll = allComponents.ContainsKey(containingName);
                var parentInherits = propSymbol?.Type is INamedTypeSymbol pt ? InheritsFrom(pt, BaseClassName) : false;

                pcDebug.Add($"{containingSymbol?.Name}.{propDecl.Identifier}: type={propTypeName} inAll={isInAll} parentInherits={parentInherits}");
            }
        }
        var debugStr = string.Join(" | ", pcDebug.Take(10)).Replace("\"", "'");
        sb.AppendLine($"    public static readonly string DebugInfo = \"{allComponents.Count} total, {childSet.Count} children, {parentToChildren.Count} composites | {debugStr}\";");
        sb.AppendLine();
        // Type registry (all, for DocGen)
        sb.AppendLine("    public static readonly IReadOnlyDictionary<string, System.Type> ComponentTypes = new Dictionary<string, System.Type>");
        sb.AppendLine("    {");
        foreach (var c in allComponents.Values.OrderBy(x => x.Name))
            sb.AppendLine($"        [\"{c.Name}\"] = typeof({c.Name}),");
        sb.AppendLine("    };");

        sb.AppendLine("}");
        context.AddSource("PlaygroundFactories.g.cs", sb.ToString());
    }

    private static void EmitSimpleFactory(StringBuilder sb, ComponentInfo c)
    {
        sb.AppendLine($"    public static RenderFragment Render{c.Name}(IReadOnlyDictionary<string, object?> parameters) => builder =>");
        sb.AppendLine("    {");
        sb.AppendLine($"        builder.OpenComponent<{c.Name}>(0);");
        sb.AppendLine("        var seq = 1;");
        sb.AppendLine("        foreach (var (key, value) in parameters)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (value is null) continue;");
        sb.AppendLine("            if (key == \"ChildContent\" && value is string text)");
        sb.AppendLine("                builder.AddAttribute(seq++, key, (RenderFragment)(b => b.AddContent(0, text)));");
        sb.AppendLine("            else");
        sb.AppendLine("                builder.AddAttribute(seq++, key, value);");
        sb.AppendLine("        }");
        sb.AppendLine("        builder.CloseComponent();");
        sb.AppendLine("    };");
        sb.AppendLine();
    }

    private static void EmitCompositeFactory(StringBuilder sb, ComponentInfo parent, List<ChildInfo> children)
    {
        var ordered = children.OrderBy(c => GetRoleOrder(c.Suffix)).ThenBy(c => c.Name).ToList();

        sb.AppendLine($"    public static RenderFragment Render{parent.Name}(IReadOnlyDictionary<string, object?> parameters) => builder =>");
        sb.AppendLine("    {");
        sb.AppendLine($"        builder.OpenComponent<{parent.Name}>(0);");
        sb.AppendLine("        var seq = 1;");
        sb.AppendLine("        foreach (var (key, value) in parameters)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (value is null || key == \"ChildContent\") continue;");
        sb.AppendLine("            builder.AddAttribute(seq++, key, value);");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        builder.AddAttribute(seq++, \"ChildContent\", (RenderFragment)(inner =>");
        sb.AppendLine("        {");
        sb.AppendLine("            #pragma warning disable CS0219");
        sb.AppendLine("            var childSeq = 0;");
        sb.AppendLine("            #pragma warning restore CS0219");

        foreach (var child in ordered)
        {
            if (child.Suffix is "Header" or "Footer" or "Group" or "List" or "Separator")
                continue;

            var defaultContent = GetDefaultContent(child.Suffix, parent.Name);
            if (defaultContent is null) continue;

            sb.AppendLine($"            inner.OpenComponent<{child.Name}>(childSeq++);");
            if (!string.IsNullOrEmpty(defaultContent))
            {
                var escaped = defaultContent.Replace("\\", "\\\\").Replace("\"", "\\\"");
                sb.AppendLine($"            inner.AddAttribute(childSeq++, \"ChildContent\", (RenderFragment)(cb => cb.AddContent(0, \"{escaped}\")));");
            }
            sb.AppendLine($"            inner.CloseComponent();");
        }

        sb.AppendLine("        }));");
        sb.AppendLine("        builder.CloseComponent();");
        sb.AppendLine("    };");
        sb.AppendLine();
    }

    private static int GetRoleOrder(string suffix) => suffix switch
    {
        "Trigger" => 0, "Content" => 1, "Title" => 2, "Description" => 3,
        "Close" => 6, "Action" => 7, "Cancel" => 8, "Input" => 9,
        "Value" => 10, "Item" => 11, "Empty" => 12, "Label" => 13,
        _ => 50
    };

    private static string? GetDefaultContent(string suffix, string parentName) => suffix switch
    {
        "Trigger" => "Open " + parentName,
        "Content" => parentName + " content goes here.",
        "Title" => parentName,
        "Description" => "This is a " + parentName.ToLowerInvariant() + " description.",
        "Close" => "Close",
        "Action" => "Confirm",
        "Cancel" => "Cancel",
        "Item" => "Sample item",
        "Input" => "",
        "Value" => "Select...",
        "Empty" => "No results found.",
        "Label" => "Label",
        _ => null
    };

    private static bool InheritsFrom(INamedTypeSymbol symbol, string baseName)
    {
        var current = symbol.BaseType;
        while (current is not null)
        {
            if (current.Name == baseName) return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool HasBrowsableFalse(INamedTypeSymbol symbol)
    {
        foreach (var attr in symbol.GetAttributes())
            if (attr.AttributeClass?.Name == "BrowsableAttribute"
                && attr.ConstructorArguments.Length == 1
                && attr.ConstructorArguments[0].Value is false)
                return true;
        return false;
    }

    private sealed class ComponentInfo
    {
        public string Name { get; }
        public string Namespace { get; }
        public string FullName { get; }
        public INamedTypeSymbol Symbol { get; }
        public ComponentInfo(string name, string ns, string fullName, INamedTypeSymbol symbol)
        { Name = name; Namespace = ns; FullName = fullName; Symbol = symbol; }
    }

    private sealed class ChildInfo
    {
        public string Name { get; }
        public string Suffix { get; }
        public string FullName { get; }
        public ChildInfo(string name, string suffix, string fullName)
        { Name = name; Suffix = suffix; FullName = fullName; }
    }
}
