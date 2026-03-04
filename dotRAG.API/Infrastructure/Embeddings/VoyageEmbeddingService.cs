namespace dotRAG.API.Infrastructure.Embeddings;

// Phase 0 stub — no logic.
// Phase 1: inject IHttpClientFactory + IConfiguration for VoyageApiKey.
// Voyage AI has no official .NET SDK; use typed HttpClient calling
// POST https://api.voyageai.com/v1/embeddings
internal sealed class VoyageEmbeddingService : Application.IEmbeddingService
{
}
