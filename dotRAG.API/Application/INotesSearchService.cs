using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface INotesSearchService
{
    Task<NotesSearchResult> SearchAsync(string question, CancellationToken ct = default);
}

public sealed record NotesSearchResult(
    IReadOnlyList<ScoredChunk> Chunks,
    long EmbedMs,
    long SearchMs,
    int EmbeddingDim);
