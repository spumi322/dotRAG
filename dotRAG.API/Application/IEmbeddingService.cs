namespace dotRAG.API.Application;

public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, string? inputType = null, CancellationToken ct = default);
    Task<float[][]> EmbedBatchAsync(IReadOnlyList<string> texts, string? inputType = null, CancellationToken ct = default);
}
