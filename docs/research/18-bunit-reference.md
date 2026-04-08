# bUnit Testing Reference

Reference for bUnit 2.7.x APIs used in BlazingSpire component tests. Based on source analysis of [`bUnit-dev/bUnit`](https://github.com/bUnit-dev/bUnit).

## BunitContext API

`BunitContext` is the test fixture base class. Each test class inherits from it (or from our `BlazingSpireTestBase` wrapper). It implements both `IDisposable` and `IAsyncDisposable`.

### Core Properties

| Property | Type | Description |
|----------|------|-------------|
| `Services` | `BunitServiceProvider` | DI container for the test. Register services before first render. |
| `JSInterop` | `BunitJSInterop` | JS interop mock. Configure handlers and verify invocations. |
| `Renderer` | `BunitRenderer` | The Blazor renderer. Created lazily on first use. |
| `RenderTree` | `RootRenderTree` | Root render tree wrapper — add layout/root components here. |
| `ComponentFactories` | `ComponentFactoryCollection` | Register component factories/stubs before rendering. |

### Static Configuration

```csharp
// Default timeout for all WaitFor* operations (default: 1 second)
BunitContext.DefaultWaitTimeout = TimeSpan.FromSeconds(3);
```

### Lifecycle

```csharp
public class MyTests : BunitContext
{
    // Constructor — register services, configure JSInterop
    public MyTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IMyService, FakeMyService>();
    }

    [Fact]
    public void Test()
    {
        var cut = Render<MyComponent>();
        // ...assertions...
    }

    // Disposal is automatic — BunitContext disposes renderer then services
}
```

### Render Methods

```csharp
// 1. Render with typed parameter builder (preferred for C# tests)
var cut = Render<Button>(p => p
    .Add(x => x.Variant, ButtonVariant.Primary)
    .Add(x => x.OnClick, () => clicked = true)
    .Add(x => x.ChildContent, "Click me"));

// 2. Render a RenderFragment and find a component within it
var cut = Render<Dialog>(@<Dialog Open="true"><p>Content</p></Dialog>);

// 3. Render raw markup fragment (returns IRenderedComponent<ContainerFragment>)
var cut = Render(@<div>Hello</div>);

// 4. Render with no parameters
var cut = Render<Badge>();
```

### DisposeComponentsAsync

Disposes all rendered components without disposing the entire context. Useful for testing disposal behavior:

```csharp
var cut = Render<Dialog>(p => p.Add(x => x.Open, true));
await DisposeComponentsAsync();
// Verify cleanup happened (e.g., JS interop dispose calls)
```

## Rendering Components

### Parameter Builder API

The `ComponentParameterCollectionBuilder<TComponent>` provides type-safe parameter setting via lambda expressions:

```csharp
var cut = Render<Select<string>>(p => p
    // Simple parameter
    .Add(x => x.Placeholder, "Choose...")
    // EventCallback parameter
    .Add(x => x.ValueChanged, (string val) => selectedValue = val)
    // RenderFragment (string markup)
    .Add(x => x.ChildContent, "<option>One</option><option>Two</option>")
    // RenderFragment (child component)
    .Add<SelectItem<string>>(x => x.ChildContent, child => child
        .Add(x => x.Value, "one")
        .Add(x => x.ChildContent, "One"))
    // RenderFragment<T> template
    .Add(x => x.ItemTemplate, (item) => $"<span>{item}</span>")
    // Unmatched/arbitrary attributes
    .AddUnmatched("data-testid", "my-select")
    .AddUnmatched("aria-describedby", "help-text"));
```

### Cascading Values

```csharp
// Via parameter builder — for [CascadingParameter] properties
var cut = Render<MenuItem>(p => p
    .Add(x => x.MenuContext, new MenuContext { Orientation = "vertical" }));

// Via RenderTree — wraps ALL components rendered in this context
RenderTree.Add<CascadingValue<ThemeContext>>(p => p
    .Add(x => x.Value, new ThemeContext { Theme = "dark" }));
var cut = Render<Button>(); // Button sees the cascading ThemeContext
```

### Re-rendering with New Parameters

```csharp
var cut = Render<Dialog>(p => p.Add(x => x.Open, false));
Assert.Empty(cut.FindAll("[role='dialog']"));

// Re-render with different parameters
cut.Render(p => p.Add(x => x.Open, true));
Assert.NotNull(cut.Find("[role='dialog']"));
```

### InvokeAsync — Programmatic State Changes

When modifying component state outside the render cycle, use `InvokeAsync` to run on the renderer's synchronization context:

```csharp
var cut = Render<Counter>();
await cut.InvokeAsync(() => cut.Instance.Increment());
// Component has re-rendered with updated state
```

## JSInterop Mocking

### Loose vs Strict Mode

| Mode | Behavior | Use When |
|------|----------|----------|
| `JSRuntimeMode.Strict` (default) | Throws `JSRuntimeUnhandledInvocationException` for unmocked calls | Testing specific JS interactions |
| `JSRuntimeMode.Loose` | Returns `default(T)` for unmocked calls | Most component tests where JS is incidental |

```csharp
// In test constructor or base class
JSInterop.Mode = JSRuntimeMode.Loose;
```

### Setup Patterns

```csharp
// Setup a void call
JSInterop.SetupVoid("scrollToElement", _ => true);

// Setup a call with a return value
JSInterop.Setup<int>("getScrollPosition").SetResult(150);

// Setup with specific arguments
JSInterop.Setup<bool>("isVisible", ".my-element").SetResult(true);

// Setup with argument matcher
JSInterop.SetupVoid("lockScroll", inv => inv.Arguments.Count == 0);

// Catch-all handler for a return type
JSInterop.Setup<string>().SetResult("fallback");
JSInterop.SetupVoid(); // catch-all void
```

### Module Mocking (Collocated .razor.js)

Blazor components import JS modules via `IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./Component.razor.js")`. bUnit provides `SetupModule` to mock these:

```csharp
// Setup module import — returns a BunitJSModuleInterop
var focusModule = JSInterop.SetupModule(
    "./_content/BlazingSpire.Primitives/js/focus.js");

// Setup calls ON the module
focusModule.SetupVoid("createFocusTrap", _ => true);
focusModule.Setup<string[]>("getFocusableElements", _ => true)
    .SetResult(new[] { "#first", "#last" });

// Module inherits Mode from parent unless overridden
focusModule.Mode = JSRuntimeMode.Strict; // optional override

// Catch-all module (matches any import)
var anyModule = JSInterop.SetupModule();
```

### Verifying JS Invocations

```csharp
// Verify a call happened exactly once
JSInterop.VerifyInvoke("lockScroll");

// Verify a call happened N times
JSInterop.VerifyInvoke("lockScroll", 2);

// Verify a call never happened
JSInterop.VerifyNotInvoke("unlockScroll");

// Verify specific arguments
var invocation = JSInterop.VerifyInvoke("scrollToElement");
Assert.Equal("#target", invocation.Arguments[0]);

// Verify an ElementReference argument points to a specific element
var button = cut.Find("button");
invocation.Arguments[0].ShouldBeElementReferenceTo(button);
```

### Built-in Handlers

bUnit auto-registers handlers for common Blazor framework JS calls:
- `FocusAsync` (ElementReference.FocusAsync)
- `Virtualize` JS interop
- `NavigationLock` prompt enable/disable
- `InputFile` interop
- `FocusOnNavigate`
- Auto-wraps `IJSObjectReference` returns in loose mode

## Event Simulation

### Keyboard Events

The `Key` class provides all standard keys with modifier support:

```csharp
var input = cut.Find("input");

// Simple key press
input.KeyDown(Key.Enter);
input.KeyUp(Key.Escape);
input.KeyPress(Key.Space);

// Arrow navigation
input.KeyDown(Key.ArrowDown);
input.KeyDown(Key.ArrowUp);
input.KeyDown(Key.Home);
input.KeyDown(Key.End);

// Modifier combinations (+ operator)
input.KeyDown(Key.Control + Key.Enter);  // Ctrl+Enter
input.KeyDown(Key.Shift + Key.Tab);      // Shift+Tab
input.KeyDown(Key.Alt + "a");            // Alt+A

// Character keys (implicit conversion from string/char)
input.KeyDown("a");
input.KeyDown('A');

// With repeat flag
input.KeyDown(Key.ArrowDown, repeat: true);
```

**Available Key constants:** `Backspace`, `Tab`, `Enter`, `Pause`, `Escape`, `Space`, `PageUp`, `PageDown`, `End`, `Home`, `Left` (ArrowLeft), `Up` (ArrowUp), `Right` (ArrowRight), `Down` (ArrowDown), `Insert`, `Delete`, `Equal`, `F1`-`F12`, `Control`, `Shift`, `Alt`, `Command`, `NumberPad0`-`NumberPad9`, `Multiply`, `Add`, `Subtract`, `Divide`, `NumberPadDecimal`.

### Mouse Events

```csharp
var button = cut.Find("button");

// Click
button.Click();
button.DoubleClick();

// Click with modifier keys
button.Click(new MouseEventArgs { CtrlKey = true, Button = 0 });

// Mouse movement
button.MouseOver();
button.MouseOut();
button.MouseEnter();
button.MouseLeave();
button.MouseDown();
button.MouseUp();
button.MouseMove(new MouseEventArgs { ClientX = 100, ClientY = 200 });

// Context menu
button.ContextMenu(new MouseEventArgs { Button = 2 });

// Pointer events
button.PointerDown();
button.PointerUp();
button.PointerEnter();
button.PointerLeave();
```

### Form / Input Events

```csharp
var input = cut.Find("input");

// Change event (triggers @onchange)
input.Change("new value");
input.Change(42);           // numeric
input.Change(true);         // bool (checkbox)
input.Change(new[] { "a", "b" }); // multi-select

// Input event (triggers @oninput)
input.Input("partial text");

// Focus events
input.Focus();
input.Blur();
input.FocusIn();
input.FocusOut();
```

### Generic Event Trigger

For any event not covered by a dedicated method:

```csharp
element.TriggerEvent("ontransitionend", EventArgs.Empty);
element.TriggerEvent("onscroll", new EventArgs());
element.TriggerEvent("onanimationend", new EventArgs());
```

**Important:** Events follow DOM bubbling rules. Non-bubbling events (`onfocus`, `onblur`, `onchange`, `onsubmit`, `onscroll`, `onmouseenter`, `onmouseleave`, etc.) only trigger on the target element. Click events on disabled elements are suppressed.

### Async Event Methods

Every event method has an `Async` variant that returns a `Task`:

```csharp
await button.ClickAsync();
await input.KeyDownAsync(Key.Enter);
await input.ChangeAsync("value");
```

Use the async variant when you need to `await` the completion of async event handlers.

## DOM Assertions

### Find / FindAll

```csharp
// Find single element (throws ElementNotFoundException if not found)
var button = cut.Find("button.primary");
var dialog = cut.Find("[role='dialog']");
var input = cut.Find("#username");

// Find with specific element type (AngleSharp typed elements)
var htmlInput = cut.Find<IHtmlInputElement>("input[type='text']");
// Now you get typed properties: htmlInput.Value, htmlInput.Type, etc.

// Find all matching elements
var items = cut.FindAll("li.menu-item");
Assert.Equal(3, items.Count);

// Find child components
var childButton = cut.FindComponent<Button>();
var allButtons = cut.FindComponents<Button>();
Assert.Equal(2, allButtons.Count);

// Check if a component exists
bool hasTooltip = cut.HasComponent<Tooltip>();
```

### MarkupMatches — Semantic HTML Comparison

`MarkupMatches` performs semantic comparison: ignores whitespace differences, attribute order, and comment nodes:

```csharp
// Assert entire component output
cut.MarkupMatches(@"<button class=""btn primary"" type=""button"">Click</button>");

// Assert a specific element
var button = cut.Find("button");
button.MarkupMatches(@"<button class=""btn primary"">Click</button>");

// Assert multiple elements
cut.FindAll("li").MarkupMatches(@"
    <li>Item 1</li>
    <li>Item 2</li>");

// Custom failure message
cut.MarkupMatches("<expected/>", "Dialog should render empty when closed");
```

### Controlling Diff Behavior

bUnit uses AngleSharp Diffing under the hood. You can annotate expected markup to customize comparison:

```csharp
// Ignore specific attributes with diff:ignoreAttributes
cut.MarkupMatches(@"<div diff:ignoreAttributes=""id class"" role=""dialog"">Content</div>");

// Ignore child content with diff:ignoreChildren
cut.MarkupMatches(@"<div role=""dialog"" diff:ignoreChildren>...</div>");

// Ignore entire element with diff:ignore
cut.MarkupMatches(@"<div><span diff:ignore /></div>");

// Regex match on attributes with diff:regex
cut.MarkupMatches(@"<div id=""dialog-\d+"">Content</div>");
```

### Direct DOM Queries (AngleSharp)

The `Nodes` property gives you full AngleSharp DOM access:

```csharp
// CSS selector queries
var nodes = cut.Nodes.QuerySelectorAll("[aria-expanded='true']");

// Read attributes
var button = cut.Find("button");
Assert.Equal("dialog", button.GetAttribute("aria-haspopup"));
Assert.Equal("true", button.GetAttribute("aria-expanded"));
Assert.True(button.ClassList.Contains("active"));
Assert.Equal("Submit", button.TextContent.Trim());

// Check element visibility via attributes
Assert.Null(button.GetAttribute("hidden"));
Assert.Equal("false", button.GetAttribute("aria-hidden"));

// Navigate the DOM tree
var parent = button.ParentElement;
var firstChild = button.FirstElementChild;
var siblings = button.ParentElement!.Children;
```

### FindByLabelText (bunit.web.query)

Query elements by their associated label text — useful for accessible form testing:

```csharp
// Finds input associated with <label for="email">Email</label>
var input = cut.FindByLabelText("Email");

// Also matches aria-label, aria-labelledby, and wrapping labels
var search = cut.FindByLabelText("Search");

// Find all elements with a label
var inputs = cut.FindAllByLabelText("Name");
```

## Render Mode Testing

### SetRendererInfo

Configure what `RendererInfo` the renderer reports. Required when components read `ComponentBase.RendererInfo`:

```csharp
// Simulate Server-side rendering (interactive)
SetRendererInfo(new RendererInfo("Server", isInteractive: true));

// Simulate WebAssembly rendering
SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));

// Simulate Static SSR (not interactive)
SetRendererInfo(new RendererInfo("Static", isInteractive: false));
```

Components access this via `RendererInfo.Name` and `RendererInfo.IsInteractive`. If a component reads `RendererInfo` and you have not called `SetRendererInfo`, bUnit throws `MissingRendererInfoException`.

### SetAssignedRenderMode

Set the `@rendermode` attribute on a component via the parameter builder:

```csharp
// Set render mode on root component
var cut = Render<Dialog>(p => p
    .SetAssignedRenderMode(RenderMode.InteractiveServer)
    .Add(x => x.Open, true));

// Render mode cascades to children automatically
var cut = Render<Panel>(p => p
    .SetAssignedRenderMode(RenderMode.InteractiveWebAssembly)
    .AddChildContent<Dialog>(child => child.Add(x => x.Open, true)));
// Dialog also gets InteractiveWebAssembly render mode

// Override render mode on child
var cut = Render<Panel>(p => p
    .AddChildContent<Dialog>(child => child
        .SetAssignedRenderMode(RenderMode.InteractiveServer)));

// Mismatched parent/child render modes throw RenderModeMisMatchException
```

### Testing SSR Fallback Patterns

```csharp
[Fact]
public void Button_renders_anchor_in_SSR_mode()
{
    SetRendererInfo(new RendererInfo("Static", isInteractive: false));
    var cut = Render<Button>(p => p
        .Add(x => x.Href, "/about")
        .Add(x => x.ChildContent, "About"));

    cut.Find("a").MarkupMatches(@"<a href=""/about"">About</a>");
}

[Fact]
public void Button_renders_button_in_interactive_mode()
{
    SetRendererInfo(new RendererInfo("Server", isInteractive: true));
    var cut = Render<Button>(p => p
        .Add(x => x.OnClick, () => { })
        .Add(x => x.ChildContent, "Submit"));

    Assert.Equal("button", cut.Find("button").TagName.ToLower());
}
```

## Async Patterns

### WaitForAssertion

Retries an assertion after each render until it passes or timeout:

```csharp
// Wait for async data to load and render
cut.WaitForAssertion(() =>
{
    var items = cut.FindAll("li");
    Assert.Equal(5, items.Count);
});

// With custom timeout
cut.WaitForAssertion(() =>
    Assert.Contains("Loaded", cut.Markup),
    timeout: TimeSpan.FromSeconds(3));

// Async variant
await cut.WaitForAssertionAsync(() =>
    Assert.NotEmpty(cut.FindAll(".item")));
```

### WaitForState

Waits for a predicate to return `true`, re-evaluating after each render:

```csharp
// Wait until loading is complete
cut.WaitForState(() => !cut.Instance.IsLoading);

// Wait until specific DOM state
cut.WaitForState(() => cut.FindAll("tr").Count > 0);

// With timeout
cut.WaitForState(() => cut.Instance.Data is not null,
    timeout: TimeSpan.FromSeconds(5));
```

### WaitForElement / WaitForElements

Waits for a CSS selector to match an element in the DOM:

```csharp
// Wait for element to appear
var dialog = cut.WaitForElement("[role='dialog']");
Assert.Equal("true", dialog.GetAttribute("aria-modal"));

// Wait for specific count of elements
var items = cut.WaitForElements("li.loaded", matchElementCount: 3);

// With timeout
var toast = cut.WaitForElement(".toast-message",
    timeout: TimeSpan.FromSeconds(2));
```

### WaitForComponent / WaitForComponents

Wait for a child component to appear in the render tree:

```csharp
// Wait for a lazy-loaded component
var tooltip = cut.WaitForComponent<Tooltip>();

// Wait for N component instances
var items = cut.WaitForComponents<ListItem>(matchComponentCount: 5);
```

### Timeout Behavior

- Default timeout: 1 second (configurable via `BunitContext.DefaultWaitTimeout`)
- When a debugger is attached, timeout is set to `Timeout.InfiniteTimeSpan`
- On timeout, throws `WaitForFailedException` with the last assertion exception as inner exception

## Component Factories

### Stubbing Components

Replace child components with stubs to isolate the component under test:

```csharp
// Stub out a component entirely (renders nothing)
ComponentFactories.AddStub<Tooltip>();

// Stub with replacement markup
ComponentFactories.AddStub<Icon>("<svg data-testid='icon-stub'/>");

// Stub with replacement RenderFragment
ComponentFactories.AddStub<Icon>(b =>
    b.AddMarkupContent(0, "<span class='icon-placeholder'/>"));

// Stub with access to the parameters that were passed
ComponentFactories.AddStub<Icon>(
    (CapturedParameterView<Icon> ps) =>
        $"<span data-icon='{ps.Get(x => x.Name)}'/>"));

// Stub by predicate (e.g., stub all third-party components)
ComponentFactories.AddStub(type =>
    type.Namespace?.StartsWith("ThirdPartyLib") == true);

// Stub with predicate and replacement markup
ComponentFactories.AddStub(
    type => type.IsAssignableTo(typeof(IIcon)),
    "<span class='icon-stub'/>");
```

### Stub<TComponent> — Inspecting Captured Parameters

The `Stub<T>` test double captures all parameters passed to the stubbed component:

```csharp
ComponentFactories.AddStub<Tooltip>();

var cut = Render<Button>(p => p
    .Add(x => x.TooltipText, "Save changes"));

// Find the stub in the render tree
var tooltipStub = cut.FindComponent<Stub<Tooltip>>();

// Read captured parameters
var parameters = tooltipStub.Instance.Parameters;
Assert.Equal("Save changes", parameters.Get(x => x.Content));
```

### Custom Component Factories

For more control, implement `IComponentFactory`:

```csharp
ComponentFactories.Add(new ConditionalComponentFactory(
    type => type == typeof(HeavyChart),
    type => new LightweightChartStub()));
```

### Ordering

Factories are checked last-added-first. The first factory that returns `true` from `CanCreate` is used. If no factory matches, the default Blazor activator creates the component.

## Focus Verification

bUnit intercepts `ElementReference.FocusAsync()` calls via a built-in handler. You can verify focus was requested:

```csharp
[Fact]
public void Dialog_focuses_first_element_on_open()
{
    var cut = Render<Dialog>(p => p.Add(x => x.Open, true));

    // Verify FocusAsync was called once
    JSInterop.VerifyFocusAsyncInvoke();
}

[Fact]
public void Select_focuses_input_on_arrow_key()
{
    var cut = Render<Select<string>>(p => p.Add(x => x.Open, true));

    cut.Find("[role='listbox']").KeyDown(Key.ArrowDown);

    // Verify FocusAsync was called exactly 2 times
    var invocations = JSInterop.VerifyFocusAsyncInvoke(calledTimes: 2);

    // Inspect which element was focused
    var focusedElement = cut.Find("[role='option']:first-child");
    invocations[1].Arguments[0].ShouldBeElementReferenceTo(focusedElement);
}
```

**Limitation:** bUnit cannot verify actual DOM focus state (`document.activeElement`). `FocusAsync` verification only confirms the .NET side called the method. For real focus testing, use Playwright.

## BlazingSpire-Specific Patterns

### Test Base Class

All BlazingSpire tests inherit from `BlazingSpireTestBase`:

```csharp
public abstract class BlazingSpireTestBase : BunitContext
{
    protected BlazingSpireTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected static void AssertAriaExpanded(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-expanded"));

    protected static void AssertRole(IElement element, string role)
        => Assert.Equal(role, element.GetAttribute("role"));

    protected static void AssertAriaLabel(IElement element, string label)
        => Assert.Equal(label, element.GetAttribute("aria-label"));

    protected static void AssertAriaHidden(IElement element, bool expected)
        => Assert.Equal(expected.ToString().ToLower(), element.GetAttribute("aria-hidden"));
}
```

**Key decisions:**
- Loose JS interop mode by default — components use JS for focus trapping, positioning, scroll lock, click-outside, and portal DOM reparenting. Strict mode would require mocking every call.
- ARIA assertion helpers keep test code readable and consistent.

### Mocking BlazingSpire JS Modules

When a test needs to verify specific JS interactions, set up the module explicitly:

```csharp
public class FocusTrapTests : BlazingSpireTestBase
{
    private readonly BunitJSModuleInterop _focusModule;

    public FocusTrapTests()
    {
        _focusModule = JSInterop.SetupModule(
            "./_content/BlazingSpire.Primitives/js/focus.js");
        _focusModule.SetupVoid("createFocusTrap", _ => true);
        _focusModule.SetupVoid("destroyFocusTrap", _ => true);
    }

    [Fact]
    public void Dialog_creates_focus_trap_on_open()
    {
        Render<Dialog>(p => p.Add(x => x.Open, true));
        _focusModule.VerifyInvoke("createFocusTrap");
    }

    [Fact]
    public void Dialog_destroys_focus_trap_on_close()
    {
        var cut = Render<Dialog>(p => p.Add(x => x.Open, true));
        cut.Render(p => p.Add(x => x.Open, false));
        _focusModule.VerifyInvoke("destroyFocusTrap");
    }
}
```

### Standard Test Patterns for BlazingSpire Components

**Render + ARIA assertion:**
```csharp
[Fact]
public void Button_renders_with_correct_role()
{
    var cut = Render<Button>(p => p.Add(x => x.ChildContent, "Save"));
    AssertRole(cut.Find("button"), "button");
}
```

**Keyboard navigation test:**
```csharp
[Fact]
public void Select_navigates_options_with_arrow_keys()
{
    var cut = Render<Select<string>>(p => p.Add(x => x.Open, true));
    var listbox = cut.Find("[role='listbox']");

    listbox.KeyDown(Key.ArrowDown);
    var firstOption = cut.Find("[role='option']:first-child");
    Assert.Equal("true", firstOption.GetAttribute("aria-selected"));

    listbox.KeyDown(Key.ArrowDown);
    var secondOption = cut.FindAll("[role='option']")[1];
    Assert.Equal("true", secondOption.GetAttribute("aria-selected"));
}
```

**Data-driven test with Theory:**
```csharp
[Theory]
[InlineData(ButtonVariant.Primary, "btn-primary")]
[InlineData(ButtonVariant.Secondary, "btn-secondary")]
[InlineData(ButtonVariant.Ghost, "btn-ghost")]
public void Button_applies_variant_class(ButtonVariant variant, string expectedClass)
{
    var cut = Render<Button>(p => p.Add(x => x.Variant, variant));
    Assert.True(cut.Find("button").ClassList.Contains(expectedClass));
}
```

**Async loading test:**
```csharp
[Fact]
public void DataTable_shows_loading_then_data()
{
    var tcs = new TaskCompletionSource<List<Item>>();
    Services.AddSingleton<IDataService>(new FakeDataService(tcs.Task));

    var cut = Render<DataTable>();

    // Loading state
    Assert.NotNull(cut.Find("[aria-busy='true']"));

    // Complete the data load
    tcs.SetResult(new List<Item> { new("A"), new("B") });

    // Wait for data to render
    cut.WaitForAssertion(() =>
    {
        var rows = cut.FindAll("tr");
        Assert.Equal(2, rows.Count);
    });
}
```

**Component stub isolation:**
```csharp
[Fact]
public void Popover_renders_without_positioning_component()
{
    ComponentFactories.AddStub<FloatingPositioner>(
        "<div data-testid='positioner-stub'/>");

    var cut = Render<Popover>(p => p
        .Add(x => x.Open, true)
        .Add(x => x.ChildContent, "Content"));

    Assert.NotNull(cut.Find("[data-testid='positioner-stub']"));
    Assert.Contains("Content", cut.Markup);
}
```

## Version Notes (2.7.x)

| Version | Key Changes |
|---------|-------------|
| **2.7.2** (2026-03-31) | Fixed `InvokeConstructorAsync` on `BunitJSRuntime` for .NET 10+ |
| **2.6.2** (2026-02-27) | Added .NET 11 (net11.0) support |
| **2.5.3** (2026-01-08) | Bug fixes |
| **2.0+** | `BunitContext` replaces legacy `TestContext`. `Render<T>()` replaces `RenderComponent<T>()`. `SetAssignedRenderMode` added. `SetRendererInfo` added (NET9+). `WaitForComponent`/`WaitForComponents` added. Source generators for stubs (`[ComponentStub]` attribute). |

### Migration from TestContext (pre-2.0)

| Old API | New API |
|---------|---------|
| `TestContext` | `BunitContext` |
| `RenderComponent<T>(p => ...)` | `Render<T>(p => ...)` |
| `ctx.JSInterop` | `JSInterop` (direct property) |
| `ctx.Services` | `Services` (direct property) |
