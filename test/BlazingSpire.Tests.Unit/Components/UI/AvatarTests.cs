using BlazingSpire.Demo.Components.Shared;
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
    public void Avatar_Has_Base_Classes()
    {
        var cut = Render<Avatar>();
        var classes = cut.Find("span").ClassName;
        Assert.Contains("relative", classes);
        Assert.Contains("flex", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("w-10", classes);
        Assert.Contains("shrink-0", classes);
        Assert.Contains("overflow-hidden", classes);
        Assert.Contains("rounded-full", classes);
    }

    [Fact]
    public void Avatar_Custom_Class_Is_Appended()
    {
        var cut = Render<Avatar>(p => p.Add(x => x.Class, "my-avatar"));
        Assert.Contains("my-avatar", cut.Find("span").ClassName);
    }

    [Fact]
    public void Avatar_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Avatar>(p =>
            p.AddUnmatched("data-testid", "user-avatar"));
        Assert.Equal("user-avatar", cut.Find("span").GetAttribute("data-testid"));
    }

    [Fact]
    public void Avatar_ChildContent_Renders_Inside_Span()
    {
        var cut = Render<Avatar>(p =>
            p.AddChildContent("<img src='test.png' alt='test' />"));
        Assert.NotNull(cut.Find("span img"));
    }

    [Fact]
    public void Avatar_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Avatar).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AvatarImage ───────────────────────────────────────────────────────────

    [Fact]
    public void AvatarImage_Renders_Img_Element()
    {
        var cut = Render<AvatarImage>();
        Assert.NotNull(cut.Find("img"));
    }

    [Fact]
    public void AvatarImage_Has_Base_Classes()
    {
        var cut = Render<AvatarImage>();
        var classes = cut.Find("img").ClassName;
        Assert.Contains("aspect-square", classes);
        Assert.Contains("h-full", classes);
        Assert.Contains("w-full", classes);
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
    public void AvatarImage_Custom_Class_Is_Appended()
    {
        var cut = Render<AvatarImage>(p => p.Add(x => x.Class, "my-image"));
        Assert.Contains("my-image", cut.Find("img").ClassName);
    }

    [Fact]
    public void AvatarImage_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AvatarImage>(p =>
            p.AddUnmatched("data-testid", "avatar-img"));
        Assert.Equal("avatar-img", cut.Find("img").GetAttribute("data-testid"));
    }

    [Fact]
    public void AvatarImage_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AvatarImage).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── AvatarFallback ────────────────────────────────────────────────────────

    [Fact]
    public void AvatarFallback_Renders_Span_Element()
    {
        var cut = Render<AvatarFallback>();
        Assert.NotNull(cut.Find("span"));
    }

    [Fact]
    public void AvatarFallback_Has_Base_Classes()
    {
        var cut = Render<AvatarFallback>();
        var classes = cut.Find("span").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("h-full", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("justify-center", classes);
        Assert.Contains("rounded-full", classes);
        Assert.Contains("bg-muted", classes);
    }

    [Fact]
    public void AvatarFallback_Custom_Class_Is_Appended()
    {
        var cut = Render<AvatarFallback>(p => p.Add(x => x.Class, "my-fallback"));
        Assert.Contains("my-fallback", cut.Find("span").ClassName);
    }

    [Fact]
    public void AvatarFallback_AdditionalAttributes_PassThrough()
    {
        var cut = Render<AvatarFallback>(p =>
            p.AddUnmatched("data-testid", "avatar-fallback"));
        Assert.Equal("avatar-fallback", cut.Find("span").GetAttribute("data-testid"));
    }

    [Fact]
    public void AvatarFallback_ChildContent_Renders_Initials()
    {
        var cut = Render<AvatarFallback>(p =>
            p.AddChildContent("CN"));
        Assert.Contains("CN", cut.Find("span").TextContent);
    }

    [Fact]
    public void AvatarFallback_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(AvatarFallback).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
