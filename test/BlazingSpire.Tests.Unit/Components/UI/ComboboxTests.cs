using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class ComboboxTests : BlazingSpireTestBase
{
    // ── ComboboxContent visibility ────────────────────────────────────────────

    [Fact]
    public void ComboboxContent_Hidden_When_Closed()
    {
        var cut = Render<Combobox>(p =>
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent("<p>body</p>")));

        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void ComboboxContent_Visible_With_DefaultIsOpen()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void ComboboxContent_Has_Role_Listbox()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent("<p>body</p>"));
        });

        Assert.Equal("listbox", cut.Find("[data-state]").GetAttribute("role"));
    }

    // ── ComboboxTrigger ───────────────────────────────────────────────────────

    [Fact]
    public void ComboboxTrigger_Renders_Button()
    {
        var cut = Render<Combobox>(p =>
            p.AddChildContent<ComboboxTrigger>());

        Assert.NotEmpty(cut.FindAll("button"));
    }

    [Fact]
    public async Task ComboboxTrigger_Click_Opens_Combobox()
    {
        var cut = Render<Combobox>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxTrigger>(0);
                builder.CloseComponent();

                builder.OpenComponent<ComboboxContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
                    b.AddContent(0, "items")));
                builder.CloseComponent();
            });
        });

        Assert.Empty(cut.FindAll("[role=listbox]"));
        await cut.Find("button").ClickAsync(new());
        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
    }

    // ── ComboboxInput ─────────────────────────────────────────────────────────

    [Fact]
    public void ComboboxInput_Renders_Search_Input_When_Open()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxInput>());
        });

        Assert.NotEmpty(cut.FindAll("input"));
    }

    // ── ComboboxItem ──────────────────────────────────────────────────────────

    [Fact]
    public void ComboboxItem_Has_Role_Option()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.Add(x => x.FilterText, "Next.js");
                    ip.AddChildContent("Next.js");
                }));
        });

        Assert.NotEmpty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public void ComboboxItem_AriaSelected_False_By_Default()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.AddChildContent("Next.js");
                }));
        });

        Assert.Equal("false", cut.Find("[role=option]").GetAttribute("aria-selected"));
    }

    [Fact]
    public void ComboboxItem_Selected_Has_AriaSelected_True()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.Value, "next");
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.AddChildContent("Next.js");
                }));
        });

        Assert.Equal("true", cut.Find("[role=option]").GetAttribute("aria-selected"));
    }

    [Fact]
    public async Task ComboboxItem_Click_Fires_ValueChanged()
    {
        string? selectedValue = null;

        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => selectedValue = v));
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.Add(x => x.FilterText, "Next.js");
                    ip.AddChildContent("Next.js");
                }));
        });

        await cut.Find("[role=option]").ClickAsync(new());
        Assert.Equal("next", selectedValue);
    }

    [Fact]
    public async Task ComboboxItem_Click_Closes_Combobox()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<ComboboxItem>(0);
                    b.AddAttribute(1, "ItemValue", "next");
                    b.AddAttribute(2, "FilterText", "Next.js");
                    b.AddAttribute(3, "ChildContent", (RenderFragment)(cb => cb.AddContent(0, "Next.js")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });

        Assert.NotEmpty(cut.FindAll("[role=listbox]"));
        await cut.Find("[role=option]").ClickAsync(new());
        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void ComboboxItem_Disabled_Has_Data_Disabled()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.Add(x => x.Disabled, true);
                    ip.AddChildContent("Next.js");
                }));
        });

        Assert.Equal("true", cut.Find("[role=option]").GetAttribute("data-disabled"));
    }

    [Fact]
    public async Task ComboboxItem_Filtered_Out_When_Search_Does_Not_Match()
    {
        var cut = Render<Combobox>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.Value, "");
            p.AddChildContent<ComboboxContent>(cp =>
                cp.AddChildContent<ComboboxItem>(ip =>
                {
                    ip.Add(x => x.ItemValue, "next");
                    ip.Add(x => x.FilterText, "Next.js");
                    ip.AddChildContent("Next.js");
                }));
        });

        await cut.InvokeAsync(() => cut.Instance.UpdateSearch("svelte"));
        Assert.Empty(cut.FindAll("[role=option]"));
    }

    // ── ComboboxEmpty ─────────────────────────────────────────────────────────

    [Fact]
    public void ComboboxEmpty_Renders_ChildContent()
    {
        var cut = Render<ComboboxEmpty>(p =>
            p.AddChildContent("No results found."));

        Assert.Contains("No results found.", cut.Markup);
    }
}
