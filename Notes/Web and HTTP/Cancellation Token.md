# CancellationToken & Parallel Programming

---

## CancellationToken

`CancellationToken` is .NET's cooperative cancellation mechanism. It lets you signal a running async operation to stop gracefully — without killing threads.

### How it works

Three parts work together:

```csharp
// 1. CancellationTokenSource — the controller
var cts = new CancellationTokenSource();

// 2. CancellationToken — the signal passed to operations
CancellationToken token = cts.Token;

// 3. Cancel from outside
cts.Cancel(); // signals the token
```

### Checking cancellation inside a method

```csharp
// Option A: Throws OperationCanceledException automatically (preferred)
token.ThrowIfCancellationRequested();

// Option B: Check and handle manually
if (token.IsCancellationRequested)
{
    // clean up, then...
    return; // or throw
}
```

### Passing through the call chain

Always propagate the token down:

```csharp
public async Task ProcessTournamentAsync(long id, CancellationToken cancellationToken = default)
{
    var tournament = await _repo.GetByIdAsync(id, cancellationToken); // pass it here
    await _unitOfWork.SaveChangesAsync(cancellationToken);            // and here
}
```

`default` means "no cancellation" if the caller doesn't provide one — safe default for internal calls.

### In ASP.NET Core — automatic cancellation

ASP.NET Core injects `HttpContext.RequestAborted` — a token that fires automatically when the client disconnects:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetTournament(long id, CancellationToken cancellationToken)
{
    // If client disconnects mid-request, DB query is cancelled — no wasted work
    var result = await _service.GetAsync(id, cancellationToken);
    return Ok(result);
}
```

The framework automatically binds the `CancellationToken` parameter from `HttpContext.RequestAborted`. This is the standard pattern — always include it in controller actions.

### Your TO2 code uses it

```csharp
// IUnitOfWork
Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
```

Every `SaveChangesAsync` call in your pipelines respects cancellation — if a request is abandoned mid-pipeline, EF Core won't try to commit a partial transaction.

### Timeout — CancellationTokenSource with deadline

```csharp
// Auto-cancel after 5 seconds
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await DoWorkAsync(cts.Token);
```

### Linked tokens — combine multiple signals

```csharp
// Cancel if either the user aborts OR the timeout fires
using var cts = CancellationTokenSource.CreateLinkedTokenSource(
    httpContext.RequestAborted,
    timeoutToken);
await DoWorkAsync(cts.Token);
```

### Common interview questions

- **"What is CancellationToken?"** → A cooperative cancellation primitive. The caller signals intent to cancel; the callee checks and stops gracefully. Nothing is forcibly killed.
- **"Why use it in controller actions?"** → If the client disconnects, the server should stop doing expensive work (DB queries, external calls). ASP.NET Core provides `HttpContext.RequestAborted` for this automatically.
- **"ThrowIfCancellationRequested vs IsCancellationRequested?"** → `ThrowIfCancellationRequested` throws `OperationCanceledException` — clean, standard way that propagates up. `IsCancellationRequested` is for when you need to do cleanup before stopping.
- **"What's the default parameter pattern?"** → `CancellationToken cancellationToken = default` — makes the token optional without breaking callers that don't supply one.

---

## Parallel Programming — key concepts

### When to use what

| Problem type | Solution |
|---|---|
| I/O-bound (DB, HTTP, file) | `async/await` — don't block threads |
| CPU-bound (calculations, transformations) | `Parallel.ForEach` or `Task.Run` |
| Multiple independent async ops | `Task.WhenAll` — run concurrently |

### Parallel.ForEach

Distributes loop iterations across thread pool threads. Use for CPU-bound work on collections:

```csharp
// Process images in parallel across CPU cores
Parallel.ForEach(images, img => ProcessImage(img));

// With cancellation support
Parallel.ForEach(images, new ParallelOptions { CancellationToken = token },
    img => ProcessImage(img));
```

⚠️ Don't use `Parallel.ForEach` for I/O-bound work — you'll exhaust the thread pool. Use `Task.WhenAll` instead.

### Task.WhenAll — concurrent async operations

```csharp
// Run multiple DB queries concurrently, wait for all
var (teams, standings) = await (
    _teamRepo.GetAllAsync(token),
    _standingRepo.GetAllAsync(token)
);

// Or with array
var tasks = ids.Select(id => _repo.GetByIdAsync(id, token));
var results = await Task.WhenAll(tasks);
```

### AggregateException

When parallel operations fail, exceptions are wrapped in `AggregateException`:

```csharp
try
{
    await Task.WhenAll(tasks);
}
catch (AggregateException ex)
{
    foreach (var inner in ex.InnerExceptions)
        _logger.LogError(inner, "Task failed: {Message}", inner.Message);
}
```

### Thread safety — shared state

Parallel operations sharing mutable state = race conditions. Solutions:

```csharp
// lock — mutual exclusion
lock (_syncObject) { _counter++; }

// Interlocked — atomic operations on primitives (faster than lock)
Interlocked.Increment(ref _counter);

// ConcurrentDictionary — thread-safe collection
var dict = new ConcurrentDictionary<int, string>();
dict.TryAdd(key, value);
```

### Common interview questions

- **"Parallel.ForEach vs Task.WhenAll?"** → `Parallel.ForEach` = CPU-bound, uses thread pool threads. `Task.WhenAll` = I/O-bound, runs async tasks concurrently without blocking threads.
- **"What is a race condition?"** → Multiple threads accessing shared mutable state without synchronization — result depends on execution order.
- **"How do you cancel a Parallel.ForEach?"** → Pass `ParallelOptions` with a `CancellationToken`.
