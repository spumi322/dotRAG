SOLID: Design principles for overall better code.

## S — Single Responsibility

**One class, one job. One reason to change.**

> If you can describe what a class does and you need the word "and" — it's doing too much.

**Your app:** `AuditInterceptor` only sets audit fields. `AuthService` only handles auth. `TournamentService` only manages tournaments. Each has one reason to change.

**Violation:** My `TournamentService`It handles CRUD **and** broadcasts SignalR events **and** generates standings **and** manages state transitions. Multiple reasons to change.

---

## O — Open/Closed

**Add new behavior by adding new code, not by editing existing code.**

> If every new feature makes you crack open an existing class — something's wrong.

**Your app:** Your pipeline steps (`UpdateGroupStatsStep`, `ValidateTeamCountStep` etc.) are each independent. Need a new step? Add a new class, register it. The pipeline runner is never touched.

**Violation:** My /Domain/ValueObjects/FormatMetaData.cs is violating classic OCP, by adding a new format touches existing code. Now, its stored as Enum and record. 

---

## L — Liskov Substitution

Derived class can **add** behavior. It cannot **remove** or **break** behavior the base promised.

> If you swap a subclass in and something explodes — you violated LSP.

**Your app:** Any `IRepository<T>` implementation (e.g. `TournamentTeamRepository`) can replace the base `Repository<T>` anywhere it's used — no surprises, no `NotImplementedException`. That's LSP respected.

**Violation:** Overriding a method just to throw `NotImplementedException`. The caller assumed the contract — you broke it.

---

## I — Interface Segregation

**Don't force a class to implement methods it doesn't need.**

> Fat interfaces = pain. Split them by what callers actually need.

**Your app:** `ITeamService`, `IStandingService`, `IWorkFlowService` — `TournamentsController` only injects what it needs. It doesn't get a bloated interface with 15 methods when it only calls 3.

**Violation:** One `ITournamentService` with `CreateTournament`, `SendEmail`, `GenerateReport`, `CalculateStandings` all in one. A controller that just needs to create a tournament now drags in all that dead weight.

---

## D — Dependency Inversion

**Depend on interfaces, not concrete classes.**

> High-level code shouldn't care HOW something is done — only THAT it can be done.

**Your app:** `TournamentsController` depends on `ITournamentService`, not `TournamentService`. `AuditInterceptor` depends on `ITenantService`, not `TenantService`. Swap the implementation (mock, test double, different DB) — controller doesn't change.

**Violation:** `new`-ing up dependencies inside a class.

```csharp
// ❌ TournamentService is now glued to EF Core forever
public class TournamentsController
{
    private TournamentService _svc = new TournamentService(new TO2DbContext());
}
```

---

## Quick cheat sheet

||One-liner|Smell to watch for|
|---|---|---|
|**SRP**|One class, one job|Class name contains "And" or "Manager" doing 5 things|
|**OCP**|Extend, don't edit|Adding a feature = editing an existing method|
|**LSP**|Subclass = drop-in replacement|`NotImplementedException` in an override|
|**ISP**|Small focused interfaces|Implementing methods just to leave them empty|
|**DIP**|Depend on interfaces|`new`-ing dependencies inside a class|

[[Single Responsibility Principle]]
[[Open-Closed Principle]]
[[Liskov Substitution Principle]]
[[Interface Segregation Principle]]
[[Dependency Inversion Principle]]
