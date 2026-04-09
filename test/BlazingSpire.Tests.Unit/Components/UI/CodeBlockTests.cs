using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CodeBlockTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Pre_And_Code_Elements()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "<Button>Click</Button>"));
        Assert.NotNull(cut.Find("pre"));
        Assert.NotNull(cut.Find("code"));
    }

    [Fact]
    public void Inherits_From_BlazingSpireComponentBase()
    {
        Assert.True(typeof(CodeBlock).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Default_Language_Is_Xml()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "test"));
        Assert.Contains("language-xml", cut.Find("code").ClassName);
        Assert.Contains("language-xml", cut.Find("pre").ClassName);
    }

    [Fact]
    public void Custom_Language_Applies()
    {
        var cut = Render<CodeBlock>(p => p
            .Add(x => x.Code, "var x = 1;")
            .Add(x => x.Language, "csharp"));
        Assert.Contains("language-csharp", cut.Find("code").ClassName);
    }

    [Fact]
    public void Code_Element_Is_Empty_Before_JS_Interop()
    {
        // Content is set via JS interop (BlazingSpire.setTextAndHighlight),
        // so the rendered <code> element is empty in bUnit (no JS runtime).
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "<Button>Click</Button>"));
        Assert.Empty(cut.Find("code").InnerHtml);
    }

    [Fact]
    public void JS_Interop_Called_With_Correct_Function()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "<Button />"));
        Assert.Contains(JSInterop.Invocations,
            i => i.Identifier == "BlazingSpire.setTextAndHighlight");
    }

    [Fact]
    public void JS_Interop_Receives_Trimmed_Code()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "\n  <Button />\n  "));
        var invocation = JSInterop.Invocations
            .First(i => i.Identifier == "BlazingSpire.setTextAndHighlight");
        Assert.Equal("<Button />", invocation.Arguments[1]);
    }

    [Fact]
    public void Empty_Code_Renders_Empty_Block()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, ""));
        Assert.NotNull(cut.Find("pre code"));
    }
}
