# Database Normalization

Normalization organizes data to **reduce redundancy** and **prevent anomalies**. Each normal form builds on the previous one.

**The three anomalies normalization solves:**

- **Insert anomaly** — can't add data without unrelated data existing first
- **Update anomaly** — changing one fact requires updating multiple rows
- **Delete anomaly** — deleting one record accidentally removes other facts

---

## 1NF — First Normal Form

**Rule:** Every column holds a single atomic value. No repeating groups. Every row is unique (has a PK).

### Violation:

|MatchId|TournamentName|Teams|
|---|---|---|
|1|Spring Cup|Alpha FC, Beta United|
|2|Spring Cup|Gamma SC, Delta FC|

`Teams` column holds multiple values — breaks atomicity.

### Fixed (1NF):

|MatchId|TournamentName|HomeTeam|AwayTeam|
|---|---|---|---|
|1|Spring Cup|Alpha FC|Beta United|
|2|Spring Cup|Gamma SC|Delta FC|

Each cell is atomic, each row is unique.

---

## 2NF — Second Normal Form

**Rule:** Must be in 1NF. Every non-key column must depend on the **whole** primary key — no partial dependencies.

> Only relevant when the PK is **composite** (multiple columns). If PK is a single column, 2NF is automatically satisfied.

### Violation (composite PK: MatchId + TournamentId):

|MatchId|TournamentId|TournamentName|HomeScore|AwayScore|
|---|---|---|---|---|
|1|10|Spring Cup|2|1|
|2|10|Spring Cup|0|3|

`TournamentName` depends only on `TournamentId`, not on the full `(MatchId, TournamentId)` PK — partial dependency.

### Fixed (2NF) — split into two tables:

**Matches:**

|MatchId|TournamentId|HomeScore|AwayScore|
|---|---|---|---|
|1|10|2|1|
|2|10|0|3|

**Tournaments:**

|TournamentId|TournamentName|
|---|---|
|10|Spring Cup|

`TournamentName` now lives in its own table — update it once, reflected everywhere.

---

## 3NF — Third Normal Form

**Rule:** Must be in 2NF. No **transitive dependencies** — non-key columns must not depend on other non-key columns.

### Violation:

|TeamId|TeamName|CityId|CityName|
|---|---|---|---|
|1|Alpha FC|5|Budapest|
|2|Beta SC|5|Budapest|
|3|Gamma FC|7|Vienna|

`CityName` depends on `CityId`, not on `TeamId`. Transitive chain: `TeamId → CityId → CityName`.

**Update anomaly:** rename "Budapest" → update every team row from that city.

### Fixed (3NF):

**Teams:**

|TeamId|TeamName|CityId|
|---|---|---|
|1|Alpha FC|5|
|2|Beta SC|5|
|3|Gamma FC|7|

**Cities:**

|CityId|CityName|
|---|---|
|5|Budapest|
|7|Vienna|

---

## Quick Reference

|Form|Requirement|Catches|
|---|---|---|
|1NF|Atomic values, unique rows, PK exists|Multi-value columns, repeating groups|
|2NF|1NF + no partial dependencies|Non-key column depends on part of composite PK|
|3NF|2NF + no transitive dependencies|Non-key column depends on another non-key column|

---

## Denormalization — when to break the rules

Normalization is the default. But in read-heavy systems you sometimes denormalize deliberately for performance:

- Pre-computed aggregates (e.g. store `StandingPoints` on the team row instead of summing match results every query)
- Reporting tables / materialized views
- EF Core projections with `.Select()` to avoid joins at the query level

> In your TO2 app: `Standing` stores computed points/wins/losses — that's a controlled denormalization. The source of truth is `Match` results, but you cache the aggregate for fast leaderboard reads.

---

## EF Core angle

EF Core migrations enforce normalization implicitly through relationships:

```csharp
// 2NF enforced — TournamentName lives on Tournament entity, not duplicated on Match
modelBuilder.Entity<Match>()
    .HasOne(m => m.Tournament)
    .WithMany(t => t.Matches)
    .HasForeignKey(m => m.TournamentId);

// 3NF enforced — City extracted to its own entity, Team holds FK
modelBuilder.Entity<Team>()
    .HasOne(t => t.City)
    .WithMany(c => c.Teams)
    .HasForeignKey(t => t.CityId);
```

If you find yourself duplicating a string column across many rows, that's a 3NF violation — extract it to a lookup table.

---

## Interview Traps

|Question|Answer|
|---|---|
|Does 2NF matter if PK is a single column?|No — partial dependency is impossible with a single-column PK|
|What's a transitive dependency in plain English?|A → B → C where B is not a key. Fix: move B→C into its own table|
|Is denormalization always bad?|No — it's a deliberate trade-off: write complexity for read performance|
|3NF vs BCNF?|BCNF is stricter — every determinant must be a candidate key. Rarely asked at junior level, but know it exists|
|Real-world normalization level?|Most production DBs target 3NF. BCNF/4NF/5NF are academic unless you're at very large scale|
