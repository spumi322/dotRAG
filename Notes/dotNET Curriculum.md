# .NET Junior Developer — Curriculum

---

## MODULE 1 — Memory & Type System

**Session 1.1 — Memory Architecture**
- Stack & Heap
- Value vs Reference types
- Boxing / Unboxing

**Session 1.2 — Fundamental Types**
- Strings (immutability, interning, StringBuilder)
- Arrays
- Enums
- System.Object (ToString, Equals, GetHashCode, GetType)
- const vs readonly

**Session 1.3 — Modern Type Features**
- Nullable types (Nullable<T>, NRTs, ?., ??, !)
- Records (immutability, with, == override)
- Tuples
- Dynamic vs object

---

## MODULE 2 — Generics, Collections, LINQ

**Session 2.1 — Generics**
- Generic types, methods, constraints (where T : class/new())
- Func<>, Action<>, Predicate<>
- Covariance / contravariance

**Session 2.2 — Collections**
- IEnumerable, IEnumerator, ICollection, IList, IDictionary
- IReadOnlyList, IReadOnlyDictionary
- List<T>, Dictionary<K,V>, HashSet<T>, Queue, Stack, LinkedList
- ConcurrentDictionary — thread-safety use case

**Session 2.3 — LINQ**
- LINQ basics, method vs query syntax
- Deferred execution
- IEnumerable vs IQueryable — EF Core implications

---

## MODULE 3 — Delegates, Events, Lambdas

**Session 3.1 — Delegates & Events**
- Delegates, multicast delegates
- Anonymous methods
- Events — event vs delegate distinction

**Session 3.2 — Lambdas & Closures**
- Lambda expressions
- Closures — variable capture trap

---

## MODULE 4 — OOP, SOLID, Patterns

**Session 4.1 — OOP Pillars**
- Encapsulation, Inheritance, Polymorphism, Abstraction
- Composition over Inheritance

**Session 4.2 — Abstraction Mechanisms**
- Abstract class vs Interface — when to use which
- C# 8 default interface methods

**Session 4.3 — C# Keywords & Polymorphism**
- static, abstract, virtual, override, new, sealed
- override vs new trap

**Session 4.4 — SOLID**
- SRP, OCP, LSP, ISP, DIP — all with code examples
- Identifying violations in code snippets

**Session 4.5 — Design Patterns**
- Creational: Singleton, Factory, Builder, Abstract Factory, Prototype
- Structural: Adapter, Decorator, Proxy, Facade
- Behavioral: Observer, Strategy, Command

---

## MODULE 5 — Advanced Language Features

**Session 5.1 — Pattern Matching**
- Type patterns, switch expressions
- Property, relational, tuple, list patterns

**Session 5.2 — Parameter Modifiers**
- ref / out / in
- params

**Session 5.3 — Exceptions**
- throw vs throw ex
- finally, custom exceptions
- Exception hierarchy

---

## MODULE 6 — .NET Runtime & Memory

**Session 6.1 — .NET Internals**
- CLR, CIL, JIT — source to IL to native pipeline
- Managed vs unmanaged code
- CTS, BCL — purpose and scope
- .NET Framework vs .NET Core/5+
- Reflection — what it is, how DI/ORM use it internally

**Session 6.2 — Garbage Collection**
- GC generations (0/1/2), mark-and-sweep, LOH
- IDisposable, Dispose vs Finalize, using
- Span<T> — what problem it solves
- Streams, Serialization — conceptual awareness

**Session 6.3 — Async & Concurrency**
- async / await, Task<T>, ValueTask
- ConfigureAwait(false)
- Common pitfalls: .Result deadlock, async void, fire-and-forget
- Thread pool, locks, SemaphoreSlim, deadlocks
- CancellationToken — propagation pattern

---

## MODULE 7 — ASP.NET Core

**Session 7.1 — Architecture & Middleware**
- Kestrel, hosting model, Program.cs
- Middleware pipeline — order, short-circuit
- Minimal APIs vs Controller-based

**Session 7.2 — Routing & Controllers**
- Attribute routing, route constraints
- Model binding & validation ([ApiController], ModelState)
- Action Filters — auth, logging, exception, caching
- Middleware vs Filter distinction

**Session 7.3 — Dependency Injection**
- Lifetimes: Transient / Scoped / Singleton
- Captive dependency problem
- IOptions<T>, IOptionsSnapshot, IOptionsMonitor

**Session 7.4 — Auth, HTTP, REST**
- JWT — validation steps, claims, [Authorize]
- Authentication vs Authorization (policies, roles, claims)
- HTTP methods, status codes, CORS
- REST principles, idempotency
- ProblemDetails / RFC 7807

**Session 7.5 — Serialization & Error Handling**
- System.Text.Json, Accept header negotiation
- Global exception handling middleware
- ILogger<T>, logging levels, structured logging

---

## MODULE 8 — Data Layer

**Session 8.1 — SQL Fundamentals**
- DDL / DML / DQL / DCL / TCL
- Data types, constraints, entity relationships
- Normalization (1NF / 2NF / 3NF)

**Session 8.2 — Querying**
- JOINs (INNER, LEFT, RIGHT, FULL)
- Indexes (clustered vs non-clustered)
- Aggregate functions, GROUP BY, HAVING
- Views, subqueries, window functions

**Session 8.3 — Transactions & Security**
- ACID properties
- BEGIN / COMMIT / ROLLBACK
- Isolation levels (READ COMMITTED, dirty reads, phantom reads)
- SQL injection prevention
- Stored Procedures, Triggers — when and why to avoid

**Session 8.4 — EF Core**
- Code-first migrations (add, update, remove, script)
- Change Tracking, AsNoTracking()
- Eager / Lazy / Explicit loading
- N+1 problem
- Projections (Select), avoid loading full entities
- Transactions in EF Core

---

## MODULE 9 — Testing

**Session 9.1 — Unit Testing**
- Unit vs Integration vs E2E — scope and purpose
- xUnit: [Fact], [Theory], [InlineData]
- AAA pattern (Arrange / Act / Assert)
- Test naming: MethodName_Scenario_ExpectedResult
- Moq: Setup, Returns, Verify

**Session 9.2 — Integration Testing**
- WebApplicationFactory<T>
- In-memory vs real DB for tests
- CQRS / MediatR — what they are, common in .NET shops

---

## MODULE 10 — Frontend & Tools

**Session 10.1 — HTML & CSS**
- Semantic HTML, accessibility, DOM
- Box model, specificity, stacking contexts
- Flexbox vs Grid
- Responsive design, container queries

**Session 10.2 — JavaScript & TypeScript**
- Event loop
- var / let / const, closures, prototypes
- Promises, async / await
- Structural typing, any vs unknown, generics, type guards

**Session 10.3 — DevOps & Tooling**
- Git: merge vs rebase, feature branch / PR workflow
- Docker: Dockerfile, docker-compose
- CI/CD concepts: pipeline stages, triggers
- Azure basics — App Service, Blob, SQL
- Caching strategies (in-memory, distributed, cache-aside)
- Performance metrics — basic awareness
