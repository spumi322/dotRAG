namespace dotRAG.API.Models;

public readonly record struct ScoredChunk(NoteChunk Chunk, float Score);
