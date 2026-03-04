using dotRAG.API.Infrastructure.RAG;

namespace dotRAG.Tests;

public sealed class MarkdownChunkerTests
{
    private readonly MarkdownChunker _chunker = new();

    [Fact]
    public void SingleHeading_ReturnsOneChunk()
    {
        var md = "# Section\nThis is content that is long enough to pass the minimum length check.";
        var result = _chunker.Chunk("file.md", md);
        Assert.Single(result);
        Assert.Equal("Section", result[0].Heading);
    }

    [Fact]
    public void MultipleHeadings_ReturnsMultipleChunks()
    {
        var md = "# A\nContent long enough for section A.\n# B\nContent long enough for section B.";
        var result = _chunker.Chunk("file.md", md);
        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Heading);
        Assert.Equal("B", result[1].Heading);
    }

    [Fact]
    public void WikiLinks_AreStripped_InnerTextPreserved()
    {
        var md = "# Section\nSee [[Stack vs Heap]] for more on memory allocation in .NET runtime.";
        var result = _chunker.Chunk("file.md", md);
        Assert.Single(result);
        Assert.Contains("Stack vs Heap", result[0].Content);
        Assert.DoesNotContain("[[", result[0].Content);
    }

    [Fact]
    public void BelowMinLength_IsOmitted()
    {
        var md = "# Section\nTiny.";
        var result = _chunker.Chunk("file.md", md);
        Assert.Empty(result);
    }

    [Fact]
    public void SourceFile_IsFileNameWithoutExtension()
    {
        var md = "# Heading\nContent long enough to be included in the results.";
        var result = _chunker.Chunk("/notes/Module1.md", md);
        Assert.Equal("Module1", result[0].SourceFile);
    }
}
