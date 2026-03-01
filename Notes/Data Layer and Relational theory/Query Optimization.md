## UNION vs UNION ALL

Combines result sets of two SELECT statements with **identical column structure**.

||UNION|UNION ALL|
|---|---|---|
|Duplicates|Removed|Kept|
|Performance|Slower (dedup step)|Faster|
|Use when|Need distinct results|Duplicates impossible or acceptable|

```sql
-- UNION: removes duplicate rows across both sets
SELECT Name FROM Customers
UNION
SELECT Name FROM Suppliers;

-- UNION ALL: keeps everything, no dedup cost
SELECT Name FROM Customers
UNION ALL
SELECT Name FROM Suppliers;
```

> **Interview tip:** Always prefer `UNION ALL` unless deduplication is required — `UNION` does an internal sort/hash to find duplicates, which costs performance.

---

## CTEs (Common Table Expressions)

A named temporary result set defined with `WITH`. Lives only for the duration of the query. Makes complex queries readable and allows referencing the same subresult multiple times.

```sql
-- Basic CTE
WITH ActiveCustomers AS (
    SELECT CustomerId, Name
    FROM Customers
    WHERE IsActive = 1
)
SELECT * FROM ActiveCustomers WHERE Name LIKE 'A%';

-- Multiple CTEs chained together
WITH
    MonthlyOrders AS (
        SELECT CustomerId, SUM(Amount) AS Total
        FROM Orders
        WHERE MONTH(OrderDate) = MONTH(GETDATE())
        GROUP BY CustomerId
    ),
    TopCustomers AS (
        SELECT CustomerId
        FROM MonthlyOrders
        WHERE Total > 1000
    )
SELECT c.Name, m.Total
FROM Customers c
JOIN MonthlyOrders m ON c.CustomerId = m.CustomerId
WHERE c.CustomerId IN (SELECT CustomerId FROM TopCustomers);
```

**CTE vs Subquery:** Functionally equivalent in most cases, but CTEs are more readable and can be referenced multiple times within the same query. Prefer CTEs for anything non-trivial.

---

## Subqueries

A query nested inside another query. Three common forms:

```sql
-- 1. Subquery in WHERE
SELECT Name FROM Products
WHERE CategoryId IN (
    SELECT Id FROM Categories WHERE Name = 'Electronics'
);

-- 2. Subquery as derived table (in FROM)
SELECT d.CustomerId, d.Avg
FROM (
    SELECT CustomerId, AVG(Amount) AS Avg
    FROM Orders
    GROUP BY CustomerId
) AS d
WHERE d.Avg > 500;

-- 3. Correlated subquery — references outer query, re-evaluated per row
SELECT p.Name, p.Price
FROM Products p
WHERE p.Price > (
    SELECT AVG(Price)
    FROM Products
    WHERE CategoryId = p.CategoryId  -- references outer p
);
```

> ⚠️ **Correlated subqueries** execute once per outer row — O(n) database calls. For large tables, rewrite as a JOIN or CTE.

---

## General Optimization Rules

- `SELECT` only needed columns — never `SELECT *` in production
- Filter early with `WHERE` — reduce rows before joins/aggregations
- Index columns used in `JOIN`, `WHERE`, `ORDER BY`
- Prefer `UNION ALL` over `UNION` unless dedup is required
- Replace correlated subqueries with JOINs or CTEs when performance matters
- Use execution plans (`EXPLAIN` in PostgreSQL/MySQL, "Include Actual Execution Plan" in SSMS) to find bottlenecks

[[SQL]] [[Group By]] [[Indexes]]