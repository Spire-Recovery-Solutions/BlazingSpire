# Behavioral Test Standards

Reference for test-writer agents rewriting BlazingSpire component tests. Each category defines what to test, what to delete, and includes a concrete bUnit example.

## The Problem With Current Tests

Current tests follow a shallow pattern:
- "Does it render a `<div>`?" — proves nothing about behavior
- "Does it have CSS class `flex`?" — breaks on any styling change, tests implementation not behavior
- "Is it assignable to base class?" — compile-time check disguised as a test
- "Does `AdditionalAttributes` pass through?" — already tested by the base class

These tests pass even when components are completely broken because they never test what the component *does*.

## Global Rules

### DELETE These Test Patterns

| Pattern | Why |
|---------|-----|
| `Assert.Contains("flex", classes)` | Individual CSS class presence is implementation detail |
| `Assert.Contains("text-sm", classes)` | Styling changes should not break tests |
| `typeof(X).IsAssignableTo(typeof(Y))` | Compile-time guarantee, not a runtime behavior |
| `Assert.NotNull(cut.Find("div"))` | Rendering *an element* proves nothing |
| `cut.Find("summary svg")` / `cut.Find("svg path")` | Icon implementation detail |
| Base class reflection tests (`GetProperty(BindingFlags.NonPublic)`) | Testing internals, not behavior |
| `_Custom_Class_Is_Appended` (when identical to base class test) | Already covered by `BlazingSpireComponentBase` tests |
| `_AdditionalAttributes_PassThrough` (when identical to base class test) | Already covered by `BlazingSpireComponentBase` tests |
| `_Has_Base_Classes` | Exact CSS strings are not behavioral contracts |

### KEEP These Test Patterns

| Pattern | Why |
|---------|-----|
| `AssertRole(el, "dialog")` | ARIA role is a behavioral contract |
| `AssertDataState(el, "open")` | `data-state` drives CSS animations and is part of the API |
| `AssertAriaExpanded(el, true)` | Assistive technology contract |
| `Assert.Equal(labelledBy, titleId)` | ARIA linkage between parts |
| `cut.Find("[role=dialog]")` / `Assert.Empty(cut.FindAll("[role=dialog]"))` | Testing visibility via semantic queries |
| `EventCallback` firing assertions | Core behavioral contract |
| Click/keyboard → state change assertions | Functional behavior |
| `data-disabled`, `data-highlighted`, `aria-selected` | State attributes that consumers and assistive tech rely on |

### Guiding Principle

**Test what the component does, not how it looks.** A test should fail when behavior breaks, not when someone changes `py-4` to `py-3`.

Query elements by role/ARIA attributes (`[role=dialog]`, `[role=option]`), not by CSS class or tag name when possible.

---

## Category 1: Static/Display Components

**Components:** Button, Badge, Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter, Alert, AlertTitle, AlertDescription, Avatar, AvatarImage, AvatarFallback, Separator, Skeleton, Label, Breadcrumb, AspectRatio, Table, Progress

### What to Test

1. **Correct HTML element** — Button renders `<button>`, Label renders `<label>`, Separator renders with `role="separator"`
2. **Variants via `[Theory]`** — one test covers all variants, asserts the element has *a* class from the variant map (not specific utility classes)
3. **Disabled state** — `disabled` attribute present, `aria-disabled` if applicable
4. **ARIA attributes** — `role`, `aria-label`, `aria-*` that the component sets
5. **Semantic child content** — ChildContent renders inside the correct element
6. **Link mode** — Button with `Href` renders `<a>` instead of `<button>`
7. **Loading state** — Button with `Loading=true` is effectively disabled
8. **Custom Class merges** — ONE test per component (not per sub-component), confirming `Class` parameter is present in output

### What to Delete

- Individual CSS class assertions (`Assert.Contains("bg-primary", ...)`)
- Multiple `_Has_Base_Classes` tests per sub-component
- `_AdditionalAttributes_PassThrough` for every sub-component (test once in base class tests)
- `_Is_Assignable_To_*` tests

### Example

```csharp
public class ButtonTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Button_Element_With_Type_Button()
    {
        var cut = Render<Button>(p => p.Add(x => x.ChildContent, "Save"));
        var btn = cut.Find("button");
        Assert.Equal("button", btn.GetAttribute("type"));
        Assert.Equal("Save", btn.TextContent.Trim());
    }

    [Theory]
    [InlineData(ButtonVariant.Default)]
    [InlineData(ButtonVariant.Destructive)]
    [InlineData(ButtonVariant.Outline)]
    [InlineData(ButtonVariant.Secondary)]
    [InlineData(ButtonVariant.Ghost)]
    [InlineData(ButtonVariant.Link)]
    public void Each_Variant_Renders_Without_Error(ButtonVariant variant)
    {
        var cut = Render<Button>(p => p.Add(x => x.Variant, variant));
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void Disabled_Button_Has_Disabled_Attribute()
    {
        var cut = Render<Button>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut.Find("button[disabled]"));
    }

    [Fact]
    public void Loading_Button_Is_Effectively_Disabled()
    {
        var cut = Render<Button>(p => p.Add(x => x.Loading, true));
        Assert.NotNull(cut.Find("button[disabled]"));
    }

    [Fact]
    public void With_Href_Renders_Anchor()
    {
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.Href, "/about");
            p.Add(x => x.ChildContent, "About");
        });
        var anchor = cut.Find("a");
        Assert.Equal("/about", anchor.GetAttribute("href"));
    }

    [Fact]
    public void Click_Fires_OnClick()
    {
        bool clicked = false;
        var cut = Render<Button>(p =>
        {
            p.Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true));
            p.Add(x => x.ChildContent, "Go");
        });
        cut.Find("button").Click();
        Assert.True(clicked);
    }

    [Fact]
    public void Custom_Class_Is_Included()
    {
        var cut = Render<Button>(p => p.Add(x => x.Class, "my-btn"));
        Assert.Contains("my-btn", cut.Find("button").ClassName);
    }
}
```

---

## Category 2: Form Input Components

**Components:** Input, Textarea, Checkbox, RadioGroup, Switch, Slider, InputOTP

### What to Test

1. **Value binding** — Setting `Value` parameter reflects in the rendered element
2. **ValueChanged fires** — User interaction (change event) triggers `ValueChanged` callback with correct value
3. **Disabled prevents change** — `disabled` attribute present, interaction does not fire `ValueChanged`
4. **Required attribute** — `required` present when `Required=true`
5. **ReadOnly attribute** — `readonly` present when `ReadOnly=true`
6. **Placeholder** — `placeholder` attribute set correctly
7. **ARIA attributes** — `aria-invalid` when in validation error state, `aria-required`
8. **Two-way binding round-trip** — Change fires callback, re-render with new value shows it

### What to Delete

- CSS class presence tests on the input element
- `_Has_Base_Classes` tests
- Tag name tests for obvious elements (`<input>` renders an input)

### Example

```csharp
public class InputTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Input_With_Type()
    {
        var cut = Render<Input>(p => p.Add(x => x.Type, "email"));
        Assert.Equal("email", cut.Find("input").GetAttribute("type"));
    }

    [Fact]
    public void Placeholder_Is_Set()
    {
        var cut = Render<Input>(p => p.Add(x => x.Placeholder, "Enter email"));
        Assert.Equal("Enter email", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Disabled_Renders_Disabled_Attribute()
    {
        var cut = Render<Input>(p => p.Add(x => x.Disabled, true));
        Assert.NotNull(cut.Find("input[disabled]"));
    }
}

public class CheckboxTests : BlazingSpireTestBase
{
    [Fact]
    public void Checked_State_Reflects_Value()
    {
        var cut = Render<Checkbox>(p => p.Add(x => x.Value, true));
        AssertAriaChecked(cut.Find("[role=checkbox]"), true);
    }

    [Fact]
    public void Click_Fires_ValueChanged()
    {
        bool? received = null;
        var cut = Render<Checkbox>(p =>
        {
            p.Add(x => x.Value, false);
            p.Add(x => x.ValueChanged,
                EventCallback.Factory.Create<bool>(this, v => received = v));
        });

        cut.Find("[role=checkbox]").Click();
        Assert.True(received);
    }

    [Fact]
    public void Disabled_Click_Does_Not_Fire_ValueChanged()
    {
        bool fired = false;
        var cut = Render<Checkbox>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.ValueChanged,
                EventCallback.Factory.Create<bool>(this, _ => fired = true));
        });

        // Disabled elements suppress click in bUnit
        var el = cut.Find("[role=checkbox]");
        Assert.NotNull(el.GetAttribute("disabled") ?? el.GetAttribute("data-disabled"));
    }
}
```

---

## Category 3: Disclosure Components

**Components:** Accordion (+ AccordionItem, AccordionTrigger, AccordionContent), Collapsible, Toggle, ToggleGroup

### What to Test

1. **Click trigger toggles content visibility** — Content hidden by default, visible after click
2. **`data-state` reflects open/closed** — `data-state="open"` when expanded, `"closed"` when collapsed
3. **`DefaultIsOpen` starts expanded** — Content visible on first render
4. **Controlled mode** — `IsOpen` + `IsOpenChanged` binding works
5. **`IsOpenChanged` fires** — Callback invoked with correct value on toggle
6. **Native `<details>` behavior** (Accordion) — The `<details>` element has correct structure
7. **Multiple items** — Only clicked item toggles (Accordion)

### What to Delete

- CSS class assertions on trigger/content elements
- SVG/chevron icon presence tests
- Individual base class tests for every sub-component

### Example

```csharp
public class AccordionTests : BlazingSpireTestBase
{
    private IRenderedComponent<Accordion> RenderAccordion()
    {
        return Render<Accordion>(p =>
            p.AddChildContent<AccordionItem>(item =>
            {
                item.AddChildContent(builder =>
                {
                    builder.OpenComponent<AccordionTrigger>(0);
                    builder.AddAttribute(1, "ChildContent",
                        (RenderFragment)(b => b.AddContent(0, "Question")));
                    builder.CloseComponent();

                    builder.OpenComponent<AccordionContent>(2);
                    builder.AddAttribute(3, "ChildContent",
                        (RenderFragment)(b => b.AddContent(0, "Answer")));
                    builder.CloseComponent();
                });
            }));
    }

    [Fact]
    public void AccordionItem_Renders_As_Details_Element()
    {
        var cut = RenderAccordion();
        Assert.NotNull(cut.Find("details"));
    }

    [Fact]
    public void AccordionTrigger_Renders_As_Summary()
    {
        var cut = RenderAccordion();
        var summary = cut.Find("summary");
        Assert.Contains("Question", summary.TextContent);
    }

    [Fact]
    public void AccordionContent_Renders_Inside_Details()
    {
        var cut = RenderAccordion();
        Assert.Contains("Answer", cut.Find("details").TextContent);
    }

    [Fact]
    public void Custom_Class_Merges_On_Accordion()
    {
        var cut = Render<Accordion>(p =>
        {
            p.Add(x => x.Class, "my-accordion");
            p.AddChildContent("<div>item</div>");
        });
        Assert.Contains("my-accordion", cut.Find("div").ClassName);
    }
}
```

**Note:** Accordion currently uses native `<details>/<summary>` which the browser handles natively. The open/close toggle is browser-managed, so bUnit cannot simulate it via click on `<summary>`. Test the structural correctness. For interactive accordion behavior (controlled state, `data-state` attributes), test components that extend `DisclosureBase`:

```csharp
public class CollapsibleTests : BlazingSpireTestBase
{
    [Fact]
    public void Content_Hidden_By_Default()
    {
        var cut = Render<Collapsible>(p =>
            p.AddChildContent<CollapsibleContent>(cp =>
                cp.AddChildContent("Hidden content")));

        Assert.Empty(cut.FindAll("[data-state=open]"));
    }

    [Fact]
    public void DefaultIsOpen_Shows_Content()
    {
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<CollapsibleContent>(cp =>
                cp.AddChildContent("Visible content"));
        });

        AssertDataState(cut.Find("[data-state]"), "open");
    }

    [Fact]
    public void Trigger_Click_Opens_Content()
    {
        var cut = Render<Collapsible>(p =>
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Toggle")));
                builder.CloseComponent();
                builder.OpenComponent<CollapsibleContent>(2);
                builder.AddAttribute(3, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Body")));
                builder.CloseComponent();
            }));

        cut.Find("button").Click();
        AssertDataState(cut.Find("[data-state]"), "open");
    }

    [Fact]
    public void Controlled_Mode_Fires_IsOpenChanged()
    {
        bool? received = null;
        var cut = Render<Collapsible>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.Add(x => x.IsOpenChanged,
                EventCallback.Factory.Create<bool>(this, v => received = v));
            p.AddChildContent<CollapsibleTrigger>(tp =>
                tp.AddChildContent("Toggle"));
        });

        cut.Find("button").Click();
        Assert.True(received);
    }
}
```

---

## Category 4: Tabs

**Components:** Tabs, TabsList, TabsTrigger, TabsContent

### What to Test

1. **Default tab is active** — `DefaultValue` determines which panel is visible
2. **Clicking a trigger switches panels** — Previous panel hidden, new panel shown
3. **`aria-selected`** — Active trigger has `aria-selected="true"`, others `"false"`
4. **`aria-controls` / `aria-labelledby` wiring** — Trigger's `aria-controls` matches panel's `id`, panel's `aria-labelledby` matches trigger's `id`
5. **`data-state`** — Active trigger/panel has `data-state="active"`, inactive has `"inactive"`
6. **`role` attributes** — `tablist`, `tab`, `tabpanel` present
7. **Disabled trigger** — Cannot be activated
8. **Controlled mode** — `Value` + `ValueChanged` binding works

### What to Delete

- CSS class tests on tab triggers/panels
- Layout/spacing class assertions

### Example

```csharp
public class TabsTests : BlazingSpireTestBase
{
    private IRenderedComponent<Tabs> RenderTabs(string? defaultValue = "tab1")
    {
        return Render<Tabs>(p =>
        {
            p.Add(x => x.DefaultValue, defaultValue);
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<TabsTrigger>(0);
                    b.AddAttribute(1, "Value", "tab1");
                    b.AddAttribute(2, "ChildContent",
                        (RenderFragment)(c => c.AddContent(0, "Tab 1")));
                    b.CloseComponent();

                    b.OpenComponent<TabsTrigger>(3);
                    b.AddAttribute(4, "Value", "tab2");
                    b.AddAttribute(5, "ChildContent",
                        (RenderFragment)(c => c.AddContent(0, "Tab 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(2);
                builder.AddAttribute(3, "Value", "tab1");
                builder.AddAttribute(4, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Panel 1")));
                builder.CloseComponent();

                builder.OpenComponent<TabsContent>(5);
                builder.AddAttribute(6, "Value", "tab2");
                builder.AddAttribute(7, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Panel 2")));
                builder.CloseComponent();
            });
        });
    }

    [Fact]
    public void TabsList_Has_Tablist_Role()
    {
        var cut = RenderTabs();
        AssertRole(cut.Find("[role=tablist]"), "tablist");
    }

    [Fact]
    public void Triggers_Have_Tab_Role()
    {
        var cut = RenderTabs();
        var triggers = cut.FindAll("[role=tab]");
        Assert.Equal(2, triggers.Count);
    }

    [Fact]
    public void Default_Tab_Is_Selected()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(triggers[0], true);
        AssertAriaSelected(triggers[1], false);
    }

    [Fact]
    public void Default_Panel_Is_Visible()
    {
        var cut = RenderTabs("tab1");
        var panels = cut.FindAll("[role=tabpanel]");
        // Active panel has data-state=active, inactive is hidden or data-state=inactive
        AssertDataState(panels[0], "active");
    }

    [Fact]
    public void Click_Trigger_Switches_Active_Tab()
    {
        var cut = RenderTabs("tab1");
        var triggers = cut.FindAll("[role=tab]");

        triggers[1].Click();

        var updatedTriggers = cut.FindAll("[role=tab]");
        AssertAriaSelected(updatedTriggers[0], false);
        AssertAriaSelected(updatedTriggers[1], true);
    }

    [Fact]
    public void Aria_Controls_Links_Trigger_To_Panel()
    {
        var cut = RenderTabs();
        var trigger = cut.Find("[role=tab]");
        var panelId = trigger.GetAttribute("aria-controls");
        Assert.NotNull(panelId);
        Assert.NotNull(cut.Find($"[role=tabpanel][id='{panelId}']"));
    }

    [Fact]
    public void ValueChanged_Fires_On_Tab_Switch()
    {
        string? received = null;
        var cut = Render<Tabs>(p =>
        {
            p.Add(x => x.Value, "tab1");
            p.Add(x => x.ValueChanged,
                EventCallback.Factory.Create<string?>(this, v => received = v));
            p.AddChildContent<TabsList>(tl =>
                tl.AddChildContent<TabsTrigger>(t =>
                {
                    t.Add(x => x.Value, "tab2");
                    t.AddChildContent("Tab 2");
                }));
        });

        cut.Find("[role=tab]").Click();
        Assert.Equal("tab2", received);
    }
}
```

---

## Category 5: Overlay Components

**Components:** Dialog (+ DialogContent, DialogTrigger, DialogClose, DialogTitle, DialogDescription, DialogHeader, DialogFooter), AlertDialog, Sheet, Drawer

### What to Test

1. **Content hidden when closed** — `[role=dialog]` not in DOM when closed
2. **Content visible when open** — `[role=dialog]` in DOM when `DefaultIsOpen=true` or `IsOpen=true`
3. **Trigger click opens** — DialogTrigger click makes `[role=dialog]` appear
4. **Close button closes** — DialogClose click removes `[role=dialog]`
5. **`role="dialog"` and `aria-modal`** — Correct ARIA roles
6. **`aria-labelledby` / `aria-describedby` linkage** — Dialog's `aria-labelledby` matches DialogTitle's `id`
7. **`data-state` reflects open/closed** — `data-state="open"` when visible
8. **Controlled mode** — `IsOpen` + `IsOpenChanged` two-way binding
9. **`IsOpenChanged` fires on close** — Callback invoked with `false`
10. **Structural sub-components** — DialogHeader, DialogFooter, DialogTitle, DialogDescription render correct elements with child content

### What to Delete

- CSS class assertions (`Assert.Contains("fixed", ...)`, `Assert.Contains("z-50", ...)`)
- Backdrop CSS class tests (`Assert.Contains("bg-black\\/80", ...)`)
- Reflection-based tests for `ShouldTrapFocus`, `ShouldLockScroll`, etc.
- `_Is_Assignable_To_OverlayBase` type hierarchy tests
- SVG icon presence in DialogClose

### Example

```csharp
public class DialogTests : BlazingSpireTestBase
{
    [Fact]
    public void Content_Hidden_When_Closed()
    {
        var cut = Render<Dialog>(p =>
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>")));

        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Content_Visible_When_DefaultIsOpen()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent("<p>Body</p>"));
        });

        var dialog = cut.Find("[role=dialog]");
        AssertAriaModal(dialog, true);
        AssertDataState(dialog, "open");
    }

    [Fact]
    public void Trigger_Click_Opens_Dialog()
    {
        var cut = Render<Dialog>(p =>
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Open")));
                builder.CloseComponent();
                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Body")));
                builder.CloseComponent();
            }));

        Assert.Empty(cut.FindAll("[role=dialog]"));
        cut.Find("div").Click(); // DialogTrigger renders a div
        Assert.NotNull(cut.Find("[role=dialog]"));
    }

    [Fact]
    public void Close_Button_Closes_Dialog()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogClose>());
        });

        Assert.NotNull(cut.Find("[role=dialog]"));
        cut.Find("button[aria-label='Close']").Click();
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Title_Linked_Via_Aria_Labelledby()
    {
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogTitle>(tp =>
                    tp.AddChildContent("My Title")));
        });

        var dialog = cut.Find("[role=dialog]");
        var titleId = dialog.GetAttribute("aria-labelledby");
        Assert.NotNull(titleId);
        var title = cut.Find($"#{titleId}");
        Assert.Equal("My Title", title.TextContent.Trim());
    }

    [Fact]
    public void Controlled_Mode_Fires_IsOpenChanged()
    {
        bool? received = null;
        var cut = Render<Dialog>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.Add(x => x.IsOpenChanged,
                EventCallback.Factory.Create<bool>(this, v => received = v));
            p.AddChildContent<DialogContent>(cp =>
                cp.AddChildContent<DialogClose>());
        });

        cut.Find("button[aria-label='Close']").Click();
        Assert.False(received);
    }

    // Structural sub-components — one test each, not class assertions
    [Fact]
    public void DialogHeader_Renders_ChildContent()
    {
        var cut = Render<DialogHeader>(p =>
            p.AddChildContent("<span>Header text</span>"));
        Assert.Contains("Header text", cut.Markup);
    }

    [Fact]
    public void DialogFooter_Renders_ChildContent()
    {
        var cut = Render<DialogFooter>(p =>
            p.AddChildContent("<button>Save</button>"));
        Assert.Contains("Save", cut.Markup);
    }
}
```

---

## Category 6: Floating Components

**Components:** Popover, HoverCard, Tooltip, DropdownMenu (+ MenuItem, MenuCheckboxItem, MenuRadioItem, MenuSeparator, MenuLabel, MenuGroup), ContextMenu, Menubar

### What to Test

1. **Content hidden when closed** — Floating content not in DOM
2. **Trigger opens content** — Click (Popover, DropdownMenu) or hover (HoverCard, Tooltip) shows content
3. **Content closes** — Click outside, Escape, or re-click trigger
4. **ARIA roles** — `role="menu"` for menus, `role="tooltip"` for tooltip, correct `aria-haspopup`
5. **`aria-expanded`** — Trigger reflects open state
6. **Menu items** — `role="menuitem"`, `role="menuitemcheckbox"`, `role="menuitemradio"`
7. **MenuItem click fires OnSelect** — Callback invoked
8. **MenuCheckboxItem toggle** — `aria-checked` toggles
9. **MenuRadioGroup selection** — `aria-checked` reflects selected item
10. **Separator** — `role="separator"` present
11. **`data-state`** — Reflects open/closed

### What to Delete

- CSS class assertions on floating content
- Positioning class assertions (`z-50`, `shadow-md`, etc.)
- Individual `_Has_Base_Classes` per menu sub-component

### Example

```csharp
public class DropdownMenuTests : BlazingSpireTestBase
{
    [Fact]
    public void Menu_Hidden_By_Default()
    {
        var cut = Render<DropdownMenu>(p =>
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                    ip.AddChildContent("Edit"))));

        Assert.Empty(cut.FindAll("[role=menu]"));
    }

    [Fact]
    public void Trigger_Opens_Menu()
    {
        var cut = Render<DropdownMenu>(p =>
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<DropdownMenuTrigger>(0);
                builder.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b => b.AddContent(0, "Options")));
                builder.CloseComponent();
                builder.OpenComponent<DropdownMenuContent>(2);
                builder.AddAttribute(3, "ChildContent",
                    (RenderFragment)(b =>
                    {
                        b.OpenComponent<DropdownMenuItem>(0);
                        b.AddAttribute(1, "ChildContent",
                            (RenderFragment)(c => c.AddContent(0, "Edit")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));

        cut.Find("button").Click();
        Assert.NotNull(cut.Find("[role=menu]"));
        AssertRole(cut.Find("[role=menuitem]"), "menuitem");
    }

    [Fact]
    public void MenuItem_Click_Fires_OnSelect()
    {
        bool fired = false;
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuItem>(ip =>
                {
                    ip.Add(x => x.OnSelect,
                        EventCallback.Factory.Create(this, () => fired = true));
                    ip.AddChildContent("Edit");
                }));
        });

        cut.Find("[role=menuitem]").Click();
        Assert.True(fired);
    }

    [Fact]
    public void Separator_Has_Separator_Role()
    {
        var cut = Render<DropdownMenu>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.AddChildContent<DropdownMenuContent>(cp =>
                cp.AddChildContent<DropdownMenuSeparator>());
        });

        AssertRole(cut.Find("[role=separator]"), "separator");
    }
}
```

---

## Category 7: Complex Selection Components

**Components:** Select (+ SelectTrigger, SelectValue, SelectContent, SelectItem, SelectGroup, SelectLabel, SelectSeparator), Combobox

### What to Test

1. **Dropdown hidden when closed** — `[role=listbox]` not in DOM
2. **Trigger opens dropdown** — Click on trigger shows `[role=listbox]`
3. **Item click fires `ValueChanged`** — Callback with correct value
4. **Item click closes dropdown** — `[role=listbox]` removed after selection
5. **Selected value shows in trigger** — `SelectValue` displays selected text, not placeholder
6. **Placeholder shows when no value** — `SelectValue` displays placeholder text
7. **`role="option"` on items** — Correct ARIA role
8. **Selected item indicator** — Selected item has visual indicator (check SVG present), unselected does not
9. **Disabled item** — `data-disabled` attribute present
10. **SelectGroup and SelectLabel** — `role="group"`, label renders text
11. **SelectSeparator** — `role="separator"`
12. **Controlled mode** — `Value` + `ValueChanged` binding

### What to Delete

- CSS class assertions on trigger, content, items
- `_Has_Base_Classes` for each sub-component
- `_Is_Assignable_To_PopoverBase` type tests
- Reflection-based `ShouldCloseOnEscape` tests

### Example

```csharp
public class SelectTests : BlazingSpireTestBase
{
    [Fact]
    public void Dropdown_Hidden_When_Closed()
    {
        var cut = Render<Select>(p =>
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent("<span>item</span>")));

        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Content_Has_Listbox_Role_When_Open()
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

        AssertRole(cut.Find("[role=listbox]"), "listbox");
        AssertRole(cut.Find("[role=option]"), "option");
    }

    [Fact]
    public void Item_Click_Fires_ValueChanged()
    {
        string? selected = null;
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.DefaultIsOpen, true);
            p.Add(x => x.ValueChanged,
                EventCallback.Factory.Create<string>(this, v => selected = v));
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
    public async Task Item_Click_Closes_Dropdown()
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

        await cut.Find("[role=option]").ClickAsync(new());
        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void Placeholder_Shows_When_No_Value()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Placeholder, "Pick a fruit");
            p.AddChildContent<SelectValue>();
        });

        Assert.Contains("Pick a fruit", cut.Find("span").TextContent);
    }

    [Fact]
    public async Task Selected_Text_Shows_After_Selection()
    {
        var cut = Render<Select>(p =>
        {
            p.Add(x => x.Placeholder, "Pick");
            p.AddChildContent<SelectValue>();
        });

        await cut.InvokeAsync(() =>
            cut.Instance.SelectItemAsync("apple", "Apple"));

        Assert.Contains("Apple", cut.Find("span").TextContent);
    }

    [Fact]
    public void Selected_Item_Shows_Check_Indicator()
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

        Assert.Contains("svg", cut.Find("[role=option]").InnerHtml);
    }

    [Fact]
    public void Unselected_Item_Has_No_Check_Indicator()
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

        Assert.DoesNotContain("svg", cut.Find("[role=option]").InnerHtml);
    }

    [Fact]
    public void Disabled_Item_Has_Data_Disabled()
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
    public void SelectGroup_Has_Group_Role()
    {
        var cut = Render<SelectGroup>(p =>
            p.AddChildContent("<span>item</span>"));
        AssertRole(cut.Find("[role=group]"), "group");
    }

    [Fact]
    public void SelectSeparator_Has_Separator_Role()
    {
        var cut = Render<SelectSeparator>();
        AssertRole(cut.Find("[role=separator]"), "separator");
    }
}
```

---

## Category 8: Data Components

**Components:** DataTable, Carousel, Command, Calendar

### What to Test

1. **DataTable** — Renders `<table>` with correct column headers, row data, sortable columns respond to header click, row selection fires callback, empty state renders
2. **Carousel** — Previous/next navigation changes visible slide, `aria-roledescription="carousel"`, slide indicators reflect current
3. **Command** — Input filters items, item click fires callback, empty state shows, groups render with labels, keyboard navigation (arrow keys move highlight)
4. **Calendar** — Renders correct month/year, day click fires date selection, navigation changes month, selected date highlighted, disabled dates not selectable

### What to Delete

- CSS class assertions on table cells, carousel slides, etc.
- Layout/grid class tests

### Example

```csharp
public class DataTableTests : BlazingSpireTestBase
{
    [Fact]
    public void Renders_Table_With_Headers()
    {
        var cut = Render<DataTable>(p =>
        {
            p.Add(x => x.Columns, new[] { "Name", "Email" });
            p.Add(x => x.Rows, new[]
            {
                new Dictionary<string, object> { ["Name"] = "Alice", ["Email"] = "a@b.com" },
            });
        });

        var headers = cut.FindAll("th");
        Assert.Equal(2, headers.Count);
        Assert.Equal("Name", headers[0].TextContent.Trim());
        Assert.Equal("Email", headers[1].TextContent.Trim());
    }

    [Fact]
    public void Renders_Row_Data()
    {
        var cut = Render<DataTable>(p =>
        {
            p.Add(x => x.Columns, new[] { "Name" });
            p.Add(x => x.Rows, new[]
            {
                new Dictionary<string, object> { ["Name"] = "Alice" },
                new Dictionary<string, object> { ["Name"] = "Bob" },
            });
        });

        var cells = cut.FindAll("td");
        Assert.Equal(2, cells.Count);
        Assert.Equal("Alice", cells[0].TextContent.Trim());
        Assert.Equal("Bob", cells[1].TextContent.Trim());
    }

    [Fact]
    public void Empty_State_Renders_When_No_Rows()
    {
        var cut = Render<DataTable>(p =>
        {
            p.Add(x => x.Columns, new[] { "Name" });
            p.Add(x => x.Rows, Array.Empty<Dictionary<string, object>>());
        });

        Assert.Contains("No results", cut.Markup);
    }
}
```

---

## Base Class Tests

The base class hierarchy (`BlazingSpireComponentBase`, `InteractiveBase`, `FormControlBase<T>`, `DisclosureBase`, `OverlayBase`, `PopoverBase`, `MenuBase`) should have their own dedicated test files that cover:

- `Class` parameter merges with `BaseClasses`
- `AdditionalAttributes` pass through
- `ChildContent` renders
- `Disabled` attribute (InteractiveBase)
- Controlled/uncontrolled state pattern (DisclosureBase, OverlayBase)
- `ValueChanged` / `SetValueAsync` (FormControlBase)

Once these are tested at the base class level, concrete components do NOT need to re-test them. A concrete component test should focus on what makes that component unique.

---

## Test File Structure

```csharp
public class ComponentTests : BlazingSpireTestBase
{
    // Optional: shared render helper for complex component trees
    private IRenderedComponent<MyComponent> RenderMyComponent(/* params */) { ... }

    // Group 1: Rendering & ARIA
    [Fact] public void Renders_Correct_Element_And_Role() { }

    // Group 2: State & behavior
    [Fact] public void Default_State_Is_Correct() { }
    [Fact] public void Interaction_Changes_State() { }

    // Group 3: Callbacks
    [Fact] public void Callback_Fires_With_Correct_Value() { }

    // Group 4: Controlled mode (if applicable)
    [Fact] public void Controlled_Mode_Works() { }

    // Group 5: Variants (if applicable, use Theory)
    [Theory] public void Each_Variant_Renders(MyVariant variant) { }
}
```

## Summary Checklist

Before submitting a rewritten test file, verify:

- [ ] Zero `Assert.Contains("some-css-class", classes)` tests (unless the class IS the behavioral contract, like `text-muted-foreground` for placeholder state)
- [ ] Zero `typeof(X).IsAssignableTo(typeof(Y))` tests
- [ ] Zero reflection-based tests for non-public members
- [ ] Every `[Fact]` tests a user-visible behavior or assistive technology contract
- [ ] Variants tested via `[Theory]` with one test, not N copy-paste tests
- [ ] Composite components tested as assembled trees (parent + children), not each sub-component in isolation with `_Has_Base_Classes`
- [ ] `Custom_Class_Is_Included` — at most ONE per component
- [ ] Callbacks verified with captured values, not just "did it throw"
- [ ] Elements queried by role/ARIA (`[role=dialog]`) not by CSS class
