using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using Bunit;
using BlazingSpire.Demo.Components.UI;

namespace BlazingSpire.Tests.Performance;

/// <summary>
/// Measures Blazor component render time for the Button component.
///
/// Inherits BunitContext so the renderer is shared across iterations — benchmarks
/// measure render cost, not context setup. IterationCleanup disposes accumulated
/// rendered components between BDN iterations to prevent GC skew.
///
/// Return IRenderedFragment (not void) to prevent dead-code elimination by the JIT.
///
/// Run with: dotnet run -c Release
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[JsonExporterAttribute.Full]
public class ButtonRenderBenchmarks : BunitContext
{
    public ButtonRenderBenchmarks()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [IterationCleanup]
    public void Cleanup() => DisposeComponentsAsync().GetAwaiter().GetResult();

    // ── Baseline ──────────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)]
    public object Render_Default()
        => Render<Button>();

    // ── Variants ─────────────────────────────────────────────────────────────

    [Benchmark]
    public object Render_Destructive()
        => Render<Button>(p => p.Add(x => x.Variant, ButtonVariant.Destructive));

    [Benchmark]
    public object Render_Outline()
        => Render<Button>(p => p.Add(x => x.Variant, ButtonVariant.Outline));

    [Benchmark]
    public object Render_Ghost()
        => Render<Button>(p => p.Add(x => x.Variant, ButtonVariant.Ghost));

    // ── State ─────────────────────────────────────────────────────────────────

    [Benchmark]
    public object Render_Loading()
        => Render<Button>(p => p
            .Add(x => x.Loading, true)
            .AddChildContent("Please wait..."));

    [Benchmark]
    public object Render_AsLink()
        => Render<Button>(p => p
            .Add(x => x.Href, "/about")
            .AddChildContent("Visit Site"));

    // ── Custom Class — exercises Cn() cache miss ──────────────────────────────

    [Benchmark]
    public object Render_AllParameters()
        => Render<Button>(p => p
            .Add(x => x.Variant, ButtonVariant.Destructive)
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.Class, "w-full")
            .AddChildContent("Delete"));
}
