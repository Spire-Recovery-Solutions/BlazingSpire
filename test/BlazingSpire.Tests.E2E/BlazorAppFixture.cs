using System.Diagnostics;

namespace BlazingSpire.Tests.E2E;

/// <summary>
/// Starts the BlazingSpire demo app exactly once per test assembly run and shares it
/// across all parallel test classes. Uses a static Lazy-style initializer behind a
/// SemaphoreSlim so any number of <see cref="IClassFixture{T}"/> instances can safely
/// call <see cref="InitializeAsync"/> — only the first one launches the process.
///
/// Each test class declares <c>IClassFixture&lt;BlazorAppFixture&gt;</c> (instead of the
/// older <c>[Collection("BlazorApp")]</c>) so xUnit treats each class as its own
/// collection and parallelizes them up to <c>maxParallelThreads</c>.
/// </summary>
public class BlazorAppFixture : IAsyncLifetime
{
    private static readonly SemaphoreSlim s_lock = new(1, 1);
    private static Process? s_process;
    private static string? s_baseUrl;

    public string BaseUrl => s_baseUrl
        ?? throw new InvalidOperationException("BlazorAppFixture not initialized");

    public async Task InitializeAsync()
    {
        if (s_baseUrl is not null) return;

        await s_lock.WaitAsync();
        try
        {
            if (s_baseUrl is not null) return;

            // CI strategy: app is already running, APP_URL env var points to it
            var externalUrl = Environment.GetEnvironmentVariable("APP_URL");
            if (externalUrl is not null)
            {
                s_baseUrl = externalUrl;
                return;
            }

            var url = "http://localhost:5299";

            // Kill any orphaned blazor-devserver from a previous killed-mid-flight
            // test run. Without this, the new fixture silently fails to bind and
            // every subsequent test points at the zombie server with stale state.
            KillOrphansOnPort(5299);

            Environment.SetEnvironmentVariable("APP_URL", url);

            var solutionRoot = FindSolutionRoot();
            var demoProject = Path.Combine(solutionRoot, "src", "BlazingSpire.Demo", "BlazingSpire.Demo.csproj");

            s_process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FindDotnet(),
                    Arguments = $"run --project \"{demoProject}\" --urls {url} --no-launch-profile",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Environment =
                    {
                        ["ASPNETCORE_URLS"] = url,
                    }
                }
            };
            s_process.Start();

            // Ensure the child process is killed when the test runner exits even on crash
            AppDomain.CurrentDomain.ProcessExit += (_, _) => TryKill();

            using var http = new HttpClient();

            // Blazor WASM cold start can take 90+ seconds — WASM compilation, asset serving, etc.
            var deadline = DateTime.UtcNow.AddSeconds(180);
            while (DateTime.UtcNow < deadline)
            {
                if (s_process.HasExited)
                {
                    var stderr = await s_process.StandardError.ReadToEndAsync();
                    var stdout = await s_process.StandardOutput.ReadToEndAsync();
                    throw new InvalidOperationException(
                        $"Demo app exited with code {s_process.ExitCode}.\nSTDERR:\n{stderr}\nSTDOUT:\n{stdout}");
                }
                try
                {
                    var response = await http.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        s_baseUrl = url;
                        return;
                    }
                }
                catch { /* not ready yet */ }
                await Task.Delay(1000);
            }

            throw new TimeoutException($"BlazingSpire demo app did not start at {url} within 180 seconds");
        }
        finally
        {
            s_lock.Release();
        }
    }

    // Intentionally a no-op — the server lives for the whole assembly run and is killed
    // via the ProcessExit hook. If we killed it here, the first class to finish would
    // take the server down for everyone else running in parallel.
    public Task DisposeAsync() => Task.CompletedTask;

    private static void TryKill()
    {
        try { s_process?.Kill(entireProcessTree: true); } catch { }
        try { s_process?.Dispose(); } catch { }
        s_process = null;
    }

    private static void KillOrphansOnPort(int port)
    {
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux()) return;
        try
        {
            using var lsof = Process.Start(new ProcessStartInfo("lsof", $"-ti :{port}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
            });
            if (lsof is null) return;
            var output = lsof.StandardOutput.ReadToEnd();
            lsof.WaitForExit(2000);
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(line.Trim(), out var pid))
                {
                    try { Process.GetProcessById(pid).Kill(entireProcessTree: true); } catch { }
                }
            }
            Thread.Sleep(500); // give kernel time to release the port
        }
        catch { /* lsof unavailable; let the startup error surface normally */ }
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
