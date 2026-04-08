using AngleSharp.Dom;

namespace BlazingSpire.Tests.Unit.Shared;

/// <summary>
/// Base class for all BlazingSpire bUnit tests.
/// Sets up loose JS interop mode and common assertion helpers.
/// </summary>
public abstract class BlazingSpireTestBase : BunitContext
{
    protected BlazingSpireTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected static void AssertAriaExpanded(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-expanded"));

    protected static void AssertRole(IElement element, string role)
        => Assert.Equal(role, element.GetAttribute("role"));

    protected static void AssertAriaLabel(IElement element, string label)
        => Assert.Equal(label, element.GetAttribute("aria-label"));

    protected static void AssertAriaHidden(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-hidden"));

    protected static void AssertDataState(IElement element, string state)
        => Assert.Equal(state, element.GetAttribute("data-state"));

    protected static void AssertAriaChecked(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-checked"));

    protected static void AssertAriaSelected(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-selected"));

    protected static void AssertAriaModal(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-modal"));
}
