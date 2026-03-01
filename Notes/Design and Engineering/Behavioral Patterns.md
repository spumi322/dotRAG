
Concerned with how objects communicate and responsibilities are distributed.

---

## Strategy (Tier 1)

Defines a family of algorithms behind a common interface. Swap the algorithm at runtime without changing the caller.

```csharp
// Contract
public interface IGameResultPipelineStep
{
    Task ExecuteAsync(GameResultContext ctx);
}

// Concrete strategies — each encapsulates one algorithm
public class ScoreGameStep : IGameResultPipelineStep
{
    public async Task ExecuteAsync(GameResultContext ctx) { ... }
}
public class UpdateGroupStatsStep : IGameResultPipelineStep
{
    public async Task ExecuteAsync(GameResultContext ctx) { ... }
}

// Caller doesn't know which steps exist — iterates the interface
public class GameResultPipeline(IEnumerable<IGameResultPipelineStep> steps)
{
    public async Task RunAsync(GameResultContext ctx)
    {
        foreach (var step in steps)
            await step.ExecuteAsync(ctx);
    }
}
```

Adding a new step = create the class, register it. Zero changes to the pipeline. This is Strategy + OCP working together.

**Interview traps:**

- "Strategy vs if/else?" — if/else bakes the algorithm selection into the caller, violates OCP. Strategy externalises it
- "Strategy vs State?" — Strategy swaps algorithm from outside (caller decides). State swaps behaviour from inside (object decides based on its own state)
- "Where's Strategy in .NET BCL?" — `IComparer<T>`, `Array.Sort(arr, comparer)` — the comparer is the strategy

---

## Decorator (Tier 1)

Wraps an object to add behaviour without modifying the original class or using inheritance. Stackable.

```csharp
// Base interface
public interface IMatchService
{
    Task<MatchDto> GetAsync(int id);
}

// Real implementation
public class MatchService(IMatchRepository repo) : IMatchService
{
    public async Task<MatchDto> GetAsync(int id) => await repo.GetAsync(id);
}

// Decorator — wraps IMatchService, adds logging, caller never knows
public class LoggingMatchService(IMatchService inner, ILogger<LoggingMatchService> log)
    : IMatchService
{
    public async Task<MatchDto> GetAsync(int id)
    {
        log.LogInformation("Fetching match {Id}", id);
        var result = await inner.GetAsync(id);
        log.LogInformation("Match {Id} fetched", id);
        return result;
    }
}

// Wired in DI — transparent to consumers
services.AddScoped<IMatchService, MatchService>();
// or wrap: services.Decorate<IMatchService, LoggingMatchService>(); (Scrutor)
```

**ASP.NET middleware IS Decorator** — each middleware wraps the next, adding behaviour (auth, logging, exception handling) without modifying what's inside.

**Interview traps:**

- "Decorator vs Inheritance?" — Inheritance is compile-time, static. Decorator is runtime, composable — you can stack multiple decorators
- "Where in ASP.NET Core?" — middleware pipeline. Also `Stream` in the BCL (`BufferedStream` decorates `FileStream`)
- "Decorator vs Proxy?" — Decorator adds behaviour. Proxy controls access (auth, caching, lazy load). Intent differs, structure is similar

---

## Observer (Tier 1)

One object (publisher) notifies many dependents (subscribers) when state changes. Subscribers don't know about each other.

```csharp
// C# event/delegate IS Observer
public class TournamentService
{
    public event EventHandler<MatchFinishedEventArgs>? MatchFinished;

    public void FinishMatch(Match match)
    {
        // ... update state
        MatchFinished?.Invoke(this, new MatchFinishedEventArgs(match));
    }
}

// Subscribers — decoupled, register independently
tournamentService.MatchFinished += async (_, e)
    => await _hubContext.Clients.All.SendAsync("MatchFinished", e.Match);
```

SignalR is Observer at the infrastructure level — your server publishes events, all connected clients are subscribers.

**Interview traps:**

- "Observer vs event/delegate in C#?" — `event`/`delegate` is C#'s built-in Observer implementation. Knowing this connection is the answer
- "Memory leak risk?" — subscribing with `+=` and never unsubscribing keeps the subscriber alive (GC can't collect it). Always unsubscribe with `-=` or use `WeakReference` patterns
- "Observer vs Pub/Sub?" — Observer: publisher knows subscribers directly. Pub/Sub: message broker in between (e.g. RabbitMQ, Azure Service Bus) — fully decoupled