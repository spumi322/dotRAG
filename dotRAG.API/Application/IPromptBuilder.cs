using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface IPromptBuilder
{
    PromptBuildResult Build(
        string question,
        IReadOnlyList<NoteChunk> chunks,
        IReadOnlyList<HistoryMessage>? history = null);
}

public sealed record PromptBuildResult(
    string Prompt,
    int EstimatedTokens,
    int HistoryIncluded,
    int HistoryTrimmed);
