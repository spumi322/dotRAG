# Dynamic Type & Object Type

## `dynamic`

Type checking is skipped at compile time and happens at runtime instead. The compiler won't catch mistakes — if the property or method doesn't exist, you get a runtime exception.

```csharp
dynamic x = 1;
x = "now I'm a string"; // no compile error — type can change
Console.WriteLine(x.Length); // fine at runtime — string has Length

x = 42;
Console.WriteLine(x.Length); // RuntimeBinderException — int has no Length
```

**When you'd actually use it:** consuming COM interop (Office APIs), working with JSON blobs where shape is unknown, or interop with dynamic languages. In normal web dev code — avoid it.

---

## `object`

Every type in C# inherits from `object` (`System.Object`). You can assign anything to an `object` variable, but you have to cast it back to use it as its actual type. The compiler enforces the cast — so errors are at compile time, not runtime.

```csharp
object obj = 42;           // boxing — int stored as object
int n = (int)obj;          // explicit cast required
int bad = (string)obj;     // ❌ compile error — caught immediately
```

---

## `dynamic` vs `object`

||`object`|`dynamic`|
|---|---|---|
|Type checking|Compile-time|Runtime|
|Cast required|✅ explicit|❌ none|
|Error timing|Compile|Runtime crash|
|Use case|Store anything safely|Interop / unknown shape|

**Interview trap:** "Can you store any type in both?" — Yes. "What's the difference?" — `object` requires explicit casting and fails at compile time. `dynamic` skips type checking entirely and fails at runtime.

---

### Related

[[Data Types]] [[Boxing and Unboxing]] [[System.Object]]