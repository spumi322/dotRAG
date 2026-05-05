using dotRAG.API.Models;

namespace dotRAG.API.Application;

// Per-query trace assembled by ChatService and persisted to PipelineTraceStore.
// Five stages: rewrite (optional), embed, search, build, llm. Ingestion stats
// live elsewhere (status footer / health endpoint).
public sealed record PipelineTrace(
    string CorrelationId,
    DateTimeOffset Timestamp,
    string Question,
    string? RewrittenQuery,
    StageTiming? QueryRewrite,
    StageTiming Embedding,
    StageTiming VectorSearch,
    StageTiming PromptBuild,
    StageTiming LlmComplete,
    long TotalMs,
    IReadOnlyList<ChunkDto> RetrievedChunks);

public sealed record StageTiming(long Ms, IReadOnlyDictionary<string, object?> Meta);
