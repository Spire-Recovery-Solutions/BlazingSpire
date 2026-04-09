using System.Diagnostics;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Starts the BlazingSpire demo app once and shares it across all tests in the BlazorApp collection.
/// If APP_URL is already set (CI strategy), skips launching a local process.
/// </summary>
public class BlazorAppFixture : IAsyncLifetime
{
    private Process? _process;

    public async Task InitializeAsync()
    {
        // CI strategy: app is already running, APP_URL env var points to it
        if (Environment.GetEnvironmentVariable("APP_URL") is not null)
            return;

        var solutionRoot = FindSolutionRoot();
        var demoProject = Path.Combine(solutionRoot, "src", "BlazingSpire.Demo", "BlazingSpire.Demo.csproj");

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FindDotnet(),
                Arguments = $"run --project \"{demoProject}\" --urls https://localhost:5001",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            }
        };
        _process.Start();

        using var http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        });

        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await http.GetAsync("https://localhost:5001");
                if (response.IsSuccessStatusCode) return;
            }
            catch { /* not ready yet */ }
            await Task.Delay(500);
        }

        throw new TimeoutException("BlazingSpire demo app did not start within 60 seconds");
    }

    public Task DisposeAsync()
    {
        _process?.Kill(entireProcessTree: true);
        _process?.Dispose();
        return Task.CompletedTask;
    }

    private static string FindDotnet()
    {
        var homeDotnet = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dotnet", "dotnet");
        return File.Exists(homeDotnet) ? homeDotnet : "dotnet";
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("BlazingSpire.sln").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Could not find BlazingSpire.sln — solution root not found");
    }
}

[CollectionDefinition("BlazorApp")]
public class BlazorAppCollection : ICollectionFixture<BlazorAppFixture>;
