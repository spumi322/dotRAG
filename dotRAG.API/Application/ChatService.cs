using System.Diagnostics;
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
            _logger.LogInformation("[QueryRewrite] Rewriting follow-up with {HistoryCount} history messages", request.History.Count);
            searchQuery = await _rewriter.RewriteAsync(request.History, request.Question, ct);
            _logger.LogInformation("[QueryRewrite] Result: {Query}", searchQuery);
        }

        _logger.LogInformation("[Retrieval] Searching for: {Query}", searchQuery);
        var chunks = await _search.SearchAsync(searchQuery, ct);

        var prompt = _prompt.Build(request.Question, chunks, request.History);
        _logger.LogInformation("[PromptBuild] {Chars} chars (~{Tokens} tokens), {ChunkCount} chunks",
            prompt.Length, prompt.Length / 4, chunks.Count);

        _logger.LogInformation("[LlmCall] Sending prompt to LLM");
        var sw = Stopwatch.StartNew();
        var response = await _llm.CompleteAsync(prompt, ct);
        sw.Stop();
        _logger.LogInformation("[LlmCall] Response received in {Elapsed}ms ({ResponseChars} chars)",
            sw.ElapsedMilliseconds, response.Length);

        return response;
    }
}
