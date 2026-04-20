using dotRAG.API.Application;
using dotRAG.API.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace dotRAG.Tests;

public sealed class ChatServiceTests
{
    private readonly Mock<INotesSearchService> _search   = new();
    private readonly Mock<IPromptBuilder>      _prompt   = new();
    private readonly Mock<ILlmService>         _llm      = new();
    private readonly Mock<IQueryRewriter>      _rewriter = new();

    private ChatService Sut() => new(_search.Object, _prompt.Object, _llm.Object, _rewriter.Object, NullLogger<ChatService>.Instance);

    [Fact]
    public async Task NoHistory_RewriterNotCalled()
    {
        _search .Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _prompt .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns("prompt");
        _llm    .Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("answer");

        await Sut().AskAsync(new ChatRequest("Q?"));

        _rewriter.Verify(r => r.RewriteAsync(It.IsAny<IReadOnlyList<HistoryMessage>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WithHistory_RewriterCalled_SearchUsesRewrittenQuery()
    {
        var history = new List<HistoryMessage> { new("user", "Q1"), new("assistant", "A1") };
        _rewriter.Setup(r => r.RewriteAsync(history, "Q2", It.IsAny<CancellationToken>())).ReturnsAsync("Rewritten Q2");
        _search  .Setup(s => s.SearchAsync("Rewritten Q2", It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _prompt  .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns("prompt");
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
        _search  .Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _prompt  .Setup(p => p.Build(It.IsAny<string>(), It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>())).Returns("prompt");
        _llm     .Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("answer");

        await Sut().AskAsync(new ChatRequest("Original Q", history));

        _prompt.Verify(p => p.Build("Original Q", It.IsAny<IReadOnlyList<NoteChunk>>(), It.IsAny<IReadOnlyList<HistoryMessage>?>()), Times.Once);
    }
}
