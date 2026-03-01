### **MODULE 1: Memory & Type System**

**Session 1.1 — Memory Architecture**

- ✅ Stack & Heap
- ✅ Reference and Value Types
- ✅ Boxing and Unboxing

**Session 1.2 — Fundamental Types**

- ✅ Strings (immutability, interning, StringBuilder, Regex)
- ✅ Arrays
- ✅ Enums
- ❌ System.Object _(missing — add this)_

**Session 1.3 — Modern Type Features**

- ✅ Nullable Types (`Nullable<T>` + NRTs)
- ✅ Records
- ⚠️ Tuples _(missing from your notes — add or skip)_
- ✅ Dynamic Type & Object Type

---

### **MODULE 2: Generics → Collections → LINQ**

**Session 2.1 — Generics**

- ✅ Generic types, methods, constraints
- ✅ Func<>, Action<>, Predicate<>

**Session 2.2 — Collection Interfaces**

- ✅ IEnumerable, IEnumerator, ICollection, IList, IDictionary
- ✅ IReadOnlyList, IReadOnlyDictionary
- ✅ ConcurrentDictionary
- ✅ Data Structures (Stack, Queue, LinkedList, HashSet, Dictionary, List)

**Session 2.3 — LINQ**

- ✅ LINQ basics, deferred execution
- ✅ IEnumerable vs IQueryable

---

### **MODULE 3: Delegates → Events → Lambdas**

**Session 3.1 — Delegates & Events**

- ✅ Delegates, multicast delegates
- ✅ Events
- ✅ Anonymous methods

**Session 3.2 — Lambdas & Closures**

- ✅ Lambda expressions
- ✅ Closures
- ⚠️ Expression-bodied members _(not in notes — add or skip)_

---

### **MODULE 4: OOP & SOLID & Patterns**

**Session 4.1 — OOP Pillars**

- ✅ Encapsulation, Inheritance, Polymorphism, Abstraction
- ✅ Inheritance vs Composition

**Session 4.2 — Abstraction Mechanisms**

- ✅ Abstract Classes vs Interfaces
- ✅ C# 8+ default methods, C# 11+ static abstract, C# 14 extensions

**Session 4.3 — C# Keywords & Polymorphism**

- ✅ static, abstract, virtual, override, new, sealed
- ✅ override vs new trap

**Session 4.4 — SOLID Principles**

- ✅ SRP, OCP, LSP, ISP, DIP (all with code examples)

**Session 4.5 — Design Patterns**

- ✅ Creational: Singleton, Factory, Abstract Factory, Builder, Prototype
- ✅ Structural: Adapter, Decorator, Proxy, Facade, Property Container
- ✅ Behavioral: Observer, Strategy, Command, Memento, State
- ✅ DDD: Entities, Value Objects, Aggregates

---

### **MODULE 5: Advanced Language Features**

**Session 5.1 — Pattern Matching**

- ✅ Type patterns, switch expressions
- ✅ Property, relational, tuple, list patterns

**Session 5.2 — Parameter Modifiers & ref/out/in**

- ⚠️ ref/out/in _(missing details — add from curriculum)_
- ⚠️ params _(missing — add)_

**Session 5.3 — Exceptions & Reflection**

- ⚠️ Exceptions _(missing detailed notes — add from curriculum)_
- ⚠️ Reflection _(missing — add from curriculum)_

---

### **MODULE 6: .NET Runtime & Memory**

**Session 6.1 — .NET Internals**

- ✅ CLR, CIL, JIT, Assemblies
- ✅ .NET Framework vs .NET Core
- ✅ CTS, BCL

**Session 6.2 — Garbage Collection**

- ✅ GC (generations, mark-and-sweep, LOH, GC modes)
- ✅ Stack & Heap (revisited)
- ✅ Managed & Unmanaged Resources
- ✅ IDisposable (Dispose vs Finalize)
- ✅ Streams (FileStream, StreamReader)
- ✅ Serialization

**Session 6.3 — Async & Concurrency**

- ✅ Multi-threading (threads, thread pool, locks, semaphores, deadlocks)
- ✅ async/await, Task, ValueTask
- ✅ Common pitfalls (.Result deadlock, fire-and-forget, async void)
- ✅ ConfigureAwait

---

### **MODULE 7: ASP.NET Core**

**Session 7.1 — Architecture & Middleware**

- ✅ Kestrel, hosting, Program.cs
- ✅ Middleware pipeline
- 
**Session 7.2 — Routing & Controllers**

- ✅ Attribute routing, route constraints
- ✅ MVC vs Web API controllers
- ⚠️ Minimal APIs _(missing — add from curriculum)_ ADDED

**Session 7.3 — Dependency Injection**

- ✅ DI lifetimes (Transient/Scoped/Singleton)
- ✅ Captive dependency problem
- ✅ IOptions, IOptionsSnapshot, IOptionsMonitor

**Session 7.4 — Auth, HTTP, REST**

- ✅ Authentication (JWT) vs Authorization (policies, roles, claims)
- ⚠️ HTTP methods, status codes, CORS _(partial — expand)_  - HTTP Fundamentals.md
- ⚠️ REST principles, idempotency _(missing — add)_ - REST.md

**Session 7.5 — MVC, Filters, Error Handling**

- ✅ MVC pattern (1 sentence concept)
- ✅ Action Filters (auth, logging, exception, caching) SKIPPED??
- ⚠️ ProblemDetails _(missing — add) ADDED
- ✅ Serialization (Accept header, System.Text.Json)

---

### **MODULE 8: Data Layer**

**Session 8.1 — SQL Fundamentals**

- ✅ DDL/DML/DQL/DCL/TCL
- ✅ Normalization (1NF/2NF/3NF)
- ✅ Entity relationships
- ✅ Data types, constraints

**Session 8.2 — Querying**

- ✅ JOINs (INNER, LEFT, RIGHT, FULL)
- ✅ Indexes
- ✅ Aggregate functions (SUM, AVG, COUNT, GROUP BY)
- ✅ Views
- ✅ Subqueries

**Session 8.3 — Transactions & Security**

- ✅ ACID
- ⚠️ Transactions (BEGIN/COMMIT/ROLLBACK) _(missing details — add)_
- ⚠️ Stored Procedures, Triggers _(missing — add)_
- ✅ SQL injection prevention
- ✅ Same Data Manipulation (concurrency)

**Session 8.4 — EF Core**

- ✅ ORM concepts
- ✅ Migrations (add, update, remove, script)
- ✅ Eager/lazy/explicit loading
- ✅ N+1 problem
- ✅ AsNoTracking, projections

---

### **MODULE 9: Frontend & Tools**

**Session 9.1 — HTML & CSS**

- ✅ Semantic HTML, accessibility, DOM
- ✅ Box model, specificity, stacking contexts
- ✅ Flexbox vs Grid
- ✅ Responsive design, container queries

**Session 9.2 — JavaScript & TypeScript**

- ✅ Event loop
- ✅ var/let/const, closures, prototypes
- ✅ Promises, async/await
- ✅ TS: structural typing, `any` vs `unknown`, generics, type guards

**Session 9.3 — Performance, Logging, Git**

- ⚠️ Performance metrics _(partial in notes)_
- ✅ Caching strategies
- ⚠️ CancellationToken, parallel programming _(missing — add)_
- ⚠️ Logging levels, structured logging _(missing — add)_
- ⚠️ Git (merge vs rebase, workflows) _(missing — add from curriculum)_
- ✅ Regex (from Strings section)