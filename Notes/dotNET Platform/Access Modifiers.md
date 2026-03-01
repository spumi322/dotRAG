### *Access modifiers in C# — controlling visibility of types and members.*

| Modifier | Accessible from |
|---|---|
| `public` | Everywhere |
| `private` | Same class/struct only |
| `protected` | Same class + derived classes |
| `internal` | Same assembly (project) only |
| `protected internal` | Same assembly **OR** derived classes (even in other assemblies) |
| `private protected` | Same assembly **AND** derived classes (C# 7.2+) |

### *Defaults (when you omit the modifier)*

- **Class members:** `private`
- **Top-level classes:** `internal`
- **Interface members:** `public` (implicit, cannot be changed before C# 8)
- **Enum members:** `public` (always)
- **Struct members:** `private`

### *Practical examples*

```csharp
public class OrderService                    // visible everywhere
{
    private readonly IOrderRepo _repo;       // only this class
    protected int MaxRetries = 3;            // this class + subclasses
    internal void Recalculate() { }          // same project only
    public Order GetOrder(int id) => ...;    // everyone
}
```

### *Common interview questions*

- "What's the default access modifier for a class?" → `internal`.
- "What's the default for class members?" → `private`.
- "Difference between `protected internal` and `private protected`?" → `protected internal` = same assembly OR derived. `private protected` = same assembly AND derived. Think of it as: `protected internal` is wider (OR), `private protected` is narrower (AND).

### *Best practice: principle of least privilege*

Expose only what's necessary. Start with `private`, widen only when needed. Public APIs should be intentional — every `public` member is a contract you must maintain.

[[Encapsulation]]
[[OOP]]
[[CSHARP]]
