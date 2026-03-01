
A transaction is a unit of work where **all operations succeed together or none of them do**. If any step fails, the entire transaction is rolled back — the database is left unchanged.

See [[ACID]] for the theoretical guarantees.

---

## BEGIN / COMMIT / ROLLBACK

```sql
BEGIN TRANSACTION;

    UPDATE Accounts SET Balance = Balance - 500 WHERE AccountId = 1;
    UPDATE Accounts SET Balance = Balance + 500 WHERE AccountId = 2;

COMMIT;  -- both updates are permanently saved
```

```sql
BEGIN TRANSACTION;

    UPDATE Accounts SET Balance = Balance - 500 WHERE AccountId = 1;
    -- something goes wrong
ROLLBACK;  -- first UPDATE is undone, balance restored
```

```sql
-- SAVEPOINT: partial rollback within a transaction
BEGIN TRANSACTION;

    INSERT INTO Orders (CustomerId, Amount) VALUES (1, 200);
    SAVE TRANSACTION AfterInsert;

    UPDATE Inventory SET Stock = Stock - 1 WHERE ProductId = 5;

    -- only roll back the UPDATE, keep the INSERT
    ROLLBACK TRANSACTION AfterInsert;

COMMIT;
```

> In EF Core, `SaveChangesAsync()` wraps all pending changes in a single implicit transaction — you don't write `BEGIN/COMMIT` manually unless you need multi-step control.

---

## Transaction Isolation Levels

Define how much one transaction can see the uncommitted work of another. Trade-off: **higher isolation = safer data, lower concurrency**.

|Level|Dirty Read|Non-Repeatable Read|Phantom Read|
|---|---|---|---|
|Read Uncommitted|✅ possible|✅ possible|✅ possible|
|Read Committed _(default)_|❌|✅ possible|✅ possible|
|Repeatable Read|❌|❌|✅ possible|
|Serializable|❌|❌|❌|

**Definitions:**

- **Dirty read** — reading uncommitted data from another transaction that might be rolled back
- **Non-repeatable read** — same row returns different values if read twice in the same transaction
- **Phantom read** — same query returns different rows if re-run (another transaction inserted/deleted rows)

```sql
-- Set isolation level for a session
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

BEGIN TRANSACTION;
    SELECT * FROM Orders WHERE CustomerId = 1;
COMMIT;
```

> **Junior interview level:** Know what isolation levels are and the default (`Read Committed`). You don't need to memorize all trade-offs — understanding dirty reads is enough.

---

## Transactions in EF Core

```csharp
// Implicit transaction — SaveChanges wraps everything automatically
await _context.SaveChangesAsync();

// Explicit transaction — when you need multiple SaveChanges in one transaction
await using var tx = await _context.Database.BeginTransactionAsync();
try
{
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    _context.Invoices.Add(invoice);
    await _context.SaveChangesAsync();

    await tx.CommitAsync();
}
catch
{
    await tx.RollbackAsync();
    throw;
}
```

[[SQL]] [[ACID]] [[ORM & EFCore]]