### *What Generic types are and why they are used.*

Generic types allow you to define classes, interfaces, and methods with placeholders for the type of data. They are used to create reusable, type-safe code without specifying the data types.

1. _Collections_: Generic collections are type-safe data structures, like 
```List<T>``` and ```Dictionary<TKey, TValue>```, designed to store elements of a specific type, ensuring improved performance and compile-time type checking.

2. _Methods_: Generic methods in C# allow you to write a method that can work with any data type. For example, you could write a generic method for sorting or comparing elements that works with any type that implements the ```IComparable<T>``` interface.

```csharp
// Works with any type that implements IComparable<T>
public T GetMax<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b) > 0 ? a : b;
}

GetMax(3, 7);        // 7
GetMax("a", "z");   // "z"
```

3. _Classes and Interfaces:_ Allows you to define generic classes and interfaces. This is useful for creating data structures, services, or utilities that can work with any data type. An example is a generic ```Repository<T>``` interface used in data access layers.

```csharp
// Generic interface — T is the entity type
public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(long id);
    Task<IReadOnlyList<T>> GetAllAsync();
}

// One implementation works for every entity
public class Repository<T> : IRepository<T> where T : EntityBase { ... }
```

### *Constraints for Generic types, such as where T : class, where T : new(), etc.*

Type Constraints: generics also support type constraints which allow you to specify that a type parameter must inherit from a particular class or implement a specific interface, adding more control and safety to your generic code.

### *Generic delegates, such as Func and Action.*

**Func<>** is a generic delegate that represents a method that returns a value. It can have zero or more input parameters.
```csharp
// Last type param is always the return type
Func<int, int, int> add = (a, b) => a + b;
int result = add(3, 4); // 7

// Common in LINQ — Select takes a Func<T, TResult>
var names = matches.Select(m => m.HomeTeam); // Func<Match, string>
```

**Action<>** is a generic delegate that represents a method that does not return a value (void). It can have zero or more input parameters.
```csharp
// No return value — void equivalent
Action<string> log = message => Console.WriteLine(message);
log("Tournament started"); // prints: Tournament started

// Common in callbacks, event handlers
```

**Predicate<T>** is a generic delegate that represents a method returning `bool`. Takes one input parameter.

```csharp
Predicate<int> isEven = n => n % 2 == 0;
Console.WriteLine(isEven(4)); // True
```

**Predicate vs Func<T, bool>:**
- Signature-identical: `Predicate<int>` = `Func<int, bool>`
- `Predicate<T>` is older (pre-LINQ), domain-specific naming
- Modern code uses `Func<T, bool>` everywhere (LINQ uses it)
- You'll see `Predicate<T>` in older APIs like `List<T>.FindAll()`
```csharp
var numbers = new List<int> { 1, 2, 3, 4 };

// Old style - uses Predicate<T>
var evens1 = numbers.FindAll(n => n % 2 == 0);

// Modern style - uses Func<T, bool>
var evens2 = numbers.Where(n => n % 2 == 0).ToList();
```

**Interview answer:** "Same as `Func<T, bool>`, but predates LINQ. Modern code prefers `Func`."

### *Covariant & Contravariant*

```csharp
// **Covariant (`out`)** → Can return MORE derived types
IEnumerable<out T>       // Can read out T
IReadOnlyList<out T>     // Can read out T
Func<out TResult>        // Returns TResult

// **Contravariant (`in`)** → Can accept LESS derived types (more general)
IComparer<in T>          // Takes in T for comparison
Action<in T>             // Takes in T as parameter
Func<in TInput, out TResult> // Takes in TInput, returns TResult
```

````csharp
public class DiscountedProductDto : ProductDto { ... }

// ✅ Covariance works with IReadOnlyList too
public IReadOnlyList<ProductDto> GetProducts()
{
    var discounted = new List<DiscountedProductDto>
    {
        new() { Id = 1, Name = "Widget", Price = 10, DiscountPercent = 20 }
    };
    
    return discounted; // ✅ Works! IReadOnlyList<out T> is covariant
}
```

---

## **Hierarchy:**
```
IEnumerable<out T>           ← Covariant
    ↑
IReadOnlyCollection<out T>   ← Covariant
    ↑
IReadOnlyList<out T>         ← Covariant + Count + Indexing
````

**All read-only interfaces are covariant.**

---

## **Why not List<T>?**

csharp

```csharp
List<T> // ← NOT covariant (no 'out')
```

Because `List<T>` allows **writing** (`.Add()`, `.Remove()`), it can't be covariant (would break type safety).

**Read-only interfaces** (`IEnumerable`, `IReadOnlyCollection`, `IReadOnlyList`) only allow **reading**, so they're safe to be covariant.
**Another real scenario — Event handlers:**

```csharp
// ASP.NET Core middleware logging
public class LoggingMiddleware
{
    // This handler works for ANY exception type
    private Action<Exception> _logger = ex => Console.WriteLine(ex.Message);
    
    public async Task InvokeAsync(HttpContext context)
    {
        try 
        { 
            await _next(context); 
        }
        catch (Exception ex)
        {
            // All these work because Action<in T> is contravariant:
            Action<InvalidOperationException> logInvalidOp = _logger; // ✅
            Action<ArgumentException> logArgument = _logger;          // ✅
            Action<HttpRequestException> logHttp = _logger;           // ✅
        }
    }
}
```