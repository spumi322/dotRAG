using dotRAG.API.Application;
using dotRAG.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace dotRAG.Tests;

public sealed class ChatServiceTests
{
    private readonly Mock<INotesSearchService>  _search   = new();
    private readonly Mock<IPromptBuilder>       _prompt   = new();
    private readonly Mock<ILlmService>          _llm      = new();
    private readonly Mock<IQueryRewriter>       _rewriter = new();
    private readonly Mock<IHttpContextAccessor> _http     = new();

    private static readonly NotesSearchResult EmptySearch =
        new(Chunks: [], EmbedMs: 0, SearchMs: 0, EmbeddingDim: 1024);

    private static readonly PromptBuildResult DummyPrompt =
        new(Prompt: "prompt", EstimatedTokens: 10, MaxTokens: 8192, HistoryIncluded: 0, HistoryTrimmed: 0);

    private static PipelineTraceStore Store() =>
        new(new ConfigurationBuilder().Build());

    private ChatService Sut() => new(
        _search.Object, _prompt.Object, _llm.Object, _rewriter.Object,
        Store(), _http.Object, NullLogger<ChatService>.Instance);

    [Fact]
    public async Task NoHistory_RewriterNotCalled()
    {
        _search .Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(EmptySearch);
        _prompt .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns(DummyPrompt);
        _llm    .Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("answer");

        await Sut().AskAsync(new ChatRequest("Q?"));

        _rewriter.Verify(r => r.RewriteAsync(It.IsAny<IReadOnlyList<HistoryMessage>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WithHistory_RewriterCalled_SearchUsesRewrittenQuery()
    {
        var history = new List<HistoryMessage> { new("user", "Q1"), new("assistant", "A1") };
        _rewriter.Setup(r => r.RewriteAsync(history, "Q2", It.IsAny<CancellationToken>())).ReturnsAsync("Rewritten Q2");
        _search  .Setup(s => s.SearchAsync("Rewritten Q2", It.IsAny<CancellationToken>())).ReturnsAsync(EmptySearch);
        _prompt  .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns(DummyPrompt);
        _llm     .Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("answer");

        await Sut().AskAsync(new ChatRequest("Q2", history));

        _rewriter.Verify(r => r.RewriteAsync(history, "Q2", It.IsAny<CancellationToken>()), Times.Once);
        _search  .Verify(s => s.SearchAsync("Rewritten Q2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithHistory_OriginalQuestionForwardedToPromptBuilder()
    {
        var history = new List<HistoryMessage> { new("user", "Q1"), new("assistant", "A1") };
        _rewriter.Setup(r => r.RewriteAsync(It.IsAny<IReadOnlyList<HistoryMessage>>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("Rewritten");
        _search  .Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(EmptySearch);
        _prompt  .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns(DummyPrompt);
        _llm     .Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("answer");

        await Sut().AskAsync(new ChatRequest("Original Q", history));

        _prompt.Verify(p => p.Build("Original Q", It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>()), Times.Once);
    }
}
