using dotRAG.API.Infrastructure.RAG;
using dotRAG.API.Models;

namespace dotRAG.Tests;

public sealed class InMemoryVectorStoreTests
{
    [Fact]
    public void Empty_AllChunks_IsEmpty_AndDimensionIsZero()
    {
        var store = new InMemoryVectorStore();
        Assert.Empty(store.AllChunks());
        Assert.Equal(0, store.EmbeddingDimension);
    }

    [Fact]
    public void AllChunks_ReturnsChunksInInsertionOrder()
    {
        var store = new InMemoryVectorStore();
        var a = new NoteChunk("File-A", "H1", "alpha content here for the test.");
        var b = new NoteChunk("File-B", "H2", "beta content here for the test.");
        store.Add(a, [1f, 0f, 0f]);
        store.Add(b, [0f, 1f, 0f]);

        var all = store.AllChunks();
        Assert.Equal(2, all.Count);
        Assert.Same(a, all[0]);
        Assert.Same(b, all[1]);
    }

    [Fact]
    public void EmbeddingDimension_ReflectsFirstEmbeddingLength()
    {
        var store = new InMemoryVectorStore();
        store.Add(new NoteChunk("F", "H", "content body for the chunk."), new float[] { 1f, 2f, 3f, 4f });
        Assert.Equal(4, store.EmbeddingDimension);
    }
}
