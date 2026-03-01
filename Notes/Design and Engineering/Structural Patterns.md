
Concerned with how classes and objects are composed into larger structures.

## Repository (Tier 1)

Abstracts data access behind an interface. The application layer talks to the interface — not to EF Core directly. Enables swapping the ORM and mocking in tests.

```csharp
// Contract — application layer only knows this
public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(TournamentId id, long tenantId);
    Task AddAsync(Tournament tournament);
}

// Implementation — EF Core detail, lives in Infrastructure
public class TournamentRepository(AppDbContext ctx) : ITournamentRepository
{
    public async Task<Tournament?> GetByIdAsync(TournamentId id, long tenantId)
        => await ctx.Tournaments
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);

    public async Task AddAsync(Tournament t)
    {
        ctx.Tournaments.Add(t);
        await ctx.SaveChangesAsync();
    }
}

// Service — never imports EF Core
public class TournamentService(ITournamentRepository repo) { ... }
```

**Interview traps:**

- "Why not inject `DbContext` directly into the service?" — couples business logic to EF Core, impossible to unit test without a real DB, violates DIP
- "Repository vs Unit of Work?" — Repository abstracts a single aggregate. Unit of Work wraps multiple repositories in one transaction (`SaveChanges()` is effectively UoW)
- "Generic vs specific repository?" — `IRepository<T>` with `GetById/Add/Delete` is convenient but leaks infrastructure concerns (you end up exposing `IQueryable<T>`). Specific repositories per aggregate are cleaner

---

## Adapter (Tier 2 — awareness)

Wraps an incompatible interface so it fits what your code expects. Common when integrating third-party SDKs.

```csharp
// You want this
public interface IPaymentGateway { Task<bool> ChargeAsync(decimal amount); }

// Third-party gives you this
public class StripeClient { public Task<StripeResult> CreateCharge(int cents) { ... } }

// Adapter bridges the gap
public class StripeAdapter(StripeClient stripe) : IPaymentGateway
{
    public async Task<bool> ChargeAsync(decimal amount)
    {
        var result = await stripe.CreateCharge((int)(amount * 100));
        return result.Status == "succeeded";
    }
}
```

Your code only knows `IPaymentGateway` — the Stripe dependency is isolated.