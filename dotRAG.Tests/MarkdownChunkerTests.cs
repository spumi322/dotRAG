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

    [Fact]
    public void H3UnderH2_HeadingIncludesParentContext()
    {
        var md = "# Title\n## Parent Section\nFiller content for the parent section area.\n### Child Section\nContent long enough for the child section to be included.";
        var result = _chunker.Chunk("file.md", md);
        var child = result.First(c => c.Content.Contains("child section"));
        Assert.Equal("Parent Section > Child Section", child.Heading);
    }

    [Fact]
    public void H2UnderH1_HeadingIsJustH2()
    {
        var md = "# Title\n## Section\nContent long enough for this section to pass the minimum length check.";
        var result = _chunker.Chunk("file.md", md);
        var section = result.First(c => c.Content.Contains("this section"));
        Assert.Equal("Section", section.Heading);
    }

    [Fact]
    public void SiblingH3s_EachGetsParentBreadcrumb()
    {
        var md = "# Title\n## Parent\nFiller content for parent section.\n### Alpha\nContent long enough for Alpha section here.\n### Beta\nContent long enough for Beta section here.";
        var result = _chunker.Chunk("file.md", md);
        var alpha = result.First(c => c.Content.Contains("Alpha section"));
        var beta = result.First(c => c.Content.Contains("Beta section"));
        Assert.Equal("Parent > Alpha", alpha.Heading);
        Assert.Equal("Parent > Beta", beta.Heading);
    }

    [Fact]
    public void NewH2_ResetsH3Context()
    {
        var md = "# Title\n## First Parent\nFiller for first parent section.\n### Child One\nContent long enough for child one section.\n## Second Parent\nFiller for second parent section.\n### Child Two\nContent long enough for child two section.";
        var result = _chunker.Chunk("file.md", md);
        var childTwo = result.First(c => c.Content.Contains("child two"));
        Assert.Equal("Second Parent > Child Two", childTwo.Heading);
    }

    [Fact]
    public void LongSection_SplitIntoParagraphChunks()
    {
        var paragraph = string.Join(" ", Enumerable.Repeat("This is a paragraph with enough words to take up a reasonable amount of space in the chunk content area.", 10));
        var md = $"# Title\n## Section\n{paragraph}\n\n{paragraph}\n\n{paragraph}\n\n{paragraph}";
        var chunker = new MarkdownChunker(); // default 1500 chars
        var result = chunker.Chunk("file.md", md);
        var sectionChunks = result.Where(c => c.Heading == "Section").ToList();
        Assert.True(sectionChunks.Count > 1, $"Expected multiple chunks but got {sectionChunks.Count}");
        Assert.All(sectionChunks, c => Assert.Equal("Section", c.Heading));
    }

    [Fact]
    public void SplitChunks_RespectMaxSize()
    {
        var paragraph = string.Join(" ", Enumerable.Repeat("This is a paragraph with enough words to take up a reasonable amount of space in the chunk content area.", 10));
        var md = $"# Title\n## Section\n{paragraph}\n\n{paragraph}\n\n{paragraph}\n\n{paragraph}";
        var chunker = new MarkdownChunker();
        var result = chunker.Chunk("file.md", md);
        var sectionChunks = result.Where(c => c.Heading == "Section").ToList();
        Assert.All(sectionChunks, c => Assert.True(c.Content.Length <= 2000 || !c.Content.Contains("\n\n")));
    }

    [Fact]
    public void SplitChunks_DiscardTinyTrailing()
    {
        var paragraph = string.Join(" ", Enumerable.Repeat("This is a paragraph with enough words to take up a reasonable amount of space in the chunk content area.", 10));
        var md = $"# Title\n## Section\n{paragraph}\n\n{paragraph}\n\nTiny.";
        var chunker = new MarkdownChunker();
        var result = chunker.Chunk("file.md", md);
        Assert.All(result, c => Assert.True(c.Content.Length >= 20));
    }
}
