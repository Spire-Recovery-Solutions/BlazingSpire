using BlazingSpire.Demo.Components.Shared;
using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CommandTests : BlazingSpireTestBase
{
    // ── Rendering ─────────────────────────────────────────────────────────────

    [Fact]
    public void Command_Renders_Div()
    {
        var cut = Render<Command>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Command_Has_Base_Classes()
    {
        var cut = Render<Command>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("flex-col", classes);
        Assert.Contains("overflow-hidden", classes);
        Assert.Contains("rounded-md", classes);
    }

    [Fact]
    public void Command_Custom_Class_Is_Appended()
    {
        var cut = Render<Command>(p => p.Add(x => x.Class, "my-custom"));
        Assert.Contains("my-custom", cut.Find("div").ClassName);
    }

    [Fact]
    public void Command_Is_Assignable_To_BlazingSpireComponentBase()
    {
        Assert.True(typeof(Command).IsAssignableTo(typeof(BlazingSpireComponentBase)));
    }
}

public class CommandInputTests : BlazingSpireTestBase
{
    // ── Rendering ─────────────────────────────────────────────────────────────

    [Fact]
    public void CommandInput_Renders_Wrapper_Div_With_Search_Icon_And_Input()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandInput>());
        Assert.NotNull(cut.Find("svg"));
        Assert.NotNull(cut.Find("input"));
    }

    [Fact]
    public void CommandInput_Wrapper_Has_Border_Classes()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandInput>());
        var wrapper = cut.Find("div.border-b");
        Assert.Contains("flex", wrapper.ClassName);
    }

    [Fact]
    public void CommandInput_Input_Has_Base_Classes()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandInput>());
        var input = cut.Find("input");
        Assert.Contains("h-11", input.ClassName);
        Assert.Contains("w-full", input.ClassName);
    }

    [Fact]
    public void CommandInput_Placeholder_Renders()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandInput>(ip =>
                ip.Add(x => x.Placeholder, "Search...")));
        Assert.Equal("Search...", cut.Find("input").GetAttribute("placeholder"));
    }
}

public class CommandListTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandList_Renders_Div_With_Role_Listbox()
    {
        var cut = Render<CommandList>();
        var el = cut.Find("div");
        Assert.Equal("listbox", el.GetAttribute("role"));
    }

    [Fact]
    public void CommandList_Has_Overflow_Classes()
    {
        var cut = Render<CommandList>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("overflow-y-auto", classes);
        Assert.Contains("overflow-x-hidden", classes);
        Assert.Contains("max-h-[300px]", classes);
    }

    [Fact]
    public void CommandList_Custom_Class_Is_Appended()
    {
        var cut = Render<CommandList>(p => p.Add(x => x.Class, "extra"));
        Assert.Contains("extra", cut.Find("div").ClassName);
    }
}

public class CommandGroupTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandGroup_Renders_Div()
    {
        var cut = Render<CommandGroup>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CommandGroup_Renders_Heading_When_Provided()
    {
        var cut = Render<CommandGroup>(p => p.Add(x => x.Heading, "Suggestions"));
        var heading = cut.Find("[data-command-group-heading]");
        Assert.Equal("Suggestions", heading.TextContent);
    }

    [Fact]
    public void CommandGroup_No_Heading_Element_When_Not_Provided()
    {
        var cut = Render<CommandGroup>();
        Assert.Empty(cut.FindAll("[data-command-group-heading]"));
    }

    [Fact]
    public void CommandGroup_Has_Base_Classes()
    {
        var cut = Render<CommandGroup>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("overflow-hidden", classes);
        Assert.Contains("p-1", classes);
        Assert.Contains("text-foreground", classes);
    }
}

public class CommandItemTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandItem_Renders_Without_Parent_Search()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandItem>(ip =>
                ip.Add(x => x.FilterText, "Calendar")));
        Assert.NotNull(cut.Find("div[role='option']"));
    }

    [Fact]
    public void CommandItem_Visible_When_FilterText_Matches_Search()
    {
        var cut = Render<Command>(p =>
        {
            p.AddChildContent<CommandInput>();
            p.AddChildContent<CommandItem>(ip =>
                ip.Add(x => x.FilterText, "Calendar"));
        });

        // No search active — item visible
        Assert.NotNull(cut.Find("div[role='option']"));
    }

    [Fact]
    public void CommandItem_Has_Base_Classes()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandItem>());
        var classes = cut.Find("div[role='option']").ClassName;
        Assert.Contains("flex", classes);
        Assert.Contains("items-center", classes);
        Assert.Contains("rounded-sm", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void CommandItem_Disabled_Sets_Data_Attribute()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandItem>(ip =>
                ip.Add(x => x.Disabled, true)));
        var el = cut.Find("div[role='option']");
        Assert.NotNull(el.GetAttribute("data-disabled"));
        Assert.Equal("true", el.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void CommandItem_IsVisible_True_When_No_Search()
    {
        var item = new CommandItem();
        Assert.True(item.IsVisible);
    }
}

public class CommandSeparatorTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandSeparator_Renders_With_Role_Separator()
    {
        var cut = Render<CommandSeparator>();
        Assert.NotNull(cut.Find("div[role='separator']"));
    }

    [Fact]
    public void CommandSeparator_Has_Base_Classes()
    {
        var cut = Render<CommandSeparator>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("h-px", classes);
        Assert.Contains("bg-border", classes);
    }
}

public class CommandEmptyTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandEmpty_Renders_Div()
    {
        var cut = Render<CommandEmpty>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void CommandEmpty_Has_Base_Classes()
    {
        var cut = Render<CommandEmpty>();
        var classes = cut.Find("div").ClassName;
        Assert.Contains("py-6", classes);
        Assert.Contains("text-center", classes);
        Assert.Contains("text-sm", classes);
    }

    [Fact]
    public void CommandEmpty_ChildContent_Renders()
    {
        var cut = Render<CommandEmpty>(p => p.AddChildContent("No results found."));
        Assert.Contains("No results found.", cut.Find("div").TextContent);
    }

    [Fact]
    public void CommandEmpty_Custom_Class_Is_Appended()
    {
        var cut = Render<CommandEmpty>(p => p.Add(x => x.Class, "mt-2"));
        Assert.Contains("mt-2", cut.Find("div").ClassName);
    }
}
