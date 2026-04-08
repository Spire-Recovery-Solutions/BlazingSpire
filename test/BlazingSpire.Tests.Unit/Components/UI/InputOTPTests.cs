using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class InputOTPTests : BlazingSpireTestBase
{
    // ── Rendering ────────────────────────────────────────────────────────────

    [Fact]
    public void Renders_Div_With_Hidden_Input()
    {
        var cut = Render<InputOTP>();
        Assert.NotNull(cut.Find("div"));
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void Hidden_Input_Has_Sr_Only_Class()
    {
        var cut = Render<InputOTP>();
        Assert.Contains("sr-only", cut.Find("input").ClassName);
    }

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

    // ── Slots ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Correct_Number_Of_Slots_Rendered()
    {
        var cut = Render<InputOTP>(p => p
            .Add(x => x.MaxLength, 6)
            .AddChildContent<InputOTPGroup>(g => g
                .AddChildContent(b =>
                {
                    b.OpenComponent<InputOTPSlot>(0);
                    b.AddAttribute(1, "Index", 0);
                    b.CloseComponent();
                    b.OpenComponent<InputOTPSlot>(2);
                    b.AddAttribute(3, "Index", 1);
                    b.CloseComponent();
                    b.OpenComponent<InputOTPSlot>(4);
                    b.AddAttribute(5, "Index", 2);
                    b.CloseComponent();
                })));

        Assert.Equal(3, cut.FindAll(".relative.flex.h-10.w-10").Count);
    }

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

        var slot = cut.Find(".relative.flex.h-10.w-10");
        Assert.Contains("4", slot.TextContent);
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

        // Active slot (index 0, value length 0) renders a pulse cursor
        Assert.NotNull(cut.Find(".animate-pulse"));
    }

    // ── Separator ─────────────────────────────────────────────────────────────

    [Fact]
    public void Separator_Renders_With_Role_Separator()
    {
        var cut = Render<InputOTPSeparator>();
        AssertRole(cut.Find("div"), "separator");
    }

    [Fact]
    public void Separator_Renders_Svg()
    {
        var cut = Render<InputOTPSeparator>();
        Assert.NotNull(cut.Find("svg"));
    }

    // ── Base classes ─────────────────────────────────────────────────────────

    [Fact]
    public void InputOTP_Has_Flex_Layout_Classes()
    {
        var cut = Render<InputOTP>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("gap-2", classes);
    }

    [Fact]
    public void InputOTPGroup_Has_Flex_Items_Center_Classes()
    {
        var cut = Render<InputOTPGroup>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
    }

    [Fact]
    public void InputOTPSlot_Has_Base_Layout_Classes()
    {
        var cut = Render<InputOTPSlot>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("h-10", classes);
        Assert.Contains("w-10", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("justify-center", classes);
        Assert.Contains("text-sm", classes);
    }

    // ── Custom Class ─────────────────────────────────────────────────────────

    [Fact]
    public void InputOTP_Custom_Class_Is_Appended()
    {
        var cut = Render<InputOTP>(p => p.Add(x => x.Class, "my-custom"));
        Assert.Contains("my-custom", cut.Find("div").ClassName);
    }

    [Fact]
    public void InputOTPSlot_Custom_Class_Is_Appended()
    {
        var cut = Render<InputOTPSlot>(p => p.Add(x => x.Class, "slot-custom"));
        Assert.Contains("slot-custom", cut.Find("div").ClassName);
    }

    // ── Inheritance ───────────────────────────────────────────────────────────

    [Fact]
    public void InputOTP_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(InputOTP).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void InputOTPGroup_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(InputOTPGroup).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void InputOTPSlot_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(InputOTPSlot).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void InputOTPSeparator_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(InputOTPSeparator).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── Disabled ─────────────────────────────────────────────────────────────

    [Fact]
    public void Disabled_Sets_Input_Disabled_Attribute()
    {
        var cut = Render<InputOTP>(p => p.Add(x => x.Disabled, true));
        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    // ── MaxLength ─────────────────────────────────────────────────────────────

    [Fact]
    public void MaxLength_Sets_Input_Maxlength_Attribute()
    {
        var cut = Render<InputOTP>(p => p.Add(x => x.MaxLength, 4));
        Assert.Equal("4", cut.Find("input").GetAttribute("maxlength"));
    }
}
