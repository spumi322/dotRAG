using System.Diagnostics;
using dotRAG.API.Application;
using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class NotesIngestionService : IHostedService
{
    private readonly IEmbeddingService   _embedder;
    private readonly InMemoryVectorStore _store;
    private readonly MarkdownChunker     _chunker;
    private readonly EmbeddingCache      _cache;
    private readonly IConfiguration      _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<NotesIngestionService> _logger;

    public bool IsReady { get; private set; }

    public NotesIngestionService(
        IEmbeddingService embedder, InMemoryVectorStore store, MarkdownChunker chunker,
        EmbeddingCache cache, IConfiguration config, IWebHostEnvironment env,
        ILogger<NotesIngestionService> logger)
    {
        (_embedder, _store, _chunker, _cache, _config, _env, _logger)
            = (embedder, store, chunker, cache, config, env, logger);
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

            var sw = Stopwatch.StartNew();

            var files = Directory.GetFiles(notesPath, "*.md", SearchOption.AllDirectories)
                .OrderBy(f => f).ToArray();
            _logger.LogInformation("Ingesting {Count} note files from {Path}", files.Length, notesPath);

            var allChunks = new List<NoteChunk>();
            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                allChunks.AddRange(_chunker.Chunk(file, content));
            }
            _logger.LogInformation("Chunking complete: {Files} files -> {Chunks} chunks", files.Length, allChunks.Count);

            var hash = _cache.ComputeHash(allChunks, _embedder.Model);
            var embeddings = _cache.TryLoad(hash);

            if (embeddings is null || embeddings.Length != allChunks.Count)
            {
                if (embeddings is not null)
                    _logger.LogWarning("[Cache] Embedding count mismatch (cached={Cached}, chunks={Chunks})",
                        embeddings.Length, allChunks.Count);

                var texts = allChunks.Select(c => c.Content).ToArray();
                embeddings = await _embedder.EmbedBatchAsync(texts, "document");
                _cache.Save(hash, embeddings);
            }
            else
            {
                _logger.LogInformation("[Cache] Loaded {Count} embeddings from cache (hash: {Hash})",
                    embeddings.Length, hash[..12]);
            }

            for (var i = 0; i < allChunks.Count; i++)
                _store.Add(allChunks[i], embeddings[i]);

            sw.Stop();
            _logger.LogInformation("Ingestion complete: {Files} files, {Chunks} chunks indexed in {Elapsed}ms",
                files.Length, allChunks.Count, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed.");
        }
        finally
        {
            IsReady = true;
        }
    }
}
