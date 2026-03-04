using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface IChatService
{
    Task<string> AskAsync(ChatRequest request, CancellationToken ct = default);
}
