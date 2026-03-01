# SQL Joins

## The Four Join Types

```
INNER JOIN          LEFT JOIN           RIGHT JOIN          FULL JOIN
  L ∩ R              L + (L ∩ R)         R + (L ∩ R)         L ∪ R

 [L | R]           [██| R]             [L |██]             [██|██]
     ↑                  ↑                   ↑                   ↑
 only overlap      all left,           all right,          everything,
                   nulls for           nulls for           nulls where
                   no match            no match            no match
```

---

## Reference Tables (TO2 context)

```sql
-- Tournaments: Id, Name
-- TournamentTeams: TournamentId, TeamId
-- Teams: Id, Name
-- Matches: Id, TournamentId, HomeTeamId, AwayTeamId, HomeScore, AwayScore
```

---

## INNER JOIN

Returns only rows with a match in **both** tables. Non-matching rows are discarded.

```sql
-- Teams that are registered to at least one tournament
SELECT t.Name, tr.Name AS Tournament
FROM Teams t
INNER JOIN TournamentTeams tt ON t.Id = tt.TeamId
INNER JOIN Tournaments tr     ON tt.TournamentId = tr.Id;
```

> If a team has no tournament registrations, it won't appear. This is the most common join — use it when you need guaranteed matches on both sides.

---

## LEFT JOIN

Returns **all rows from the left table**. Right side is `NULL` where no match exists.

```sql
-- All teams, including those not registered to any tournament
SELECT t.Name, tr.Name AS Tournament
FROM Teams t
LEFT JOIN TournamentTeams tt ON t.Id = tt.TeamId
LEFT JOIN Tournaments tr     ON tt.TournamentId = tr.Id;
```

> Classic use case: "find records with no match" — filter `WHERE tr.Id IS NULL` to get teams not in any tournament.

---

## RIGHT JOIN

Returns **all rows from the right table**. Rarely used in practice — swap table order and use LEFT JOIN instead. Included for completeness.

```sql
-- All tournaments, including those with no registered teams
SELECT t.Name AS Team, tr.Name AS Tournament
FROM Teams t
RIGHT JOIN TournamentTeams tt ON t.Id = tt.TeamId
RIGHT JOIN Tournaments tr     ON tt.TournamentId = tr.Id;
```

> In practice, rewrite as a LEFT JOIN with tables swapped. RIGHT JOIN exists but most teams avoid it for readability.

---

## FULL OUTER JOIN

Returns **all rows from both tables**, with `NULL` on the side without a match.

```sql
-- All teams AND all tournaments, matched where possible
SELECT t.Name AS Team, tr.Name AS Tournament
FROM Teams t
FULL OUTER JOIN TournamentTeams tt ON t.Id = tt.TeamId
FULL OUTER JOIN Tournaments tr     ON tt.TournamentId = tr.Id;
```

> Useful for auditing/reconciliation — "show me everything that matched and everything that didn't." Not supported in MySQL (use UNION of LEFT + RIGHT instead).

---

## SELF JOIN

A table joined to itself. Useful for hierarchical or comparative data.

```sql
-- Find matches where the same team plays both home and away (data integrity check)
SELECT m1.Id
FROM Matches m1
INNER JOIN Matches m2 ON m1.HomeTeamId = m2.AwayTeamId
                      AND m1.AwayTeamId = m2.HomeTeamId
                      AND m1.Id <> m2.Id;
```

---

## EF Core / LINQ equivalents

EF Core handles most joins implicitly via navigation properties — you rarely write `.Join()` manually.

```csharp
// INNER JOIN — via navigation property (preferred)
var result = await _context.TournamentTeams
    .Include(tt => tt.Team)
    .Include(tt => tt.Tournament)
    .ToListAsync();

// LEFT JOIN — GroupJoin + SelectMany + DefaultIfEmpty
var result = await _context.Teams
    .GroupJoin(
        _context.TournamentTeams,
        t => t.Id,
        tt => tt.TeamId,
        (t, tts) => new { t, tts })
    .SelectMany(
        x => x.tts.DefaultIfEmpty(),
        (x, tt) => new { x.t.Name, TournamentId = tt == null ? (int?)null : tt.TournamentId })
    .ToListAsync();
```

> In real EF Core work: use `Include()` for INNER, and raw SQL or views for complex LEFT/FULL joins. The LINQ GroupJoin syntax is verbose and rarely written by hand — but you should know it exists.

---

## Interview Traps

|Question|Answer|
|---|---|
|INNER vs LEFT — when to use which?|INNER when both sides must exist; LEFT when you need all of one side regardless|
|Why avoid RIGHT JOIN?|Equivalent to LEFT JOIN with tables swapped — LEFT is the convention, easier to read|
|FULL OUTER JOIN in MySQL?|Not supported — simulate with `LEFT JOIN UNION RIGHT JOIN`|
|Does JOIN order affect results?|For INNER: no. For LEFT/RIGHT: yes — the "preserved" table changes|
|What if JOIN condition matches multiple rows?|Result set multiplies — each left row pairs with every matching right row (Cartesian-like)|
|NULL in JOIN column?|`NULL != NULL` in SQL — rows with NULL keys never match in any JOIN|