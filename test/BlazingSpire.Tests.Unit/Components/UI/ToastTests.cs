using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ToastTests : BlazingSpireTestBase
{
    // ── ToastService ─────────────────────────────────────────────────────────

    [Fact]
    public void ToastService_Show_FiresOnShowEvent()
    {
        var service = new ToastService();
        ToastMessage? received = null;
        service.OnShow += msg => received = msg;

        service.Show("Hello", "World");

        Assert.NotNull(received);
        Assert.Equal("Hello", received!.Title);
        Assert.Equal("World", received.Description);
        Assert.Equal(ToastVariant.Default, received.Variant);
    }

    [Fact]
    public void ToastService_Show_WithMessage_FiresOnShowEvent()
    {
        var service = new ToastService();
        ToastMessage? received = null;
        service.OnShow += msg => received = msg;

        var message = new ToastMessage("Title", "Desc", ToastVariant.Destructive, 3000);
        service.Show(message);

        Assert.NotNull(received);
        Assert.Equal("Title", received!.Title);
        Assert.Equal(ToastVariant.Destructive, received.Variant);
        Assert.Equal(3000, received.DurationMs);
    }

    [Fact]
    public void ToastService_Show_DefaultVariantIsDefault()
    {
        var service = new ToastService();
        ToastMessage? received = null;
        service.OnShow += msg => received = msg;

        service.Show("Test");

        Assert.Equal(ToastVariant.Default, received!.Variant);
    }

    // ── ToastProvider ─────────────────────────────────────────────────────────

    [Fact]
    public void ToastProvider_RendersContainerDiv()
    {
        Services.AddScoped<IToastService, ToastService>();
        var cut = Render<ToastProvider>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void ToastProvider_HasBaseClasses()
    {
        Services.AddScoped<IToastService, ToastService>();
        var cut = Render<ToastProvider>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("fixed", classes);
        Assert.Contains("bottom-0", classes);
        Assert.Contains("right-0", classes);
        Assert.Contains("z-[100]", classes);
    }

    [Fact]
    public void ToastProvider_Custom_Class_Is_Appended()
    {
        Services.AddScoped<IToastService, ToastService>();
        var cut = Render<ToastProvider>(p => p.Add(x => x.Class, "my-provider"));
        Assert.Contains("my-provider", cut.Find("div").ClassName);
    }

    [Fact]
    public void ToastProvider_UnsubscribesOnDispose()
    {
        var service = new ToastService();
        Services.AddScoped<IToastService>(_ => service);
        var cut = Render<ToastProvider>();
        cut.Instance.Dispose();

        // After dispose, showing a toast should not throw
        service.Show("After dispose");
        Assert.Empty(cut.FindAll("div[class*='rounded-md']"));
    }

    // ── Toast ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Toast_RendersRootDiv()
    {
        var msg = new ToastMessage("Title", "Desc");
        var cut = Render<Toast>(p => p.Add(x => x.Message, msg));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Toast_DefaultVariant_HasCorrectClasses()
    {
        var msg = new ToastMessage("T", null, ToastVariant.Default);
        var cut = Render<Toast>(p => p.Add(x => x.Message, msg));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("bg-background", classes);
        Assert.Contains("text-foreground", classes);
    }

    [Fact]
    public void Toast_DestructiveVariant_HasCorrectClasses()
    {
        var msg = new ToastMessage("T", null, ToastVariant.Destructive);
        var cut = Render<Toast>(p =>
        {
            p.Add(x => x.Message, msg);
            p.Add(x => x.Variant, ToastVariant.Destructive);
        });
        var classes = cut.Find("div").ClassName;
        Assert.Contains("border-destructive", classes);
        Assert.Contains("bg-destructive", classes);
    }

    [Fact]
    public void Toast_HasBaseLayoutClasses()
    {
        var msg = new ToastMessage("T", null);
        var cut = Render<Toast>(p => p.Add(x => x.Message, msg));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-lg", classes);
        Assert.Contains("overflow-hidden", classes);
    }

    [Fact]
    public void Toast_Custom_Class_Is_Appended()
    {
        var msg = new ToastMessage("T", null);
        var cut = Render<Toast>(p =>
        {
            p.Add(x => x.Message, msg);
            p.Add(x => x.Class, "my-toast");
        });
        Assert.Contains("my-toast", cut.Find("div").ClassName);
    }

    [Fact]
    public void Toast_Is_Assignable_To_PresentationalBase()
    {
        Assert.True(typeof(Toast).IsAssignableTo(typeof(PresentationalBase<ToastVariant>)));
    }

    // ── ToastTitle ────────────────────────────────────────────────────────────

    [Fact]
    public void ToastTitle_RendersDiv_WithBaseClasses()
    {
        var cut = Render<ToastTitle>(p => p.AddChildContent("My Title"));
        var el = cut.Find("div");
        Assert.Contains("text-sm", el.ClassName);
        Assert.Contains("font-semibold", el.ClassName);
        Assert.Contains("My Title", el.TextContent);
    }

    [Fact]
    public void ToastTitle_Custom_Class_Is_Appended()
    {
        var cut = Render<ToastTitle>(p => p.Add(x => x.Class, "title-cls"));
        Assert.Contains("title-cls", cut.Find("div").ClassName);
    }

    // ── ToastDescription ──────────────────────────────────────────────────────

    [Fact]
    public void ToastDescription_RendersDiv_WithBaseClasses()
    {
        var cut = Render<ToastDescription>(p => p.AddChildContent("Desc text"));
        var el = cut.Find("div");
        Assert.Contains("text-sm", el.ClassName);
        Assert.Contains("opacity-90", el.ClassName);
        Assert.Contains("Desc text", el.TextContent);
    }

    [Fact]
    public void ToastDescription_Custom_Class_Is_Appended()
    {
        var cut = Render<ToastDescription>(p => p.Add(x => x.Class, "desc-cls"));
        Assert.Contains("desc-cls", cut.Find("div").ClassName);
    }

    // ── ToastClose ────────────────────────────────────────────────────────────

    [Fact]
    public void ToastClose_RendersButton_WithAriaLabel()
    {
        var cut = Render<ToastClose>();
        var btn = cut.Find("button");
        Assert.Equal("Close", btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void ToastClose_HasBaseClasses()
    {
        var cut = Render<ToastClose>();
        var classes = cut.Find("button").ClassName;
        Assert.Contains("absolute", classes);
        Assert.Contains("right-2", classes);
        Assert.Contains("top-2", classes);
        Assert.Contains("rounded-md", classes);
    }

    [Fact]
    public void ToastClose_InvokesOnDismiss_OnClick()
    {
        var dismissed = false;
        var cut = Render<ToastClose>(p =>
            p.Add(x => x.OnDismiss, EventCallback.Factory.Create(this, () => dismissed = true)));
        cut.Find("button").Click();
        Assert.True(dismissed);
    }

    // ── ToastAction ───────────────────────────────────────────────────────────

    [Fact]
    public void ToastAction_RendersButton()
    {
        var cut = Render<ToastAction>(p => p.AddChildContent("Undo"));
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void ToastAction_Click_Fires_OnClick()
    {
        var clicked = false;
        var cut = Render<ToastAction>(p => p.Add(x => x.OnClick, () => clicked = true));
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    // ── Multiple toasts ───────────────────────────────────────────────────────

    [Fact]
    public void ToastProvider_Shows_Multiple_Toasts()
    {
        var service = new ToastService();
        Services.AddScoped<IToastService>(_ => service);
        var cut = Render<ToastProvider>();

        service.Show("First", "First description");
        service.Show("Second", "Second description");

        cut.WaitForState(() => cut.FindAll("button[aria-label='Close']").Count == 2);
        Assert.Equal(2, cut.FindAll("button[aria-label='Close']").Count);
    }

    // ── Dismiss integration ───────────────────────────────────────────────────

    [Fact]
    public void ToastProvider_Dismiss_RemovesToast()
    {
        var service = new ToastService();
        Services.AddScoped<IToastService>(_ => service);
        var cut = Render<ToastProvider>();

        service.Show("Hello");
        cut.WaitForState(() => cut.FindAll("button[aria-label='Close']").Count > 0);

        cut.Find("button[aria-label='Close']").Click();
        cut.WaitForState(() => cut.FindAll("button[aria-label='Close']").Count == 0);

        Assert.Empty(cut.FindAll("button[aria-label='Close']"));
    }
}
