namespace dotRAG.API.Application;

public interface IChatService
{
    Task<string> AskAsync(string question, CancellationToken ct = default);
}
