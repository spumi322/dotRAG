## At a Glance

| Structure         | Internal            | Ordered | Duplicates | Index Access | Use when                        |
| ----------------- | ------------------- | ------- | ---------- | ------------ | ------------------------------- |
| `Array`           | Contiguous block    | ✅       | ✅          | ✅ O(1)       | Fixed size, max performance     |
| `List<T>`         | Dynamic array       | ✅       | ✅          | ✅ O(1)       | Default ordered collection      |
| `Dictionary<K,V>` | Hash table          | ❌       | Keys: ❌    | By key O(1)  | Key-value lookup                |
| `HashSet<T>`      | Hash table          | ❌       | ❌          | ❌            | Uniqueness, set operations      |
| `Stack<T>`        | Dynamic array       | LIFO    | ✅          | Top only     | Undo, call tracking             |
| `Queue<T>`        | Circular array      | FIFO    | ✅          | Front only   | Processing in order             |
| `LinkedList<T>`   | Doubly-linked nodes | ✅       | ✅          | ❌ O(n)       | Frequent mid-list insert/delete |
| `ArrayList`       | Dynamic array       | ✅       | ✅          | ✅ O(1)       | ⛔ Legacy only — use `List<T>`   |

> Complexity reference: see `Algorithms & BigO.md`

---

## Array

Fixed size, allocated as a contiguous block. Fastest possible index access.

```csharp
int[] scores = new int[3];
scores[0] = 10;

int[] seeded = { 10, 20, 30 };
int first = seeded[0];       // O(1)

// Useful Array statics
Array.Sort(seeded);          // in-place, IntroSort O(n log n)
Array.Reverse(seeded);
int idx = Array.BinarySearch(seeded, 20); // O(log n), requires sorted

// 2D
int[,] matrix = new int[3, 3];
matrix[0, 1] = 5;
```

Arrays implement `IList<T>` but `Add()`/`Remove()` throw — size is fixed at allocation.

---

## List<T>

Dynamic array under the hood. Doubles capacity when full (amortized O(1) append).

```csharp
var teams = new List<string> { "Alpha", "Beta" };

teams.Add("Gamma");           // O(1) amortized
teams.Insert(0, "Delta");     // O(n) — shifts everything right
teams.Remove("Beta");         // O(n) — finds then shifts
teams.RemoveAt(2);            // O(n) — shifts
bool has = teams.Contains("Alpha"); // O(n) linear scan

// Prefer LINQ for querying
var sorted = teams.OrderBy(t => t).ToList();
```

---

## Dictionary<TKey, TValue>

Hash table. Key is hashed → bucket index. Average O(1) for get/set/delete. Worst case O(n) on hash collision (rare with good hash functions).

```csharp
var standings = new Dictionary<string, int>
{
    ["Alpha FC"] = 9,
    ["Beta SC"]  = 6
};

standings["Gamma"] = 3;               // add or update
bool found = standings.TryGetValue("Alpha FC", out int pts); // safe get
standings.ContainsKey("Delta");       // O(1)
standings.Remove("Beta SC");          // O(1)

foreach (var (team, points) in standings) // unordered iteration
    Console.WriteLine($"{team}: {points}");
```

Keys must implement `GetHashCode()` and `Equals()` correctly — custom types need this explicitly.

---

## HashSet<T>

Hash table storing only keys (no values). O(1) add/remove/contains. Guarantees uniqueness. Supports set math.

```csharp
var registeredTeams = new HashSet<string> { "Alpha", "Beta", "Gamma" };

registeredTeams.Add("Alpha");         // false — already exists, no duplicate added
registeredTeams.Contains("Beta");     // O(1) — this is the key advantage over List
registeredTeams.Remove("Gamma");

var qualified  = new HashSet<string> { "Alpha", "Beta" };
var eliminated = new HashSet<string> { "Beta", "Gamma" };

qualified.IntersectWith(eliminated);  // { "Beta" } — in both
qualified.UnionWith(eliminated);      // { "Alpha", "Beta", "Gamma" }
qualified.ExceptWith(eliminated);     // { "Alpha" } — in qualified but not eliminated
```

---

## Stack<T>

LIFO — last in, first out. Only the top is accessible.

```csharp
var history = new Stack<string>();

history.Push("GroupStage");
history.Push("Quarterfinal");
history.Push("Semifinal");

string current = history.Peek();  // "Semifinal" — look without removing
string last    = history.Pop();   // "Semifinal" — remove and return
int depth      = history.Count;   // 2
```

Real use: undo/redo, expression parsing, DFS traversal, call stack simulation.

---

## Queue<T>

FIFO — first in, first out. Only front and back are accessible.

```csharp
var matchQueue = new Queue<string>();

matchQueue.Enqueue("Match A");
matchQueue.Enqueue("Match B");
matchQueue.Enqueue("Match C");

string next = matchQueue.Peek();    // "Match A" — look without removing
string done = matchQueue.Dequeue(); // "Match A" — remove and return
int pending  = matchQueue.Count;    // 2
```

Real use: job queues, BFS traversal, request buffering, rate limiting pipelines.

---

## LinkedList<T>

Doubly-linked nodes — each node holds value + ref to previous + next. No index access. O(1) insert/delete **if you already have the node**.

```csharp
var pipeline = new LinkedList<string>();

var node1 = pipeline.AddLast("Validate");
var node2 = pipeline.AddLast("Process");
var node3 = pipeline.AddLast("Notify");

pipeline.AddBefore(node2, "Enrich");   // O(1) — no shifting
pipeline.Remove(node1);                // O(1) — just rewire pointers

// Traversal is O(n) — no index access
foreach (var stage in pipeline)
    Console.WriteLine(stage);
```

Finding a node by value is still O(n). The O(1) advantage only applies when you hold the `LinkedListNode<T>` reference.

---

## ArrayList (legacy)

Non-generic, pre-.NET 2.0. Stores `object` — every value type gets boxed. No compile-time type safety.

```csharp
// ⛔ Don't use this
var list = new ArrayList();
list.Add(1);
list.Add("oops"); // compiles fine, blows up at runtime
```

Only exists for legacy COM interop and old APIs. Always use `List<T>` instead.

---

## Interview Questions

**List<T> vs Array — when to use which?** Array when size is fixed and you need raw performance (e.g. buffer, matrix math). `List<T>` for everything else — it's dynamic and has LINQ support. Arrays are marginally faster because no resize logic, but the difference is irrelevant in typical app code.

**List<T> vs LinkedList<T>** `List<T>` wins for almost everything — O(1) index access, cache-friendly contiguous memory, works with LINQ. `LinkedList<T>` only wins when you're doing frequent insertions/deletions in the middle **and** you already hold node references. In practice, this is rare — most "I need a LinkedList" instincts are wrong.

**Dictionary vs HashSet — what's the difference?** `Dictionary<K,V>` maps keys to values. `HashSet<T>` stores only keys. Use `HashSet` when you care about membership/uniqueness, not about storing anything against a key. Internally they're the same hash table mechanism.

**HashSet.Contains() vs List.Contains()** `HashSet`: O(1) — computes hash, checks bucket. `List`: O(n) — linear scan. If you're checking membership repeatedly, put your data in a `HashSet`. This is one of the most common junior performance mistakes.

**Stack vs Queue — how to remember?** Stack = LIFO = the last plate you put on a pile. Queue = FIFO = a checkout line. Stack: `Push`/`Pop`/`Peek`. Queue: `Enqueue`/`Dequeue`/`Peek`.

**Why doesn't Dictionary guarantee order?** Hash tables store items in bucket order determined by hash codes, not insertion order. If you need insertion order, use `SortedDictionary<K,V>` (sorted by key, O(log n)) or keep a separate `List<K>` of keys. `OrderedDictionary` exists but is non-generic — avoid it.

**What happens on Dictionary hash collision?** .NET uses chaining — colliding keys go into the same bucket as a linked list. With a good hash function this is rare. Worst case (all keys hash to same bucket) degrades to O(n). This is why `GetHashCode()` must be well-distributed for custom key types.

**What does "amortized O(1)" mean for List.Add()?** `List<T>` doubles its internal array when full. Most appends are O(1). Occasionally one append triggers an O(n) copy. Averaged across many appends, it works out to O(1) per operation. If you know the final size upfront, pass it to the constructor: `new List<T>(capacity)` to avoid any resizing.

[[Array methods]]