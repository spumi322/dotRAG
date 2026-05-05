using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface IChatService
{
    Task<ChatResult> AskAsync(ChatRequest request, CancellationToken ct = default);
}

public sealed record ChatResult(string Answer, IReadOnlyList<ChunkDto> Chunks);
