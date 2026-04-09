using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CommandTests : BlazingSpireTestBase
{
    [Fact]
    public void Command_Renders_Without_Error()
    {
        var cut = Render<Command>();
        Assert.NotNull(cut.Find("div"));
    }

    [Fact]
    public void Command_Custom_Class_Is_Appended()
    {
        var cut = Render<Command>(p => p.Add(x => x.Class, "my-custom"));
        Assert.Contains("my-custom", cut.Find("div").ClassName);
    }
}

public class CommandInputTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandInput_Renders_Input()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandInput>());
        Assert.NotNull(cut.Find("input"));
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
    public void CommandList_Renders_With_Role_Listbox()
    {
        var cut = Render<CommandList>();
        Assert.Equal("listbox", cut.Find("div").GetAttribute("role"));
    }
}

public class CommandGroupTests : BlazingSpireTestBase
{
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
}

public class CommandItemTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandItem_Renders_With_Role_Option()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandItem>(ip =>
                ip.Add(x => x.FilterText, "Calendar")));
        Assert.NotNull(cut.Find("div[role='option']"));
    }

    [Fact]
    public void CommandItem_Renders_ChildContent()
    {
        var cut = Render<Command>(p =>
            p.AddChildContent<CommandItem>(ip =>
            {
                ip.Add(x => x.FilterText, "Calendar");
                ip.AddChildContent("Calendar");
            }));
        Assert.Contains("Calendar", cut.Find("div[role='option']").TextContent);
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

        Assert.NotNull(cut.Find("div[role='option']"));
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
}

public class CommandSeparatorTests : BlazingSpireTestBase
{
    [Fact]
    public void CommandSeparator_Renders_With_Role_Separator()
    {
        var cut = Render<CommandSeparator>();
        Assert.NotNull(cut.Find("div[role='separator']"));
    }
}

public class CommandEmptyTests : BlazingSpireTestBase
{
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
