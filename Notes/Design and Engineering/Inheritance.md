

A class acquires members (fields, properties, methods) of another class. C# supports **single class inheritance** only — one base class. Multiple **interface** implementation is allowed.

---

## Basic inheritance

```csharp
// Base — shared structure and behaviour
public abstract class EntityBase
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public long TenantId { get; protected set; }
}

// Derived — inherits Id, CreatedAt, TenantId; adds its own members
public class Tournament : EntityBase
{
    public string Name { get; private set; }
    public TournamentStatus Status { get; private set; }

    public Tournament(string name, long tenantId)
    {
        Name = name;
        TenantId = tenantId;        // protected setter — accessible from derived
        Status = TournamentStatus.Registration;
    }
}

// Further derived
public abstract class AggregateRootBase : EntityBase { }
// Tournament could extend AggregateRootBase instead — same chain
```

`base` keyword calls the parent constructor or method:

```csharp
public class SpecialTournament(string name, long tenantId, int maxRounds)
    : Tournament(name, tenantId)          // calls Tournament's constructor
{
    public int MaxRounds { get; } = maxRounds;

    public override string ToString()
        => $"{base.ToString()} — {MaxRounds} rounds"; // calls parent ToString
}
```

---

## Inheritance vs Composition

**Inheritance — "is-a":** use when the derived type genuinely is a specialisation of the base.

```csharp
// ✅ Tournament IS an EntityBase — inheritance is correct
public class Tournament : EntityBase { }
```

**Composition — "has-a":** use when a class _uses_ another class but isn't a specialisation of it.

```csharp
// ✅ Tournament HAS a PrizePool — composition is correct
public class Tournament : EntityBase
{
    public PrizePool Prize { get; private set; }  // composed
    public Format Format { get; private set; }     // composed value object
}

// ❌ Don't inherit just to reuse code
public class Tournament : PrizePool { }  // Tournament is NOT a PrizePool
```

**Rule of thumb:** if you can't say "A is a B" naturally in plain English, use composition. Inheritance for reuse alone creates fragile, tightly-coupled hierarchies.

---

## Interview traps

- **"C# has multiple inheritance?"** — No, single class inheritance only. Multiple interface implementation is not the same thing.
- **"When does inheritance break?"** — When the base class changes and all derived classes are affected. Deep hierarchies (3+ levels) are a maintenance problem.
- **"Inheritance vs composition — which do you prefer?"** — Composition. Favour it by default. Inheritance is for genuine is-a relationships and when you need polymorphism through a base type.
- **"Can you inherit from a sealed class?"** — No, compile error. `string` is sealed — you can't subclass it.
- **"What does `protected` give a derived class?"** — Access to the base class member. `private` members are inherited but not accessible — they exist in memory but the derived class cannot reference them by name.