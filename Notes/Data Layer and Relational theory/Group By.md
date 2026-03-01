
Groups rows that share the same value in specified columns so aggregate functions can be applied per group.

---

## WHERE vs HAVING

**Critical interview distinction** — both filter rows, but at different stages.

||WHERE|HAVING|
|---|---|---|
|Filters|Individual rows|Groups (after aggregation)|
|Runs|Before GROUP BY|After GROUP BY|
|Can use aggregates|❌ No|✅ Yes|

```sql
-- WHERE filters rows BEFORE grouping
SELECT CustomerId, COUNT(*) AS OrderCount
FROM Orders
WHERE Status = 'Shipped'        -- filters individual rows first
GROUP BY CustomerId;

-- HAVING filters groups AFTER aggregation
SELECT CustomerId, COUNT(*) AS OrderCount
FROM Orders
GROUP BY CustomerId
HAVING COUNT(*) > 5;            -- filters groups by their aggregate result

-- Combined: WHERE first, then GROUP BY, then HAVING
SELECT CustomerId, SUM(Amount) AS Total
FROM Orders
WHERE Status = 'Shipped'        -- 1. remove non-shipped rows
GROUP BY CustomerId             -- 2. group remaining rows
HAVING SUM(Amount) > 1000;      -- 3. keep only high-value customers
```

> **Rule of thumb:** If you can write it in WHERE, do it there — it's faster because it reduces rows before grouping.

---

## Basic GROUP BY examples

```sql
-- Count orders per customer
SELECT CustomerId, COUNT(*) AS OrderCount
FROM Orders
GROUP BY CustomerId;

-- Total amount per customer
SELECT CustomerId, SUM(Amount) AS TotalAmount
FROM Orders
GROUP BY CustomerId;

-- Average order amount, filtered by HAVING
SELECT CustomerId, AVG(Amount) AS AverageAmount
FROM Orders
GROUP BY CustomerId
HAVING AVG(Amount) > 1000;

-- Group by multiple columns
SELECT CustomerId, YEAR(OrderDate) AS OrderYear, COUNT(*) AS OrderCount
FROM Orders
GROUP BY CustomerId, YEAR(OrderDate);

-- GROUP BY with JOIN
SELECT c.CountryName, COUNT(*) AS OrderCount
FROM Orders o
JOIN Customers c ON o.CustomerId = c.CustomerId
GROUP BY c.CountryName;
```

---

## Query execution order (mental model)

```
FROM → WHERE → GROUP BY → HAVING → SELECT → ORDER BY
```

This is why you **cannot use a SELECT alias in HAVING** — HAVING runs before SELECT is evaluated.

```sql
-- ❌ Invalid — alias not yet defined at HAVING stage
SELECT CustomerId, COUNT(*) AS OrderCount
FROM Orders
GROUP BY CustomerId
HAVING OrderCount > 5;

-- ✅ Correct
SELECT CustomerId, COUNT(*) AS OrderCount
FROM Orders
GROUP BY CustomerId
HAVING COUNT(*) > 5;
```

[[SQL]] [[Aggregate Functions]] [[Query Optimization]]