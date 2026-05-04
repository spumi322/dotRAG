using dotRAG.API.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class MarkdownChunker
{
    private static readonly Regex WikiLink = new(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);

    private readonly int _maxChunkChars;

    public MarkdownChunker(IConfiguration? config = null)
    {
        _maxChunkChars = config?.GetValue<int>("Chunking:MaxChunkChars", 2000) ?? 2000;
    }

    public IReadOnlyList<NoteChunk> Chunk(string filePath, string content)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var results  = new List<NoteChunk>();
        var headingStack = new string?[3]; // levels 1, 2, 3
        var currentHeading = fileName;
        var body = new StringBuilder();

        foreach (var line in content.Split('\n'))
        {
            var level = CountLeadingHashes(line);
            if (level >= 1 && level <= 3)
            {
                FlushWithSplit(fileName, currentHeading, body, results);
                var text = line.AsSpan().TrimStart('#').Trim().ToString();

                headingStack[level - 1] = text;
                for (var i = level; i < 3; i++)
                    headingStack[i] = null;

                currentHeading = BuildBreadcrumb(headingStack, text);
                body.Clear();
            }
            else
            {
                body.AppendLine(line);
            }
        }

        FlushWithSplit(fileName, currentHeading, body, results);
        return results;
    }

    // H1 is the file title (matches SourceFile), so breadcrumb starts from H2.
    private static string BuildBreadcrumb(string?[] stack, string fallback)
    {
        var parts = new List<string>(2);
        for (var i = 1; i < stack.Length; i++)
        {
            if (stack[i] is { } part)
                parts.Add(part);
        }

        return parts.Count > 0 ? string.Join(" > ", parts) : fallback;
    }

    private static int CountLeadingHashes(string line)
    {
        var count = 0;
        foreach (var ch in line)
        {
            if (ch == '#') count++;
            else break;
        }

        if (count == 0 || count > 3) return 0;
        if (count >= line.Length || line[count] != ' ') return 0;
        return count;
    }

    private void FlushWithSplit(string source, string heading, StringBuilder body, List<NoteChunk> results)
    {
        var cleaned = WikiLink.Replace(body.ToString(), "$1").Replace("\r\n", "\n").Trim();
        if (cleaned.Length < 20)
            return;

        if (cleaned.Length <= _maxChunkChars)
        {
            results.Add(new NoteChunk(source, heading, cleaned));
            return;
        }

        var paragraphs = cleaned.Split("\n\n");
        var group = new StringBuilder();

        foreach (var para in paragraphs)
        {
            if (group.Length + para.Length + 2 > _maxChunkChars && group.Length > 0)
            {
                EmitChunk(source, heading, group, results);
                group.Clear();
            }

            if (group.Length > 0)
                group.Append("\n\n");
            group.Append(para);
        }

        EmitChunk(source, heading, group, results);
    }

    private static void EmitChunk(string source, string heading, StringBuilder group, List<NoteChunk> results)
    {
        var text = group.ToString().Trim();
        if (text.Length >= 20)
            results.Add(new NoteChunk(source, heading, text));
    }
}
