The Garbage Collector (GC) in .NET manages the allocation and release of memory for applications.

_Role of Garbage Collector:_

- Automatically manages memory by freeing objects that are no longer in use, preventing memory leaks.
- Allocates memory for new objects and reclaims memory from objects no longer referenced by the application.
- **Only handles managed memory (heap-allocated reference types).** Unmanaged resources (file handles, DB connections, sockets) are NOT cleaned up by the GC — you must use `IDisposable` / `using` for those. See [[Managed & Unmanaged Resources]].

_Operation and Algorithm:_

- The GC periodically runs to identify and collect objects that are no longer reachable from the roots (variables on the stack, static variables, GC handles, etc.).
- Uses a **mark-and-sweep** approach: marks objects that are reachable, then sweeps away unmarked objects, reclaiming their memory.
- The GC also **compacts** memory after collection (in Gen 0/1), moving surviving objects together to reduce fragmentation.

_Generations in Garbage Collection:_

Objects are categorized into three generations (0, 1, and 2) based on their lifetime to optimize the garbage collection process.

**Generation 0:** Contains short-lived objects (local variables, temporary objects). Collected frequently. Most objects die here.

**Generation 1:** Buffer between short-lived and long-lived objects. Collected less frequently than Gen 0. Objects that survive Gen 0 collection are promoted here.

**Generation 2:** Contains long-lived objects (static data, caches, singletons). Collected infrequently. A Gen 2 collection is a **full GC** and is expensive.

The use of generations improves efficiency: most objects are short-lived, so collecting Gen 0 frequently and cheaply handles the majority of garbage. Less frequent Gen 1/2 collections reduce performance impact.

### Large Object Heap (LOH)

Objects ≥ 85,000 bytes are allocated on the **Large Object Heap**, which is collected with Gen 2 but is **not compacted by default** (compaction can be enabled with `GCSettings.LargeObjectHeapCompactionMode`). This can lead to fragmentation.

### Common interview question: `GC.Collect()`

⚠️ Calling `GC.Collect()` manually is **almost always wrong**. It forces a full collection, disrupts the GC's generational optimization, and degrades performance. The GC is self-tuning. Only legitimate use cases: benchmarking, or after releasing a massive one-time resource.

```csharp
// ❌ Don't do this in production code
GC.Collect();
GC.WaitForPendingFinalizers();

// ✅ Instead, let the GC manage itself and use IDisposable for cleanup
using var stream = new FileStream("file.txt", FileMode.Open);
```

### GC Modes

- **Workstation GC:** Default for client apps. Single GC thread, lower latency.
- **Server GC:** For server apps (ASP.NET Core default). One GC thread per core, higher throughput.

Configure in `.csproj`:
```xml
<ServerGarbageCollection>true</ServerGarbageCollection>
```

### Related

[[Stack  & Heap]]
[[Managed & Unmanaged Resources]]
[[CLR]]