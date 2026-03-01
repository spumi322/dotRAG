### *What Abstraction is and the abstraction mechanisms implemented in the .NET platform: abstract classes and interfaces.*

Hiding **how** something works, exposing only **what** it does. Abstract classes and interfaces are offering templates for other classes.

### *Abstract Class*

Abstract class = blueprint. Defines HOW derived classes must look like.
Cannot be instantiated directly — only inherited.

```csharp
public abstract class EntityBase
{
    // FIELD — state data (interfaces can't have this)
    private DateTime _createdAt = DateTime.UtcNow;

    // PROPERTY — concrete (inherited as-is) or abstract (must override)
    public long Id { get; protected set; }
    public abstract string DisplayName { get; } // derived class MUST implement

    // CONSTRUCTOR — called via base() from derived class
    protected EntityBase(long id) => Id = id;

    // REGULAR METHOD — default implementation, optionally overridable
    public virtual string Describe() => $"Entity #{Id}";

    // ABSTRACT METHOD — no body, derived class MUST override
    public abstract bool IsValid();

    // STATIC MEMBER — belongs to the type, not the instance
    public static EntityBase Create() => throw new NotImplementedException();

    // OPERATOR OVERLOAD — define what == means for your type
    public static bool operator ==(EntityBase a, EntityBase b) => a?.Id == b?.Id;
    public static bool operator !=(EntityBase a, EntityBase b) => !(a == b);
}

// Derived class must implement all abstract members
public class Tournament : EntityBase
{
    public string Name { get; }
    public override string DisplayName => Name;           // required
    public override bool IsValid() => !string.IsNullOrEmpty(Name); // required
    public override string Describe() => $"Tournament: {Name}";   // optional override

    public Tournament(long id, string name) : base(id)   // calls EntityBase(id)
        => Name = name;
}
```

### *Interfaces*

Interface = contract. Defines WHAT a class can do, not how.
Stateless — no fields, no instance state.
```csharp
public interface IRepository<T>
{
    // METHOD — public abstract by default, no body
    Task<T?> GetByIdAsync(long id);

    // PROPERTY — public, no implementation
    string EntityName { get; }

    // EVENT — signals something happened (consumer can subscribe)
    event EventHandler OnSaved;

    // DEFAULT METHOD — has a body, implementing class inherits it
    // unless it overrides. Useful for adding methods without breaking
    // all existing implementations.
    void Log(string msg) => Console.WriteLine($"[{EntityName}] {msg}");

    // STATIC MEMBER — belongs to the interface type itself
    static string Version => "1.0";
}
```
### Key decision: Abstract class vs Interface

| Criteria | Abstract Class | Interface |
|---|---|---|
| State (fields) | ✅ Yes | ❌ No |
| Constructors | ✅ Yes | ❌ No |
| Multiple inheritance | ❌ Single only | ✅ Multiple |
| Default implementation | ✅ Always could | ✅ Since C# 8 |
| Static abstract members | ❌ No | ✅ Since C# 11 |
| Access modifiers on members | ✅ Any | ✅ Since C# 8 (limited) |
| Use when | Shared state/behavior among related types | Defining a capability/contract across unrelated types |
