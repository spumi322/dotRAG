using dotRAG.API.Application;
using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class NotesSearchService : INotesSearchService
{
    private const int TopK = 3;

    private readonly IEmbeddingService   _embedder;
    private readonly InMemoryVectorStore _store;

    public NotesSearchService(IEmbeddingService embedder, InMemoryVectorStore store)
        => (_embedder, _store) = (embedder, store);

    public async Task<IReadOnlyList<NoteChunk>> SearchAsync(string question, CancellationToken ct = default)
    {
        var embedding = await _embedder.EmbedAsync(question, ct);
        return _store.Search(embedding, TopK);
    }
}
