namespace dotRAG.API.Models;

public record ChatRequest(
    string Question,
    IReadOnlyList<HistoryMessage>? History = null);
