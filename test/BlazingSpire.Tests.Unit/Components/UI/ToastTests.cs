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
    public void ToastProvider_Renders_Container()
    {
        Services.AddScoped<IToastService, ToastService>();
        var cut = Render<ToastProvider>();
        Assert.NotNull(cut.Find("div"));
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
        Assert.Empty(cut.FindAll("button[aria-label='Close']"));
    }

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

    // ── Toast ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Toast_Renders_With_Title()
    {
        var msg = new ToastMessage("My Title", null);
        var cut = Render<Toast>(p => p.Add(x => x.Message, msg));
        Assert.NotNull(cut.Find("div"));
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

    // ── ToastTitle ────────────────────────────────────────────────────────────

    [Fact]
    public void ToastTitle_Renders_ChildContent()
    {
        var cut = Render<ToastTitle>(p => p.AddChildContent("My Title"));
        Assert.Contains("My Title", cut.Find("div").TextContent);
    }

    // ── ToastDescription ──────────────────────────────────────────────────────

    [Fact]
    public void ToastDescription_Renders_ChildContent()
    {
        var cut = Render<ToastDescription>(p => p.AddChildContent("Desc text"));
        Assert.Contains("Desc text", cut.Find("div").TextContent);
    }

    // ── ToastClose ────────────────────────────────────────────────────────────

    [Fact]
    public void ToastClose_Renders_Button_WithAriaLabel()
    {
        var cut = Render<ToastClose>();
        Assert.Equal("Close", cut.Find("button").GetAttribute("aria-label"));
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
    public void ToastAction_Renders_Button()
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
}
