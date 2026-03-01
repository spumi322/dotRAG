### _JavaScript is single-threaded — the Event Loop_

JavaScript runs on **one thread**. It can handle async work (HTTP calls, timers, user events) without blocking because of the **event loop**.

**How it works:**

1. **Call stack** — executes synchronous code, one function at a time (LIFO).
2. When async work is encountered (`setTimeout`, `fetch`, DOM event), it's handed off to the **browser/Node API**.
3. When async work completes, its callback is placed in a **queue**.
4. The event loop checks: "Is the call stack empty?" If yes → move the next item from the queue to the stack.

**Two queues, different priorities:**

- **Microtask queue** (higher priority): `Promise.then/catch/finally`, `queueMicrotask`. Drained completely before any macrotask runs.
- **Macrotask queue** (lower priority): `setTimeout`, `setInterval`, I/O, UI rendering.

```js
console.log("1");
setTimeout(() => console.log("2"), 0);
Promise.resolve().then(() => console.log("3"));
console.log("4");

// Output: 1, 4, 3, 2
// sync first (1, 4) → microtasks (3) → macrotasks (2)
```

**Why this matters:** Heavy computation on the call stack freezes the entire UI. Explains why `setTimeout(..., 0)` doesn't run immediately.

---

### _`var` / `let` / `const`_

`var` is function-scoped, hoisted, and initialized to `undefined` — leads to subtle bugs, never use it. `let` and `const` are block-scoped and sit in a **Temporal Dead Zone** until their declaration line. Always use `const` by default, `let` only when reassignment is needed.

**Classic `var` closure trap:**

```js
for (var i = 0; i < 3; i++) {
    setTimeout(() => console.log(i), 0);
}
// Output: 3, 3, 3 (not 0, 1, 2)
// Fix: use `let` — creates a new binding per iteration
```

---

### _`==` vs `===`_

`==` checks equality with **type coercion** — JavaScript converts types before comparing. `===` checks value AND type, no conversion.

```js
0 == "0"   // true  (coercion)
0 === "0"  // false (different types)
null == undefined  // true (special case)
null === undefined // false
```

**Always use `===`.** `==` is a source of bugs. Interviewers use it as a trap to test understanding of JS coercion.

---

### _Closures_

A closure is a function that retains access to variables from the scope where it was created, even after that scope has finished executing.

```js
function makeCounter() {
    let count = 0;
    return function() {
        count++;
        return count;
    };
}
const counter = makeCounter();
counter(); // 1
counter(); // 2 — `count` is kept alive by the closure
```

Used for: data privacy, factory functions, callbacks that access surrounding context.

---

### _Prototypes — how JS does inheritance_

JavaScript has no classes like C#. The `class` keyword (ES6) is **syntactic sugar over prototypes**. Every object has a hidden `[[Prototype]]` link forming a chain. When a property isn't found on an object, JS walks up this chain.

`extends` just wires up the prototype chain. **Interview trap:** saying "JS has classes like C#/Java" is wrong — the mechanism is prototype-based, not class-based.

---

### _Event Bubbling_

When an event fires on an element (e.g. a click on a `<button>` inside a `<div>`), it first triggers handlers on the target, then **bubbles up** through ancestors to the root.

```html
<div id="parent">
  <button id="child">Click me</button>
</div>
```

Clicking the button: `button` handler fires first → then `div` handler fires.

`event.stopPropagation()` stops the bubble. The opposite phase is **capturing** (root → target), rarely used.

**Why it matters:** Event delegation — attach one handler to a parent instead of many handlers to children. Efficient for dynamic lists.

---

### _Promises & async/await_

A Promise represents the eventual result of an async operation. Three states: **pending → fulfilled or rejected**. Once settled, it cannot change.

`async/await` is syntactic sugar over Promises — `await` suspends the function (not the thread) until the Promise settles.

Key methods:

- `Promise.all([p1, p2])` — resolves when ALL resolve, rejects if ANY rejects.
- `Promise.allSettled([p1, p2])` — waits for all, never rejects.
- `Promise.race([p1, p2])` — resolves/rejects with the first to settle.
- `fetch` does NOT reject on HTTP 404/500 — only on network errors. Always check `response.ok`.

---

### _ES6+ features to know_

- **Arrow functions** — concise syntax, **no own `this`** (inherits from enclosing scope).
- **Destructuring** — `const { name, price } = product;`
- **Spread/rest (`...`)** — shallow copy/merge objects and arrays, collect function arguments.
- **Template literals** — `` `Hello, ${name}` ``
- **Modules (`import`/`export`)** — file-level scope, explicit dependencies.
- **`map`/`filter`/`reduce`** — return new arrays, don't mutate.
- **Optional chaining (`?.`)** — `user?.address?.city` instead of nested if checks.
- **Nullish coalescing (`??`)** — returns default only for `null`/`undefined`, NOT for `0` or `""`.

---

### _Common interview questions_

- "What is the event loop?" → Single-threaded, call stack + microtask queue (Promises) + macrotask queue (setTimeout). Microtasks drain first.
- "Explain closures" → Inner function retains access to outer scope after outer function returns.
- "What's the output of [code with var/let/closure]?" → Know the `var` loop trap.
- "`==` vs `===`?" → `===` always. `==` does type coercion.
- "What is event bubbling?" → Events propagate from target up through ancestors. `stopPropagation()` to stop it.

---

---

## TypeScript

### _What TypeScript is_

TypeScript is a **statically-typed superset of JavaScript**. All valid JS is valid TS. It adds a type system checked at **compile time** that is completely **erased at runtime** — the output is plain JavaScript with zero performance overhead.

---

### _Structural typing — the key difference from C#_

C# uses **nominal typing**: two types are compatible only if they explicitly declare a relationship. TypeScript uses **structural typing**: compatibility is based on **shape** — same properties and types — regardless of name.

```ts
interface Cat { name: string; meow(): void; }
interface Robot { name: string; meow(): void; }
const r: Robot = { name: "RoboCat", meow() {} };
const c: Cat = r; // ✅ Works — same shape, different names
```

This is fundamental to understanding TS and comes up frequently in interviews.

---

### _`any` vs `unknown` vs `never`_

- **`any`** — disables all type checking. Defeats the purpose of TypeScript. Use only during JS migration.
- **`unknown`** — accepts any value but **forces you to narrow the type** before using it. The safe alternative to `any`.
- **`never`** — represents values that never occur. A function that always throws returns `never`. Used in exhaustive switch checks.

---

### _Interface vs Type alias_

Both describe shapes. Practical differences:

- **Interface** — supports declaration merging (same name = auto-merged), can be `extended`. Preferred for **object shapes and class contracts**.
- **Type alias** — can represent unions (`string | number`), intersections (`A & B`), primitives, tuples. More flexible.

**Rule of thumb:** Use `interface` for objects/APIs. Use `type` for everything else.

---

### _Generics_

Let you write reusable code that works with multiple types while preserving type safety. Same concept as C# generics (`List<T>`, `Repository<T>`).

```ts
function identity<T>(value: T): T {
    return value;
}
```

---

### _Type narrowing_

TypeScript narrows broad types to specific ones through control flow analysis:

- `typeof x === "string"` → narrows to `string`
- `"property" in obj` → narrows to the type that has that property
- `instanceof` → narrows to class type
- Custom type predicates: `function isFish(pet: Fish | Bird): pet is Fish`

---

### _Utility Types (awareness level)_

Built-in generic types for common transformations:

```ts
Partial<T>    // all properties optional
Required<T>   // all properties required
Readonly<T>   // all properties readonly
Pick<T, K>    // only keep specified properties
Omit<T, K>    // remove specified properties
```

```ts
interface Tournament { id: number; name: string; status: string; }
type TournamentUpdate = Partial<Tournament>; // all optional — useful for PATCH DTOs
type TournamentSummary = Pick<Tournament, "id" | "name">; // subset
```

**Interview answer:** "I use `Partial<T>` for update DTOs and `Omit<T, K>` to remove fields I don't want to expose, like IDs in create requests."

---

### _Common interview questions_

- "Why TypeScript over JavaScript?" → Compile-time error detection, self-documenting, better IDE support. Zero runtime cost.
- "Structural vs nominal typing?" → TS checks shape compatibility, C# checks declared type hierarchy.
- "Interface vs Type?" → Interface for objects (extendable, mergeable). Type for unions, intersections.
- "`any` vs `unknown`?" → Both accept anything. `unknown` forces narrowing — safer.
- "What are utility types?" → Built-in generics like `Partial<T>`, `Pick<T,K>`, `Readonly<T>` for common type transformations.

[[HTML]] [[CSS]]