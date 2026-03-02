## MODULE 1 — Memory & Type System

**Session 1.1 — Memory Architecture**

- Stack & Heap
- Value vs Reference types 
- Boxing / Unboxing
- `struct` vs `class` <- add new segment

**Session 1.2 — Fundamental Types**

- Strings (immutability, interning, StringBuilder)
- Arrays
- Enums
- System.Object (ToString, Equals, GetHashCode, GetType)
- `const` vs `readonly`
- - `ref` / `out` / `in`, `params` <- Moved from dissolved module 6.2

**Session 1.3 — Modern Type Features**

- Nullable types (`Nullable<T>`, NRTs, `?.`, `??`, `!`)
- Records (immutability, `with`, `==` override)
- Tuples
- `dynamic` vs `object` <- drop this segment

## MODULE 2 — Generics, Collections, LINQ

**Session 2.1 — Generics**

- Generic types, methods, constraints (`where T : class/new()`)
- `Func<>`, `Action<>`, `Predicate<>` ← **MOVED HERE from old 3.1** (logically belong with generics)
- Covariance / contravariance ← **replace with interview-trap code-snippet**

**Session 2.2 — Collections**

- `IEnumerable`, `IEnumerator`, `ICollection`, `IList`, `IDictionary`
- `IReadOnlyList`, `IReadOnlyDictionary`
- `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Queue`, `Stack`, `LinkedList`
- `ConcurrentDictionary` — thread-safety use case <- Remove, not junior level

**Session 2.3 — LINQ**

- LINQ basics, method vs query syntax
- Deferred execution
- `IEnumerable` vs `IQueryable` — EF Core implications

## MODULE 3 — Delegates, Lambdas & Closures

_(merged from 2 sessions → 1 — old anonymous methods / multicast internals trimmed)_

**Session 3.1 — Delegates, Lambdas & Closures**

- Delegates — type-safe function pointer, why they exist
- Events — `event` vs delegate distinction (access restriction)
- Lambda expressions
- Closures — variable capture trap (`for` loop pitfall)

> Removed: anonymous method syntax, multicast delegate return value internals — low junior ROI

## MODULE 4 — OOP & C# Keywords

**Session 4.1 — OOP Pillars**

- Encapsulation, Inheritance, Polymorphism, Abstraction
- Composition over Inheritance

**Session 4.2 — Abstraction Mechanisms**

- Abstract class vs Interface — when to use which
- C# 8 default interface methods

**Session 4.3 — C# Keywords & Polymorphism**

- `static`, `abstract`, `virtual`, `override`, `new`, `sealed`
- Overloading — compile-time polymorphism
- Overriding — runtime polymorphism
- Hiding — `new` keyword
- Upcasting & Downcasting — implicit vs explicit, `as` / `is` patterns
- Pattern matching <- Moved form dissolved module 6.1
## MODULE 5 — SOLID & Design Patterns

**Session 5.1 — SOLID**

- SRP, OCP, LSP, ISP, DIP — all with code examples
- Identifying violations in code snippets

**Session 5.2 — Design Patterns**

- Creational: Singleton, Factory, Builder
- Structural: Adapter, Decorator, Proxy
- Behavioral: Strategy, Observer, Command (+ MediatR as real-world Command)

> Removed from Creational: Abstract Factory, Prototype — junior interviews don't reach these Removed from Structural: Facade — low frequency

## MODULE 6 — .NET Runtime & Memory

**Session 6.1 — .NET Internals**

- CLR, CIL, JIT — source to IL to native pipeline
- Managed vs unmanaged code
- CTS, BCL — purpose and scope
- .NET Framework vs .NET Core/5+
- Reflection — conceptual only (how DI/ORM use it internally)

**Session 6.2 — Garbage Collection & Disposal**

- GC generations (0/1/2), mark-and-sweep, LOH
- `IDisposable`, `Dispose` vs `Finalize`, `using`

**Session 6.3 — Async & Concurrency**

- `async` / `await`, `Task<T>`, `ValueTask`
- `.Result` deadlock + `ConfigureAwait(false)` — combined
- `async void`, fire-and-forget pitfalls
- Thread pool, locks, `SemaphoreSlim`, deadlocks
- `CancellationToken` — conceptual awareness only

**Session 6.4 — Exceptions** ← **NEW**

- `throw` vs `throw ex`
- `finally` — deterministic cleanup link to 6.2
- Custom exceptions — when to create one
- Exception hierarchy

## MODULE 7 — ASP.NET Core

**Session 7.1 — Architecture & Middleware**

- Kestrel, hosting model, Program.cs
- Middleware pipeline — order, short-circuit
- Minimal APIs vs Controller-based

**Session 7.2 — Routing & Controllers**

- Attribute routing, route constraints
- Model binding & validation (`[ApiController]`, ModelState)
- Action Filters — auth, logging, exception, caching
- Middleware vs Filter distinction
- `IHostedService` <- add new segment

**Session 7.3 — Dependency Injection**

- Lifetimes: Transient / Scoped / Singleton
- Captive dependency problem
- `IOptions<T>`, `IOptionsSnapshot`, `IOptionsMonitor` <- Remove this, not junior level

**Session 7.4 — Auth, HTTP & REST**

- JWT — validation steps, claims, `[Authorize]`
- Authentication vs Authorization (policies, roles, claims)
- HTTP methods, status codes, CORS
- REST principles, idempotency
- ProblemDetails / RFC 7807

**Session 7.5 — Serialization & Error Handling**

- `System.Text.Json`, Accept header negotiation
- Global exception handling middleware
- `ILogger<T>`, logging levels, structured logging

**Session 7.6 — HTTP Client & Resiliency**

- Why `new HttpClient()` is broken (socket exhaustion, DNS caching)
- `IHttpClientFactory` — Named, Typed clients
- Polly basics — retry, circuit breaker (awareness level)

## MODULE 8 — Data Layer

**Session 8.1 — SQL Fundamentals**

- DDL / DML / DQL / DCL / TCL
- Data types, constraints, entity relationships
- Normalization (1NF / 2NF / 3NF)

**Session 8.2 — Querying**

- JOINs (INNER, LEFT, RIGHT, FULL)
- Indexes (clustered vs non-clustered)
- Aggregate functions, GROUP BY, HAVING
- Views, subqueries
- Window functions: `ROW_NUMBER()`, `RANK()`, `DENSE_RANK()`, `SUM() OVER`, `PARTITION BY`

**Session 8.3 — Transactions & Security**

- ACID properties
- BEGIN / COMMIT / ROLLBACK
- Isolation levels (READ COMMITTED, dirty reads, phantom reads)
- SQL injection prevention
- Stored Procedures, Triggers — when and why to avoid

**Session 8.4 — EF Core**

- Code-first migrations (add, update, remove, script)
- Change Tracking, `AsNoTracking()`
- Eager / Lazy / Explicit loading
- N+1 problem
- Projections (`Select`), avoid loading full entities
- Transactions in EF Core
## MODULE 9 — Testing

**Session 9.1 — Unit Testing (keep as-is)
  - Unit vs Integration vs E2E
  - xUnit: [Fact], [Theory], [InlineData]
  - AAA pattern
  - Test naming convention
  - Moq: Setup, Returns, Verify
  - In-memory vs real DB for tests — one bullet
  - Integration testing
## MODULE 10 — Architecture & Infrastructure

**Session 10.1 — Architectural Styles**

- Monolith vs Microservices — tradeoffs, when NOT to use microservices
- Layered / N-Tier architecture
- Clean Architecture vs Onion — key distinction
- Modular Monolith — awareness (middle ground between monolith and microservices, increasingly popular)
- CQRS, MediatR, event-driven — one-liner each, "heard of it" level

---

**Session 10.2 — Design Principles**

- Separation of concerns
- Loose coupling / high cohesion
- God class anti-pattern
- Technical debt — what it is, how to manage it
- Anemic vs rich domain model — you have a real opinion here from TO2
- DDD awareness — Entity, Value Object, Aggregate, Ubiquitous Language, Repository in domain context — framed as _"vocabulary for what you're already doing"_
- Business logic placement — not in controllers, not in stored procs, not in the DB

---

**Session 10.3 — Infrastructure Awareness**

- SQL vs NoSQL — when to choose which
- Caching — in-memory vs distributed, cache-aside pattern ← moves here from DevOps module, better fit
- Message brokers — awareness only (RabbitMQ / Azure Service Bus, why you'd use one)
- Eventual consistency — one paragraph, ties to microservices and NoSQL
- Azure basics — App Service, Blob, SQL ← moves here from DevOps, it's infrastructure not tooling

## MODULE 11— Frontend & Tools

**Session 10.1 — HTML & CSS

- Semantic HTML, accessibility, DOM
- Box model, specificity, stacking contexts
- Flexbox vs Grid
- Responsive design, container queries

**Session 10.2 — JavaScript & TypeScript

- Event loop
- var / let / const, closures, prototypes
- Promises, async / await
- Structural typing, any vs unknown, generics, type guards

**Session 10.3 Angular** ← **NEW SESSION** (replaces the old mixed DevOps blob)

Actual junior .NET+Angular interview topics:

- Components — `@Component`, template, selector, `ngOnInit` vs constructor
- Data binding — `[property]`, `(event)`, `[(ngModel)]`, string interpolation
- Services & DI — `@Injectable({ providedIn: 'root' })`, singleton scope
- Lifecycle hooks — `ngOnInit`, `ngOnDestroy`, `ngOnChanges` — when each fires
- RxJS basics — `Observable` vs `Promise`, `subscribe`, `map`/`filter`/`switchMap`
- `HttpClient` — making API calls, interceptors (auth header injection)
- Routing — `RouterModule`, `<router-outlet>`, route guards (`CanActivate`)
- Forms — template-driven vs reactive (`FormGroup`, `FormControl`, `Validators`)

### MODULE 12— DevOps & Tooling (1 session, awareness-level)

**11.1 DevOps & Tooling** ← Current 10.3 content, split cleanly

- Git: merge vs rebase, feature branch / PR workflow 
- Docker: Dockerfile, multi-stage build, docker-compose 
- CI/CD: pipeline stages, GitHub Actions basics 
- Azure basics: App Service, Blob, SQL, Key Vault 
- Caching strategies: in-memory vs distributed, cache-aside 