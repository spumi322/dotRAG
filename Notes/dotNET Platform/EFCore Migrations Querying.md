## Migrations

Migrations let you evolve your database schema incrementally as your model changes, without losing data. EF Core compares your current model to a snapshot and generates migration files.

```bash
dotnet ef migrations add AddProductCategory   # create migration
dotnet ef database update                     # apply to DB
dotnet ef migrations remove                   # undo last (if not applied)
dotnet ef migrations script --idempotent      # generate SQL for production
```

Each migration has `Up()` (apply) and `Down()` (rollback):

```csharp
public partial class AddProductCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "Products",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Category", table: "Products");
    }
}
```

`__EFMigrationsHistory` table tracks which migrations have been applied.

**Best practices:**

- Always review generated migration code before applying
- Use `--idempotent` for production deployments
- Never edit the model snapshot manually
- Name descriptively: `AddOrderStatus`, not `Migration3`

---

## Loading Related Data

```csharp
// Eager loading — one query with JOIN
var orders = await _context.Orders
    .Include(o => o.Items)
    .ThenInclude(i => i.Product)
    .ToListAsync();

// Explicit loading — load on demand for a specific entity
await _context.Entry(order).Collection(o => o.Items).LoadAsync();

// Lazy loading — auto-loads on property access (requires proxies)
// ⚠️ Avoid in APIs — causes N+1 if accessed in a loop
```

---

## N+1 Query Problem

```csharp
// ❌ N+1: 1 query for orders + N queries for items
var orders = await _context.Orders.ToListAsync();
foreach (var o in orders)
    Console.WriteLine(o.Items.Count); // triggers a separate query per order

// ✅ Fixed: single query with JOIN
var orders = await _context.Orders
    .Include(o => o.Items)
    .ToListAsync();
```

---

## AsNoTracking — Read-Only Performance

```csharp
// EF tracks every entity by default — costs memory and CPU
// For read-only queries, disable tracking:
var products = await _context.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .ToListAsync();
```

Use `AsNoTracking()` for: GET endpoints, reports, anything you won't modify.

---

## Change Tracking & SaveChanges

```csharp
var product = await _context.Products.FindAsync(id); // tracked
product.Price = 19.99m;                               // EF detects change
await _context.SaveChangesAsync();                    // generates UPDATE SQL
```

`SaveChangesAsync()` wraps all pending changes in a **single implicit transaction**.

---

## Projection — Select Only What You Need

```csharp
// ❌ Loads all columns
var products = await _context.Products.ToListAsync();

// ✅ Loads only needed columns — less data transferred
var dtos = await _context.Products
    .Where(p => p.IsActive)
    .Select(p => new ProductDto(p.Id, p.Name, p.Price))
    .ToListAsync();
```

---

## Raw SQL (Escape Hatch)

```csharp
// Parameterized — safe from SQL injection
var products = await _context.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Category = {category}")
    .ToListAsync();
```

---

## Optimistic Concurrency

**The problem:** two users load the same entity, both modify it, last write wins and silently overwrites the first change.

**The solution:** attach a `RowVersion` (timestamp) to the entity. EF includes it in every `UPDATE WHERE` clause — if the row was changed by someone else since you loaded it, the update affects 0 rows and EF throws `DbUpdateConcurrencyException`.

```csharp
// Entity — add a RowVersion property
public class Tournament
{
    public long Id { get; set; }
    public string Name { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }  // auto-incremented by SQL Server on every update
}

// Fluent config alternative
modelBuilder.Entity<Tournament>()
    .Property(t => t.RowVersion)
    .IsRowVersion();
```

This generates a migration (like `AddRowVersionForConcurrency` in TO2):

```sql
ALTER TABLE Tournaments ADD RowVersion rowversion NOT NULL;
```

EF Core then generates:

```sql
UPDATE Tournaments SET Name = @name
WHERE Id = @id AND RowVersion = @originalRowVersion  -- fails if someone else updated it
```

**Handling the exception:**

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // Option 1: tell the user their data is stale, ask them to retry
    throw new ConflictException("The record was modified by another user.");

    // Option 2: reload and merge (more complex)
    var entry = ex.Entries.Single();
    await entry.ReloadAsync();
}
```

> **Interview one-liner:** "Optimistic concurrency assumes conflicts are rare — it doesn't lock the row, but detects conflicts at save time using a rowversion token. If a conflict occurs, EF throws `DbUpdateConcurrencyException`."

[[ORM & EFCore]] [[LINQ]] [[IEnumerable vs IQueryable]] [[Transactions]]