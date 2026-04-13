using System.Text;
using Microsoft.CodeAnalysis;

namespace BlazingSpire.TestGenerator;

/// <summary>
/// Emits concrete xUnit test class shells for the BlazingSpire E2E suite.
/// All test logic lives in hand-written abstract base classes in Infrastructure/.
/// The generator's only job is to emit the right number of shard classes and
/// the four singleton class shells, so the Generated/ directory contains no
/// hand-authored code.
///
/// Input: <c>TestGeneratorShardCount</c> MSBuild property (default 8).
/// Output: 22 concrete class shells — (ShardCount × 2) sharded + 4 singleton.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TestShellGenerator : IIncrementalGenerator
{
    private const int DefaultShardCount = 8;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var shardCount = context.AnalyzerConfigOptionsProvider.Select(
            (options, _) =>
            {
                if (options.GlobalOptions.TryGetValue("build_property.TestGeneratorShardCount", out var val)
                    && int.TryParse(val, out var n)
                    && n > 0)
                    return n;
                return DefaultShardCount;
            });

        context.RegisterSourceOutput(shardCount, GenerateAll);
    }

    private static void GenerateAll(SourceProductionContext ctx, int shardCount)
    {
        ctx.AddSource("InvolutionTests.g.cs",           BuildInvolutionTests(shardCount));
        ctx.AddSource("ParameterPermutationTests.g.cs", BuildParameterPermutationTests(shardCount));
        ctx.AddSource("ComponentSmokeTests.g.cs",       BuildComponentSmokeTests());
        ctx.AddSource("StructuralInvariantTests.g.cs",  BuildStructuralInvariantTests());
        ctx.AddSource("FloatingGeometryTests.g.cs",     BuildFloatingGeometryTests());
        ctx.AddSource("RepeatingSlotTests.g.cs",        BuildRepeatingSlotTests());
    }

    // ── Sharded families ────────────────────────────────────────────────────

    private static string BuildInvolutionTests(int shardCount)
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();

        for (int i = 0; i < shardCount; i++)
        {
            sb.AppendLine($"public class InvolutionTestsShard{i} : InvolutionTestsBase");
            sb.AppendLine("{");
            sb.AppendLine($"    public InvolutionTestsShard{i}(PlaywrightBrowserFixture f) : base(f) {{ }}");
            sb.AppendLine();
            sb.AppendLine($"    public static System.Collections.Generic.IEnumerable<object?[]> Enums() => EnumShard({i});");
            sb.AppendLine($"    public static System.Collections.Generic.IEnumerable<object[]>  Bools() => BoolShard({i});");
            sb.AppendLine();
            sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(Enums))]");
            sb.AppendLine("    public System.Threading.Tasks.Task Enum_Involution(string c, string p, string a, string? b) => RunEnumAsync(c, p, a, b);");
            sb.AppendLine();
            sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(Bools))]");
            sb.AppendLine("    public System.Threading.Tasks.Task Bool_Involution(string c, string p, bool a) => RunBoolAsync(c, p, a);");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string BuildParameterPermutationTests(int shardCount)
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();

        for (int i = 0; i < shardCount; i++)
        {
            sb.AppendLine($"public class ParameterPermutationTestsShard{i} : ParameterPermutationTestsBase");
            sb.AppendLine("{");
            sb.AppendLine($"    public ParameterPermutationTestsShard{i}(PlaywrightBrowserFixture f) : base(f) {{ }}");
            sb.AppendLine();
            sb.AppendLine($"    public static System.Collections.Generic.IEnumerable<object[]> Enums() => EnumShard({i});");
            sb.AppendLine($"    public static System.Collections.Generic.IEnumerable<object[]> Bools() => BoolShard({i});");
            sb.AppendLine();
            sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(Enums))]");
            sb.AppendLine("    public System.Threading.Tasks.Task Enum_Renders(string c, string p, string v) => RunEnumAsync(c, p, v);");
            sb.AppendLine();
            sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(Bools))]");
            sb.AppendLine("    public System.Threading.Tasks.Task Bool_Renders(string c, string p, bool v) => RunBoolAsync(c, p, v);");
            sb.AppendLine("}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    // ── Singleton families ───────────────────────────────────────────────────

    private static string BuildComponentSmokeTests()
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();
        sb.AppendLine("public class ComponentSmokeTests : ComponentSmokeTestsBase");
        sb.AppendLine("{");
        sb.AppendLine("    public ComponentSmokeTests(PlaywrightBrowserFixture f) : base(f) { }");
        sb.AppendLine();
        sb.AppendLine("    public static System.Collections.Generic.IEnumerable<object[]> AllComponents() => AllComponentsData();");
        sb.AppendLine();
        sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(AllComponents))]");
        sb.AppendLine("    public System.Threading.Tasks.Task Playground_Page_Loads_Without_Errors(string name) => RunSmokeAsync(name);");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildStructuralInvariantTests()
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();
        sb.AppendLine("public class StructuralInvariantTests : StructuralInvariantTestsBase");
        sb.AppendLine("{");
        sb.AppendLine("    public StructuralInvariantTests(PlaywrightBrowserFixture f) : base(f) { }");
        sb.AppendLine();
        sb.AppendLine("    public static System.Collections.Generic.IEnumerable<object[]> TopLevelComponents() => TopLevelComponentsData();");
        sb.AppendLine();
        sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(TopLevelComponents))]");
        sb.AppendLine("    public System.Threading.Tasks.Task Component_Passes_Structural_Invariants(string name) => RunStructuralInvariantAsync(name);");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildFloatingGeometryTests()
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();
        sb.AppendLine("public class FloatingGeometryTests : FloatingGeometryTestsBase");
        sb.AppendLine("{");
        sb.AppendLine("    public FloatingGeometryTests(PlaywrightBrowserFixture f) : base(f) { }");
        sb.AppendLine();
        sb.AppendLine("    public static System.Collections.Generic.IEnumerable<object[]> ClickTriggeredPopoverComponents() => ClickTriggeredPopoverComponentsData();");
        sb.AppendLine();
        sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(ClickTriggeredPopoverComponents))]");
        sb.AppendLine("    public System.Threading.Tasks.Task Floating_Content_Respects_Side_Parameter(string name) => RunGeometryAsync(name);");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildRepeatingSlotTests()
    {
        var sb = new StringBuilder();
        AppendFileHeader(sb);
        sb.AppendLine("using BlazingSpire.Tests.E2E.Infrastructure;");
        sb.AppendLine();
        sb.AppendLine("namespace BlazingSpire.Tests.E2E.Generated;");
        sb.AppendLine();
        sb.AppendLine("public class RepeatingSlotTests : RepeatingSlotTestsBase");
        sb.AppendLine("{");
        sb.AppendLine("    public RepeatingSlotTests(PlaywrightBrowserFixture f) : base(f) { }");
        sb.AppendLine();
        sb.AppendLine("    public static System.Collections.Generic.IEnumerable<object[]> SlotComponents() => SlotComponentsData();");
        sb.AppendLine();
        sb.AppendLine("    [Xunit.Theory, Xunit.MemberData(nameof(SlotComponents))]");
        sb.AppendLine("    public System.Threading.Tasks.Task Slot_Count_Tracks_Count_Parameter(string slotName, string countOwner, string countParam) => RunSlotAsync(slotName, countOwner, countParam);");
        sb.AppendLine("}");
        return sb.ToString();
    }

    // ── Shared helpers ───────────────────────────────────────────────────────

    private static void AppendFileHeader(StringBuilder sb)
    {
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("// Generated by BlazingSpire.TestGenerator — do not edit");
        sb.AppendLine("#pragma warning disable CS1591");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
    }
}
