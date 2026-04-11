using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// a11y-tree slot-count liveness tests for IRepeatingSlot components.
///
/// For every component implementing IRepeatingSlot&lt;TRoot&gt; (discovered via
/// <see cref="ComponentMetadata.RepeatingSlots"/>), toggles the count-driving parameter
/// across {4, 6, 8} and asserts that the DOM contains exactly that many slot elements.
///
/// This validates that the source generator's for-loop (driven by GetSampleCount at render
/// time) actually re-fires when the count parameter changes in the playground, and that the
/// slot component renders a discoverable DOM element for each iteration.
///
/// Slot discovery uses data-slot="{SlotName}" attribute — a convention added to each
/// IRepeatingSlot component's markup (currently: InputOTPSlot emits data-slot="InputOTPSlot").
///
/// Currently exercises: InputOTP (MaxLength drives InputOTPSlot count).
/// Automatically picks up any future IRepeatingSlot components when DocGen emits their
/// isRepeatingSlot / countParameterOwner / countParameterName metadata.
/// </summary>
public class RepeatingSlotTests : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    private static readonly int[] SlotCounts = [4, 6, 8];

    public RepeatingSlotTests(PlaywrightBrowserFixture browserFixture) : base(browserFixture) { }

    public static IEnumerable<object[]> SlotComponents() =>
        ComponentMetadata.RepeatingSlots
            .Select(s => new object[] { s.Slot.Name, s.CountOwner, s.CountParam });

    [Theory, MemberData(nameof(SlotComponents))]
    public async Task Slot_Count_Tracks_Count_Parameter(string slotName, string countOwner, string countParam)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);

        // Navigate to the root component's playground (countOwner = e.g. "InputOTP")
        await driver.NavigateTo(countOwner);

        foreach (var count in SlotCounts)
        {
            driver.ClearErrors();
            await driver.SetNumberParam(countParam, count);

            // Count slot DOM elements using the data-slot convention
            var slotLocator = driver.Preview.Locator($"[data-slot='{slotName}']");
            var actual = await slotLocator.CountAsync();

            Assert.True(
                actual == count,
                $"[{slotName}] Expected {count} slots when {countParam}={count}, " +
                $"but found {actual}. " +
                $"Check that {countOwner}.{countParam} is the count-driving parameter " +
                $"and that {slotName}.razor emits data-slot=\"{slotName}\" on its root element.");

            Assert.Empty(driver.ConsoleErrors);
            Assert.Empty(driver.PageErrors);
        }
    }
}
