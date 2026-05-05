using System.Collections.Concurrent;

namespace dotRAG.API.Application;

// Bounded ring-buffer of recent pipeline traces, keyed by correlation-id.
// In-memory and ephemeral by design — traces are debugging info, not data.
internal sealed class PipelineTraceStore
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<string, PipelineTrace> _byId = new();
    private readonly ConcurrentQueue<string> _order = new();
    private readonly Lock _evictLock = new();

    public PipelineTraceStore(IConfiguration config)
    {
        _capacity = Math.Max(1, config.GetValue<int>("Trace:Capacity", 50));
    }

    public void Add(PipelineTrace trace)
    {
        _byId[trace.CorrelationId] = trace;
        _order.Enqueue(trace.CorrelationId);

        // Eviction guarded so we don't drop entries below capacity under contention.
        lock (_evictLock)
        {
            while (_order.Count > _capacity && _order.TryDequeue(out var evicted))
                _byId.TryRemove(evicted, out _);
        }
    }

    public PipelineTrace? Get(string id) => _byId.GetValueOrDefault(id);

    // Snapshot, newest first.
    public IReadOnlyList<PipelineTrace> Recent()
    {
        var ids = _order.ToArray();
        var traces = new List<PipelineTrace>(ids.Length);
        for (var i = ids.Length - 1; i >= 0; i--)
        {
            if (_byId.TryGetValue(ids[i], out var t))
                traces.Add(t);
        }
        return traces;
    }
}
