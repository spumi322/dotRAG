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
        Assert.Contains("Stack vs Heap", result);
        Assert.Contains("What is the stack?", result);
        Assert.DoesNotContain("Conversation History", result);
    }

    [Fact]
    public void WithHistory_ContainsHistorySection()
    {
        var history = new List<HistoryMessage> { new("user", "Prior Q"), new("assistant", "Prior A") };
        var result  = Make().Build("Follow-up?", OneChunk, history);
        Assert.Contains("Conversation History", result);
        Assert.Contains("Prior Q", result);
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
        Assert.NotNull(result);
        Assert.Contains("Final?", result);
    }

    [Fact]
    public void EmptyHistory_BehavesLikeNoHistory()
    {
        var result = Make().Build("Q?", OneChunk, []);
        Assert.DoesNotContain("Conversation History", result);
    }
}
