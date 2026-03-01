## What is an ORM?

ORM (Object-Relational Mapper) bridges the gap between relational databases and object-oriented code. Instead of writing raw SQL, you work with C# classes and the ORM generates the SQL.

**Without ORM:**

```csharp
var cmd = new SqlCommand("SELECT * FROM Products WHERE Id = @id", conn);
cmd.Parameters.AddWithValue("@id", id);
var reader = cmd.ExecuteReader();
// manually map every column to a property...
```

**With EF Core:**

```csharp
var product = await _context.Products.FindAsync(id);
```

**Advantages:** Productivity, LINQ support, migrations, change tracking  
**Disadvantages:** Performance overhead on complex queries, less control than raw SQL

---

## DbContext

`DbContext` is the central class in EF Core. It represents a **session with the database**.

**Full interview answer — DbContext is responsible for:**

- Managing the database connection
- Exposing `DbSet<T>` collections (one per entity)
- Tracking changes to entities
- Translating LINQ queries to SQL
- Executing queries and saving data via `SaveChanges()`
- Managing transactions (implicit per `SaveChanges()`)

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // fluent configuration — relationships, constraints, indexes
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId);
    }
}
```

> `DbContext` is registered as **Scoped** in ASP.NET Core — one instance per HTTP request. Never register it as Singleton.

---

## Change Tracking

EF Core tracks every entity it loads so it knows what to persist on `SaveChanges()`.

**How it works (Snapshot tracking — default):**

1. Entity is loaded → EF takes a snapshot of its original values
2. You modify properties
3. `SaveChanges()` is called → EF compares current values to snapshot → generates `INSERT`/`UPDATE`/`DELETE`

```csharp
var product = await _context.Products.FindAsync(id); // tracked, snapshot taken
product.Price = 19.99m;                               // EF detects the change
await _context.SaveChangesAsync();                    // generates: UPDATE Products SET Price = 19.99 WHERE Id = @id
```

**Entity states:**

|State|Meaning|
|---|---|
|`Added`|New entity, will INSERT|
|`Modified`|Existing entity with changes, will UPDATE|
|`Deleted`|Marked for removal, will DELETE|
|`Unchanged`|Loaded, no changes|
|`Detached`|Not tracked by this context|

```csharp
// Check or set state manually
var entry = _context.Entry(product);
Console.WriteLine(entry.State); // EntityState.Modified

// Force delete without loading
_context.Products.Remove(product);           // sets state to Deleted
await _context.SaveChangesAsync();
```

**Disable tracking for read-only queries:**

```csharp
// No snapshot taken — faster, less memory
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
```

---

## DbSet<T>

Represents a table in the database. Entry point for all CRUD operations on that entity.

```csharp
_context.Products.Add(product);          // INSERT
_context.Products.Remove(product);       // DELETE
_context.Products.Find(id);             // SELECT by PK (checks tracker first)
_context.Products.Where(p => p.IsActive) // SELECT with filter
```

---

## Code First vs Database First

||Code First|Database First|
|---|---|---|
|Start from|C# entity classes|Existing database|
|Schema managed by|EF Core Migrations|DBA / SQL scripts|
|Used in|Greenfield projects|Legacy systems|

> You use **Code First** in your TO2 API — entities define the schema, migrations keep the DB in sync.

[[SQL]] [[EFCore Migrations Querying]] [[IEnumerable vs IQueryable]]