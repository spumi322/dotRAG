namespace dotRAG.API.Models;

public record ChatResponse(string Answer, IReadOnlyList<ChunkDto> Chunks);
