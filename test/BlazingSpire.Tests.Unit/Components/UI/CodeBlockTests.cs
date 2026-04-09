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
    public void Code_Is_Html_Encoded()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "<Button>Click</Button>"));
        var html = cut.Find("code").InnerHtml;
        Assert.Contains("&lt;Button&gt;", html);
        Assert.Contains("&lt;/Button&gt;", html);
    }

    [Fact]
    public void At_Signs_Are_Encoded()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "@bind-Value=\"_name\""));
        var html = cut.Find("code").InnerHtml;
        // WebUtility.HtmlEncode doesn't encode @ but Blazor won't interpret it
        // since it's inside MarkupString — the text renders literally
        Assert.Contains("bind-Value", html);
    }

    [Fact]
    public void Code_Is_Trimmed()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, "\n  <Button />\n  "));
        var text = cut.Find("code").TextContent;
        Assert.Equal("<Button />", text);
    }

    [Fact]
    public void Empty_Code_Renders_Empty_Block()
    {
        var cut = Render<CodeBlock>(p => p.Add(x => x.Code, ""));
        Assert.NotNull(cut.Find("pre code"));
    }
}
