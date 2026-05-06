using System.Diagnostics;
using dotRAG.API.Models;
using Microsoft.AspNetCore.Http;

namespace dotRAG.API.Application;

internal sealed class ChatService : IChatService
{
    private readonly INotesSearchService  _search;
    private readonly IPromptBuilder       _prompt;
    private readonly ILlmService          _llm;
    private readonly IQueryRewriter       _rewriter;
    private readonly PipelineTraceStore   _traces;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        INotesSearchService search,
        IPromptBuilder prompt,
        ILlmService llm,
        IQueryRewriter rewriter,
        PipelineTraceStore traces,
        IHttpContextAccessor http,
        ILogger<ChatService> logger)
        => (_search, _prompt, _llm, _rewriter, _traces, _http, _logger)
         = (search, prompt, llm, rewriter, traces, http, logger);

    public async Task<ChatResult> AskAsync(ChatRequest request, CancellationToken ct = default)
    {
        var totalSw       = Stopwatch.StartNew();
        var correlationId = ResolveCorrelationId();
        var startedAt     = DateTimeOffset.UtcNow;

        if (correlationId is not null)
            _traces.StartTrace(correlationId, startedAt, request.Question);

        var searchQuery = request.Question;
        StageTiming? rewriteStage = null;
        string? rewrittenQuery = null;

        if (request.History is { Count: > 0 })
        {
            _logger.LogInformation("[QueryRewrite] Rewriting follow-up with {HistoryCount} history messages", request.History.Count);
            var rewriteSw = Stopwatch.StartNew();
            searchQuery = await _rewriter.RewriteAsync(request.History, request.Question, ct);
            rewriteSw.Stop();
            rewrittenQuery = searchQuery;
            rewriteStage = new StageTiming(rewriteSw.ElapsedMilliseconds, new Dictionary<string, object?>
            {
                ["historyMessages"] = request.History.Count,
                ["rewrittenQuery"]  = rewrittenQuery,
            });
            _logger.LogInformation("[QueryRewrite] Result: {Query}", searchQuery);
            RecordStage(correlationId, "queryRewrite", rewriteStage);
        }

        _logger.LogInformation("[Retrieval] Searching for: {Query}", searchQuery);
        var searchResult = await _search.SearchAsync(searchQuery, ct);
        var noteChunks = searchResult.Chunks.Select(s => s.Chunk).ToList();

        var embeddingStage = new StageTiming(searchResult.EmbedMs, new Dictionary<string, object?>
        {
            ["dim"] = searchResult.EmbeddingDim,
        });
        RecordStage(correlationId, "embedding", embeddingStage);

        var vectorSearchStage = new StageTiming(searchResult.SearchMs, new Dictionary<string, object?>
        {
            ["chunks"] = searchResult.Chunks.Count,
        });
        RecordStage(correlationId, "vectorSearch", vectorSearchStage);

        var promptSw = Stopwatch.StartNew();
        var promptResult = _prompt.Build(request.Question, noteChunks, request.History);
        promptSw.Stop();
        _logger.LogInformation("[PromptBuild] {Chars} chars (~{Tokens} tokens), {ChunkCount} chunks",
            promptResult.Prompt.Length, promptResult.EstimatedTokens, noteChunks.Count);
        var promptStage = new StageTiming(promptSw.ElapsedMilliseconds, new Dictionary<string, object?>
        {
            ["estimatedTokens"]  = promptResult.EstimatedTokens,
            ["maxTokens"]        = promptResult.MaxTokens,
            ["historyIncluded"]  = promptResult.HistoryIncluded,
            ["historyTrimmed"]   = promptResult.HistoryTrimmed,
        });
        RecordStage(correlationId, "promptBuild", promptStage);

        _logger.LogInformation("[LlmCall] Sending prompt to LLM");
        var llmSw = Stopwatch.StartNew();
        var response = await _llm.CompleteAsync(promptResult.Prompt, ct);
        llmSw.Stop();
        _logger.LogInformation("[LlmCall] Response received in {Elapsed}ms ({ResponseChars} chars)",
            llmSw.ElapsedMilliseconds, response.Length);
        var llmStage = new StageTiming(llmSw.ElapsedMilliseconds, new Dictionary<string, object?>
        {
            ["responseChars"] = response.Length,
        });
        RecordStage(correlationId, "llmComplete", llmStage);

        totalSw.Stop();

        var chunkDtos = searchResult.Chunks.Select(ChunkDto.FromScored).ToList();

        if (correlationId is not null)
        {
            _traces.Complete(new PipelineTrace(
                CorrelationId:   correlationId,
                Timestamp:       startedAt,
                Question:        request.Question,
                RewrittenQuery:  rewrittenQuery,
                QueryRewrite:    rewriteStage,
                Embedding:       embeddingStage,
                VectorSearch:    vectorSearchStage,
                PromptBuild:     promptStage,
                LlmComplete:     llmStage,
                TotalMs:         totalSw.ElapsedMilliseconds,
                Running:         false,
                RetrievedChunks: chunkDtos));
        }

        return new ChatResult(response, chunkDtos);
    }

    private void RecordStage(string? correlationId, string stage, StageTiming timing)
    {
        if (correlationId is null) return;
        _traces.RecordStage(correlationId, stage, timing.Ms, timing.Meta);
    }

    private string? ResolveCorrelationId() =>
        _http.HttpContext?.Response.Headers["X-Correlation-ID"].FirstOrDefault();
}
