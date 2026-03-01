# Polymorphism

Letting different types be treated as the same type through a shared interface or base class. The correct behaviour is resolved based on the actual object type.

---

## Overriding — runtime polymorphism

Resolved at **runtime**. Base declares `virtual`/`abstract`, derived uses `override`.

```csharp
public abstract class Standing
{
    public abstract string GetSummary(); // must override
}

public class GroupStanding : Standing
{
    public override string GetSummary() => "Group stage standings";
}

public class BracketStanding : Standing
{
    public override string GetSummary() => "Bracket standings";
}

// Caller works against the base type — doesn't care which concrete type it is
Standing s = new GroupStanding();
s.GetSummary(); // "Group stage standings" — resolved at runtime
```

---

## Overloading — compile-time polymorphism

Same method name, different parameters. Resolved at **compile time**.

```csharp
public class MatchService
{
    public Task<Match> GetAsync(int matchId) => ...;
    public Task<Match> GetAsync(int matchId, long tenantId) => ...;
    public Task<IReadOnlyList<Match>> GetAsync(int[] matchIds) => ...;
}
```

The compiler picks the right overload based on argument types and count.

---

## Hiding — `new` keyword

Hides the base member instead of overriding. Resolved at **compile time** based on the **variable type**, not the object type. Almost always a design smell — see `CsharpKeywords.md` for the full `override` vs `new` breakdown.

```csharp
public class BaseService
{
    public virtual string GetName() => "Base";
}

public class TournamentService : BaseService
{
    public new string GetName() => "Tournament"; // hides, not overrides
}

BaseService s = new TournamentService();
s.GetName(); // "Base" ← not polymorphic, variable type wins
```

---

## Upcasting

A derived type treated as its base type. Happens implicitly.

```csharp
GroupStanding group = new GroupStanding();
Standing s = group;             // upcast — implicit, always safe
s.GetSummary();                 // still calls GroupStanding.GetSummary() — polymorphism
```

---

## Interview traps

- **"Runtime vs compile-time polymorphism?"** — Override = runtime (actual object type decides). Overload = compile-time (parameter signature decides).
- **"What's the difference between `override` and `new`?"** — `override` is polymorphic, dispatch based on object type. `new` hides, dispatch based on variable type. See `CsharpKeywords.md`.
- **"Can you overload on return type alone?"** — No. C# doesn't consider return type in overload resolution.
- **"What is upcasting?"** — Treating a derived type as its base. Always implicit and safe. Downcasting (base → derived) requires an explicit cast and can throw `InvalidCastException` — use `as` + null check or `is` pattern instead.