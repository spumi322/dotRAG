using System.Diagnostics;
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

    public async Task<NotesSearchResult> SearchAsync(string question, CancellationToken ct = default)
    {
        var embedSw = Stopwatch.StartNew();
        var embedding = await _embedder.EmbedAsync(question, "query", ct);
        embedSw.Stop();

        var searchSw = Stopwatch.StartNew();
        var scored = _store.Search(embedding, _topK, _minScore);
        searchSw.Stop();

        if (scored.Count == 0)
        {
            var best = _store.Search(embedding, 1, minScore: 0f);
            var bestScore = best.Count > 0 ? best[0].Score : 0f;
            _logger.LogWarning(
                "[Retrieval] No chunks above MinScore {MinScore} (best was {BestScore:F3}, searched {Total} chunks)",
                _minScore, bestScore, _store.Count);
        }
        else
        {
            for (var i = 0; i < scored.Count; i++)
            {
                var (chunk, score) = scored[i];
                _logger.LogInformation(
                    "[Retrieval] #{Rank}  score={Score:F3}  {Source} — {Heading}  ({Chars} chars)",
                    i + 1, score, chunk.SourceFile, chunk.Heading, chunk.Content.Length);
            }
        }

        return new NotesSearchResult(
            Chunks:        scored.Select(s => new ScoredChunk(s.Chunk, s.Score)).ToList(),
            EmbedMs:       embedSw.ElapsedMilliseconds,
            SearchMs:      searchSw.ElapsedMilliseconds,
            EmbeddingDim:  embedding.Length);
    }
}
