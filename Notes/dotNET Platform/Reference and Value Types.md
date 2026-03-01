# Reference and Value Types

## Value types

Store the actual data directly. Copied on assignment and when passed to a method.

Examples: `int`, `double`, `bool`, `decimal`, `char`, `struct`, `enum`

## Reference types

Store a reference (pointer) to where the data lives on the heap. Assignment and method passing copies the reference, not the object.

Examples: `class`, `string`, `array`, `List<T>`, all user-defined classes

---

## Where they live in memory

Value types live on the **stack** when they are local variables or method parameters. When a value type is a field inside a class, it lives on the **heap** as part of that object — the type alone doesn't determine location, context does.

```csharp
class Order              // heap-allocated (reference type)
{
    private int _id;     // int lives on the HEAP — it's a field of a class
}

void Process()
{
    int x = 5;               // stack — local variable
    var order = new Order(); // reference on stack, Order object on heap
}
```

---

## Passing to methods

**Value type — a copy goes in, original is untouched:**

```csharp
void AddTen(int number)
{
    number += 10; // modifies the local copy only
}

int x = 5;
AddTen(x);
Console.WriteLine(x); // still 5
```

**Reference type — the reference is copied, but both point to the same object:**

```csharp
void Rename(Tournament t)
{
    t.Name = "New Name"; // modifies the actual object on the heap
}

var tournament = new Tournament { Name = "Old Name" };
Rename(tournament);
Console.WriteLine(tournament.Name); // "New Name"
```

---

## Assignment

**Value type — creates an independent copy:**

```csharp
int a = 10;
int b = a;  // b is a separate copy
b = 99;
Console.WriteLine(a); // 10 — unaffected
```

**Reference type — both variables point to the same object:**

```csharp
var t1 = new Tournament { Name = "ESL" };
var t2 = t1;        // t2 points to the same object, not a copy
t2.Name = "BLAST";
Console.WriteLine(t1.Name); // "BLAST"
```

---

## Other differences

- Value types inherit from `System.ValueType`, reference types from `System.Object`
- Value types are always **sealed** — you cannot inherit from them
- Reference types are cleaned up by the **Garbage Collector**; value types are reclaimed when they go out of scope

---

## Interview traps

- **"Where are value types stored?"** — On the stack _when local variables_. If they're fields of a class, they're on the heap. Don't just say "stack."
- **"Is string a value or reference type?"** — Reference type. But it _behaves_ like a value type because it's immutable — every modification creates a new string object.
- **"What happens when you pass a class to a method and reassign it inside?"** — Reassigning the parameter only changes the local reference. The caller's variable still points to the original object. Mutating properties of the object does affect the caller.

---

### Related

[[Stack & Heap]] [[Boxing and Unboxing]] [[Ref & Out]] [[Data Types]]