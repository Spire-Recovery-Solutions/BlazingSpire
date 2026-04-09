using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AccordionTests : BlazingSpireTestBase
{
    // Accordion uses native <details>/<summary> — browser handles toggle natively.
    // bUnit cannot simulate click on <summary> to open/close. Test structural correctness.

    private IRenderedComponent<Accordion> RenderAccordion()
    {
        return Render<Accordion>(p =>
            p.AddChildContent<AccordionItem>(item =>
                item.AddChildContent(builder =>
                {
                    builder.OpenComponent<AccordionTrigger>(0);
                    builder.AddAttribute(1, "ChildContent",
                        (RenderFragment)(b => b.AddContent(0, "Question")));
                    builder.CloseComponent();

                    builder.OpenComponent<AccordionContent>(2);
                    builder.AddAttribute(3, "ChildContent",
                        (RenderFragment)(b => b.AddContent(0, "Answer")));
                    builder.CloseComponent();
                })));
    }

    [Fact]
    public void AccordionItem_Renders_As_Details_Element()
    {
        var cut = RenderAccordion();
        Assert.NotNull(cut.Find("details"));
    }

    [Fact]
    public void AccordionTrigger_Renders_As_Summary_With_Content()
    {
        var cut = RenderAccordion();
        var summary = cut.Find("summary");
        Assert.Contains("Question", summary.TextContent);
    }

    [Fact]
    public void AccordionContent_Renders_Inside_Details()
    {
        var cut = RenderAccordion();
        Assert.Contains("Answer", cut.Find("details").TextContent);
    }

    [Fact]
    public void Custom_Class_Merges_On_Accordion()
    {
        var cut = Render<Accordion>(p => p.Add(x => x.Class, "my-accordion"));
        Assert.Contains("my-accordion", cut.Find("div").ClassName);
    }

    [Fact]
    public void Multiple_Items_Each_Render_Separate_Details_Element()
    {
        var cut = Render<Accordion>(p =>
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Item 1")));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(2);
                builder.AddAttribute(3, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Item 2")));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(4);
                builder.AddAttribute(5, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Item 3")));
                builder.CloseComponent();
            }));

        Assert.Equal(3, cut.FindAll("details").Count);
    }

    [Fact]
    public void AccordionTrigger_Summary_Has_No_Default_List_Style()
    {
        var cut = RenderAccordion();
        // list-style:none is applied inline to suppress the disclosure triangle
        Assert.Contains("list-style: none", cut.Find("summary").GetAttribute("style") ?? "");
    }
}
