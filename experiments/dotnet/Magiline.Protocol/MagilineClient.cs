using System.Net.Http.Json;
using System.Text.Json;

namespace Magiline.Protocol;

public sealed class MagilineClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsClient;

    public MagilineClient(string host, int port = 11000, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("L'adresse du boîtier est obligatoire.", nameof(host));

        _ownsClient = httpClient is null;
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri($"http://{host}:{port}");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<JsonDocument> GetPoolInfoAsync(CancellationToken cancellationToken = default)
        => await GetJsonAsync(MagilineEndpoints.PoolInfo, cancellationToken);

    public async Task<JsonDocument> GetPoolStateAsync(CancellationToken cancellationToken = default)
        => await GetJsonAsync(MagilineEndpoints.PoolLocal, cancellationToken);

    public async Task SendSpotlightAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        var wanted = enabled ? 2 : 1;
        await PostModeAsync(MagilineEndpoints.Spotlight, wanted, cancellationToken);
    }

    public async Task SendFiltrationModeAsync(int wanted, CancellationToken cancellationToken = default)
    {
        if (wanted is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(wanted), "La valeur wanted doit être comprise entre 0 et 5.");

        await PostModeAsync(MagilineEndpoints.ConfigureFiltration, wanted, cancellationToken);
    }

    private async Task<JsonDocument> GetJsonAsync(string endpoint, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new MagilineApiException(endpoint, response.StatusCode, content);

        return JsonDocument.Parse(content);
    }

    private async Task PostModeAsync(string endpoint, int wanted, CancellationToken cancellationToken)
    {
        var payload = new { mode = new { wanted } };
        using var response = await _httpClient.PostAsJsonAsync(endpoint, payload, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new MagilineApiException(endpoint, response.StatusCode, content);
    }

    public void Dispose()
    {
        if (_ownsClient)
            _httpClient.Dispose();
    }
}
