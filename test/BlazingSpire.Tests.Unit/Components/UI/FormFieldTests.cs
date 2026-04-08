using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class FormFieldTests : BlazingSpireTestBase
{
    // ── FormField ─────────────────────────────────────────────────────────────

    [Fact]
    public void FormField_Renders_Div_With_SpaceY2()
    {
        var cut = Render<FormField>();
        Assert.Contains("space-y-2", cut.Find("div").ClassName);
    }

    [Fact]
    public void FormField_Custom_Class_Is_Appended()
    {
        var cut = Render<FormField>(p => p.Add(x => x.Class, "my-class"));
        Assert.Contains("my-class", cut.Find("div").ClassName);
    }

    [Fact]
    public void FormField_ChildContent_Renders()
    {
        var cut = Render<FormField>(p =>
            p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut.Find("div span"));
    }

    [Fact]
    public void FormField_ItemId_Uses_Name_When_Provided()
    {
        var cut = Render<FormField>(p => p.Add(x => x.Name, "username"));
        Assert.Equal("username-form-item", cut.Instance.ItemId);
    }

    [Fact]
    public void FormField_ItemId_Auto_Generated_When_No_Name()
    {
        var cut = Render<FormField>();
        Assert.StartsWith("bs-form-", cut.Instance.ItemId);
    }

    [Fact]
    public void FormField_DescriptionId_Derived_From_ItemId()
    {
        var cut = Render<FormField>(p => p.Add(x => x.Name, "email"));
        Assert.Equal("email-form-item-description", cut.Instance.DescriptionId);
    }

    [Fact]
    public void FormField_MessageId_Derived_From_ItemId()
    {
        var cut = Render<FormField>(p => p.Add(x => x.Name, "email"));
        Assert.Equal("email-form-item-message", cut.Instance.MessageId);
    }

    [Fact]
    public void FormField_HasError_True_When_ErrorMessage_Set()
    {
        var cut = Render<FormField>(p => p.Add(x => x.ErrorMessage, "Required"));
        Assert.True(cut.Instance.HasError);
    }

    [Fact]
    public void FormField_HasError_False_When_No_ErrorMessage()
    {
        var cut = Render<FormField>();
        Assert.False(cut.Instance.HasError);
    }

    [Fact]
    public void FormField_AdditionalAttributes_PassThrough()
    {
        var cut = Render<FormField>(p => p.AddUnmatched("data-testid", "form-field"));
        Assert.Equal("form-field", cut.Find("div").GetAttribute("data-testid"));
    }

    [Fact]
    public void FormField_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(FormField).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── FormFieldLabel ────────────────────────────────────────────────────────

    [Fact]
    public void FormFieldLabel_Renders_Label_Element()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldLabel>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Username")));
                b.CloseComponent();
            })));
        Assert.NotNull(cut.Find("label"));
    }

    [Fact]
    public void FormFieldLabel_For_Attribute_Matches_Field_ItemId()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.Name, "username");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldLabel>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Label")));
                b.CloseComponent();
            }));
        });
        Assert.Equal("username-form-item", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void FormFieldLabel_Has_Base_Classes()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldLabel>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Label")));
                b.CloseComponent();
            })));
        var classes = cut.Find("label").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-medium", classes);
    }

    [Fact]
    public void FormFieldLabel_Custom_Class_Is_Appended()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldLabel>(0);
                b.AddAttribute(1, "class", "custom-label");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Label")));
                b.CloseComponent();
            })));
        Assert.Contains("custom-label", cut.Find("label").ClassName);
    }

    [Fact]
    public void FormFieldLabel_ChildContent_Renders()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldLabel>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "My Label")));
                b.CloseComponent();
            })));
        Assert.Contains("My Label", cut.Find("label").TextContent);
    }

    [Fact]
    public void FormFieldLabel_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(FormFieldLabel).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── FormFieldDescription ──────────────────────────────────────────────────

    [Fact]
    public void FormFieldDescription_Renders_P_Element()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldDescription>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Helper")));
                b.CloseComponent();
            })));
        Assert.NotNull(cut.Find("p"));
    }

    [Fact]
    public void FormFieldDescription_Id_Matches_DescriptionId()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.Name, "email");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldDescription>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Enter your email")));
                b.CloseComponent();
            }));
        });
        Assert.Equal("email-form-item-description", cut.Find("p").GetAttribute("id"));
    }

    [Fact]
    public void FormFieldDescription_Has_Muted_Class()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldDescription>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Helper")));
                b.CloseComponent();
            })));
        Assert.Contains("text-muted-foreground", cut.Find("p").ClassName);
    }

    [Fact]
    public void FormFieldDescription_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(FormFieldDescription).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    // ── FormFieldError ────────────────────────────────────────────────────────

    [Fact]
    public void FormFieldError_Does_Not_Render_When_No_Error()
    {
        var cut = Render<FormField>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.CloseComponent();
            })));
        Assert.Empty(cut.FindAll("p[role='alert']"));
    }

    [Fact]
    public void FormFieldError_Renders_When_Error_Present()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.ErrorMessage, "This field is required");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.CloseComponent();
            }));
        });
        Assert.NotNull(cut.Find("p[role='alert']"));
    }

    [Fact]
    public void FormFieldError_Id_Matches_MessageId()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.Name, "username");
            p.Add(x => x.ErrorMessage, "Required");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.CloseComponent();
            }));
        });
        Assert.Equal("username-form-item-message", cut.Find("p[role='alert']").GetAttribute("id"));
    }

    [Fact]
    public void FormFieldError_Shows_ErrorMessage_When_No_ChildContent()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.ErrorMessage, "Username is required");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.CloseComponent();
            }));
        });
        Assert.Contains("Username is required", cut.Find("p[role='alert']").TextContent);
    }

    [Fact]
    public void FormFieldError_ChildContent_Overrides_ErrorMessage()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.ErrorMessage, "Auto error");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Custom error message")));
                b.CloseComponent();
            }));
        });
        Assert.Contains("Custom error message", cut.Find("p[role='alert']").TextContent);
    }

    [Fact]
    public void FormFieldError_Has_Destructive_Class()
    {
        var cut = Render<FormField>(p =>
        {
            p.Add(x => x.ErrorMessage, "Error");
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FormFieldError>(0);
                b.CloseComponent();
            }));
        });
        Assert.Contains("text-destructive", cut.Find("p[role='alert']").ClassName);
    }

    [Fact]
    public void FormFieldError_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(FormFieldError).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}
