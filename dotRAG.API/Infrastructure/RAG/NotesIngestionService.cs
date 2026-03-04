using dotRAG.API.Application;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class NotesIngestionService : IHostedService
{
    private readonly IEmbeddingService   _embedder;
    private readonly InMemoryVectorStore _store;
    private readonly MarkdownChunker     _chunker;
    private readonly IConfiguration      _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<NotesIngestionService> _logger;

    public NotesIngestionService(
        IEmbeddingService embedder, InMemoryVectorStore store, MarkdownChunker chunker,
        IConfiguration config, IWebHostEnvironment env,
        ILogger<NotesIngestionService> logger)
    {
        (_embedder, _store, _chunker, _config, _env, _logger)
            = (embedder, store, chunker, config, env, logger);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var notesPath = Path.GetFullPath(
            Path.Combine(_env.ContentRootPath, _config["NotesPath"] ?? "../Notes"));

        if (!Directory.Exists(notesPath))
        {
            _logger.LogWarning("Notes directory not found at {Path}. Ingestion skipped.", notesPath);
            return;
        }

        var files = Directory.GetFiles(notesPath, "*.md", SearchOption.AllDirectories);
        _logger.LogInformation("Ingesting {Count} note files from {Path}", files.Length, notesPath);

        var total = 0;
        foreach (var file in files)
        {
            var content = await File.ReadAllTextAsync(file, cancellationToken);
            var chunks  = _chunker.Chunk(file, content);
            foreach (var chunk in chunks)
            {
                var embedding = await _embedder.EmbedAsync(chunk.Content, cancellationToken);
                _store.Add(chunk, embedding);
                total++;
            }
        }
        _logger.LogInformation("Ingestion complete. {Total} chunks indexed.", total);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
