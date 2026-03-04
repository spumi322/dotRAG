using dotRAG.API.Models;
using System.Text;

namespace dotRAG.API.Application;

internal sealed class QueryRewriter : IQueryRewriter
{
    private readonly ILlmService _llm;
    private readonly ILogger<QueryRewriter> _logger;

    public QueryRewriter(ILlmService llm, ILogger<QueryRewriter> logger)
        => (_llm, _logger) = (llm, logger);

    public async Task<string> RewriteAsync(
        IReadOnlyList<HistoryMessage> history,
        string question,
        CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Given the conversation history below, rewrite the final question as a single standalone question that contains all context needed to search a knowledge base. Output ONLY the rewritten question with no explanation.");
        sb.AppendLine();
        sb.AppendLine("--- History ---");
        foreach (var msg in history)
            sb.AppendLine($"{msg.Role}: {msg.Content}");
        sb.AppendLine();
        sb.AppendLine("--- Question to rewrite ---");
        sb.AppendLine(question);

        var rewritten = await _llm.CompleteAsync(sb.ToString(), ct);
        _logger.LogDebug("Query rewritten: {Original} -> {Rewritten}", question, rewritten);
        return rewritten.Trim();
    }
}
