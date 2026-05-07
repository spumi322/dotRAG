using dotRAG.API.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class MarkdownChunker
{
    private static readonly Regex WikiLink = new(@"\[\[([^\]]+)\]\]", RegexOptions.Compiled);

    private readonly IConfiguration? _config;

    public MarkdownChunker(IConfiguration? config = null)
    {
        _config = config;
    }

    public IReadOnlyList<NoteChunk> Chunk(string filePath, string content)
    {
        var maxChunkChars  = _config?.GetValue("Chunking:MaxChunkChars",  2000) ?? 2000;
        var headingDepth   = Math.Clamp(_config?.GetValue("Chunking:HeadingDepth", 3) ?? 3, 1, 6);
        var minChunkLength = Math.Max(_config?.GetValue("Chunking:MinChunkLength", 20) ?? 20, 1);

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var results  = new List<NoteChunk>();
        var headingStack = new string?[headingDepth];
        var currentHeading = fileName;
        var body = new StringBuilder();

        foreach (var line in content.Split('\n'))
        {
            var level = CountLeadingHashes(line, headingDepth);
            if (level >= 1 && level <= headingDepth)
            {
                FlushWithSplit(fileName, currentHeading, body, results, maxChunkChars, minChunkLength);
                var text = line.AsSpan().TrimStart('#').Trim().ToString();

                headingStack[level - 1] = text;
                for (var i = level; i < headingDepth; i++)
                    headingStack[i] = null;

                currentHeading = BuildBreadcrumb(headingStack, text);
                body.Clear();
            }
            else
            {
                body.AppendLine(line);
            }
        }

        FlushWithSplit(fileName, currentHeading, body, results, maxChunkChars, minChunkLength);
        return results;
    }

    // H1 is the file title (matches SourceFile), so breadcrumb starts from H2.
    private static string BuildBreadcrumb(string?[] stack, string fallback)
    {
        var parts = new List<string>(stack.Length);
        for (var i = 1; i < stack.Length; i++)
        {
            if (stack[i] is { } part)
                parts.Add(part);
        }

        return parts.Count > 0 ? string.Join(" > ", parts) : fallback;
    }

    private static int CountLeadingHashes(string line, int maxDepth)
    {
        var count = 0;
        foreach (var ch in line)
        {
            if (ch == '#') count++;
            else break;
        }

        if (count == 0 || count > maxDepth) return 0;
        if (count >= line.Length || line[count] != ' ') return 0;
        return count;
    }

    private static void FlushWithSplit(string source, string heading, StringBuilder body, List<NoteChunk> results, int maxChunkChars, int minChunkLength)
    {
        var cleaned = WikiLink.Replace(body.ToString(), "$1").Replace("\r\n", "\n").Trim();
        if (cleaned.Length < minChunkLength)
            return;

        if (cleaned.Length <= maxChunkChars)
        {
            results.Add(new NoteChunk(source, heading, cleaned));
            return;
        }

        var paragraphs = cleaned.Split("\n\n");
        var group = new StringBuilder();

        foreach (var para in paragraphs)
        {
            if (group.Length + para.Length + 2 > maxChunkChars && group.Length > 0)
            {
                EmitChunk(source, heading, group, results, minChunkLength);
                group.Clear();
            }

            if (group.Length > 0)
                group.Append("\n\n");
            group.Append(para);
        }

        EmitChunk(source, heading, group, results, minChunkLength);
    }

    private static void EmitChunk(string source, string heading, StringBuilder group, List<NoteChunk> results, int minChunkLength)
    {
        var text = group.ToString().Trim();
        if (text.Length >= minChunkLength)
            results.Add(new NoteChunk(source, heading, text));
    }
}
