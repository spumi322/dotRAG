using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using dotRAG.API.Application;

namespace dotRAG.API.Infrastructure.Embeddings;

internal sealed class VoyageEmbeddingService : IEmbeddingService
{
    private const string Endpoint  = "https://api.voyageai.com/v1/embeddings";
    private const string ModelName = "voyage-3-large";

    public string Model => ModelName;

    private readonly IHttpClientFactory _http;
    private readonly string _apiKey;
    private readonly ILogger<VoyageEmbeddingService> _logger;

    public VoyageEmbeddingService(IHttpClientFactory http, IConfiguration config, ILogger<VoyageEmbeddingService> logger)
    {
        _http   = http;
        _apiKey = config["ApiKeys:VoyageApiKey"]
            ?? throw new InvalidOperationException("ApiKeys:VoyageApiKey not configured.");
        _logger = logger;
    }

    public async Task<float[]> EmbedAsync(string text, string? inputType = null, CancellationToken ct = default)
    {
        var results = await EmbedBatchAsync([text], inputType, ct);
        return results[0];
    }

    public async Task<float[][]> EmbedBatchAsync(IReadOnlyList<string> texts, string? inputType = null, CancellationToken ct = default)
    {
        using var client = _http.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var sw = Stopwatch.StartNew();
        var resp = await client.PostAsJsonAsync(Endpoint, new EmbedReq(texts.ToArray(), ModelName, inputType), ct);
        if (!resp.IsSuccessStatusCode)
        {
            var errorBody = await resp.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Voyage API {(int)resp.StatusCode}: {errorBody}");
        }

        var body = await resp.Content.ReadFromJsonAsync<EmbedResp>(ct)
            ?? throw new InvalidOperationException("Voyage API returned null.");
        sw.Stop();

        var embeddings = body.Data.OrderBy(d => d.Index).Select(d => d.Embedding).ToArray();
        _logger.LogInformation("[Embedding] {Count} texts embedded in {Elapsed}ms (dim={Dimensions})",
            texts.Count, sw.ElapsedMilliseconds, embeddings[0].Length);
        return embeddings;
    }

    private record EmbedReq(
        [property: JsonPropertyName("input")]      string[] Input,
        [property: JsonPropertyName("model")]      string   Model,
        [property: JsonPropertyName("input_type")] string?  InputType);
    private record EmbedResp(
        [property: JsonPropertyName("data")]  EmbedData[] Data);
    private record EmbedData(
        [property: JsonPropertyName("index")]     int Index,
        [property: JsonPropertyName("embedding")] float[] Embedding);
}
