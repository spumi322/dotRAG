### Key class and member keywords in C# 

### *`static` — belongs to the type, not an instance*

```csharp
public class MathHelper
{
    public static double Pi = 3.14159;              // shared across all code
    public static int Add(int a, int b) => a + b;  // called via MathHelper.Add()
}

// Static classes cannot be instantiated or inherited
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
}
```

No instance needed. Static members are shared, live for the app lifetime. Static classes can only contain static members.

### *`abstract` — must be overridden, no implementation (unless method body provided)*

```csharp
public abstract class Shape
{
    public abstract double Area();        // no body — derived classes MUST implement
    public virtual string Color => "Red"; // has body — derived classes CAN override
}

public class Circle : Shape
{
    private double _r;
    public Circle(double r) => _r = r;
    public override double Area() => Math.PI * _r * _r; // required
}

// var s = new Shape(); // ❌ Cannot instantiate abstract class
```

### *`virtual` + `override` — runtime polymorphism*

`virtual` = "this method can be overridden." `override` = "I'm replacing the base implementation." The correct method is resolved **at runtime** based on the actual object type.

```csharp
public class Animal
{
    public virtual string Speak() => "...";
}

public class Dog : Animal
{
    public override string Speak() => "Bark";
}

Animal a = new Dog();
a.Speak(); // "Bark" — runtime dispatch to Dog.Speak()
```

### *`new` (member hiding) — compile-time, NOT polymorphic*

`new` hides the base member instead of overriding it. The method called depends on the **variable type**, not the object type. Almost always a design smell.

```csharp
class Base
{
    public string GetName() => "Base";        // NOT virtual
}

class Derived : Base
{
    public new string GetName() => "Derived"; // hides Base.GetName()
}

Base obj = new Derived();
obj.GetName();    // "Base"  ← variable type wins

Derived obj2 = new Derived();
obj2.GetName();   // "Derived"
```

### *`override` vs `new` — the #1 interview trap*

`override` = runtime decides which method runs. `new` = compile-time decides based on variable type.

### *`sealed` — prevent inheritance or further overriding*

```csharp
public sealed class Singleton { } // cannot be inherited

public class Base
{
    public virtual void Execute() { }
}

public class Middle : Base
{
    public sealed override void Execute() { } // subclasses of Middle cannot override
}
```

`string` is a sealed class — you cannot inherit from it. Sealing can also improve performance (the JIT can devirtualize calls).

### *Summary table*

| Keyword | Applies to | Meaning |
|---|---|---|
| `static` | Class, member | Belongs to type, not instance |
| `abstract` | Class, method | Must be inherited/overridden |
| `virtual` | Method, property | Can be overridden in derived class |
| `override` | Method, property | Replaces virtual/abstract base member |
| `new` | Member | Hides base member (non-polymorphic) |
| `sealed` | Class, override method | Prevents further inheritance/overriding |

[[OOP]]
[[Inheritance]]
[[Polymorphism]]
[[Access Modifiers]]
