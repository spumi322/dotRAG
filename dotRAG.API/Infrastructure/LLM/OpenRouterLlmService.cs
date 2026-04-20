using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using dotRAG.API.Application;

namespace dotRAG.API.Infrastructure.LLM;

internal sealed class OpenRouterLlmService : ILlmService
{
    private const string Endpoint = "https://openrouter.ai/api/v1/chat/completions";

    private readonly IHttpClientFactory _http;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger<OpenRouterLlmService> _logger;

    public OpenRouterLlmService(IHttpClientFactory http, IConfiguration config, ILogger<OpenRouterLlmService> logger)
    {
        _http   = http;
        _logger = logger;
        _apiKey = config["ApiKeys:OpenRouterApiKey"]
            ?? throw new InvalidOperationException("ApiKeys:OpenRouterApiKey not configured.");
        _model = config["OpenRouter:Model"]
            ?? throw new InvalidOperationException("OpenRouter:Model not configured.");
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        using var client = _http.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var request = new OpenRouterRequest([new OpenRouterMessage("user", prompt)], _model);

        _logger.LogDebug("Sending completion request to OpenRouter (model: {Model})", _model);

        var resp = await client.PostAsJsonAsync(Endpoint, request, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var errorBody = await resp.Content.ReadAsStringAsync(ct);
            _logger.LogError("OpenRouter API error {StatusCode}: {Body}", (int)resp.StatusCode, errorBody);
            resp.EnsureSuccessStatusCode();
        }

        var body = await resp.Content.ReadFromJsonAsync<OpenRouterResponse>(ct)
            ?? throw new InvalidOperationException("OpenRouter API returned null.");

        return body.Choices[0].Message.Content;
    }
}

internal record OpenRouterMessage(
    [property: JsonPropertyName("role")]    string Role,
    [property: JsonPropertyName("content")] string Content);

internal record OpenRouterRequest(
    [property: JsonPropertyName("messages")] OpenRouterMessage[] Messages,
    [property: JsonPropertyName("model")]    string Model);

internal record OpenRouterResponse(
    [property: JsonPropertyName("choices")] OpenRouterChoice[] Choices);

internal record OpenRouterChoice(
    [property: JsonPropertyName("message")] OpenRouterMessage Message);
