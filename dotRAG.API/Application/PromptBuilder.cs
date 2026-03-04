using dotRAG.API.Models;
using System.Text;

namespace dotRAG.API.Application;

internal sealed class PromptBuilder : IPromptBuilder
{
    private const string SystemText =
        "You are a .NET interview study assistant. " +
        "Answer the question strictly based on the provided notes. " +
        "Be concise. If the notes do not contain relevant information, say so.";

    private readonly int _maxTokens;
    private readonly ILogger<PromptBuilder> _logger;

    public PromptBuilder(IConfiguration config, ILogger<PromptBuilder> logger)
    {
        _maxTokens = config.GetValue<int>("MaxPromptTokens", 8192);
        _logger    = logger;
    }

    public string Build(
        string question,
        IReadOnlyList<NoteChunk> chunks,
        IReadOnlyList<HistoryMessage>? history = null)
    {
        var window = history is null ? null : new List<HistoryMessage>(history);

        while (true)
        {
            var candidate = Assemble(question, chunks, window);
            if (EstimateTokens(candidate) <= _maxTokens)
                return candidate;

            if (window is { Count: >= 2 })
            {
                window.RemoveAt(0); // drop oldest user turn
                window.RemoveAt(0); // drop oldest assistant turn
                continue;
            }

            _logger.LogWarning(
                "Prompt exceeds token budget ({Est} tokens > {Max} max) with no history to trim. Sending anyway.",
                EstimateTokens(candidate), _maxTokens);
            return candidate;
        }
    }

    private static string Assemble(string question, IReadOnlyList<NoteChunk> chunks, List<HistoryMessage>? window)
    {
        var sb = new StringBuilder();
        sb.AppendLine(SystemText);
        sb.AppendLine();
        sb.AppendLine("--- Notes ---");
        for (var i = 0; i < chunks.Count; i++)
        {
            var c = chunks[i];
            sb.AppendLine();
            sb.AppendLine($"[{i + 1}] {c.SourceFile} — {c.Heading}");
            sb.AppendLine(c.Content);
        }

        if (window is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("--- Conversation History ---");
            foreach (var msg in window)
                sb.AppendLine($"{msg.Role}: {msg.Content}");
        }

        sb.AppendLine();
        sb.AppendLine("--- Question ---");
        sb.AppendLine(question);
        return sb.ToString();
    }

    // 1 token ≈ 4 chars (English prose) — good enough for a demo budget guard.
    private static int EstimateTokens(string text) => text.Length / 4;
}
