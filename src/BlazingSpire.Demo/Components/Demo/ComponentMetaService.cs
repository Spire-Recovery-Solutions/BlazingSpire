using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BlazingSpire.Demo.Components.Demo;

[JsonSerializable(typeof(List<ComponentMeta>))]
internal sealed partial class ComponentMetaJsonContext : JsonSerializerContext;

public interface IComponentMetaService
{
    Task<IReadOnlyList<ComponentMeta>> GetAllAsync();
    Task<ComponentMeta?> GetAsync(string componentName);
}

public sealed class ComponentMetaService : IComponentMetaService
{
    private readonly HttpClient _http;
    private IReadOnlyList<ComponentMeta>? _cache;

    public ComponentMetaService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ComponentMeta>> GetAllAsync()
    {
        _cache ??= await _http.GetFromJsonAsync("components.json", ComponentMetaJsonContext.Default.ListComponentMeta) ?? [];
        return _cache;
    }

    public async Task<ComponentMeta?> GetAsync(string componentName)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(c => c.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));
    }
}
