using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SelectTests : BlazingSpireTestBase
{
    // ── Select ───────────────────────────────────────────────────────────────

    [Fact]
    public void Select_Renders_CascadingValue()
    {
        var cut = Render<Select>(p => p.AddChildContent("<span>content</span>"));
        Assert.NotNull(cut);
    }

    [Fact]
    public void Select_Is_Assignable_To_PopoverBase()
    {
        Assert.True(typeof(Select).IsAssignableTo(typeof(PopoverBase)));
    }

    [Fact]
    public void Select_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Select).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }

    [Fact]
    public void Select_ShouldCloseOnEscape_Is_True()
    {
        var select = new Select();
        var prop = typeof(Select).GetProperty("ShouldCloseOnEscape",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(select)!);
    }

    [Fact]
    public void Select_ShouldCloseOnInteractOutside_Is_True()
    {
        var select = new Select();
        var prop = typeof(Select).GetProperty("ShouldCloseOnInteractOutside",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(prop);
        Assert.True((bool)prop!.GetValue(select)!);
    }

    // ── SelectContent ────────────────────────────────────────────────────────

    [Fact]
    public void SelectContent_Hidden_When_Closed()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>")));

        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void SelectContent_Has_Role_Listbox()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        Assert.Equal("listbox", cut.Find("[role=listbox]").GetAttribute("role"));
    }

    [Fact]
    public void SelectContent_Has_Base_Classes()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        var classes = cut.Find("[role=listbox]").ClassName;
        Assert.Contains("z-50", classes);
        Assert.Contains("rounded-md", classes);
        Assert.Contains("shadow-md", classes);
    }

    [Fact]
    public void SelectContent_Custom_Class_Is_Appended()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.Add(x => x.Class, "my-custom-class"));
        });

        Assert.Contains("my-custom-class", cut.Find("[role=listbox]").ClassName);
    }

    // ── SelectItem ───────────────────────────────────────────────────────────

    [Fact]
    public void SelectItem_Has_Role_Option()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        Assert.NotNull(cut.Find("[role=option]"));
    }

    [Fact]
    public void SelectItem_Has_Base_Classes()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        var classes = cut.Find("[role=option]").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("rounded-sm", classes);
    }

    [Fact]
    public void SelectItem_Selecting_Changes_Value()
    {
        string? selected = null;
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => selected = v));
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        cut.Find("[role=option]").Click();
        Assert.Equal("apple", selected);
    }

    [Fact]
    public void SelectItem_Selected_Shows_Check()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Value, "apple");
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        // Check SVG is present when selected
        var option = cut.Find("[role=option]");
        Assert.Contains("svg", option.InnerHtml);
    }

    [Fact]
    public void SelectItem_Not_Selected_No_Check()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Value, "banana");
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        var option = cut.Find("[role=option]");
        Assert.DoesNotContain("svg", option.InnerHtml);
    }

    [Fact]
    public void SelectItem_Disabled_Has_Data_Disabled()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.Add(x => x.Disabled, true);
                    ip.AddChildContent("Apple");
                }));
        });

        Assert.Equal("true", cut.Find("[role=option]").GetAttribute("data-disabled"));
    }

    [Fact]
    public void SelectItem_Custom_Class_Is_Appended()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.Add(x => x.Class, "custom-item");
                    ip.AddChildContent("Apple");
                }));
        });

        Assert.Contains("custom-item", cut.Find("[role=option]").ClassName);
    }

    // ── SelectSeparator ──────────────────────────────────────────────────────

    [Fact]
    public void SelectSeparator_Has_Role_Separator()
    {
        var cut = Render<SelectSeparator>();
        Assert.Equal("separator", cut.Find("[role=separator]").GetAttribute("role"));
    }

    [Fact]
    public void SelectSeparator_Has_Base_Classes()
    {
        var cut = Render<SelectSeparator>();
        var classes = cut.Find("[role=separator]").ClassName;
        Assert.Contains("h-px", classes);
        Assert.Contains("bg-muted", classes);
        Assert.Contains("my-1", classes);
    }

    // ── SelectLabel ──────────────────────────────────────────────────────────

    [Fact]
    public void SelectLabel_Renders_Div()
    {
        var cut = Render<SelectLabel>(p => p.AddChildContent("Fruits"));
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void SelectLabel_Has_Base_Classes()
    {
        var cut = Render<SelectLabel>(p => p.AddChildContent("Fruits"));
        var classes = cut.Find("div").ClassName;
        Assert.Contains("text-sm", classes);
        Assert.Contains("font-semibold", classes);
    }

    // ── SelectGroup ──────────────────────────────────────────────────────────

    [Fact]
    public void SelectGroup_Has_Role_Group()
    {
        var cut = Render<SelectGroup>(p => p.AddChildContent("<span>item</span>"));
        Assert.Equal("group", cut.Find("[role=group]").GetAttribute("role"));
    }

    [Fact]
    public void SelectGroup_Renders_ChildContent()
    {
        var cut = Render<SelectGroup>(p =>
            p.AddChildContent("<span data-testid='child'>child</span>"));
        Assert.NotNull(cut.Find("[data-testid=child]"));
    }

    // ── SelectTrigger ────────────────────────────────────────────────────────

    [Fact]
    public void SelectTrigger_Renders_Button()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectTrigger>());
        Assert.NotEmpty(cut.FindAll("button"));
    }

    [Fact]
    public void SelectTrigger_Has_Base_Classes()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectTrigger>());
        var classes = cut.Find("button").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("h-10", classes);
        Assert.Contains("w-full", classes);
        Assert.Contains("rounded-md", classes);
    }

    [Fact]
    public void SelectTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectTrigger>(t => t.Add(x => x.Class, "my-trigger")));
        Assert.Contains("my-trigger", cut.Find("button").ClassName);
    }

    // ── SelectValue ──────────────────────────────────────────────────────────

    [Fact]
    public void SelectValue_Shows_Placeholder_When_No_Value()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Placeholder, "Pick a fruit");
            p.AddChildContent<SelectValue>();
        });
        var span = cut.Find("span");
        Assert.Contains("Pick a fruit", span.TextContent);
        Assert.Contains("text-muted-foreground", span.ClassName);
    }

    [Fact]
    public async Task SelectValue_Shows_SelectedText_After_Item_Selected()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Placeholder, "Pick a fruit");
            p.AddChildContent<SelectValue>();
        });

        await cut.InvokeAsync(() => cut.Instance.SelectItemAsync("apple", "Apple"));
        Assert.Contains("Apple", cut.Find("span").TextContent);
    }

    // ── SelectItem closes dropdown ────────────────────────────────────────────

    [Fact]
    public async Task SelectItem_Click_Closes_Dropdown()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "apple");
                    ip.AddChildContent("Apple");
                }));
        });

        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
        await cut.Find("[role=option]").ClickAsync(new());
        Assert.Empty(cut.FindAll("[role=listbox]"));
    }
}
