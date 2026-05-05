using dotRAG.API.Models;
using System.Text;

namespace dotRAG.API.Application;

internal sealed class PromptBuilder : IPromptBuilder
{
    private const string SystemText =
        "You are a .NET interview study assistant. " +
        "Use the provided notes as your primary source. " +
        "If the notes only partially cover the topic, supplement with your own knowledge but note what came from the notes versus general knowledge. " +
        "Be concise.";

    private const string FallbackSystemText =
        "You are a .NET interview study assistant. " +
        "No relevant notes were found for this question. " +
        "Answer from your own knowledge but keep the response concise and interview-focused.";

    private readonly int _maxTokens;
    private readonly ILogger<PromptBuilder> _logger;

    public PromptBuilder(IConfiguration config, ILogger<PromptBuilder> logger)
    {
        _maxTokens = config.GetValue<int>("MaxPromptTokens", 8192);
        _logger    = logger;
    }

    public PromptBuildResult Build(
        string question,
        IReadOnlyList<NoteChunk> chunks,
        IReadOnlyList<HistoryMessage>? history = null)
    {
        var window = history is null ? null : new List<HistoryMessage>(history);
        var originalHistoryCount = history?.Count ?? 0;

        while (true)
        {
            var candidate = Assemble(question, chunks, window);
            var tokenEstimate = EstimateTokens(candidate);
            if (tokenEstimate <= _maxTokens)
            {
                var includedCount = window?.Count ?? 0;
                var trimmedCount = originalHistoryCount - includedCount;
                _logger.LogInformation(
                    "Prompt assembled: ~{Tokens} tokens, {Included} history messages included, {Trimmed} trimmed",
                    tokenEstimate, includedCount, trimmedCount);
                return new PromptBuildResult(candidate, tokenEstimate, includedCount, trimmedCount);
            }

            if (window is { Count: >= 2 })
            {
                window.RemoveAt(0); // drop oldest user turn
                window.RemoveAt(0); // drop oldest assistant turn
                continue;
            }

            _logger.LogWarning(
                "Prompt exceeds token budget ({Est} tokens > {Max} max) with no history to trim. Sending anyway.",
                tokenEstimate, _maxTokens);
            return new PromptBuildResult(candidate, tokenEstimate, window?.Count ?? 0, originalHistoryCount - (window?.Count ?? 0));
        }
    }

    private static string Assemble(string question, IReadOnlyList<NoteChunk> chunks, List<HistoryMessage>? window)
    {
        var sb = new StringBuilder();

        if (chunks.Count == 0)
        {
            sb.AppendLine(FallbackSystemText);
        }
        else
        {
            sb.AppendLine(SystemText);
            sb.AppendLine();
            sb.AppendLine($"--- Notes ({chunks.Count} matched) ---");
            for (var i = 0; i < chunks.Count; i++)
            {
                var c = chunks[i];
                sb.AppendLine();
                sb.AppendLine($"[{i + 1}] {c.SourceFile} — {c.Heading}");
                sb.AppendLine(c.Content);
            }
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
