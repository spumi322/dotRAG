
A data structure that speeds up row lookups at the cost of extra storage and slower writes (INSERT/UPDATE/DELETE must also update the index).

Think of it like a book index — instead of scanning every page, you jump directly to the right one.

---

## Clustered vs Non-Clustered

**The most common interview question on indexes.**

||Clustered|Non-Clustered|
|---|---|---|
|Stores|Actual data rows, sorted by key|Separate structure with pointers to rows|
|Per table|**Only one**|Many (up to ~999 in SQL Server)|
|Default|Primary key becomes clustered|All other indexes|
|Speed|Fastest for range scans|Slightly slower (extra pointer lookup)|

```sql
-- Clustered: physical row order = index order
-- Primary key is clustered by default
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY,  -- clustered index created automatically
    CustomerId INT,
    Amount DECIMAL
);

-- Non-clustered: separate index pointing back to the data
CREATE INDEX IX_Orders_CustomerId
ON Orders (CustomerId);
```

**Interview one-liner:**

> "A clustered index determines the physical storage order of the table — there can only be one. A non-clustered index is a separate structure that stores the key value plus a pointer to the actual row."

---

## When to Add an Index

Index columns that appear frequently in:

- `WHERE` clauses (filtering)
- `JOIN` conditions
- `ORDER BY` / `GROUP BY`

```sql
-- Without index: full table scan on every query
SELECT * FROM Orders WHERE CustomerId = 42;

-- With index on CustomerId: direct lookup
CREATE INDEX IX_Orders_CustomerId ON Orders (CustomerId);
```

---

## Index Trade-offs

Indexes are **not free** — every write operation must update all relevant indexes.

- ✅ Speed up reads
- ❌ Slow down writes (INSERT, UPDATE, DELETE)
- ❌ Consume disk space

> Don't index every column. Index based on actual query patterns.

---

## Other Index Types (know they exist)

- **Composite index** — multiple columns: `CREATE INDEX IX ON Orders (CustomerId, Status)`. Column order matters — most selective column first.
- **Unique index** — enforces uniqueness, like a constraint
- **Covering index** — includes all columns a query needs so it never touches the main table (`INCLUDE` clause in SQL Server)

[[SQL]] [[Query Optimization]]