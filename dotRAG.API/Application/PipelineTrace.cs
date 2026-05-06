using dotRAG.API.Models;

namespace dotRAG.API.Application;

// Per-query trace. Stages are nullable so the same shape works for both
// completed traces (all stages present) and in-flight traces being filled
// in stage-by-stage. `Running` flips to false once the pipeline finishes.
public sealed record PipelineTrace(
    string CorrelationId,
    DateTimeOffset Timestamp,
    string Question,
    string? RewrittenQuery,
    StageTiming? QueryRewrite,
    StageTiming? Embedding,
    StageTiming? VectorSearch,
    StageTiming? PromptBuild,
    StageTiming? LlmComplete,
    long TotalMs,
    bool Running,
    IReadOnlyList<ChunkDto> RetrievedChunks);

public sealed record StageTiming(long Ms, IReadOnlyDictionary<string, object?> Meta);

public sealed record RecentTraceSummary(
    string CorrelationId,
    DateTimeOffset Timestamp,
    string Question,
    long TotalMs,
    bool Running);
