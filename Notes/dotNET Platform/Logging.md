## What logging is and why it matters

Logging records events, errors, and state changes during program execution. In production, logs are the primary tool for debugging — you can't attach a debugger to a live server. Good logging is the difference between a 5-minute fix and a 2-hour investigation.

---

## Log levels — full order

```
Trace < Debug < Information < Warning < Error < Critical
```

|Level|When to use|
|---|---|
|`Trace`|Step-by-step execution detail. Usually disabled in prod — very noisy.|
|`Debug`|Development diagnostics. Variable values, flow tracing. Off in prod by default.|
|`Information`|Normal operational events: request received, tournament started, user logged in.|
|`Warning`|Something unexpected happened but the app recovered. Worth investigating.|
|`Error`|Operation failed. Requires attention. Exception should be logged here.|
|`Critical`|System failure. App cannot continue. DB down, out of memory.|

**Interview answer:** "The level controls the minimum severity that gets written. In production, I typically set `Information` as the default and `Warning` for noisy namespaces like `Microsoft.AspNetCore`."

Your TO2 `appsettings.json` already does this:

```json
"LogLevel": {
  "Default": "Information",
  "Microsoft.AspNetCore": "Warning"
}
```

---

## `ILogger<T>` — the .NET standard interface

ASP.NET Core ships with a built-in logging abstraction. You never depend on a concrete logger — always inject `ILogger<T>`:

```csharp
public class MarkStandingsAsSeededStep
{
    private readonly ILogger<MarkStandingsAsSeededStep> _logger;

    public MarkStandingsAsSeededStep(ILogger<MarkStandingsAsSeededStep> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(StartGroupsContext context)
    {
        _logger.LogInformation("Step 6: Marking standings as seeded for tournament {TournamentId}",
            context.TournamentId);
        // ...
    }
}
```

The `<T>` becomes the **category** in the log output — tells you exactly which class emitted the log. This is how all your TO2 pipeline steps are already wired.

---

## Structured logging — the critical concept

### The problem with string interpolation

```csharp
// ❌ BAD — string interpolation
_logger.LogInformation($"Tournament {tournamentId} started by {user}");
// Output: "Tournament 42 started by alice"
// The values are baked into the string — not queryable, not filterable.
```

### Structured logging with message templates

```csharp
// ✅ GOOD — message template
_logger.LogInformation("Tournament {TournamentId} started by {UserName}", tournamentId, user);
// Output: message="Tournament {TournamentId} started by {UserName}" TournamentId=42 UserName="alice"
```

The named placeholders (`{TournamentId}`, `{UserName}`) become **searchable properties** in log aggregators. You can query: "show me all logs where `TournamentId = 42`" instead of doing a string grep.

**Your TO2 code already does this correctly:**

```csharp
_logger.LogInformation("Starting bracket pipeline for tournament {TournamentId}", tournamentId);
_logger.LogError(ex, "Pipeline execution failed, changes rolled back: {Message}", ex.Message);
```

**Interview answer:** "Structured logging emits key-value pairs alongside the message. Instead of interpolating values into a string, you pass named parameters that log sinks can index and query.  I can filter by `TournamentId = 42` rather than grepping strings."

---

## Log level-based routing in GlobalExceptionHandler (TO2)

Your middleware already demonstrates best practice — log level matches HTTP status:

```csharp
if (statusCode >= 500)
    _logger.LogError(exception, "Internal server error: {Message}", exception.Message);
else if (statusCode >= 400)
    _logger.LogWarning(exception, "Client error ({StatusCode}): {Message}", statusCode, exception.Message);
```

500s = `Error` (your fault). 400s = `Warning` (client's fault, but worth tracking).

---

## Serilog — the go-to third-party library

The built-in `ILogger<T>` abstraction is fine, but **Serilog** is the standard third-party provider for structured logging in .NET. It plugs into `ILogger<T>` — your code doesn't change, only the sink (output destination) configuration does.

```csharp
// Program.cs
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .WriteTo.Console()
       .WriteTo.Seq("http://localhost:5341"));
```

**Common Serilog sinks:**

|Sink|Use case|
|---|---|
|`Console`|Local dev|
|`File`|Simple persistent logs|
|`Seq`|Local structured log viewer (queryable UI)|
|`Application Insights`|Azure cloud monitoring|
|`Elasticsearch`|Large-scale log aggregation|

**Interview answer:** "I use `ILogger<T>` in all my classes so the code is decoupled from the logging provider. In TO2, the built-in provider writes to console. In production I'd add Serilog with Application Insights or Seq as the sink — one line change in `Program.cs`."

---

## Common interview questions

- **"What is structured logging?"** → Log events with named properties, not string-concatenated messages. Properties stay queryable in aggregators like Seq or Application Insights.
- **"What's the log level order?"** → Trace < Debug < Information < Warning < Error < Critical.
- **"Why `ILogger<T>` and not a static logger?"** → Testable (can be mocked), respects DI lifetime, category name is automatic from `<T>`.
- **"String interpolation vs message template?"** → Interpolation bakes values into the string and loses them as data. Templates preserve them as structured properties.
- **"What log level for a caught exception that was handled?"** → `Warning` if the app recovered, `Error` if the operation failed.