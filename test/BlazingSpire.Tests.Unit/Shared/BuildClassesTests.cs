using BlazingSpire.Demo.Components.Shared;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazingSpire.Tests.Unit.Shared;

/// <summary>
/// Exposes the protected static BuildClasses method for testing.
/// </summary>
file sealed class BuildClassesExposed : BlazingSpireComponentBase
{
    protected override string BaseClasses => "";

    protected override void BuildRenderTree(RenderTreeBuilder builder) { }

    public static string Invoke(params string?[] parts) => BuildClasses(parts);
}

public class BuildClassesTests
{
    [Fact]
    public void MultipleNonEmptyParts_JoinedWithSpaces()
        => Assert.Equal("foo bar baz", BuildClassesExposed.Invoke("foo", "bar", "baz"));

    [Fact]
    public void NullParts_Skipped()
        => Assert.Equal("foo baz", BuildClassesExposed.Invoke("foo", null, "baz"));

    [Fact]
    public void EmptyParts_Skipped()
        => Assert.Equal("foo baz", BuildClassesExposed.Invoke("foo", "", "baz"));

    [Fact]
    public void WhitespaceParts_Skipped()
        => Assert.Equal("foo baz", BuildClassesExposed.Invoke("foo", "   ", "baz"));

    [Fact]
    public void SinglePart_ReturnsThatPart()
        => Assert.Equal("foo", BuildClassesExposed.Invoke("foo"));

    [Fact]
    public void AllNullOrEmpty_ReturnsEmptyString()
        => Assert.Equal("", BuildClassesExposed.Invoke(null, "", "   "));

    [Fact]
    public void OrderPreserved()
        => Assert.Equal("a b c d", BuildClassesExposed.Invoke("a", "b", "c", "d"));
}
