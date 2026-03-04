namespace dotRAG.API.Application;

internal sealed class ChatService : IChatService
{
    private readonly INotesSearchService _search;
    private readonly IPromptBuilder      _prompt;
    private readonly ILlmService         _llm;

    public ChatService(INotesSearchService search, IPromptBuilder prompt, ILlmService llm)
        => (_search, _prompt, _llm) = (search, prompt, llm);

    public async Task<string> AskAsync(string question, CancellationToken ct = default)
    {
        var chunks = await _search.SearchAsync(question, ct);
        var prompt = _prompt.Build(question, chunks);
        return await _llm.CompleteAsync(prompt, ct);
    }
}
