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

    public bool IsReady { get; private set; }

    public NotesIngestionService(
        IEmbeddingService embedder, InMemoryVectorStore store, MarkdownChunker chunker,
        IConfiguration config, IWebHostEnvironment env,
        ILogger<NotesIngestionService> logger)
    {
        (_embedder, _store, _chunker, _config, _env, _logger)
            = (embedder, store, chunker, config, env, logger);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = IngestAsync(); // fire-and-forget; errors caught inside
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task IngestAsync()
    {
        try
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
                var content = await File.ReadAllTextAsync(file);
                var chunks  = _chunker.Chunk(file, content);
                foreach (var chunk in chunks)
                {
                    var embedding = await _embedder.EmbedAsync(chunk.Content);
                    _store.Add(chunk, embedding);
                    total++;
                }
            }
            _logger.LogInformation("Ingestion complete. {Total} chunks indexed.", total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed.");
        }
        finally
        {
            IsReady = true; // always mark ready (even on failure) so health check doesn't stall
        }
    }
}
