using dotRAG.API.Infrastructure.RAG;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace dotRAG.API.Infrastructure.Health;

internal sealed class IngestionHealthCheck : IHealthCheck
{
    private readonly NotesIngestionService _ingestion;

    public IngestionHealthCheck(NotesIngestionService ingestion) => _ingestion = ingestion;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext ctx, CancellationToken ct = default) =>
        Task.FromResult(_ingestion.IsReady
            ? HealthCheckResult.Healthy("Notes ingestion complete.")
            : HealthCheckResult.Degraded("Notes ingestion in progress."));
}
