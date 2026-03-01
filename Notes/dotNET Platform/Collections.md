## Collection interfaces

### 1. IEnumerable<T>

The most basic collection interface — it just means "you can loop over this." Any type that implements it works with `foreach` and LINQ. Nothing more, nothing less.

- `GetEnumerator()` — returns an `IEnumerator<T>`

### 2. IEnumerator<T>

The actual cursor that moves through the collection. When you write `foreach`, the compiler calls `GetEnumerator()` to get one of these, then repeatedly calls `MoveNext()` and reads `Current` until `MoveNext()` returns false.

- `MoveNext()` — move to next element, returns `false` when done
- `Current` — the element you're currently on
- `Reset()` — go back to the start (rarely used)

### 3. ICollection<T>

Builds on `IEnumerable<T>` — so you can still loop over it, but now you can also add and remove items and check how many there are.

- `Add(T)`, `Remove(T)`, `Clear()`, `Contains(T)`
- `Count`, `IsReadOnly`

### 4. IList<T>

Builds on `ICollection<T>` — adds the ability to access elements by position (index). `List<T>` implements this.

- `this[int index]` — indexer
- `IndexOf(T)`, `Insert(int, T)`, `RemoveAt(int)`

### 5. IDictionary<TKey, TValue>

A key-value collection — look up values by key instead of by position. `Dictionary<K,V>` implements this.

- `Add(TKey, TValue)`, `Remove(TKey)`, `ContainsKey(TKey)`
- `TryGetValue(TKey, out TValue)` — safe lookup, no exception on missing key
- `Keys`, `Values`

---

## Read-only interfaces (interview-important)

### IReadOnlyList<T>

Read-only indexed access — `Count` and `this[int index]`, no mutation methods. Use as **return type** to signal callers cannot modify the collection.

```csharp
public class TournamentService
{
    private readonly List<Tournament> _tournaments = new();

    public IReadOnlyList<Tournament> GetAll() => _tournaments.AsReadOnly();
}
```

Your `IRepository<T>` already uses this: `Task<IReadOnlyList<T>> GetAllAsync()`.

### IReadOnlyDictionary<TKey, TValue>

Read-only key-value access. Exposes `TryGetValue`, `ContainsKey`, `Keys`, `Values` — no `Add`/`Remove`.

**Why this matters in interviews:** It's about encapsulation and intent. Returning a mutable `List<T>` from a service leaks internal state. `IReadOnlyList<T>` is the contract: _you can read, not modify._

---

## Thread-safe collections (System.Collections.Concurrent)

### ConcurrentDictionary<TKey, TValue>

Thread-safe dictionary — no explicit locking needed for reads and writes.

Key methods:

- `TryAdd(TKey, TValue)` — adds if key absent
- `TryRemove(TKey, out TValue)` — removes and returns value
- `TryGetValue(TKey, out TValue)` — safe lookup
- `GetOrAdd(TKey, Func<TKey, TValue>)` — atomic get-or-create
- `AddOrUpdate(TKey, TValue, Func<TKey,TValue,TValue>)` — atomic add-or-update

```csharp
var cache = new ConcurrentDictionary<string, TournamentDto>();

// Atomic: only calls LoadFromDb if key doesn't exist
var dto = cache.GetOrAdd(tournamentId, id => LoadFromDb(id));
```

**When to use:** ASP.NET Core in-memory caching, background services, parallel processing. For single-threaded code, `Dictionary<TKey,TValue>` is faster.

Other concurrent collections (awareness level):

- `ConcurrentQueue<T>` — thread-safe FIFO
- `ConcurrentStack<T>` — thread-safe LIFO
- `ConcurrentBag<T>` — thread-safe unordered
- `BlockingCollection<T>` — producer-consumer wrapper

---

## Concrete data structures

|Type|Ordered|Duplicates|Key access|Notes|
|---|---|---|---|---|
|`List<T>`|✅ insertion|✅|By index|General purpose, backed by array|
|`Dictionary<K,V>`|❌|Keys: ❌|By key O(1)|Hash-based|
|`HashSet<T>`|❌|❌|—|Unique elements, set operations|
|`Queue<T>`|✅ FIFO|✅|—|`Enqueue`/`Dequeue`|
|`Stack<T>`|✅ LIFO|✅|—|`Push`/`Pop`|
|`LinkedList<T>`|✅|✅|—|O(1) insert/remove at known node|
|`SortedDictionary<K,V>`|✅ by key|Keys: ❌|By key O(log n)|Red-black tree|

---

## Interface hierarchy

```
IEnumerable<T>
├── ICollection<T>
│   ├── IList<T>           → List<T>, arrays
│   └── IDictionary<K,V>  → Dictionary<K,V>
├── IReadOnlyCollection<T>
│   ├── IReadOnlyList<T>
│   └── IReadOnlyDictionary<K,V>
└── (Concurrent)
    ├── ConcurrentDictionary<K,V>
    ├── ConcurrentQueue<T>
    └── ConcurrentBag<T>
```

---

## Interview traps

- **"What's the difference between `IEnumerable` and `ICollection`?"** — `IEnumerable` is read-only iteration only. `ICollection` adds mutation (`Add`/`Remove`) and `Count`.
- **"Why return `IReadOnlyList<T>` instead of `List<T>` from a service?"** — Encapsulation. Callers get no ability to mutate your internal state. Intent is explicit at the interface level.
- **"How does `foreach` work under the hood?"** — Calls `GetEnumerator()`, then loops `MoveNext()` / `Current`. `yield return` lets the compiler build the enumerator state machine for you.
- **"When would you use `ConcurrentDictionary` over `Dictionary`?"** — Any shared mutable state accessed from multiple threads. In ASP.NET Core, the DI container and `IMemoryCache` handle most of this for you — reaching for `ConcurrentDictionary` directly usually means you're building a custom cache or registry.
- **"Dictionary vs HashSet?"** — Both are hash-based O(1) lookup. `HashSet<T>` stores values only, used when you care about existence not association. Use for deduplication or set intersection/union operations.

---

### Related

[[Generic]] [[LINQ]] [[IEnumerable vs IQueryable]] [[Array methods]]