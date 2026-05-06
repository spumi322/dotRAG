using System.Collections.Concurrent;
using dotRAG.API.Models;

namespace dotRAG.API.Application;

// In-memory pipeline trace store with two compartments:
//   _active   — traces currently running, partial state, updated stage-by-stage
//   _byId/_order — completed traces, bounded ring-buffer for browse-history
//
// On completion the trace moves from _active to _byId. Frontend polls
// /api/debug/recent + /api/debug/query/{id} to render in-flight progression.
internal sealed class PipelineTraceStore
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<string, PipelineTrace> _byId   = new();
    private readonly ConcurrentDictionary<string, ActiveTrace>   _active = new();
    private readonly ConcurrentQueue<string> _order = new();
    private readonly Lock _evictLock = new();

    public PipelineTraceStore(IConfiguration config)
    {
        _capacity = Math.Max(1, config.GetValue<int>("Trace:Capacity", 50));
    }

    public void StartTrace(string correlationId, DateTimeOffset timestamp, string question)
    {
        _active[correlationId] = new ActiveTrace(correlationId, timestamp, question);
    }

    public void RecordStage(string correlationId, string stage, long ms, IReadOnlyDictionary<string, object?> meta)
    {
        if (_active.TryGetValue(correlationId, out var active))
            active.Stages[stage] = new StageTiming(ms, meta);
    }

    public void Complete(PipelineTrace trace)
    {
        _byId[trace.CorrelationId] = trace;
        _order.Enqueue(trace.CorrelationId);
        _active.TryRemove(trace.CorrelationId, out _);

        lock (_evictLock)
        {
            while (_order.Count > _capacity && _order.TryDequeue(out var evicted))
                _byId.TryRemove(evicted, out _);
        }
    }

    public PipelineTrace? Get(string correlationId)
    {
        if (_byId.TryGetValue(correlationId, out var done)) return done;
        return _active.TryGetValue(correlationId, out var active) ? active.Snapshot() : null;
    }

    // Active first (newest first), then completed (newest first).
    public IReadOnlyList<RecentTraceSummary> Recent()
    {
        var list = new List<RecentTraceSummary>();

        foreach (var a in _active.Values.OrderByDescending(a => a.Timestamp))
            list.Add(new RecentTraceSummary(a.CorrelationId, a.Timestamp, a.Question, 0, Running: true));

        var ids = _order.ToArray();
        for (var i = ids.Length - 1; i >= 0; i--)
        {
            if (_byId.TryGetValue(ids[i], out var t))
                list.Add(new RecentTraceSummary(t.CorrelationId, t.Timestamp, t.Question, t.TotalMs, Running: false));
        }
        return list;
    }

    private sealed class ActiveTrace
    {
        public string         CorrelationId { get; }
        public DateTimeOffset Timestamp     { get; }
        public string         Question      { get; }
        public ConcurrentDictionary<string, StageTiming> Stages { get; } = new();

        public ActiveTrace(string id, DateTimeOffset ts, string question)
        {
            CorrelationId = id;
            Timestamp     = ts;
            Question      = question;
        }

        public PipelineTrace Snapshot()
        {
            Stages.TryGetValue("queryRewrite", out var qr);
            Stages.TryGetValue("embedding",    out var em);
            Stages.TryGetValue("vectorSearch", out var vs);
            Stages.TryGetValue("promptBuild",  out var pb);
            Stages.TryGetValue("llmComplete",  out var lc);

            string? rewritten = null;
            if (qr is not null && qr.Meta.TryGetValue("rewrittenQuery", out var v) && v is string s)
                rewritten = s;

            return new PipelineTrace(
                CorrelationId:   CorrelationId,
                Timestamp:       Timestamp,
                Question:        Question,
                RewrittenQuery:  rewritten,
                QueryRewrite:    qr,
                Embedding:       em,
                VectorSearch:    vs,
                PromptBuild:     pb,
                LlmComplete:     lc,
                TotalMs:         0,
                Running:         true,
                RetrievedChunks: []);
        }
    }
}
