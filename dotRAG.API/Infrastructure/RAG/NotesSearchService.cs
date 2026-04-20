using dotRAG.API.Application;
using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class NotesSearchService : INotesSearchService
{
    private readonly IEmbeddingService   _embedder;
    private readonly InMemoryVectorStore _store;
    private readonly int   _topK;
    private readonly float _minScore;

    public NotesSearchService(IEmbeddingService embedder, InMemoryVectorStore store, IConfiguration config)
    {
        _embedder = embedder;
        _store    = store;
        _topK     = config.GetValue<int>  ("Retrieval:TopK",     3);
        _minScore = config.GetValue<float>("Retrieval:MinScore", 0.5f);
    }

    public async Task<IReadOnlyList<NoteChunk>> SearchAsync(string question, CancellationToken ct = default)
    {
        var embedding = await _embedder.EmbedAsync(question, "query", ct);
        return _store.Search(embedding, _topK, _minScore);
    }
}
