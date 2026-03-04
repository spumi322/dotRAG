using dotRAG.API.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class MarkdownChunker
{
    private static readonly Regex WikiLink = new(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);

    public IReadOnlyList<NoteChunk> Chunk(string filePath, string content)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var results  = new List<NoteChunk>();
        var heading  = fileName;
        var body     = new StringBuilder();

        foreach (var line in content.Split('\n'))
        {
            if (line.StartsWith('#'))
            {
                Flush(fileName, heading, body, results);
                heading = line.TrimStart('#').Trim();
                body.Clear();
            }
            else
            {
                body.AppendLine(line);
            }
        }
        Flush(fileName, heading, body, results);
        return results;
    }

    private static void Flush(string source, string heading, StringBuilder body, List<NoteChunk> results)
    {
        var cleaned = WikiLink.Replace(body.ToString(), "$1").Trim();
        if (cleaned.Length >= 20)
            results.Add(new NoteChunk(source, heading, cleaned));
    }
}
