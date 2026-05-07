namespace dotRAG.API.Models;

/// <summary>
/// Effective (merged) view of dotRAG settings. Returned by GET /api/settings and
/// accepted by PUT /api/settings. API key fields are masked on read; on write,
/// a null value means "leave unchanged" so the UI never has to round-trip the
/// real secret.
/// </summary>
public sealed record SettingsDto(
    // Ingestion (re-ingest required)
    string NotesPath,
    string FileGlob,
    int    MaxChunkChars,
    int    HeadingDepth,
    int    MinChunkLength,
    string EmbeddingModel,

    // Runtime (effective on next request)
    string  Provider,
    string? LlmModel,
    string? OpenRouterApiKey,
    string? VoyageApiKey,
    int     TopK,
    double  MinScore,
    int     MaxPromptTokens
);

/// <summary>
/// Response from PUT /api/settings — tells the UI whether the save touched any
/// ingestion-dependent key, in which case re-ingestion has been triggered.
/// </summary>
public sealed record SaveSettingsResult(bool ReingestTriggered);
