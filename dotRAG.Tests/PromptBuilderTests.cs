using dotRAG.API.Application;
using dotRAG.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace dotRAG.Tests;

public sealed class PromptBuilderTests
{
    private static PromptBuilder Make(int maxTokens = 8192)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["MaxPromptTokens"] = maxTokens.ToString() })
            .Build();
        return new PromptBuilder(config, NullLogger<PromptBuilder>.Instance);
    }

    private static readonly IReadOnlyList<NoteChunk> OneChunk =
        [new NoteChunk("Module1", "Stack vs Heap", "Stack is LIFO. Heap is garbage collected.")];

    [Fact]
    public void NoHistory_ContainsQuestionAndNotes_NoHistorySection()
    {
        var result = Make().Build("What is the stack?", OneChunk);
        Assert.Contains("Stack vs Heap", result.Prompt);
        Assert.Contains("What is the stack?", result.Prompt);
        Assert.DoesNotContain("Conversation History", result.Prompt);
        Assert.Equal(0, result.HistoryIncluded);
        Assert.Equal(0, result.HistoryTrimmed);
        Assert.Equal(8192, result.MaxTokens);
    }

    [Fact]
    public void MaxTokens_ReflectsConfiguredBudget()
    {
        var result = Make(maxTokens: 4096).Build("Q?", OneChunk);
        Assert.Equal(4096, result.MaxTokens);
    }

    [Fact]
    public void WithHistory_ContainsHistorySection()
    {
        var history = new List<HistoryMessage> { new("user", "Prior Q"), new("assistant", "Prior A") };
        var result  = Make().Build("Follow-up?", OneChunk, history);
        Assert.Contains("Conversation History", result.Prompt);
        Assert.Contains("Prior Q", result.Prompt);
        Assert.Equal(2, result.HistoryIncluded);
        Assert.Equal(0, result.HistoryTrimmed);
    }

    [Fact]
    public void ExceedsBudget_TrimsOldestHistoryPair()
    {
        // Budget so small the long history entries force trimming.
        var history = new List<HistoryMessage>
        {
            new("user",      new string('a', 200)),
            new("assistant", new string('b', 200)),
            new("user",      "Short"),
            new("assistant", "Short")
        };
        var result = Make(maxTokens: 50).Build("Final?", OneChunk, history);
        Assert.NotNull(result.Prompt);
        Assert.Contains("Final?", result.Prompt);
        Assert.True(result.HistoryTrimmed > 0);
    }

    [Fact]
    public void EmptyHistory_BehavesLikeNoHistory()
    {
        var result = Make().Build("Q?", OneChunk, []);
        Assert.DoesNotContain("Conversation History", result.Prompt);
        Assert.Equal(0, result.HistoryIncluded);
    }
}
