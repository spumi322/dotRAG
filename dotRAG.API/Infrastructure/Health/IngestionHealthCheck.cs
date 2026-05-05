using dotRAG.API.Infrastructure.RAG;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace dotRAG.API.Infrastructure.Health;

internal sealed class IngestionHealthCheck : IHealthCheck
{
    private readonly NotesIngestionService _ingestion;
    private readonly InMemoryVectorStore   _store;

    public IngestionHealthCheck(NotesIngestionService ingestion, InMemoryVectorStore store)
    {
        _ingestion = ingestion;
        _store     = store;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext ctx, CancellationToken ct = default)
    {
        var data = new Dictionary<string, object>
        {
            ["chunks"] = _store.Count,
            ["files"]  = _store.FileCount,
        };

        return Task.FromResult(_ingestion.IsReady
            ? HealthCheckResult.Healthy("Notes ingestion complete.", data)
            : HealthCheckResult.Degraded("Notes ingestion in progress.", data: data));
    }
}
