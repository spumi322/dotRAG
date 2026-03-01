
---

## Managed vs Unmanaged

||Managed|Unmanaged|
|---|---|---|
|**What**|Objects handled by the CLR|OS-level resources outside CLR control|
|**Cleanup**|GC handles it automatically|You must release explicitly|
|**Examples**|`string`, `List<T>`, class instances, arrays|`FileStream`, `SqlConnection`, `Socket`, `HttpClient`|

> **Key rule:** GC frees managed memory. It cannot free unmanaged resources — that's your job.

---

## IDisposable — The Solution

Implement `IDisposable` on any class that holds unmanaged resources (or wraps another `IDisposable`).

```csharp
// Simple case — wrapping another IDisposable (most common in practice)
public class TournamentExporter : IDisposable
{
    private FileStream _file;
    private bool _disposed = false;

    public TournamentExporter(string path)
    {
        _file = new FileStream(path, FileMode.Create);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // tell GC: no need to call finalizer
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _file?.Dispose(); // free managed IDisposable objects
        }
        // free raw unmanaged resources here (if any)

        _disposed = true;
    }

    ~TournamentExporter() // finalizer = safety net only
    {
        Dispose(false);
    }
}
```

### The `disposing` parameter — why it exists

|Called from|`disposing` value|What to clean|
|---|---|---|
|`Dispose()` / `using`|`true`|Managed + unmanaged resources|
|Finalizer (`~Class`)|`false`|Unmanaged resources only (managed may already be GC'd)|

---

## `using` — Always Prefer This

`using` compiles to a `try/finally` that calls `Dispose()` automatically — even if an exception is thrown.

```csharp
// Classic block (any C# version)
using (var conn = new SqlConnection(connStr))
{
    // conn.Dispose() called here even if exception thrown
}

// using declaration (C# 8+) — same guarantee, less nesting
using var conn = new SqlConnection(connStr);
using var reader = new StreamReader(filePath);
// both disposed when method exits
```

> **Rule of thumb:** If you see a class implementing `IDisposable` — wrap it in `using`. Common ones: `SqlConnection`, `FileStream`, `StreamReader`, `HttpClient`, `DbContext`

---

## Finalizer — The Safety Net

A finalizer (`~ClassName`) is called by the GC if `Dispose()` was never called. It's a fallback, not the primary mechanism.

⚠️ **Finalizer penalty:** Objects with a finalizer are NOT collected in Gen 0. They survive to Gen 1 first, then get finalized and collected in the next cycle. This costs an extra GC pass.

```csharp
// This is why you call GC.SuppressFinalize(this) in Dispose()
// — it removes the object from the finalization queue immediately
// so it gets collected in Gen 0 like a normal object
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this); // skip the finalizer penalty
}
```

**Only implement a finalizer if your class directly owns raw unmanaged handles.** If you're just wrapping another `IDisposable` — no finalizer needed.

---

## Dispose vs Finalize — Quick Reference

||`Dispose()`|`Finalize()` / Destructor|
|---|---|---|
|**Called by**|Developer / `using`|GC (automatically)|
|**Timing**|Deterministic — you control when|Non-deterministic — GC decides|
|**Performance**|Fast|Slow (extra GC cycle)|
|**Use for**|Primary cleanup path|Safety net fallback only|

---

## Streams — Practical Usage

You don't need to know stream architecture theory. Know how to use them safely:

```csharp
// Reading a file
using var stream = new FileStream("data.json", FileMode.Open);
using var reader = new StreamReader(stream);
var content = await reader.ReadToEndAsync();

// Writing a file
using var writer = new StreamWriter("output.txt");
await writer.WriteLineAsync("Tournament results");
```

- `FileStream` — raw bytes, read/write files
- `StreamReader` / `StreamWriter` — text-friendly wrapper over a stream

---

## Serialization — What Actually Matters

Only JSON serialization matters for your day-to-day and interviews.

```csharp
// Serialize object → JSON string
var json = JsonSerializer.Serialize(tournament);

// Deserialize JSON string → object
var tournament = JsonSerializer.Deserialize<Tournament>(json);

// In ASP.NET Core — done automatically by the framework
// Controllers serialize return values to JSON via System.Text.Json
```

> `System.Text.Json` is the default in ASP.NET Core (replaced `Newtonsoft.Json`). Know it exists, know it's automatic in controllers — no deep knowledge needed at junior level.

---

## Related

- [[GC]]
- [[CLR]]
- [[Stack & Heap]]