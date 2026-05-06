namespace dotRAG.API.Models;

// Wire shapes for the Notes browser screen. Exposed by /api/notes/* endpoints.

public record NotesIndexDto(
    int TotalFiles,
    int TotalChunks,
    IReadOnlyList<NotesModuleDto> Modules);

public record NotesModuleDto(
    string Name,
    IReadOnlyList<NotesFileSummaryDto> Files);

public record NotesFileSummaryDto(
    string RelativePath,
    string FileName,
    int ChunkCount,
    IReadOnlyList<string> Headings);

public record NotesFileDetailDto(
    string RelativePath,
    string FileName,
    string Module,
    IReadOnlyList<NoteChunkDto> Chunks,
    EmbeddingMetadataDto Embedding);

public record NoteChunkDto(int Index, string Heading, string Content);

public record EmbeddingMetadataDto(string Model, int Dimension, string CachePath);
