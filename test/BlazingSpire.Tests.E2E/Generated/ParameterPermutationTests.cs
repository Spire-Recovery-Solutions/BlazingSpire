using BlazingSpire.Tests.E2E.Infrastructure;

namespace BlazingSpire.Tests.E2E.Generated;

/// <summary>
/// Metadata-driven parameter permutation tests.
/// For each component × parameter × value combination, navigate to the playground,
/// set the parameter, and verify no errors occur. This catches type coercion bugs,
/// missing variant mappings, and any render crashes that happen only on parameter
/// change — the exact class of bugs bUnit tests miss.
/// </summary>
[Collection("BlazorApp")]
public class ParameterPermutationTests : BlazingSpireE2EBase
{
    public static IEnumerable<object[]> EnumPermutations() =>
        ComponentMetadata.EnumPermutations
            .Select(x => new object[] { x.Component.Name, x.Parameter.Name, x.Value });

    public static IEnumerable<object[]> BoolPermutations() =>
        ComponentMetadata.BoolPermutations
            .Select(x => new object[] { x.Component.Name, x.Parameter.Name, x.Value });

    [Theory]
    [MemberData(nameof(EnumPermutations))]
    public async Task Enum_Parameter_Renders_Without_Error(string component, string param, string value)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetEnumParam(param, value);

        // Give the render cycle a moment to complete
        await Page.WaitForTimeoutAsync(200);

        await Expect(driver.Preview).ToBeVisibleAsync();
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }

    [Theory]
    [MemberData(nameof(BoolPermutations))]
    public async Task Bool_Parameter_Renders_Without_Error(string component, string param, bool value)
    {
        var driver = new PlaygroundDriver(Page, BaseUrl);
        await driver.NavigateTo(component);

        driver.ClearErrors();
        await driver.SetBoolParam(param, value);

        await Page.WaitForTimeoutAsync(200);

        await Expect(driver.Preview).ToBeVisibleAsync();
        Assert.Empty(driver.ConsoleErrors);
        Assert.Empty(driver.PageErrors);
    }
}
