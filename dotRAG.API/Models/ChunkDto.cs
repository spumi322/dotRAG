namespace dotRAG.API.Models;

// Canonical wire shape for chunk references — used by Chat, Debug, and Notes.
// Score is null when the chunk is not from a search result (e.g. Notes index browsing).
public record ChunkDto(string SourceFile, string Heading, string Content, float? Score)
{
    public static ChunkDto FromScored(ScoredChunk s) =>
        new(s.Chunk.SourceFile, s.Chunk.Heading, s.Chunk.Content, s.Score);
}
