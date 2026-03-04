using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using dotRAG.API.Application;

namespace dotRAG.API.Infrastructure.Embeddings;

internal sealed class VoyageEmbeddingService : IEmbeddingService
{
    private const string Endpoint = "https://api.voyageai.com/v1/embeddings";
    private const string Model    = "voyage-3";

    private readonly IHttpClientFactory _http;
    private readonly string _apiKey;

    public VoyageEmbeddingService(IHttpClientFactory http, IConfiguration config)
    {
        _http   = http;
        _apiKey = config["ApiKeys:VoyageApiKey"]
            ?? throw new InvalidOperationException("ApiKeys:VoyageApiKey not configured.");
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    {
        using var client = _http.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var resp = await client.PostAsJsonAsync(Endpoint, new EmbedReq([text], Model), ct);
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<EmbedResp>(ct)
            ?? throw new InvalidOperationException("Voyage API returned null.");
        return body.Data[0].Embedding;
    }

    private record EmbedReq(
        [property: JsonPropertyName("input")] string[] Input,
        [property: JsonPropertyName("model")] string   Model);
    private record EmbedResp(
        [property: JsonPropertyName("data")]  EmbedData[] Data);
    private record EmbedData(
        [property: JsonPropertyName("embedding")] float[] Embedding);
}
