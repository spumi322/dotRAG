### _System.Object — the base class for all types in .NET_

Every type in .NET (classes, structs, enums, delegates, interfaces at runtime) inherits from `System.Object`. This provides 4 core methods available to all types.

---
## Equals vs ReferenceEquals vs ==
#### **1. Value Types (primitives)**

csharp

```csharp
int a = 42;
int b = 42;

a.Equals(b);                    // TRUE — value equality
Object.ReferenceEquals(a, b);   // FALSE — boxing creates 2 separate objects
a == b;                         // TRUE — value equality
```

#### **2. Value Types (custom struct)**

csharp

```csharp
struct Point { public int X, Y; }

var p1 = new Point { X = 1, Y = 2 };
var p2 = new Point { X = 1, Y = 2 };

p1.Equals(p2);                  // TRUE — default value equality (slow, uses reflection)
Object.ReferenceEquals(p1, p2); // FALSE — boxing creates different objects
p1 == p2;                       // ❌ COMPILE ERROR — no == operator defined
                                // (unless you overload it)
```

#### **3. Reference Types (class)**

csharp

```csharp
class Person { public string Name; }

var p1 = new Person { Name = "Alice" };
var p2 = new Person { Name = "Alice" };
var p3 = p1;

p1.Equals(p2);                  // FALSE — different objects in memory
p1.Equals(p3);                  // TRUE — same reference
Object.ReferenceEquals(p1, p2); // FALSE — different objects
Object.ReferenceEquals(p1, p3); // TRUE — same object
p1 == p2;                       // FALSE — reference equality
p1 == p3;                       // TRUE — same reference
```

#### **4. String (special case)**

csharp

```csharp
string s1 = "hello";
string s2 = "hello";
string s3 = new string("hello".ToCharArray()); // force new object

s1.Equals(s2);                  // TRUE — value equality
s1.Equals(s3);                  // TRUE — value equality
Object.ReferenceEquals(s1, s2); // TRUE — string interning (same object)
Object.ReferenceEquals(s1, s3); // FALSE — different objects
s1 == s2;                       // TRUE — overloaded for value equality
s1 == s3;                       // TRUE — overloaded for value equality
```

**String interning:** The CLR maintains a pool of unique string literals. `"hello"` appears twice in code → same object in memory.
### **Key Methods**

#### **1. ToString() — string representation**

**Virtual method.** Default returns the type's fully qualified name. Override for meaningful output.

```csharp
// Default behavior (not useful)
object obj = new Person();
Console.WriteLine(obj.ToString()); // "Namespace.Person"

// Override for useful output
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    
    public override string ToString() => $"{Name}, {Age} years old";
}

var p = new Person { Name = "Alice", Age = 30 };
Console.WriteLine(p); // "Alice, 30 years old" (ToString() called implicitly)
```

**Use cases:** Debugging, logging, displaying objects in UI.

---

#### **2. Equals(object obj) — equality comparison**

**Virtual method.** Default behavior differs by type category:

- **Reference types (classes):** Reference equality — same object in memory?
- **Value types (structs):** Value equality — all fields equal? (uses reflection, slow)

```csharp
// Reference types - default reference equality
class Person { public string Name; }

var p1 = new Person { Name = "Alice" };
var p2 = new Person { Name = "Alice" };
p1.Equals(p2); // FALSE — different objects in memory

// Value types - default value equality
struct Point { public int X, Y; }

var pt1 = new Point { X = 1, Y = 2 };
var pt2 = new Point { X = 1, Y = 2 };
pt1.Equals(pt2); // TRUE — all fields match
```

**Override for custom equality:**

```csharp
class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Compare by Id only
    public override bool Equals(object obj)
    {
        if (obj is not Product other) return false;
        return Id == other.Id;
    }
    
    // MUST override GetHashCode when overriding Equals
    public override int GetHashCode() => Id.GetHashCode();
}
```

**CRITICAL RULE:** Override `Equals()` and `GetHashCode()` together, or neither. Breaking this rule corrupts `Dictionary<T>`, `HashSet<T>`.

---

#### **3. GetHashCode() — hash code for hash-based collections**

**Virtual method.** Returns an `int` used by `Dictionary`, `HashSet`, `Hashtable` for fast lookups.

**Contract rules:**

1. If `a.Equals(b)` is true, then `a.GetHashCode() == b.GetHashCode()` **must** be true.
2. Reverse is not required — different objects can have the same hash code (collision).
3. Hash code should remain constant for the object's lifetime.

```csharp
class Order
{
    public int Id { get; set; }
    public string Customer { get; set; }
    
    public override bool Equals(object obj) =>
        obj is Order o && o.Id == Id;
    
    // C# 9+ helper - preferred
    public override int GetHashCode() => HashCode.Combine(Id);
    
    // Pre-C# 9 manual approach
    // public override int GetHashCode() => Id.GetHashCode();
}

var dict = new Dictionary<Order, string>();
var order = new Order { Id = 1, Customer = "Alice" };
dict[order] = "Pending";

// Without GetHashCode override, dictionary lookup would fail
Console.WriteLine(dict[order]); // "Pending"
```

**Interview trap:** Override only `Equals()` → `Dictionary` breaks because equal objects may hash differently.

---

#### **4. GetType() — runtime type information**

**Non-virtual method** (cannot be overridden). Returns a `Type` object representing the exact runtime type.

```csharp
object obj = "hello";
Type t = obj.GetType();

Console.WriteLine(t.Name);           // "String"
Console.WriteLine(t.FullName);       // "System.String"
Console.WriteLine(t.Namespace);      // "System"
Console.WriteLine(t.IsValueType);    // False
Console.WriteLine(t.BaseType?.Name); // "Object"

// Common use: type checking
if (obj.GetType() == typeof(string))
    Console.WriteLine("It's a string");

// vs pattern matching (preferred in modern C#)
if (obj is string s)
    Console.WriteLine($"Length: {s.Length}");
```

**Reflection use case:**

```csharp
Type type = typeof(Person);
var methods = type.GetMethods();
var properties = type.GetProperties();

// Dynamic instantiation
object instance = Activator.CreateInstance(type);
```

---

### **Static Method: ReferenceEquals()**

**Non-virtual static method.** Tests if two references point to the same object in memory.

```csharp
var p1 = new Person { Name = "Alice" };
var p2 = p1; // same reference
var p3 = new Person { Name = "Alice" }; // different object

Object.ReferenceEquals(p1, p2); // TRUE — same object
Object.ReferenceEquals(p1, p3); // FALSE — different objects

// Even if Equals() is overridden, ReferenceEquals checks memory address
```

**Use case:** Checking object identity when `Equals()` is overridden for value equality.

---

### **Virtual vs Non-Virtual Summary**

|Method|Virtual?|Can Override?|Purpose|
|---|---|---|---|
|`ToString()`|✅ Yes|✅ Yes|String representation|
|`Equals(object)`|✅ Yes|✅ Yes|Equality logic|
|`GetHashCode()`|✅ Yes|✅ Yes|Hash-based collections|
|`GetType()`|❌ No|❌ No|Runtime type info|
|`ReferenceEquals()`|❌ No (static)|❌ No|Reference identity|

---

### **Common Interview Questions**

**Q: "What happens if you override `Equals()` but not `GetHashCode()`?"**  
A: Dictionary and HashSet break. Equal objects may hash differently, so lookups fail.

**Q: "Why is `GetType()` not virtual?"**  
A: It must return the exact runtime type. If overridable, derived classes could lie about their type.

**Q: "Difference between `==` and `Equals()`?"**  
A: `==` is an operator (can be overloaded per type). `Equals()` is a method. For reference types, `==` defaults to reference equality unless overloaded (e.g., `string` overloads `==` for value equality).

**Q: "When would you use `ReferenceEquals()`?"**  
A: When you need to check if two variables point to the same object, regardless of `Equals()` implementation.

---

### **Best Practices**

1. **Always override `Equals()` and `GetHashCode()` together.**
2. Use `HashCode.Combine()` helper (C# 9+) for multi-field hash codes.
3. Override `ToString()` for debugging/logging — not for business logic.
4. Prefer pattern matching (`is`, `as`) over `GetType()` for type checks.
5. For value types, consider `IEquatable<T>` for performance (avoids boxing).

---

### **Related Topics**

- [[Reference and Value Types]]
- [[Records]] — auto-generate correct Equals/GetHashCode
- [[Boxing and Unboxing]]
- [[Reflection]]