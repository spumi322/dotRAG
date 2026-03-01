
A reusable solution to a commonly occurring problem in software design. Not a finished piece of code — a template for how to solve a problem.

---

## Tier 1 — Know Deeply (theory + code)

These appear consistently in junior .NET interviews. Expect theory questions and a coding exercise.

|Pattern|Category|Intent|TO2|
|---|---|---|---|
|**Singleton**|Creational|One instance, global access|DI `AddSingleton<T>()`|
|**Factory**|Creational|Delegate object creation, caller gets interface|Notification senders, DI keyed services|
|**Repository**|Structural|Abstract data access behind interface|`ITournamentRepository`, `IMatchRepository`|
|**Strategy**|Behavioral|Swap algorithms at runtime|`IGameResultPipelineStep`|
|**Decorator**|Behavioral|Add behaviour without changing class|ASP.NET middleware pipeline|
|**Observer**|Behavioral|Notify dependents of state change|SignalR broadcasting|

---

## Tier 2 — Know It (one-liner + .NET example, no coding)

|Pattern|What to know|
|---|---|
|**Builder**|Constructs complex objects step by step. `WebApplication.CreateBuilder()`, `StringBuilder` — you use it daily without thinking about it.|
|**Adapter**|Makes incompatible interfaces work together. Wrapping a third-party SDK behind your own interface.|

---

## Tier 3 — Skip Entirely

Confirmed not asked at junior level. Skip for now, learn on the job.

Abstract Factory, Prototype, Proxy, Facade, Property Container, Command, Memento, State, DDD (Entities, Value Objects, Aggregates).

---

## DI Containers & Patterns

Modern .NET DI makes several classical patterns largely obsolete or changes how they're implemented:

| Pattern         | Classic                         | Modern .NET                                                          |
| --------------- | ------------------------------- | -------------------------------------------------------------------- |
| Singleton       | Manual `Lazy<T>`                | `services.AddSingleton<T>()`                                         |
| Factory         | Switch-case returning interface | `IEnumerable<T>` injection + dictionary, or keyed services (.NET 8+) |
| Service Locator | Global static resolver          | ⛔ Anti-pattern — avoid even with DI                                  |

> If you find yourself writing Singleton or Factory by hand in ASP.NET Core, ask whether DI already solves it.

---

## Files

- `Creational Patterns.md` — Singleton, Factory, Builder (awareness)
- `Structural Patterns.md` — Repository, Adapter (awareness)
- `Behavioral Patterns.md` — Strategy, Decorator, Observer