using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class InputOTPTests : BlazingSpireTestBase
{
    // ── Hidden input attributes (accessibility) ───────────────────────────────

    [Fact]
    public void Hidden_Input_Has_Correct_InputMode()
    {
        var cut = Render<InputOTP>();
        Assert.Equal("numeric", cut.Find("input").GetAttribute("inputmode"));
    }

    [Fact]
    public void Hidden_Input_Has_Autocomplete_One_Time_Code()
    {
        var cut = Render<InputOTP>();
        Assert.Equal("one-time-code", cut.Find("input").GetAttribute("autocomplete"));
    }

    [Fact]
    public void Hidden_Input_Has_Aria_Label()
    {
        var cut = Render<InputOTP>();
        Assert.Equal("One-time password", cut.Find("input").GetAttribute("aria-label"));
    }

    // ── MaxLength ─────────────────────────────────────────────────────────────

    [Fact]
    public void MaxLength_Sets_Input_Maxlength_Attribute()
    {
        var cut = Render<InputOTP>(p => p.Add(x => x.MaxLength, 4));
        Assert.Equal("4", cut.Find("input").GetAttribute("maxlength"));
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Sets_Input_Disabled_Attribute()
    {
        var cut = Render<InputOTP>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    // ── UpdateValueAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateValueAsync_Fires_ValueChanged()
    {
        string? received = null;
        var cut = Render<InputOTP>(p =>
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v)));

        await cut.InvokeAsync(() => cut.Instance.UpdateValueAsync("123"));

        Assert.Equal("123", received);
    }

    [Fact]
    public async Task UpdateValueAsync_Rejects_Value_Exceeding_MaxLength()
    {
        string? received = null;
        var cut = Render<InputOTP>(p =>
        {
            p.Add(x => x.MaxLength, 4);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v));
        });

        await cut.InvokeAsync(() => cut.Instance.UpdateValueAsync("12345")); // 5 chars > MaxLength 4

        Assert.Null(received);
    }

    [Fact]
    public async Task OnInput_Filters_Non_Digit_Characters()
    {
        // The oninput handler strips non-digits before calling UpdateValueAsync.
        // UpdateValueAsync itself does not filter — it only enforces MaxLength.
        string? received = null;
        var cut = Render<InputOTP>(p =>
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v)));

        await cut.Find("input").TriggerEventAsync("oninput",
            new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "abc123" });

        Assert.Equal("123", received);
    }

    // ── Slot behavior ─────────────────────────────────────────────────────────

    [Fact]
    public void Slot_Displays_Character_From_Value()
    {
        var cut = Render<InputOTP>(p => p
            .Add(x => x.Value, "4")
            .AddChildContent<InputOTPGroup>(g => g
                .AddChildContent(b =>
                {
                    b.OpenComponent<InputOTPSlot>(0);
                    b.AddAttribute(1, "Index", 0);
                    b.CloseComponent();
                })));

        Assert.Contains("4", cut.Find("div > div > div").TextContent);
    }

    [Fact]
    public void Active_Slot_Shows_Cursor_When_Value_Empty()
    {
        var cut = Render<InputOTP>(p => p
            .Add(x => x.Value, "")
            .AddChildContent<InputOTPGroup>(g => g
                .AddChildContent(b =>
                {
                    b.OpenComponent<InputOTPSlot>(0);
                    b.AddAttribute(1, "Index", 0);
                    b.CloseComponent();
                })));

        // Active slot (index 0, value length 0) renders a pulsing cursor indicator
        Assert.NotNull(cut.Find(".animate-pulse"));
    }
}

public class InputOTPSeparatorTests : BlazingSpireTestBase
{
    // ── ARIA role ─────────────────────────────────────────────────────────────

    [Fact]
    public void Separator_Renders_With_Role_Separator()
    {
        var cut = Render<InputOTPSeparator>();
        AssertRole(cut.Find("div"), "separator");
    }
}
