using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class DialogServiceTests : BlazingSpireTestBase
{
    [Fact]
    public void ShowAsync_Adds_Dialog_To_OpenDialogs()
    {
        var service = new DialogService();
        _ = service.ShowAsync<MessageBoxDialog>("Test");
        Assert.Single(service.OpenDialogs);
    }

    [Fact]
    public void ShowAsync_Raises_OnDialogsChanged()
    {
        var service = new DialogService();
        var raised = false;
        service.OnDialogsChanged += () => raised = true;
        _ = service.ShowAsync<MessageBoxDialog>("Test");
        Assert.True(raised);
    }

    [Fact]
    public async Task Close_With_Ok_Returns_Non_Cancelled_Result()
    {
        var service = new DialogService();
        var task = service.ShowAsync<MessageBoxDialog>("Test");
        service.OpenDialogs[0].Close(DialogResult.Ok("data"));
        var result = await task;
        Assert.False(result.Cancelled);
        Assert.Equal("data", result.Data);
    }

    [Fact]
    public async Task Close_With_Cancel_Returns_Cancelled_Result()
    {
        var service = new DialogService();
        var task = service.ShowAsync<MessageBoxDialog>("Test");
        service.OpenDialogs[0].Close(DialogResult.Cancel());
        var result = await task;
        Assert.True(result.Cancelled);
    }

    [Fact]
    public async Task Closed_Dialog_Is_Removed_From_OpenDialogs()
    {
        var service = new DialogService();
        var task = service.ShowAsync<MessageBoxDialog>("Test");
        service.OpenDialogs[0].Close(DialogResult.Ok());
        await task;
        // Give the ContinueWith a chance to run
        await Task.Delay(50);
        Assert.Empty(service.OpenDialogs);
    }

    [Fact]
    public void ShowMessageBoxAsync_Adds_Dialog_With_Parameters()
    {
        var service = new DialogService();
        _ = service.ShowMessageBoxAsync("Title", "Message", "Yes", "No");
        Assert.Single(service.OpenDialogs);
        var dialog = service.OpenDialogs[0];
        Assert.Equal("Title", dialog.Title);
        Assert.Equal("Message", dialog.Message);
    }

    [Fact]
    public void DialogParameters_Set_And_Get()
    {
        var p = new DialogParameters().Set("Name", "test").Set("Count", 42);
        Assert.Equal("test", p.Get<string>("Name"));
        Assert.Equal(42, p.Get<int>("Count"));
    }

    [Fact]
    public void DialogParameters_Get_Missing_Returns_Default()
    {
        var p = new DialogParameters();
        Assert.Null(p.Get<string>("missing"));
        Assert.Equal(0, p.Get<int>("missing"));
    }

    [Fact]
    public void DialogResult_Ok_Is_Not_Cancelled()
    {
        var result = DialogResult.Ok("data");
        Assert.False(result.Cancelled);
        Assert.Equal("data", result.Data);
    }

    [Fact]
    public void DialogResult_Cancel_Is_Cancelled()
    {
        var result = DialogResult.Cancel();
        Assert.True(result.Cancelled);
        Assert.Null(result.Data);
    }
}
