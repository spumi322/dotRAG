### *What LINQ is and why it's used*

LINQ (Language-Integrated Query) provides a unified syntax for querying different data sources — in-memory collections, databases, XML — directly in C#. Type-safe, compile-time checked, IntelliSense support.

**Two syntaxes:**
- **Query syntax** (SQL-like): `from x in list where x > 5 select x`
- **Method syntax** (preferred): `list.Where(x => x > 5)`

---

### *Deferred execution — queries don't run when you write them*

LINQ queries are **lazy** — they don't execute until you enumerate the results.
```csharp
var numbers = new List<int> { 1, 2, 3 };
var query = numbers.Where(n => n > 1); // ⚠️ NOT executed yet — just built

numbers.Add(4); // Modify source AFTER defining query

foreach (var n in query) // ✅ Executes NOW
    Console.WriteLine(n); // Prints: 2, 3, 4
```

**When queries execute:**
- Iteration: `foreach`, `.ToList()`, `.ToArray()`, `.ToDictionary()`
- Aggregation: `.Count()`, `.Sum()`, `.Any()`, `.First()`, `.Single()`

---

### *Core LINQ operators*

**Filtering:**
```csharp
var adults = users.Where(u => u.Age >= 18);
var active = products.Where(p => p.IsActive && p.Stock > 0);
```

**Projection (transforming):**
```csharp
// Select — transform each element
var names = users.Select(u => u.Name);
var dtos = users.Select(u => new UserDto(u.Id, u.Name));

// SelectMany — flatten nested collections
var allOrders = customers.SelectMany(c => c.Orders);
// Input: [Customer1[Order1, Order2], Customer2[Order3]]
// Output: [Order1, Order2, Order3]
```

**Sorting:**
```csharp
var sorted = products.OrderBy(p => p.Price);          // ascending
var desc = products.OrderByDescending(p => p.Price);  // descending
var multi = products.OrderBy(p => p.Category).ThenBy(p => p.Name);
```

**Grouping:**
```csharp
var byCategory = products.GroupBy(p => p.Category);

foreach (var group in byCategory)
{
    Console.WriteLine($"Category: {group.Key}");
    foreach (var product in group)
        Console.WriteLine($"  - {product.Name}");
}

// With aggregation
var categoryCounts = products
    .GroupBy(p => p.Category)
    .Select(g => new { Category = g.Key, Count = g.Count() });
```

**Joining:**
```csharp
// Inner join
var result = users.Join(
    orders,
    user => user.Id,           // outer key
    order => order.UserId,     // inner key
    (user, order) => new { user.Name, order.Total }
);

// Left join (with GroupJoin + SelectMany)
var leftJoin = users
    .GroupJoin(orders, u => u.Id, o => o.UserId, (u, orders) => new { u, orders })
    .SelectMany(x => x.orders.DefaultIfEmpty(), (x, order) => new { x.u.Name, order?.Total });
```

**Element operators:**
```csharp
// First vs Single — critical difference
var first = users.First();              // First element, throws if empty
var firstOrDefault = users.FirstOrDefault(); // First or null/default, doesn't throw

var single = users.Single();            // Exactly ONE element, throws if 0 or >1
var singleOrDefault = users.SingleOrDefault(); // Exactly one or default, throws if >1

// With predicate
var admin = users.First(u => u.Role == "Admin");
var premium = users.SingleOrDefault(u => u.IsPremium); // Expects 0 or 1 premium user
```

**Existence checks:**
```csharp
bool hasAdults = users.Any(u => u.Age >= 18);  // at least one
bool allActive = users.All(u => u.IsActive);   // every single one
```

**Pagination:**
```csharp
var page = products
    .OrderBy(p => p.Id)
    .Skip(pageSize * pageNumber)
    .Take(pageSize);
```

---

### *Common pitfalls*

**1. Multiple enumeration — executes query multiple times**
```csharp
var expensiveQuery = dbContext.Products.Where(p => CalculateDiscount(p) > 0);

var count = expensiveQuery.Count();      // ⚠️ Executes query
var list = expensiveQuery.ToList();      // ⚠️ Executes AGAIN

// ✅ Fix: materialize once
var results = expensiveQuery.ToList();
var count = results.Count;
```

**2. Modifying collection during iteration**
```csharp
var numbers = new List<int> { 1, 2, 3 };
foreach (var n in numbers)
    numbers.Add(n * 2); // ☠️ InvalidOperationException: Collection was modified

// ✅ Fix: iterate over a copy
foreach (var n in numbers.ToList())
    numbers.Add(n * 2);
```

**3. Forgetting deferred execution**
```csharp
var threshold = 100;
var query = products.Where(p => p.Price > threshold);
threshold = 200; // Changed AFTER query definition
var list = query.ToList(); // Uses threshold = 200, not original value
```

---

### *When to use `.ToList()` / `.ToArray()`*

**Materialize (call `.ToList()`) when:**
- You'll enumerate multiple times
- You need the count AND the items
- You want a snapshot (ignore future changes to source)
- You're returning from a method and want deferred execution to stop

**Keep deferred when:**
- You'll enumerate only once (e.g., single `foreach`)
- You're chaining more LINQ operations
- You're building a query pipeline

---

### *Common interview questions*

**Q:** "What's deferred execution?"  
**A:** LINQ queries don't execute when you write them — they execute when you enumerate (foreach, .ToList(), .Count()). This allows query composition and reflects changes to the source collection.

**Q:** "Difference between `First()` and `Single()`?"  
**A:** `First()` returns the first element, throws if empty. `Single()` expects exactly one element, throws if zero OR multiple. Use `Single()` when you expect uniqueness (e.g., "get user by unique email").

**Q:** "`Select()` vs `SelectMany()`?"  
**A:** `Select()` transforms each element (one-to-one). `SelectMany()` flattens nested collections (one-to-many) — e.g., customers → all their orders in a flat list.

**Q:** "What's the problem with this code: `var count = query.Count(); var list = query.ToList();`?"  
**A:** The query executes twice — once for Count(), once for ToList(). If it's a database query, that's two round trips. Fix: `var list = query.ToList(); var count = list.Count;`

---

[[Collections]]
[[IEnumerable vs IQueryable]]
[[Generic]]