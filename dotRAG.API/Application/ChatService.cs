using dotRAG.API.Models;

namespace dotRAG.API.Application;

internal sealed class ChatService : IChatService
{
    private readonly INotesSearchService _search;
    private readonly IPromptBuilder      _prompt;
    private readonly ILlmService         _llm;
    private readonly IQueryRewriter      _rewriter;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        INotesSearchService search,
        IPromptBuilder prompt,
        ILlmService llm,
        IQueryRewriter rewriter,
        ILogger<ChatService> logger)
        => (_search, _prompt, _llm, _rewriter, _logger) = (search, prompt, llm, rewriter, logger);

    public async Task<string> AskAsync(ChatRequest request, CancellationToken ct = default)
    {
        var searchQuery = request.Question;

        if (request.History is { Count: > 0 })
        {
            _logger.LogInformation("[QueryRewrite] Rewriting follow-up question with {HistoryCount} history messages", request.History.Count);
            searchQuery = await _rewriter.RewriteAsync(request.History, request.Question, ct);
            _logger.LogInformation("[QueryRewrite] Rewritten query: {Query}", searchQuery);
        }

        _logger.LogInformation("[Retrieval] Searching for: {Query}", searchQuery);
        var chunks = await _search.SearchAsync(searchQuery, ct);
        _logger.LogInformation("[Retrieval] {ChunkCount} chunks retrieved", chunks.Count);

        _logger.LogInformation("[PromptBuild] Building prompt with {ChunkCount} chunks", chunks.Count);
        var prompt = _prompt.Build(request.Question, chunks, request.History);

        _logger.LogInformation("[LlmCall] Sending prompt to LLM");
        return await _llm.CompleteAsync(prompt, ct);
    }
}
