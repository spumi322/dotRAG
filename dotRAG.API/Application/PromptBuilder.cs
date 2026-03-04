using dotRAG.API.Models;
using System.Text;

namespace dotRAG.API.Application;

internal sealed class PromptBuilder : IPromptBuilder
{
    private const string System =
        "You are a .NET interview study assistant. " +
        "Answer the question strictly based on the provided notes. " +
        "Be concise. If the notes do not contain relevant information, say so.";

    public string Build(string question, IReadOnlyList<NoteChunk> chunks)
    {
        var sb = new StringBuilder();
        sb.AppendLine(System);
        sb.AppendLine();
        sb.AppendLine("--- Notes ---");
        for (var i = 0; i < chunks.Count; i++)
        {
            var c = chunks[i];
            sb.AppendLine();
            sb.AppendLine($"[{i + 1}] {c.SourceFile} — {c.Heading}");
            sb.AppendLine(c.Content);
        }
        sb.AppendLine();
        sb.AppendLine("--- Question ---");
        sb.AppendLine(question);
        return sb.ToString();
    }
}
