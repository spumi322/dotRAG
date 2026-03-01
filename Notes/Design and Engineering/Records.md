### *What records are and how they differ from classes and structs.*

Records (C# 9+) are types designed for **immutable data** with **value-based equality**. The compiler auto-generates `Equals`, `GetHashCode`, `ToString`, `Deconstruct`, and a copy constructor (`with` expression).

```csharp
// Positional record class (C# 9) — reference type, immutable by default
public record ProductDto(int Id, string Name, decimal Price);

var p1 = new ProductDto(1, "Widget", 9.99m);
var p2 = new ProductDto(1, "Widget", 9.99m);

p1 == p2       // true  — value equality, not reference equality
p1.ToString()  // "ProductDto { Id = 1, Name = Widget, Price = 9.99 }"
```

**`with` expression** — creates a copy with modified properties:
```csharp
var discounted = p1 with { Price = 7.99m };
// ProductDto { Id = 1, Name = Widget, Price = 7.99 }
```

### *record class vs record struct*

```csharp
// record class (C# 9) — reference type, positional props are init-only
public record OrderResponse(int OrderId, string Status);

// record struct (C# 10) — value type, positional props are mutable by default
public record struct Coordinate(double Lat, double Lng);

// readonly record struct (C# 10) — value type, immutable
public readonly record struct Money(decimal Amount, string Currency);
```

| Feature | `record` (class) | `record struct` | `readonly record struct` |
|---|---|---|---|
| Type | Reference | Value | Value |
| Immutable by default | ✅ (positional) | ❌ (need `readonly`) | ✅ |
| `with` expression | ✅ | ✅ | ✅ |
| Inheritance | ✅ (from other records) | ❌ | ❌ |
| Value equality | ✅ | ✅ | ✅ |
| Heap/Stack | Heap | Stack (when local) | Stack (when local) |

### *When to use records*

- **DTOs** (Data Transfer Objects) — API request/response models
- **Value Objects** in DDD (e.g., `Money`, `Address`)
- **Immutable configuration** or event data
- Anywhere you need **value equality without boilerplate**

### *When NOT to use records*

- **EF Core entities** — EF relies on reference equality and change tracking, records break this
- **Mutable state** — if properties need to change frequently, use a class
- **Complex behavior** — records are data-first; classes are behavior-first

### *Deconstruction*

Positional records auto-generate a `Deconstruct` method:
```csharp
var (id, name, price) = p1; // deconstruct into variables
```

[[Reference and Value Types]]
[[Data Types]]
[[Abstraction]]