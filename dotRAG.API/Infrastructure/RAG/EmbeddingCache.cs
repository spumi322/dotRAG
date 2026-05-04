using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using dotRAG.API.Models;

namespace dotRAG.API.Infrastructure.RAG;

internal sealed class EmbeddingCache
{
    private readonly string _cachePath;
    private readonly ILogger<EmbeddingCache> _logger;

    public EmbeddingCache(IConfiguration config, IWebHostEnvironment env, ILogger<EmbeddingCache> logger)
    {
        var relative = config["CachePath"] ?? "../cache/embeddings.json";
        _cachePath = Path.GetFullPath(Path.Combine(env.ContentRootPath, relative));
        _logger = logger;
    }

    public string ComputeHash(IReadOnlyList<NoteChunk> chunks, string model)
    {
        using var sha = SHA256.Create();
        var sb = new StringBuilder();
        sb.AppendLine(model);
        foreach (var c in chunks)
            sb.Append(c.SourceFile).Append('|').Append(c.Heading).Append('|').AppendLine(c.Content);

        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexStringLower(bytes);
    }

    public float[][]? TryLoad(string contentHash)
    {
        try
        {
            if (!File.Exists(_cachePath))
            {
                _logger.LogInformation("[Cache] No cache file at {Path}", _cachePath);
                return null;
            }

            var json = File.ReadAllText(_cachePath);
            var cache = JsonSerializer.Deserialize<CacheFile>(json);

            if (cache is null || cache.ContentHash != contentHash)
            {
                _logger.LogInformation("[Cache] Hash mismatch — cached={Cached}, current={Current}",
                    cache?.ContentHash?[..12] ?? "null", contentHash[..12]);
                return null;
            }

            if (cache.Embeddings is not { Length: > 0 })
            {
                _logger.LogWarning("[Cache] Cache file has no embeddings");
                return null;
            }

            return cache.Embeddings;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Cache] Failed to load cache from {Path}", _cachePath);
            return null;
        }
    }

    public void Save(string contentHash, float[][] embeddings)
    {
        try
        {
            var dir = Path.GetDirectoryName(_cachePath);
            if (dir is not null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var cache = new CacheFile { ContentHash = contentHash, Embeddings = embeddings };
            var json = JsonSerializer.Serialize(cache);
            File.WriteAllText(_cachePath, json);
            _logger.LogInformation("[Cache] Saved {Count} embeddings to {Path}", embeddings.Length, _cachePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Cache] Failed to save cache to {Path}", _cachePath);
        }
    }

    private sealed class CacheFile
    {
        [JsonPropertyName("contentHash")]
        public string ContentHash { get; set; } = "";

        [JsonPropertyName("embeddings")]
        public float[][]? Embeddings { get; set; }
    }
}
