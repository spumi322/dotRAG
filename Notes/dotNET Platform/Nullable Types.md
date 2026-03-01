### *What nullable data types are and how they represent values that can be null.*

Nullable type is an instance of the `Nullable<T>`
Nullable data types in .NET allow value types, which normally cannot be null, to represent null values in addition to their standard range of values. This feature is particularly useful when dealing with scenarios where data can be optional or missing, such as in databases or user input.

Nullable types use an underlying structure that includes a boolean flag to indicate whether the value is null and a field to store the actual value.

When the value is null, the boolean flag is set to false, indicating the absence of a valid value.

```C#
int? nullableNumber = null;
```
### *Nullable Reference Types (C# 8+) — different from Nullable\<T\>*

`Nullable<T>` (above) is for **value types**. Nullable Reference Types (NRTs) are a **compile-time safety feature** for reference types. Enabled by default in .NET 6+ project templates.

```csharp
#nullable enable

string name = "John";   // non-nullable — compiler warns if you assign null
string? nickname = null; // nullable — explicitly allowed to be null

Console.WriteLine(nickname.Length); // ⚠️ CS8602 warning: possible null dereference
Console.WriteLine(nickname?.Length); // ✅ safe with null-conditional operator
```

**Key points:**
- NRTs are a **compile-time** feature only — no runtime effect, no `Nullable<T>` wrapper.
- `string?` signals "this might be null, handle it." `string` signals "this should never be null."
- Enable per-project in `.csproj`: `<Nullable>enable</Nullable>` (default in .NET 6+).
- Common interview question: "What's the difference between `int?` and `string?`?" → `int?` is `Nullable<int>` (value type wrapper, runtime), `string?` is an annotation (reference type, compile-time only).
### _Nullable operators_

**Null-conditional (`?.`)** - Safe navigation, returns null if operand is null:
```csharp
string? name = null;
int? length = name?.Length; // null (no exception)
string? city = user?.Address?.City; // chain safely
```

**Null-coalescing (`??`)** - Fallback value if null:
```csharp
string display = userName ?? "Guest";
int value = nullableInt ?? 0;
```

**Null-coalescing assignment (`??=`)** - Assign only if null:
```csharp
items ??= new List<string>(); // assigns only if items is null
```

**Null-forgiving (`!`)** - Suppress compiler warning (NO runtime check):
```csharp
string validated = input!; // "I know it's not null"
```
