using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface IQueryRewriter
{
    Task<string> RewriteAsync(
        IReadOnlyList<HistoryMessage> history,
        string question,
        CancellationToken ct = default);
}
