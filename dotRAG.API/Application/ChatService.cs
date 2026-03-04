using dotRAG.API.Models;

namespace dotRAG.API.Application;

internal sealed class ChatService : IChatService
{
    private readonly INotesSearchService _search;
    private readonly IPromptBuilder      _prompt;
    private readonly ILlmService         _llm;
    private readonly IQueryRewriter      _rewriter;

    public ChatService(
        INotesSearchService search,
        IPromptBuilder prompt,
        ILlmService llm,
        IQueryRewriter rewriter)
        => (_search, _prompt, _llm, _rewriter) = (search, prompt, llm, rewriter);

    public async Task<string> AskAsync(ChatRequest request, CancellationToken ct = default)
    {
        var searchQuery = request.Question;

        if (request.History is { Count: > 0 })
            searchQuery = await _rewriter.RewriteAsync(request.History, request.Question, ct);

        var chunks = await _search.SearchAsync(searchQuery, ct);

        // Pass original question (not rewritten) to prompt builder.
        var prompt = _prompt.Build(request.Question, chunks, request.History);
        return await _llm.CompleteAsync(prompt, ct);
    }
}
