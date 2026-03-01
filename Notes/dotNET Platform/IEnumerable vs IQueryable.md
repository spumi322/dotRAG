### *IEnumerable\<T\> vs IQueryable\<T\> 

Both represent sequences of elements, but they execute in fundamentally different places.

### *IEnumerable\<T\> — in-memory execution*

- Defined in `System.Collections.Generic`.
- Evaluates queries **in memory** (client-side).
- Uses **delegates** (`Func<T, bool>`) for filtering.
- Once data is pulled from the database, all further operations happen in C#.

```csharp
IEnumerable<Product> products = _context.Products; // loads ALL rows into memory
var expensive = products.Where(p => p.Price > 100); // filtered in C# memory
```

### *IQueryable\<T\> — database execution*

- Defined in `System.Linq`, extends `IEnumerable<T>`.
- Builds an **expression tree** that is translated to SQL and executed on the database.
- Uses **expressions** (`Expression<Func<T, bool>>`) — the provider (EF Core) parses these into SQL.

```csharp
IQueryable<Product> products = _context.Products; // no query yet — just a query builder
var expensive = products.Where(p => p.Price > 100); // translated to: WHERE Price > 100
var list = await expensive.ToListAsync();           // NOW the SQL executes
```

### *Why it matters*

```csharp
// ❌ Loads entire Products table, then filters in C#
public IEnumerable<Product> GetExpensive()
{
    IEnumerable<Product> all = _context.Products;
    return all.Where(p => p.Price > 100); // WHERE happens in memory
}

// ✅ Database does the filtering — only matching rows are transferred
public async Task<List<Product>> GetExpensive()
{
    return await _context.Products
        .Where(p => p.Price > 100) // translated to SQL WHERE
        .ToListAsync();
}
```

### *Key differences*

| | IEnumerable\<T\> | IQueryable\<T\> |
|---|---|---|
| Execution | In-memory (C#) | Database (SQL) |
| Filter type | `Func<T, bool>` | `Expression<Func<T, bool>>` |
| Deferred execution | ✅ | ✅ |
| Best for | In-memory collections, LINQ to Objects | EF Core, database queries |
| Performance risk | Loads all data first | Only fetches matching rows |

[[LINQ]]
[[Collections]]
[[ORM & EFCore]]
