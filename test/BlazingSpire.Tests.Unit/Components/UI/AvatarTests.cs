using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class AvatarTests : BlazingSpireTestBase
{
    // ── Avatar ────────────────────────────────────────────────────────────────

    [Fact]
    public void Avatar_Renders_Span_Element()
    {
        var cut = Render<Avatar>();
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void Avatar_ChildContent_Renders_Inside_Span()
    {
        var cut = Render<Avatar>(p => p.AddChildContent("<img src='test.png' alt='test' />"));
        Assert.NotNull(cut.Find("span img"));
    }

    [Fact]
    public void Avatar_Custom_Class_Is_Included()
    {
        var cut = Render<Avatar>(p => p.Add(x => x.Class, "my-avatar"));
        Assert.Contains("my-avatar", cut.Find("span").ClassName);
    }

    // ── AvatarImage ───────────────────────────────────────────────────────────

    [Fact]
    public void AvatarImage_Renders_Img_Element()
    {
        var cut = Render<AvatarImage>();
        Assert.NotNull(cut.Find("img"));
    }

    [Fact]
    public void AvatarImage_Src_Attribute_Is_Set()
    {
        var cut = Render<AvatarImage>(p => p.Add(x => x.Src, "https://example.com/avatar.png"));
        Assert.Equal("https://example.com/avatar.png", cut.Find("img").GetAttribute("src"));
    }

    [Fact]
    public void AvatarImage_Alt_Attribute_Is_Set()
    {
        var cut = Render<AvatarImage>(p => p.Add(x => x.Alt, "User avatar"));
        Assert.Equal("User avatar", cut.Find("img").GetAttribute("alt"));
    }

    [Fact]
    public void AvatarImage_Custom_Class_Is_Included()
    {
        var cut = Render<AvatarImage>(p => p.Add(x => x.Class, "my-image"));
        Assert.Contains("my-image", cut.Find("img").ClassName);
    }

    // ── AvatarFallback ────────────────────────────────────────────────────────

    [Fact]
    public void AvatarFallback_Renders_Span_Element()
    {
        var cut = Render<AvatarFallback>();
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void AvatarFallback_ChildContent_Renders_Initials()
    {
        var cut = Render<AvatarFallback>(p => p.AddChildContent("CN"));
        Assert.Contains("CN", cut.Find("span").TextContent);
    }

    [Fact]
    public void AvatarFallback_Custom_Class_Is_Included()
    {
        var cut = Render<AvatarFallback>(p => p.Add(x => x.Class, "my-fallback"));
        Assert.Contains("my-fallback", cut.Find("span").ClassName);
    }

    [Fact]
    public void AvatarFallback_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AvatarFallback>(p => p.AddUnmatched("data-testid", "avatar-fallback"));
        Assert.Equal("avatar-fallback", cut.Find("span").GetAttribute("data-testid"));
    }

    // ── Additional Avatar behavioral tests ────────────────────────────────────

    [Fact]
    public void Avatar_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Avatar>(p => p.AddUnmatched("data-testid", "avatar-root"));
        Assert.Equal("avatar-root", cut.Find("span").GetAttribute("data-testid"));
    }

    [Fact]
    public void AvatarImage_Both_Src_And_Alt_Set()
    {
        var cut = Render<AvatarImage>(p =>
        {
            p.Add(x => x.Src, "https://example.com/photo.jpg");
            p.Add(x => x.Alt, "Jane Doe");
        });
        var img = cut.Find("img");
        Assert.Equal("https://example.com/photo.jpg", img.GetAttribute("src"));
        Assert.Equal("Jane Doe", img.GetAttribute("alt"));
    }

    [Fact]
    public void AvatarImage_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AvatarImage>(p => p.AddUnmatched("data-testid", "avatar-img"));
        Assert.Equal("avatar-img", cut.Find("img").GetAttribute("data-testid"));
    }

    [Fact]
    public void Avatar_Renders_AvatarImage_Child()
    {
        var cut = Render<Avatar>(p =>
            p.AddChildContent<AvatarImage>(ip =>
            {
                ip.Add(x => x.Src, "https://example.com/x.png");
                ip.Add(x => x.Alt, "User");
            }));
        Assert.Equal("https://example.com/x.png", cut.Find("span img").GetAttribute("src"));
    }

    [Fact]
    public void Avatar_Renders_AvatarFallback_With_Initials()
    {
        var cut = Render<Avatar>(p =>
            p.AddChildContent<AvatarFallback>(fp => fp.AddChildContent("JD")));
        Assert.Contains("JD", cut.Find("span span").TextContent);
    }
}
