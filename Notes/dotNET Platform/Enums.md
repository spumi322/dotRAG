# Enums

Named constants backed by an integer. Instead of passing magic numbers around, you give them meaningful names.

```csharp
public enum TournamentStatus
{
    Setup = 1,
    GroupsInProgress = 3,
    Finished = 7,
    Cancelled = 8
}
```

Underlying type is `int` by default. You can assign explicit values — useful when the number has meaning (e.g. stored in DB, used in state machines). Your TO2 app does this everywhere.

---

## [Flags] — combining values

When you want a variable to hold multiple states at once, use `[Flags]` and powers of 2 so each value occupies a unique bit.

```csharp
[Flags]
public enum Permissions
{
    None  = 0,
    Read  = 1,  // 001
    Write = 2,  // 010
    Admin = 4   // 100
}

var p = Permissions.Read | Permissions.Write; // combine with |
p.HasFlag(Permissions.Read);                  // true — check with HasFlag
```

---

## Common operations

```csharp
// Parse from string (throws if invalid)
var status = Enum.Parse<TournamentStatus>("Finished");

// Safe parse
Enum.TryParse("Cancelled", out TournamentStatus result);

// Get all values
var all = Enum.GetValues<TournamentStatus>();

// Cast to/from int
int raw = (int)TournamentStatus.Finished;       // 7
var s = (TournamentStatus)7;                    // Finished
```

---

## Interview traps

- **"Are enums value or reference types?"** — Value type, stored on the stack.
- **"What happens if you cast an invalid int to an enum?"** — No exception, you just get an undefined value. `Enum.IsDefined()` to validate.
- **"When would you use [Flags]?"** — When a field needs to represent a combination of states, like permissions or feature toggles.

---

### Related

[[Data Types]] [[Pattern Matching]]