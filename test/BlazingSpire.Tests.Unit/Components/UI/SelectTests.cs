using Microsoft.AspNetCore.Components;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class SelectTests : BlazingSpireTestBase
{
    // ── SelectContent visibility ──────────────────────────────────────────────

    [Fact]
    public void SelectContent_Hidden_When_Closed()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>")));

        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void SelectContent_Visible_With_DefaultIsOpen()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>"));
        });

        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
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

    // ── SelectTrigger ─────────────────────────────────────────────────────────

    [Fact]
    public void SelectTrigger_Renders_Button()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectTrigger>());

        Assert.NotEmpty(cut.FindAll("button"));
    }

    [Fact]
    public async Task SelectTrigger_Click_Opens_Listbox()
    {
        var cut = Render<Select>(p =>
        {
            p.AddChildContent(b =>
            {
                b.OpenComponent<SelectTrigger>(0);
                b.CloseComponent();
                b.OpenComponent<SelectContent>(1);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddContent(0, "items")));
                b.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=listbox]"));
        await cut.Find("button").ClickAsync(new());
        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void SelectTrigger_Custom_Class_Is_Appended()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectTrigger>(t => t.Add(x => x.Class, "my-trigger")));

        Assert.Contains("my-trigger", cut.Find("button").ClassName);
    }

    // ── SelectValue ───────────────────────────────────────────────────────────

    [Fact]
    public void SelectValue_Shows_Placeholder_When_No_Value()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Placeholder, "Pick a fruit");
            p.AddChildContent<SelectValue>();
        });

        Assert.Contains("Pick a fruit", cut.Find("span").TextContent);
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

    // ── SelectItem ────────────────────────────────────────────────────────────

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
    public void SelectItem_Click_Fires_ValueChanged()
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

    // ── Keyboard navigation ───────────────────────────────────────────────────

    [Fact]
    public async Task ArrowDown_On_Closed_Opens_And_Highlights_First_Item()
    {
        var cut = Render<Select>(p =>
        {
            p.AddChildContent(b =>
            {
                b.OpenComponent<SelectTrigger>(0);
                b.CloseComponent();
                b.OpenComponent<SelectContent>(1);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c =>
                {
                    c.OpenComponent<SelectItem>(0);
                    c.AddAttribute(1, "ItemValue", "apple");
                    c.AddAttribute(2, "ChildContent", (RenderFragment)(cc => cc.AddContent(0, "Apple")));
                    c.CloseComponent();
                }));
                b.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=listbox]"));
        await cut.Find("button").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });
        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
        Assert.Equal("true", cut.Find("[role=option]").GetAttribute("data-highlighted"));
    }

    [Fact]
    public async Task ArrowDown_Twice_Highlights_Second_Item()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent(b =>
            {
                b.OpenComponent<SelectTrigger>(0);
                b.CloseComponent();
                b.OpenComponent<SelectContent>(1);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c =>
                {
                    c.OpenComponent<SelectItem>(0);
                    c.AddAttribute(1, "ItemValue", "apple");
                    c.AddAttribute(2, "ChildContent", (RenderFragment)(cc => cc.AddContent(0, "Apple")));
                    c.CloseComponent();
                    c.OpenComponent<SelectItem>(3);
                    c.AddAttribute(4, "ItemValue", "banana");
                    c.AddAttribute(5, "ChildContent", (RenderFragment)(cc => cc.AddContent(0, "Banana")));
                    c.CloseComponent();
                }));
                b.CloseComponent();
            });
        });

        var button = cut.Find("button");
        await button.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });
        await button.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });

        var options = cut.FindAll("[role=option]");
        Assert.Equal("true", options[1].GetAttribute("data-highlighted"));
        Assert.Null(options[0].GetAttribute("data-highlighted"));
    }

    [Fact]
    public async Task Enter_On_Highlighted_Item_Fires_ValueChanged()
    {
        string? selected = null;
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => selected = v));
            p.AddChildContent(b =>
            {
                b.OpenComponent<SelectTrigger>(0);
                b.CloseComponent();
                b.OpenComponent<SelectContent>(1);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c =>
                {
                    c.OpenComponent<SelectItem>(0);
                    c.AddAttribute(1, "ItemValue", "apple");
                    c.AddAttribute(2, "ChildContent", (RenderFragment)(cc => cc.AddContent(0, "Apple")));
                    c.CloseComponent();
                }));
                b.CloseComponent();
            });
        });

        var trigger = cut.FindComponent<SelectTrigger>();
        await trigger.Find("button").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });
        await trigger.Find("button").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });
        Assert.Equal("apple", selected);
    }

    // ── SelectSeparator ───────────────────────────────────────────────────────

    [Fact]
    public void SelectSeparator_Has_Role_Separator()
    {
        var cut = Render<SelectSeparator>();
        Assert.Equal("separator", cut.Find("[role=separator]").GetAttribute("role"));
    }

    // ── SelectLabel ───────────────────────────────────────────────────────────

    [Fact]
    public void SelectLabel_Renders_ChildContent()
    {
        var cut = Render<SelectLabel>(p => p.AddChildContent("Fruits"));
        Assert.Contains("Fruits", cut.Markup);
    }

    // ── SelectGroup ───────────────────────────────────────────────────────────

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
}
