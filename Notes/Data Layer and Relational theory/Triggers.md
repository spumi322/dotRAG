
A trigger is a special database object that **automatically executes** in response to a data modification event on a table.

```sql
-- Example: log every deletion from Orders
CREATE TRIGGER trg_OrderDeleted
ON Orders
AFTER DELETE
AS
BEGIN
    INSERT INTO AuditLog (Event, OrderId, DeletedAt)
    SELECT 'ORDER_DELETED', deleted.OrderId, GETDATE()
    FROM deleted;  -- 'deleted' is a special table available inside triggers
END;
```

**Three events:** `INSERT`, `UPDATE`, `DELETE`  
**Two timings:** `BEFORE` (validation) / `AFTER` (auditing, cascading)

---

## Junior-level awareness

As a .NET developer you'll almost never write triggers. Know:

- What they are — automatic DB-side reactions to data changes
- Common use case — **audit logging** (who changed what, when)
- Why they're avoided in modern apps — same problems as stored procedures: hidden logic, untestable, hard to debug

> In your TO2 API you handle audit logging with an **EF Core interceptor** (`SaveChangesInterceptor`) — that's the modern, testable equivalent of a trigger.

[[SQL]] [[Stored Procedures]]