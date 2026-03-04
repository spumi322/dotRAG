using dotRAG.API.Models;

namespace dotRAG.API.Application;

public interface INotesSearchService
{
    Task<IReadOnlyList<NoteChunk>> SearchAsync(string question, CancellationToken ct = default);
}
