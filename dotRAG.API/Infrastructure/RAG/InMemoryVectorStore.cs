using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class InMemoryVectorStore
{
    private readonly List<(NoteChunk Chunk, float[] Embedding)> _store = [];

    public void Add(NoteChunk chunk, float[] embedding) => _store.Add((chunk, embedding));

    public IReadOnlyList<NoteChunk> Search(float[] query, int topK, float minScore = 0f) =>
        _store.Count == 0 ? [] :
        _store
            .Select(e => (e.Chunk, Score: Cosine(query, e.Embedding)))
            .Where(x => x.Score >= minScore)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToList();

    private static float Cosine(float[] a, float[] b)
    {
        float dot = 0, magA = 0, magB = 0;
        var len = Math.Min(a.Length, b.Length);
        for (var i = 0; i < len; i++) { dot += a[i] * b[i]; magA += a[i] * a[i]; magB += b[i] * b[i]; }
        var denom = MathF.Sqrt(magA) * MathF.Sqrt(magB);
        return denom == 0f ? 0f : dot / denom;
    }
}
