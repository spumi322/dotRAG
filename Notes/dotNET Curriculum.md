# .NET Junior Developer — Curriculum

---

## MODULE 1 — Memory & Type System

**[[Module 1 — Memory & Type System/1.1 Memory Architecture|Session 1.1 — Memory Architecture]]**
- Stack & Heap
- Value vs Reference types
- Boxing / Unboxing
- `struct` vs `class`

**[[Module 1 — Memory & Type System/1.2 Fundamental Types|Session 1.2 — Fundamental Types]]**
- Strings (immutability, interning, StringBuilder)
- Arrays
- Enums
- System.Object (ToString, Equals, GetHashCode, GetType)
- `const` vs `readonly`
- `ref` / `out` / `in`, `params`

**[[Module 1 — Memory & Type System/1.3 Modern Type Features|Session 1.3 — Modern Type Features]]**
- Nullable types (`Nullable<T>`, NRTs, `?.`, `??`, `!`)
- Records (immutability, `with`, `==` override)
- Tuples

---

## MODULE 2 — Generics, Collections, LINQ

**[[Module 2 — Generics, Collections, LINQ/2.1 Generics|Session 2.1 — Generics]]**
- Generic types, methods, constraints (`where T : class/new()`)
- `Func<>`, `Action<>`, `Predicate<>`
- Covariance / contravariance — interview-trap code snippet

**[[Module 2 — Generics, Collections, LINQ/2.2 Collections|Session 2.2 — Collections]]**
- `IEnumerable`, `IEnumerator`, `ICollection`, `IList`, `IDictionary`
- `IReadOnlyList`, `IReadOnlyDictionary`
- `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Queue`, `Stack`, `LinkedList`

**[[Module 2 — Generics, Collections, LINQ/2.3 Delegates, Lambdas & Closures|Session 2.3 — Delegates, Lambdas & Closures]]**
- Delegates — type-safe function pointer, why they exist
- Events — `event` vs delegate distinction (access restriction)
- Lambda expressions
- Closures — variable capture trap (`for` loop pitfall)

**[[Module 2 — Generics, Collections, LINQ/2.4 LINQ|Session 2.4 — LINQ]]**
- LINQ basics, method vs query syntax
- Deferred execution
- `IEnumerable` vs `IQueryable` — EF Core implications

---

## MODULE 3 — OOP & C# Keywords


- Encapsulation, Inheritance, Polymorphism, Abstraction
- Composition over Inheritance

**[[Module 3 — OOP & C# Keywords/3.2 Abstraction Mechanisms|Session 3.2 — Abstraction Mechanisms]]**
- Abstract class vs Interface — when to use which
- C# 8 default interface methods

**[[Module 3 — OOP & C# Keywords/3.3 Csharp Keywords & Polymorphism|Session 3.3 — C# Keywords & Polymorphism]]**
- `static`, `abstract`, `virtual`, `override`, `new`, `sealed`
- Overloading — compile-time polymorphism
- Overriding — runtime polymorphism
- Hiding — `new` keyword
- Upcasting & Downcasting — `as` / `is` patterns
- Pattern matching

---

## MODULE 4 — SOLID & Design Patterns

**[[Module 4 — SOLID & Design Patterns/4.1 SOLID|Session 4.1 — SOLID]]**
- SRP, OCP, LSP, ISP, DIP — all with code examples
- Identifying violations in code snippets

**[[Module 4 — SOLID & Design Patterns/4.2 Design Patterns|Session 4.2 — Design Patterns]]**
- Creational: Singleton, Factory, Builder
- Structural: Adapter, Decorator, Proxy
- Behavioral: Strategy, Observer, Command (+ MediatR as real-world Command)

---

## MODULE 5 — .NET Runtime & Memory

**[[Module 5 — .NET Runtime & Memory/5.1 .NET Internals|Session 5.1 — .NET Internals]]**
- CLR, CIL, JIT — source to IL to native pipeline
- Managed vs unmanaged code
- CTS, BCL — purpose and scope
- .NET Framework vs .NET Core/5+
- Reflection — conceptual only (how DI/ORM use it internally)

**[[Module 5 — .NET Runtime & Memory/5.2 Garbage Collection|Session 5.2 — Garbage Collection & Disposal]]**
- GC generations (0/1/2), mark-and-sweep, LOH
- `IDisposable`, `Dispose` vs `Finalize`, `using`

**[[Module 5 — .NET Runtime & Memory/5.3 Async & Concurrency|Session 5.3 — Async & Concurrency]]**
- `async` / `await`, `Task<T>`, `ValueTask`
- `.Result` deadlock + `ConfigureAwait(false)`
- `async void`, fire-and-forget pitfalls
- Thread pool, locks, `SemaphoreSlim`, deadlocks
- `CancellationToken` — conceptual awareness

**[[Module 5 — .NET Runtime & Memory/5.4 Exceptions|Session 5.4 — Exceptions]]**
- `throw` vs `throw ex`
- `finally` — deterministic cleanup
- Custom exceptions — when to create one
- Exception hierarchy

---

## MODULE 6 — ASP.NET Core

**[[Module 6 — ASP.NET Core/6.1 Architecture & Middleware|Session 6.1 — Architecture & Middleware]]**
- Kestrel, hosting model, Program.cs
- Middleware pipeline — order, short-circuit
- Minimal APIs vs Controller-based

**[[Module 6 — ASP.NET Core/6.2 Routing & Controllers|Session 6.2 — Routing & Controllers]]**
- Attribute routing, route constraints
- Model binding & validation (`[ApiController]`, ModelState)
- Action Filters — auth, logging, exception, caching
- Middleware vs Filter distinction
- `IHostedService`

**[[Module 6 — ASP.NET Core/6.3 Dependency Injection|Session 6.3 — Dependency Injection]]**
- Lifetimes: Transient / Scoped / Singleton
- Captive dependency problem

**[[Module 6 — ASP.NET Core/6.4 Auth, HTTP & REST|Session 6.4 — Auth, HTTP & REST]]**
- JWT — validation steps, claims, `[Authorize]`
- Authentication vs Authorization (policies, roles, claims)
- HTTP methods, status codes, CORS
- REST principles, idempotency
- ProblemDetails / RFC 7807

**[[Module 6 — ASP.NET Core/6.5 Serialization & Error Handling|Session 6.5 — Serialization & Error Handling]]**
- `System.Text.Json`, Accept header negotiation
- Global exception handling middleware
- `ILogger<T>`, logging levels, structured logging

**[[Module 6 — ASP.NET Core/6.6 HTTP Client & Resiliency|Session 6.6 — HTTP Client & Resiliency]]**
- Why `new HttpClient()` is broken (socket exhaustion, DNS caching)
- `IHttpClientFactory` — Named, Typed clients
- Polly basics — retry, circuit breaker (awareness level)

---

## MODULE 7 — Data Layer

**[[Module 7 — Data Layer/7.1 SQL Fundamentals|Session 7.1 — SQL Fundamentals]]**
- DDL / DML / DQL / DCL / TCL
- Data types, constraints, entity relationships
- Normalization (1NF / 2NF / 3NF)

**[[Module 7 — Data Layer/7.2 Querying|Session 7.2 — Querying]]**
- JOINs (INNER, LEFT, RIGHT, FULL)
- Indexes (clustered vs non-clustered)
- Aggregate functions, GROUP BY, HAVING
- Views, subqueries
- Window functions: `ROW_NUMBER()`, `RANK()`, `DENSE_RANK()`, `SUM() OVER`, `PARTITION BY`

**[[Module 7 — Data Layer/7.3 Transactions & Security|Session 7.3 — Transactions & Security]]**
- ACID properties
- BEGIN / COMMIT / ROLLBACK
- Isolation levels (READ COMMITTED, dirty reads, phantom reads)
- SQL injection prevention
- Stored Procedures, Triggers — when and why to avoid

**[[Module 7 — Data Layer/7.4 EF Core|Session 7.4 — EF Core]]**
- Code-first migrations (add, update, remove, script)
- Change Tracking, `AsNoTracking()`
- Eager / Lazy / Explicit loading
- N+1 problem
- Projections (`Select`), avoid loading full entities
- Transactions in EF Core

---

## MODULE 8 — Testing

**[[Module 8 — Testing/8.1 Unit Testing|Session 8.1 — Unit Testing]]**
- Unit vs Integration vs E2E — scope and purpose
- xUnit: `[Fact]`, `[Theory]`, `[InlineData]`
- AAA pattern (Arrange / Act / Assert)
- Test naming: `MethodName_Scenario_ExpectedResult`
- Moq: Setup, Returns, Verify
- Integration tests

**[[Module 8 — Testing/8.2 Integration Testing|Session 8.2 — Integration Testing]]**
- `WebApplicationFactory<T>`
- In-memory vs real DB for tests

---

## MODULE 9 — Architecture & Infrastructure

**[[Module 9 — Architecture & Infrastructure/9.1 Architectural Styles|Session 9.1 — Architectural Styles]]**
- Monolith vs Microservices — tradeoffs, when NOT to use microservices
- Layered / N-Tier architecture
- Clean Architecture vs Onion — key distinction
- Modular Monolith — awareness
- CQRS, MediatR, event-driven — "heard of it" level

**[[Module 9 — Architecture & Infrastructure/9.2 Design Principles|Session 9.2 — Design Principles]]**
- Separation of concerns
- Loose coupling / high cohesion
- God class anti-pattern
- Technical debt — what it is, how to manage it
- Anemic vs rich domain model
- DDD awareness — Entity, Value Object, Aggregate, Ubiquitous Language
- Business logic placement

**[[Module 9 — Architecture & Infrastructure/9.3 Infrastructure Awareness|Session 9.3 — Infrastructure Awareness]]**
- SQL vs NoSQL — when to choose which
- Caching — in-memory vs distributed, cache-aside pattern
- Message brokers — awareness only
- Eventual consistency
- Azure basics — App Service, Blob, SQL

---

## MODULE 10 — Frontend & Tools

**[[Module 10 — Frontend & Tools/10.1 HTML & CSS|Session 10.1 — HTML & CSS]]**
- Semantic HTML, accessibility, DOM
- Box model, specificity, stacking contexts
- Flexbox vs Grid
- Responsive design, container queries

**[[Module 10 — Frontend & Tools/10.2 JavaScript & TypeScript|Session 10.2 — JavaScript & TypeScript]]**
- Event loop
- `var` / `let` / `const`, closures, prototypes
- Promises, async / await
- Structural typing, `any` vs `unknown`, generics, type guards

**[[Module 10 — Frontend & Tools/10.3 Angular|Session 10.3 — Angular]]**
- Components — `@Component`, template, selector, `ngOnInit` vs constructor
- Data binding — `[property]`, `(event)`, `[(ngModel)]`, string interpolation
- Services & DI — `@Injectable({ providedIn: 'root' })`, singleton scope
- Lifecycle hooks — `ngOnInit`, `ngOnDestroy`, `ngOnChanges`
- RxJS basics — `Observable` vs `Promise`, `subscribe`, `map`/`filter`/`switchMap`
- `HttpClient` — making API calls, interceptors
- Routing — `RouterModule`, `<router-outlet>`, route guards (`CanActivate`)
- Forms — template-driven vs reactive (`FormGroup`, `FormControl`, `Validators`)

---

## MODULE 11 — DevOps & Tooling

**[[Module 11 — DevOps & Tooling/11.1 DevOps & Tooling|Session 11.1 — DevOps & Tooling]]**
- Git: merge vs rebase, feature branch / PR workflow
- Docker: Dockerfile, multi-stage build, docker-compose
- CI/CD: pipeline stages, GitHub Actions basics
- Azure basics: App Service, Blob, SQL, Key Vault
- Caching strategies: in-memory vs distributed, cache-aside
