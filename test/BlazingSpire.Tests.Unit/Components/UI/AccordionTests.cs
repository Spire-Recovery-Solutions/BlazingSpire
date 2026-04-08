using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AccordionTests : BlazingSpireTestBase
{
    // ── Accordion ─────────────────────────────────────────────────────────────

    [Fact]
    public void Accordion_Renders_Div_Element()
    {
        var cut = Render<Accordion>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Accordion_Has_Base_Classes()
    {
        var cut = Render<Accordion>();
        Assert.Contains("w-full", cut.Find("div").ClassName);
    }

    [Fact]
    public void Accordion_Custom_Class_Is_Appended()
    {
        var cut = Render<Accordion>(p => p.Add(x => x.Class, "my-accordion"));
        Assert.Contains("my-accordion", cut.Find("div").ClassName);
    }

    [Fact]
    public void Accordion_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Accordion>(p => p.AddUnmatched("data-testid", "accordion"));
        Assert.Equal("accordion", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void Accordion_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<Accordion>(p => p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void Accordion_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Accordion).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AccordionItem ─────────────────────────────────────────────────────────

    [Fact]
    public void AccordionItem_Renders_Details_Element()
    {
        var cut = Render<AccordionItem>();
        Assert.NotNull(cut.Find("details"));
    }

    [Fact]
    public void AccordionItem_Has_Base_Classes()
    {
        var cut = Render<AccordionItem>();
        Assert.Contains("border-b", cut.Find("details").ClassName);
    }

    [Fact]
    public void AccordionItem_Custom_Class_Is_Appended()
    {
        var cut = Render<AccordionItem>(p => p.Add(x => x.Class, "my-item"));
        Assert.Contains("my-item", cut.Find("details").ClassName);
    }

    [Fact]
    public void AccordionItem_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AccordionItem>(p => p.AddUnmatched("data-testid", "accordion-item"));
        Assert.Equal("accordion-item", cut.Find("details").GetAttribute("data-testid"));
    }

    [Fact]
    public void AccordionItem_ChildContent_Renders_Inside_Details()
    {
        var cut = Render<AccordionItem>(p => p.AddChildContent("<p>Item content</p>"));
        Assert.NotNull(cut.Find("details p"));
    }

    [Fact]
    public void AccordionItem_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AccordionItem).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AccordionTrigger ──────────────────────────────────────────────────────

    [Fact]
    public void AccordionTrigger_Renders_Summary_Element()
    {
        var cut = Render<AccordionTrigger>();
        Assert.NotNull(cut.Find("summary"));
    }

    [Fact]
    public void AccordionTrigger_Has_Base_Classes()
    {
        var cut = Render<AccordionTrigger>();
        var classes = cut.Find("summary").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("justify-between", classes);
        Assert.Contains("py-4", classes);
        Assert.Contains("font-medium", classes);
        Assert.Contains("hover:underline", classes);
    }

    [Fact]
    public void AccordionTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<AccordionTrigger>(p => p.Add(x => x.Class, "my-trigger"));
        Assert.Contains("my-trigger", cut.Find("summary").ClassName);
    }

    [Fact]
    public void AccordionTrigger_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AccordionTrigger>(p => p.AddUnmatched("data-testid", "accordion-trigger"));
        Assert.Equal("accordion-trigger", cut.Find("summary").GetAttribute("data-testid"));
    }

    [Fact]
    public void AccordionTrigger_ChildContent_Renders_Inside_Summary()
    {
        var cut = Render<AccordionTrigger>(p => p.AddChildContent("Question text"));
        Assert.Contains("Question text", cut.Find("summary").TextContent);
    }

    [Fact]
    public void AccordionTrigger_Has_Chevron_SVG()
    {
        var cut = Render<AccordionTrigger>();
        Assert.NotNull(cut.Find("summary svg"));
    }

    [Fact]
    public void AccordionTrigger_Chevron_SVG_Has_Path()
    {
        var cut = Render<AccordionTrigger>();
        Assert.NotNull(cut.Find("summary svg path"));
    }

    [Fact]
    public void AccordionTrigger_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AccordionTrigger).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AccordionContent ──────────────────────────────────────────────────────

    [Fact]
    public void AccordionContent_Renders_Div_Element()
    {
        var cut = Render<AccordionContent>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void AccordionContent_Has_Base_Classes()
    {
        var cut = Render<AccordionContent>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("overflow-hidden", classes);
        Assert.Contains("text-sm", classes);
        Assert.Contains("pb-4", classes);
        Assert.Contains("pt-0", classes);
    }

    [Fact]
    public void AccordionContent_Custom_Class_Is_Appended()
    {
        var cut = Render<AccordionContent>(p => p.Add(x => x.Class, "my-content"));
        Assert.Contains("my-content", cut.Find("div").ClassName);
    }

    [Fact]
    public void AccordionContent_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AccordionContent>(p => p.AddUnmatched("data-testid", "accordion-content"));
        Assert.Equal("accordion-content", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void AccordionContent_ChildContent_Renders_Inside_Div()
    {
        var cut = Render<AccordionContent>(p => p.AddChildContent("<p>Answer text</p>"));
        Assert.NotNull(cut.Find("div p"));
    }

    [Fact]
    public void AccordionContent_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AccordionContent).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
