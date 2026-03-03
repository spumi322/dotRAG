dotRAG
A personal .NET learning repository — structured study notes and interview prep for a junior .NET developer role.

All code examples use TO2 (Tournament Organizer 2) as the reference domain: `Tournament`, `Match`, `Team`, `Standing`, `Game`, `Group`, `Tenant`, `User`. This keeps examples consistent and grounded in a real codebase rather than generic `Product`/`Order` placeholders.

---

Curriculum
Module 1 — Memory & Type System
  1.1 Memory Architecture — stack/heap, value vs reference, boxing, struct vs class
  1.2 Fundamental Types — strings, arrays, enums, System.Object, ref/out/in
  1.3 Modern Type Features — nullable types, records, tuples, pattern matching

Module 2 — Generics, Collections, LINQ
  2.1 Generics — constraints, Func/Action/Predicate, covariance/contravariance
  2.2 Collections — IEnumerable hierarchy, List/Dictionary/HashSet/Queue/Stack
  2.3 Delegates, Lambdas & Closures — events, lambda expressions, closure traps
  2.4 LINQ — deferred execution, core operators, IEnumerable vs IQueryable

Module 3 — OOP & C# Keywords
  3.1 OOP Pillars — encapsulation, inheritance, polymorphism, abstraction
  3.2 Abstraction Mechanisms — abstract class vs interface, default interface methods
  3.3 C# Keywords & Polymorphism — virtual/override/new/sealed, upcasting/downcasting

Module 4 — SOLID & Design Patterns
  4.1 SOLID — SRP, OCP, LSP, ISP, DIP with code examples
  4.2 Design Patterns — Singleton, Factory, Builder, Adapter, Decorator, Strategy, Observer, Command

Module 5 — .NET Runtime & Memory
  5.1 .NET Internals — CLR, CIL, JIT, managed vs unmanaged, reflection
  5.2 Garbage Collection — generations, mark-and-sweep, IDisposable, using
  5.3 Async & Concurrency — async/await, Task/ValueTask, deadlocks, SemaphoreSlim, CancellationToken
  5.4 Exceptions — throw vs throw ex, finally, custom exceptions, exception hierarchy

Module 6 — ASP.NET Core
  6.1 Architecture & Middleware — Kestrel, Program.cs, middleware pipeline
  6.2 Routing & Controllers — attribute routing, model binding, BackgroundService
  6.3 Dependency Injection — lifetimes, captive dependency, IOptions
  6.4 Auth, HTTP & REST — JWT, policies, HTTP methods/status codes, REST, CORS
  6.5 Serialization & Error Handling — System.Text.Json, global exception handler, pagination
  6.6 HTTP Client & Resiliency — IHttpClientFactory, named/typed clients, Polly

Module 7 — Data Layer
  7.1 SQL Fundamentals — DDL/DML, data types, constraints, normalization
  7.2 Querying — JOINs, indexes, aggregates, GROUP BY
  7.3 Transactions & Security — ACID, BEGIN/COMMIT/ROLLBACK, isolation levels, SQL injection
  7.4 EF Core — migrations, change tracking, loading strategies, N+1, transactions, optimistic concurrency

Module 8 — Testing
  8 Testing — pyramid, AAA, xUnit, Moq, WebApplicationFactory, test DB strategies

Module 9 — Architecture & Infrastructure
  9.1 Architecture — Clean Architecture, monolith vs microservices, CQRS, MediatR, event-driven
  9.2 Design Principles & Infrastructure — SoC, coupling/cohesion, DDD awareness, caching, Azure

Module 10 — Frontend & Tools
  10.1 HTML & CSS — semantic HTML, box model, flexbox/grid, responsive design
  10.2 JavaScript & TypeScript — event loop, closures, promises, structural typing, utility types
  10.3 Angular — components, data binding, services, lifecycle hooks, RxJS, HttpClient, routing, forms

Module 11 — DevOps & Tooling
  11.1 DevOps & Tooling — Git, Docker, CI/CD (GitHub Actions), Azure, caching strategies

---

Document Editing Rules
All notes are maintained in Obsidian
Edit .md files in Obsidian only — do not edit directly in Visual Studio or GitHub
After each study session: git add . → git commit → git push
Commit message format: docs(module-X): short description
Do not delete or rename files without updating internal Obsidian links first
