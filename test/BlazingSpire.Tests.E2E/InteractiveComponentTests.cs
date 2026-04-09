using Microsoft.Playwright;

namespace BlazingSpire.Tests.E2E;

// ── Dialog ────────────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the Dialog component at /components/dialog.
/// Covers: open, Escape close, click-outside close, close button, focus restoration.
/// </summary>
[Collection("BlazorApp")]
public class DialogE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Dialog_Opens_On_Trigger_Click()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dialog_Shows_Title_And_Description()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Expect(Page.GetByText("Edit profile")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Make changes to your profile here.")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dialog_Closes_On_Escape()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Dialog_Closes_Via_Close_Button()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        await dialog.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();

        await Expect(dialog).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Dialog_Closes_On_Click_Outside()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        // Click in the top-left corner, which lands on the backdrop outside the centered dialog
        await Page.Mouse.ClickAsync(5, 5);

        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Dialog_Focus_Moves_Into_Dialog_On_Open()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        // Focus trap moves focus to first focusable element inside dialog (the Close button)
        await Expect(dialog.GetByRole(AriaRole.Button, new() { Name = "Close" })).ToBeFocusedAsync();
    }

    [Fact]
    public async Task Dialog_Focus_Returns_To_Trigger_After_Close()
    {
        await NavigateAndWaitForBlazor("/components/dialog");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Edit Profile" });
        await trigger.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");
        await Expect(Page.GetByRole(AriaRole.Dialog)).ToBeHiddenAsync();

        // Focus should be restored to the element that opened the dialog
        await Expect(trigger).ToBeFocusedAsync();
    }
}

// ── Select ────────────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the Select component at /components/select.
/// Covers: open listbox, click item to select, close on Escape.
/// Note: Select does not implement keyboard Arrow navigation.
/// </summary>
[Collection("BlazorApp")]
public class SelectE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Select_Opens_Listbox_On_Trigger_Click()
    {
        await NavigateAndWaitForBlazor("/components/select");

        // Use the Framework select which has a visible placeholder
        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." });
        await trigger.ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Listbox)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Select_Shows_Options_When_Open()
    {
        await NavigateAndWaitForBlazor("/components/select");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." }).ClickAsync();

        var listbox = Page.GetByRole(AriaRole.Listbox);
        await Expect(listbox).ToBeVisibleAsync();
        await Expect(listbox.GetByRole(AriaRole.Option, new() { Name = "Blazor" })).ToBeVisibleAsync();
        await Expect(listbox.GetByRole(AriaRole.Option, new() { Name = "React" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Select_Item_Click_Closes_Listbox_And_Updates_Trigger()
    {
        await NavigateAndWaitForBlazor("/components/select");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." });
        await trigger.ClickAsync();

        var listbox = Page.GetByRole(AriaRole.Listbox);
        await Expect(listbox).ToBeVisibleAsync();

        await listbox.GetByRole(AriaRole.Option, new() { Name = "Blazor" }).ClickAsync();

        // Listbox closes after selection
        await Expect(listbox).ToBeHiddenAsync();

        // Trigger text updates to selected value
        await Expect(trigger).ToContainTextAsync("Blazor");
    }

    [Fact]
    public async Task Select_Closes_On_Click_Outside()
    {
        await NavigateAndWaitForBlazor("/components/select");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listbox)).ToBeVisibleAsync();

        await Page.Mouse.ClickAsync(5, 5);

        await Expect(Page.GetByRole(AriaRole.Listbox)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Select_Fruit_Shows_Apple_Option()
    {
        await NavigateAndWaitForBlazor("/components/select");

        // First select on the page (Fruit section) — trigger has no placeholder, click by position
        var fruitSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Fruit" }) });
        await fruitSection.GetByRole(AriaRole.Button).ClickAsync();

        var listbox = Page.GetByRole(AriaRole.Listbox);
        await Expect(listbox).ToBeVisibleAsync();
        await Expect(listbox.GetByRole(AriaRole.Option, new() { Name = "Apple" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Select_ArrowDown_Opens_And_Highlights_First_Option()
    {
        await NavigateAndWaitForBlazor("/components/select");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." });
        await trigger.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listbox)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("ArrowDown");

        var listbox = Page.GetByRole(AriaRole.Listbox);
        var firstOption = listbox.GetByRole(AriaRole.Option).First;
        await Expect(firstOption).ToHaveAttributeAsync("data-highlighted", "true");
    }

    [Fact]
    public async Task Select_ArrowDown_Twice_Highlights_Second_Option()
    {
        await NavigateAndWaitForBlazor("/components/select");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." });
        await trigger.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listbox)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.Keyboard.PressAsync("ArrowDown");

        var listbox = Page.GetByRole(AriaRole.Listbox);
        var secondOption = listbox.GetByRole(AriaRole.Option).Nth(1);
        await Expect(secondOption).ToHaveAttributeAsync("data-highlighted", "true");
    }

    [Fact]
    public async Task Select_Enter_Selects_Highlighted_Option()
    {
        await NavigateAndWaitForBlazor("/components/select");

        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Select a framework..." });
        await trigger.ClickAsync();
        var listbox = Page.GetByRole(AriaRole.Listbox);
        await Expect(listbox).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.Keyboard.PressAsync("Enter");

        // Listbox closes after selection
        await Expect(listbox).ToBeHiddenAsync();
        // Trigger text updates to the first option's value
        await Expect(trigger).Not.ToContainTextAsync("Select a framework...");
    }
}

// ── Accordion ─────────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the Accordion component at /components/accordion.
/// Uses native details/summary elements — expand/collapse checked via content visibility.
/// </summary>
[Collection("BlazorApp")]
public class AccordionE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Accordion_Items_Are_Collapsed_By_Default()
    {
        await NavigateAndWaitForBlazor("/components/accordion");

        // Native <details> hides content until open; Playwright's ToBeHiddenAsync checks this
        await Expect(Page.GetByText("Yes. It uses native HTML details/summary elements.")).ToBeHiddenAsync();
        await Expect(Page.GetByText("Yes. It comes with default Tailwind styles.")).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Accordion_Click_Expands_Item()
    {
        await NavigateAndWaitForBlazor("/components/accordion");

        await Page.GetByText("Is it accessible?").ClickAsync();

        // Content inside the <details> becomes visible when open
        await Expect(Page.GetByText("Yes. It uses native HTML details/summary elements.")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Accordion_Click_Again_Collapses_Item()
    {
        await NavigateAndWaitForBlazor("/components/accordion");

        var trigger = Page.GetByText("Is it accessible?");

        // Open
        await trigger.ClickAsync();
        await Expect(Page.GetByText("Yes. It uses native HTML details/summary elements.")).ToBeVisibleAsync();

        // Close
        await trigger.ClickAsync();
        await Expect(Page.GetByText("Yes. It uses native HTML details/summary elements.")).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Accordion_Multiple_Items_Can_Be_Expanded()
    {
        await NavigateAndWaitForBlazor("/components/accordion");

        await Page.GetByText("Is it accessible?").ClickAsync();
        await Page.GetByText("Is it styled?").ClickAsync();

        await Expect(Page.GetByText("Yes. It uses native HTML details/summary elements.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Yes. It comes with default Tailwind styles.")).ToBeVisibleAsync();
    }
}

// ── Tabs ──────────────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the Tabs component at /components/tabs.
/// Covers: default active tab, switching panels by click.
/// </summary>
[Collection("BlazorApp")]
public class TabsE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Tabs_First_Tab_Is_Active_By_Default()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        var accountTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Account" });
        await Expect(accountTab).ToHaveAttributeAsync("aria-selected", "true");
    }

    [Fact]
    public async Task Tabs_Second_Tab_Is_Inactive_By_Default()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        var passwordTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Password" });
        await Expect(passwordTab).ToHaveAttributeAsync("aria-selected", "false");
    }

    [Fact]
    public async Task Tabs_First_Panel_Is_Visible_By_Default()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        var panel = Page.GetByRole(AriaRole.Tabpanel);
        await Expect(panel).ToBeVisibleAsync();
        // Account panel contains "Make changes to your account here."
        await Expect(panel.GetByText("Make changes to your account here.")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tabs_Click_Second_Tab_Shows_Second_Panel()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        // Click the Password tab
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Password" }).ClickAsync();

        // Password tab becomes selected
        await Expect(Page.GetByRole(AriaRole.Tab, new() { Name = "Password" }))
            .ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.GetByRole(AriaRole.Tab, new() { Name = "Account" }))
            .ToHaveAttributeAsync("aria-selected", "false");

        // Password panel is now visible
        var panel = Page.GetByRole(AriaRole.Tabpanel);
        await Expect(panel.GetByText("Change your password here.")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tabs_Clicking_Back_Restores_First_Panel()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        await Page.GetByRole(AriaRole.Tab, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Account" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Tab, new() { Name = "Account" }))
            .ToHaveAttributeAsync("aria-selected", "true");
        await Expect(Page.GetByRole(AriaRole.Tabpanel).GetByText("Make changes to your account here."))
            .ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tabs_Active_Tab_Has_Tabindex_Zero()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        var accountTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Account" });
        await Expect(accountTab).ToHaveAttributeAsync("tabindex", "0");
    }

    [Fact]
    public async Task Tabs_ArrowRight_Moves_To_Next_Tab()
    {
        await NavigateAndWaitForBlazor("/components/tabs");

        // Focus the first (active) tab and press ArrowRight
        var accountTab = Page.GetByRole(AriaRole.Tab, new() { Name = "Account" });
        await accountTab.FocusAsync();
        await Page.Keyboard.PressAsync("ArrowRight");

        // Second tab should now be active
        await Expect(Page.GetByRole(AriaRole.Tab, new() { Name = "Password" }))
            .ToHaveAttributeAsync("aria-selected", "true");
        await Expect(accountTab).ToHaveAttributeAsync("aria-selected", "false");
    }
}

// ── Tooltip ───────────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the Tooltip component at /components/tooltip.
/// Covers: hover shows tooltip, mouse out hides it.
/// </summary>
[Collection("BlazorApp")]
public class TooltipE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task Tooltip_Hidden_By_Default()
    {
        await NavigateAndWaitForBlazor("/components/tooltip");

        await Expect(Page.GetByRole(AriaRole.Tooltip)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Tooltip_Hover_Shows_Content()
    {
        await NavigateAndWaitForBlazor("/components/tooltip");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Hover me" }).HoverAsync();

        await Expect(Page.GetByRole(AriaRole.Tooltip)).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Tooltip)).ToContainTextAsync("Add to library");
    }

    [Fact]
    public async Task Tooltip_Mouse_Out_Hides_Content()
    {
        await NavigateAndWaitForBlazor("/components/tooltip");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        var button = basicSection.GetByRole(AriaRole.Button, new() { Name = "Hover me" });
        await button.HoverAsync();
        await Expect(Page.GetByRole(AriaRole.Tooltip)).ToBeVisibleAsync();

        // Move mouse to a neutral position (top-left corner away from any component)
        await Page.Mouse.MoveAsync(5, 5);

        await Expect(Page.GetByRole(AriaRole.Tooltip)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Tooltip_Side_Variants_All_Render_Triggers()
    {
        await NavigateAndWaitForBlazor("/components/tooltip");

        var sidesSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Sides" }) });

        await Expect(sidesSection.GetByRole(AriaRole.Button, new() { Name = "Top" })).ToBeVisibleAsync();
        await Expect(sidesSection.GetByRole(AriaRole.Button, new() { Name = "Bottom" })).ToBeVisibleAsync();
        await Expect(sidesSection.GetByRole(AriaRole.Button, new() { Name = "Left" })).ToBeVisibleAsync();
        await Expect(sidesSection.GetByRole(AriaRole.Button, new() { Name = "Right" })).ToBeVisibleAsync();
    }
}

// ── Dropdown Menu ─────────────────────────────────────────────────────────────

/// <summary>
/// E2E tests for the DropdownMenu component at /components/dropdown-menu.
/// Covers: open on trigger click, Escape to close, item click to close.
/// </summary>
[Collection("BlazorApp")]
public class DropdownMenuE2ETests : BlazingSpireE2EBase
{
    [Fact]
    public async Task DropdownMenu_Opens_On_Trigger_Click()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Menu)).ToBeVisibleAsync();
    }

    [Fact]
    public async Task DropdownMenu_Shows_Expected_Items()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();

        var menu = Page.GetByRole(AriaRole.Menu).First;
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Profile" })).ToBeVisibleAsync();
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Billing" })).ToBeVisibleAsync();
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Settings" })).ToBeVisibleAsync();
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Log out" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task DropdownMenu_Escape_Closes_Menu()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Menu)).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync("Escape");

        await Expect(Page.GetByRole(AriaRole.Menu)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task DropdownMenu_Item_Click_Closes_Menu()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        var menu = Page.GetByRole(AriaRole.Menu).First;
        await Expect(menu).ToBeVisibleAsync();

        await menu.GetByRole(AriaRole.Menuitem, new() { Name = "Profile" }).ClickAsync();

        await Expect(menu).ToBeHiddenAsync();
    }

    [Fact]
    public async Task DropdownMenu_Click_Outside_Closes_Menu()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var basicSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "Basic" }) });

        await basicSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Menu)).ToBeVisibleAsync();

        await Page.Mouse.ClickAsync(5, 5);

        await Expect(Page.GetByRole(AriaRole.Menu)).ToBeHiddenAsync();
    }

    [Fact]
    public async Task DropdownMenu_Disabled_Item_Is_Present()
    {
        await NavigateAndWaitForBlazor("/components/dropdown-menu");

        var disabledSection = Page.Locator("section").Filter(
            new() { Has = Page.GetByRole(AriaRole.Heading, new() { Level = 2, Name = "With Disabled Item" }) });

        await disabledSection.GetByRole(AriaRole.Button, new() { Name = "Open Menu" }).ClickAsync();

        var menu = Page.GetByRole(AriaRole.Menu).Last;
        await Expect(menu.GetByRole(AriaRole.Menuitem, new() { Name = "Billing (disabled)" })).ToBeVisibleAsync();
    }
}
