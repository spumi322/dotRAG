### *Pattern matching in C# — concise conditional logic.*

Pattern matching lets you test a value against a pattern and extract data from it. Evolved across C# 7–12.

### *Type pattern (C# 7)*

```csharp
object obj = "hello";

if (obj is string s)
    Console.WriteLine(s.Length); // s is already cast to string
```

### *Switch expression (C# 8)*

Replaces verbose `switch` statements. Returns a value.

```csharp
string GetStatusText(int code) => code switch
{
    200 => "OK",
    404 => "Not Found",
    500 => "Internal Server Error",
    _ => "Unknown" // _ is the discard (default)
};
```

### *Property pattern (C# 8)*

Match on property values directly:

```csharp
decimal GetDiscount(Order order) => order switch
{
    { Total: > 1000, IsPremium: true } => 0.20m,
    { Total: > 500 } => 0.10m,
    { Total: > 100 } => 0.05m,
    _ => 0m
};
```

### *Relational & logical patterns (C# 9)*

Use `<`, `>`, `<=`, `>=`, `and`, `or`, `not`:

```csharp
string Classify(int temp) => temp switch
{
    < 0 => "Freezing",
    >= 0 and < 15 => "Cold",
    >= 15 and < 30 => "Comfortable",
    >= 30 => "Hot"
};
```

### *Tuple pattern (C# 8)*

Match on multiple values at once:

```csharp
string RockPaperScissors(string p1, string p2) => (p1, p2) switch
{
    ("rock", "scissors") => "P1 wins",
    ("scissors", "paper") => "P1 wins",
    ("paper", "rock") => "P1 wins",
    (_, _) when p1 == p2 => "Draw",
    _ => "P2 wins"
};
```

### *List pattern (C# 11)*

Match on collection elements:

```csharp
int[] numbers = { 1, 2, 3 };

var result = numbers switch
{
    [1, 2, 3] => "exact match",
    [1, ..] => "starts with 1",
    [.., 3] => "ends with 3",
    [] => "empty",
    _ => "other"
};
```

### *Practical use in ASP.NET Core*

```csharp
// Exception → status code mapping in middleware
var statusCode = exception switch
{
    NotFoundException => StatusCodes.Status404NotFound,
    ValidationException => StatusCodes.Status400BadRequest,
    UnauthorizedAccessException => StatusCodes.Status403Forbidden,
    _ => StatusCodes.Status500InternalServerError
};
```

[[Data Types]]
[[Exceptions]]