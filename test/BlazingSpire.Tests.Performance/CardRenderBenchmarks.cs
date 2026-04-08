using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using Bunit;
using BlazingSpire.Demo.Components.UI;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Performance;

/// <summary>
/// Measures render time for the Card component family.
///
/// Key things measured:
/// - Individual sub-components: baseline for each piece (Card, CardHeader, CardTitle, etc.)
/// - Composed cards: measures render tree depth cost vs. single-component render
/// - Card with custom Class: exercises Cn() cache miss on first call
/// - Grid pattern (4 cards): realistic page-load scenario
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
[JsonExporterAttribute.Full]
public class CardRenderBenchmarks : BunitContext
{
    public CardRenderBenchmarks()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [IterationCleanup]
    public void Cleanup() => DisposeComponentsAsync().GetAwaiter().GetResult();

    // ── Individual sub-components ─────────────────────────────────────────────

    [Benchmark(Baseline = true)]
    public object Render_Card_Empty()
        => Render<Card>();

    [Benchmark]
    public object Render_CardTitle_WithText()
        => Render<CardTitle>(p => p.AddChildContent("Create project"));

    [Benchmark]
    public object Render_CardDescription_WithText()
        => Render<CardDescription>(p => p.AddChildContent("Deploy your new project in one-click."));

    [Benchmark]
    public object Render_Card_WithCustomClass()
        => Render<Card>(p => p.Add(x => x.Class, "border-primary/50"));

    // ── Composed card patterns ────────────────────────────────────────────────

    /// <summary>
    /// Full card: Header (Title + Description) + Content + Footer (Button).
    /// Mirrors "Create project" card from demo. Measures render tree depth cost.
    /// </summary>
    [Benchmark]
    public object Render_BasicCard_FullComposition()
        => Render<Card>(p => p.AddChildContent(
            b =>
            {
                b.OpenComponent<CardHeader>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(h =>
                {
                    h.OpenComponent<CardTitle>(0);
                    h.AddAttribute(1, "ChildContent", (RenderFragment)(t => t.AddContent(0, "Create project")));
                    h.CloseComponent();
                    h.OpenComponent<CardDescription>(2);
                    h.AddAttribute(3, "ChildContent", (RenderFragment)(d => d.AddContent(0, "Deploy your new project in one-click.")));
                    h.CloseComponent();
                }));
                b.CloseComponent();

                b.OpenComponent<CardContent>(4);
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Pre-configured with .NET 10.")));
                b.CloseComponent();

                b.OpenComponent<CardFooter>(6);
                b.AddAttribute(7, "ChildContent", (RenderFragment)(f =>
                {
                    f.OpenComponent<Button>(0);
                    f.AddAttribute(1, "ChildContent", (RenderFragment)(btn => btn.AddContent(0, "Deploy")));
                    f.CloseComponent();
                }));
                b.CloseComponent();
            }));

    /// <summary>
    /// Stats card: Header (Description + oversized CardTitle). No footer.
    /// Mirrors the stats cards (Total Downloads, Active Users, etc.) from demo.
    /// </summary>
    [Benchmark]
    public object Render_StatsCard_Composed()
        => Render<Card>(p => p.AddChildContent(
            b =>
            {
                b.OpenComponent<CardHeader>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(h =>
                {
                    h.OpenComponent<CardDescription>(0);
                    h.AddAttribute(1, "ChildContent", (RenderFragment)(d => d.AddContent(0, "Total Downloads")));
                    h.CloseComponent();
                    h.OpenComponent<CardTitle>(2);
                    h.AddAttribute(3, "ChildContent", (RenderFragment)(t => t.AddContent(0, "2,350")));
                    h.AddAttribute(4, "Class", "text-3xl");
                    h.CloseComponent();
                }));
                b.CloseComponent();

                b.OpenComponent<CardContent>(4);
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddContent(0, "+180 from last month")));
                b.CloseComponent();
            }));

    /// <summary>
    /// Renders four stats cards — representative of a card grid page section.
    /// Returned value is the last rendered card (prevents DCE on the loop).
    /// </summary>
    [Benchmark]
    public object Render_FourStatsCards()
    {
        string[] labels = ["Total Downloads", "Active Users", "Components", "Lighthouse Score"];
        string[] values = ["2,350", "573", "12", "100"];
        object last = null!;

        for (var i = 0; i < labels.Length; i++)
        {
            var label = labels[i];
            var value = values[i];

            last = Render<Card>(p => p.AddChildContent(
                b =>
                {
                    b.OpenComponent<CardHeader>(0);
                    b.AddAttribute(1, "ChildContent", (RenderFragment)(h =>
                    {
                        h.OpenComponent<CardDescription>(0);
                        h.AddAttribute(1, "ChildContent", (RenderFragment)(d => d.AddContent(0, label)));
                        h.CloseComponent();
                        h.OpenComponent<CardTitle>(2);
                        h.AddAttribute(3, "ChildContent", (RenderFragment)(t => t.AddContent(0, value)));
                        h.CloseComponent();
                    }));
                    b.CloseComponent();
                }));
        }

        return last;
    }
}
