using dotRAG.API.Application;
using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class NotesSearchService : INotesSearchService
{
    private readonly IEmbeddingService   _embedder;
    private readonly InMemoryVectorStore _store;
    private readonly int   _topK;
    private readonly float _minScore;
    private readonly ILogger<NotesSearchService> _logger;

    public NotesSearchService(IEmbeddingService embedder, InMemoryVectorStore store, IConfiguration config, ILogger<NotesSearchService> logger)
    {
        _embedder = embedder;
        _store    = store;
        _topK     = config.GetValue<int>  ("Retrieval:TopK",     3);
        _minScore = config.GetValue<float>("Retrieval:MinScore", 0.5f);
        _logger   = logger;
    }

    public async Task<IReadOnlyList<NoteChunk>> SearchAsync(string question, CancellationToken ct = default)
    {
        _logger.LogInformation("Searching notes for: {Query}", question);

        var embedding = await _embedder.EmbedAsync(question, "query", ct);
        var chunks = _store.Search(embedding, _topK, _minScore);

        if (chunks.Count == 0)
        {
            _logger.LogWarning("No chunks found above minimum score {MinScore} for query: {Query}", _minScore, question);
        }
        else
        {
            foreach (var c in chunks)
                _logger.LogInformation("  Matched chunk: {Source} — {Heading}", c.SourceFile, c.Heading);
        }

        return chunks;
    }
}
