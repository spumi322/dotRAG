using System.Text.Json;
using System.Text.Json.Nodes;
using dotRAG.API.Infrastructure.RAG;
using dotRAG.API.Models;

namespace dotRAG.API.Application;

internal sealed class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions WriteOpts = new() { WriteIndented = true };

    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly NotesIngestionService _ingestion;
    private readonly ILogger<SettingsService> _logger;

    public SettingsService(
        IConfiguration config,
        IWebHostEnvironment env,
        NotesIngestionService ingestion,
        ILogger<SettingsService> logger)
    {
        _config    = config;
        _env       = env;
        _ingestion = ingestion;
        _logger    = logger;
    }

    private string OverrideFilePath =>
        Path.Combine(_env.ContentRootPath, "user-settings.json");

    public SettingsDto GetEffective() => new(
        NotesPath:        _config["NotesPath"]                                ?? "../Notes",
        FileGlob:         _config["Ingestion:FileGlob"]                       ?? "*.md",
        MaxChunkChars:    _config.GetValue("Chunking:MaxChunkChars",  2000),
        HeadingDepth:     _config.GetValue("Chunking:HeadingDepth",   3),
        MinChunkLength:   _config.GetValue("Chunking:MinChunkLength", 20),
        EmbeddingModel:   _config["Embedding:Model"]                          ?? "voyage-3-large",
        Provider:         "OpenRouter",
        LlmModel:         _config["OpenRouter:Model"],
        OpenRouterApiKey: Mask(_config["ApiKeys:OpenRouterApiKey"]),
        VoyageApiKey:     Mask(_config["ApiKeys:VoyageApiKey"]),
        TopK:             _config.GetValue("Retrieval:TopK",     3),
        MinScore:         _config.GetValue("Retrieval:MinScore", 0.5),
        MaxPromptTokens:  _config.GetValue("MaxPromptTokens",    8192));

    public async Task<SaveSettingsResult> SaveAsync(SettingsDto incoming, CancellationToken ct = default)
    {
        var current = GetEffective();
        var ingestionChanged =
            incoming.NotesPath      != current.NotesPath      ||
            incoming.FileGlob       != current.FileGlob       ||
            incoming.MaxChunkChars  != current.MaxChunkChars  ||
            incoming.HeadingDepth   != current.HeadingDepth   ||
            incoming.MinChunkLength != current.MinChunkLength ||
            incoming.EmbeddingModel != current.EmbeddingModel;

        var root = await LoadOverridesAsync(ct);

        Set(root, "NotesPath",                  incoming.NotesPath);
        SetIn(root, "Ingestion", "FileGlob",    incoming.FileGlob);
        SetIn(root, "Chunking", "MaxChunkChars",  incoming.MaxChunkChars);
        SetIn(root, "Chunking", "HeadingDepth",   incoming.HeadingDepth);
        SetIn(root, "Chunking", "MinChunkLength", incoming.MinChunkLength);
        SetIn(root, "Embedding", "Model",       incoming.EmbeddingModel);
        SetIn(root, "OpenRouter", "Model",      incoming.LlmModel);
        SetIn(root, "Retrieval", "TopK",        incoming.TopK);
        SetIn(root, "Retrieval", "MinScore",    incoming.MinScore);
        Set(root, "MaxPromptTokens",            incoming.MaxPromptTokens);

        // API keys: null means "leave alone" (UI never sees the unmasked value)
        if (!string.IsNullOrWhiteSpace(incoming.OpenRouterApiKey) && !IsMasked(incoming.OpenRouterApiKey))
            SetIn(root, "ApiKeys", "OpenRouterApiKey", incoming.OpenRouterApiKey);
        if (!string.IsNullOrWhiteSpace(incoming.VoyageApiKey) && !IsMasked(incoming.VoyageApiKey))
            SetIn(root, "ApiKeys", "VoyageApiKey", incoming.VoyageApiKey);

        await SaveOverridesAsync(root, ct);

        if (ingestionChanged)
        {
            _logger.LogInformation("[Settings] Ingestion-dependent change detected — triggering re-ingest");
            // Fire-and-forget: PUT returns immediately, /health flips to not-ready,
            // frontend banner polls until ingestion completes.
            _ = _ingestion.TriggerReingestAsync();
        }

        return new SaveSettingsResult(ingestionChanged);
    }

    public async Task ResetToDefaultsAsync(CancellationToken ct = default)
    {
        var path = OverrideFilePath;
        if (File.Exists(path))
        {
            File.Delete(path);
            _logger.LogInformation("[Settings] Deleted override file {Path}", path);
        }

        // user-settings.json removal triggers reloadOnChange; next read sees defaults.
        // Re-ingest with the restored config so the vector store reflects defaults too.
        _ = _ingestion.TriggerReingestAsync();
        await Task.CompletedTask;
    }

    private async Task<JsonObject> LoadOverridesAsync(CancellationToken ct)
    {
        var path = OverrideFilePath;
        if (!File.Exists(path))
            return new JsonObject();

        try
        {
            await using var fs = File.OpenRead(path);
            var node = await JsonNode.ParseAsync(fs, cancellationToken: ct);
            return node as JsonObject ?? new JsonObject();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Settings] Override file unreadable, starting fresh");
            return new JsonObject();
        }
    }

    private async Task SaveOverridesAsync(JsonObject root, CancellationToken ct)
    {
        var path = OverrideFilePath;
        var json = root.ToJsonString(WriteOpts);
        await File.WriteAllTextAsync(path, json, ct);
        _logger.LogInformation("[Settings] Wrote {Bytes} bytes to {Path}", json.Length, path);
    }

    private static void Set(JsonObject root, string key, JsonNode? value) =>
        root[key] = value;

    private static void Set(JsonObject root, string key, string? value) =>
        root[key] = value is null ? null : JsonValue.Create(value);

    private static void Set(JsonObject root, string key, int value) =>
        root[key] = JsonValue.Create(value);

    private static void SetIn(JsonObject root, string section, string key, JsonNode? value)
    {
        if (root[section] is not JsonObject sect)
        {
            sect = new JsonObject();
            root[section] = sect;
        }
        sect[key] = value;
    }

    private static void SetIn(JsonObject root, string section, string key, string? value) =>
        SetIn(root, section, key, value is null ? null : JsonValue.Create(value));

    private static void SetIn(JsonObject root, string section, string key, int value) =>
        SetIn(root, section, key, JsonValue.Create(value));

    private static void SetIn(JsonObject root, string section, string key, double value) =>
        SetIn(root, section, key, JsonValue.Create(value));

    private static string? Mask(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        if (value.Length <= 8) return new string('*', value.Length);
        var prefix = value[..Math.Min(4, value.Length)];
        var suffix = value[^4..];
        return $"{prefix}••••••••{suffix}";
    }

    private static bool IsMasked(string value) => value.Contains('•');
}
