using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using Bunit;
using BlazingSpire.Demo.Components.UI;

namespace BlazingSpire.Tests.Performance;

/// <summary>
/// Measures render time for the Badge component across variants and usage patterns.
/// Badge extends PresentationalBase — simpler than Button (no interaction, no JS).
///
/// Key things measured:
/// - Variant combinations: exercises FrozenDictionary lookup + BuildClasses()
/// - With custom Class: exercises Cn() cache miss on first call, then hit on repeat
/// - Tag-list pattern (7 badges): real-world usage scenario from the demo page
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[JsonExporterAttribute.Full]
public class BadgeRenderBenchmarks : BunitContext
{
    public BadgeRenderBenchmarks()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [IterationCleanup]
    public void Cleanup() => DisposeComponentsAsync().GetAwaiter().GetResult();

    [Benchmark(Baseline = true)]
    public object Render_Default()
        => Render<Badge>();

    [Benchmark]
    public object Render_Default_WithText()
        => Render<Badge>(p => p.AddChildContent("Passing"));

    [Benchmark]
    public object Render_Secondary()
        => Render<Badge>(p => p
            .Add(x => x.Variant, BadgeVariant.Secondary)
            .AddChildContent("Preview"));

    [Benchmark]
    public object Render_Destructive()
        => Render<Badge>(p => p
            .Add(x => x.Variant, BadgeVariant.Destructive)
            .AddChildContent("3 Critical"));

    [Benchmark]
    public object Render_Outline()
        => Render<Badge>(p => p
            .Add(x => x.Variant, BadgeVariant.Outline)
            .AddChildContent("12 Open"));

    /// <summary>
    /// Custom Class parameter forces a Cn() call with a non-empty extra string.
    /// Measures the overhead of the Class merging path vs. the base variant-only path.
    /// </summary>
    [Benchmark]
    public object Render_WithCustomClass()
        => Render<Badge>(p => p
            .Add(x => x.Variant, BadgeVariant.Secondary)
            .Add(x => x.Class, "ml-auto shrink-0")
            .AddChildContent("1h ago"));

    /// <summary>
    /// Renders 7 outline badges in sequence — mirrors the tag-list on the demo page.
    /// Representative of a real-world page render workload.
    /// </summary>
    [Benchmark]
    public void Render_TagList_SevenBadges()
    {
        string[] tags = ["Blazor", ".NET 10", "Tailwind v4", "OKLCH", "WASM", "AOT", "shadcn/ui"];
        foreach (var tag in tags)
        {
            Render<Badge>(p => p
                .Add(x => x.Variant, BadgeVariant.Outline)
                .AddChildContent(tag));
        }
    }
}
