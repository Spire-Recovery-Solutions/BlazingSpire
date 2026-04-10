using System.Diagnostics;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Starts the BlazingSpire demo app once and shares it across all tests in the BlazorApp collection.
/// If APP_URL is already set (CI strategy), skips launching a local process.
/// </summary>
public class BlazorAppFixture : IAsyncLifetime
{
    private Process? _process;

    public string BaseUrl { get; private set; } =
        Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:5299";

    public async Task InitializeAsync()
    {
        // CI strategy: app is already running, APP_URL env var points to it
        if (Environment.GetEnvironmentVariable("APP_URL") is not null)
            return;

        // Publish BaseUrl to env so BlazingSpireE2EBase picks it up
        Environment.SetEnvironmentVariable("APP_URL", BaseUrl);

        var solutionRoot = FindSolutionRoot();
        var demoProject = Path.Combine(solutionRoot, "src", "BlazingSpire.Demo", "BlazingSpire.Demo.csproj");

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FindDotnet(),
                Arguments = $"run --project \"{demoProject}\" --urls {BaseUrl} --no-launch-profile",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Environment =
                {
                    // Prevent HTTPS redirect from browser launch behavior
                    ["ASPNETCORE_URLS"] = BaseUrl,
                }
            }
        };
        _process.Start();

        using var http = new HttpClient();

        // Blazor WASM cold start can take 90+ seconds — WASM compilation, asset serving, etc.
        var deadline = DateTime.UtcNow.AddSeconds(180);
        while (DateTime.UtcNow < deadline)
        {
            if (_process.HasExited)
            {
                var stderr = await _process.StandardError.ReadToEndAsync();
                var stdout = await _process.StandardOutput.ReadToEndAsync();
                throw new InvalidOperationException(
                    $"Demo app exited with code {_process.ExitCode}.\nSTDERR:\n{stderr}\nSTDOUT:\n{stdout}");
            }
            try
            {
                var response = await http.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode) return;
            }
            catch { /* not ready yet */ }
            await Task.Delay(1000);
        }

        throw new TimeoutException($"BlazingSpire demo app did not start at {BaseUrl} within 180 seconds");
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
