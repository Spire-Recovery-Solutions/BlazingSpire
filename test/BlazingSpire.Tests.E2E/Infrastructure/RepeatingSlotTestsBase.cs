using BlazingSpire.Tests.E2E;

namespace BlazingSpire.Tests.E2E.Infrastructure;

/// <summary>
/// Abstract base for IRepeatingSlot slot-count liveness tests.
/// Concrete class is emitted by BlazingSpire.TestGenerator.
/// </summary>
public abstract class RepeatingSlotTestsBase : BlazingSpireE2EBase,
    IClassFixture<BlazorAppFixture>,
    IClassFixture<PlaywrightBrowserFixture>
{
    protected RepeatingSlotTestsBase(PlaywrightBrowserFixture browserFixture)
        : base(browserFixture) { }

    private static readonly int[] SlotCounts = [4, 6, 8];

    protected static System.Collections.Generic.IEnumerable<object[]> SlotComponentsData() =>
        ComponentMetadata.RepeatingSlots
            .Select(s => new object[] { s.Slot.Name, s.CountOwner, s.CountParam });

    protected async Task RunSlotAsync(string slotName, string countOwner, string countParam)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(countOwner);

        foreach (var count in SlotCounts)
        {
            driver.ClearErrors();
            await driver.SetNumberParam(countParam, count);

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
