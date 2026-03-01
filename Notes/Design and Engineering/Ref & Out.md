
By default, value types are passed **by value** (a copy). These keywords change that behavior.

### _`ref` — pass by reference, must be initialized before the call_

```csharp
void Double(ref int number)
{
    number *= 2; // modifies the original variable
}

int x = 5;
Double(ref x);
// x is now 10
```

The caller **must** initialize the variable before passing it. The method **may** read and modify it.

### _`out` — pass by reference, must be assigned inside the method_

```csharp
bool TryParse(string input, out int result)
{
    return int.TryParse(input, out result);
}

if (TryParse("42", out int value))
    Console.WriteLine(value); // 42
```

The caller does **not** need to initialize the variable. The method **must** assign it before returning. Common pattern: `TryXxx` methods (`int.TryParse`, `Dictionary.TryGetValue`).

### _`in` — pass by reference, read-only

```csharp
double CalculateDistance(in Point a, in Point b)
{
    // a.X = 0; // ❌ Compile error — cannot modify an `in` parameter
    return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
}
```

Passes by reference for performance (avoids copying large structs) but prevents modification. Use for large `readonly struct` parameters.

### _Summary_

|Modifier|Must init before call|Must assign in method|Can modify|Use case|
|---|---|---|---|---|
|(none)|✅|—|❌ (copy)|Default|
|`ref`|✅|❌|✅|Modify caller's variable|
|`out`|❌|✅|✅|Return multiple values|
|`in`|✅|❌|❌|Read-only, avoid copy of large structs|

### _Common interview question_

"Difference between `ref` and `out`?" → Both pass by reference. `ref` requires the variable to be initialized before the call. `out` requires the method to assign it before returning. Use `out` when the method produces a value; use `ref` when the method reads and modifies an existing value.

[[Reference and Value Types]]
[[Data Types]]