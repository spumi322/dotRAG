using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface IPromptBuilder
{
    string Build(
        string question,
        IReadOnlyList<NoteChunk> chunks,
        IReadOnlyList<HistoryMessage>? history = null);
}
