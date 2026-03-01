A named, saved SQL block that can be executed repeatedly. Lives in the database, not the application.

```sql
-- Create
CREATE PROCEDURE GetOrdersByCustomer
    @CustomerId INT
AS
BEGIN
    SELECT * FROM Orders WHERE CustomerId = @CustomerId;
END;

-- Execute
EXEC GetOrdersByCustomer @CustomerId = 42;
```

**With output parameter:**

```sql
CREATE PROCEDURE GetOrderCount
    @CustomerId INT,
    @Count INT OUTPUT
AS
BEGIN
    SELECT @Count = COUNT(*) FROM Orders WHERE CustomerId = @CustomerId;
END;

-- Call
DECLARE @Total INT;
EXEC GetOrderCount @CustomerId = 42, @Count = @Total OUTPUT;
SELECT @Total;
```

---

## Stored Procedure vs Function

||Stored Procedure|Function|
|---|---|---|
|Returns|Optional / OUTPUT params|Must return a value|
|Used in SELECT|❌ No|✅ Yes|
|Can modify data|✅ Yes|❌ Generally no|
|Called with|`EXEC`|Inside a query|

```sql
-- Function: used inline in queries
CREATE FUNCTION GetDiscountedPrice (@Price DECIMAL, @Discount DECIMAL)
RETURNS DECIMAL
AS
BEGIN
    RETURN @Price - (@Price * @Discount / 100);
END;

-- Usage
SELECT ProductId, dbo.GetDiscountedPrice(Price, 10) AS FinalPrice
FROM Products;
```

---

## Junior-level awareness

As a .NET developer you'll rarely **write** stored procedures — EF Core handles most data access. But you need to know:

- What they are and why they exist (reusability, encapsulation, permissions)
- How to **call** one from EF Core:

```csharp
// Call a stored procedure from EF Core
var orders = await _context.Orders
    .FromSqlRaw("EXEC GetOrdersByCustomer @CustomerId = {0}", customerId)
    .ToListAsync();
```

- Their main downside: **business logic hidden in the database**, hard to version-control and test

[[SQL]] [[Transactions]] [[ORM & EFCore]]