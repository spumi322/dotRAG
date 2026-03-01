# Reflection

Reflection lets you inspect and interact with types at runtime — read class names, list methods and properties, create instances, invoke methods — without knowing the type at compile time.

Used by frameworks under the hood: ASP.NET Core model binding, EF Core, AutoMapper, xUnit — all use reflection. You rarely write it directly.

---

## Getting type info

```csharp
// From a type name
Type type = typeof(Tournament);

// From an instance
var t = new Tournament();
Type type = t.GetType();
```

## Inspecting members

```csharp
Type type = typeof(Tournament);

// Get all public properties
var props = type.GetProperties();
foreach (var p in props)
    Console.WriteLine($"{p.Name}: {p.PropertyType}");

// Get all public methods
var methods = type.GetMethods();
foreach (var m in methods)
    Console.WriteLine(m.Name);
```

## Invoking a method dynamically

```csharp
var obj = new Tournament();
Type type = typeof(Tournament);

MethodInfo method = type.GetMethod("Cancel");
method.Invoke(obj, null); // calls obj.Cancel()
```

---

## Attributes

Attributes are metadata you attach to classes, methods, or properties. Reflection is how you read them at runtime — which is exactly what frameworks like ASP.NET Core do.

```csharp
// Built-in attribute
[Required]
[MaxLength(100)]
public string Name { get; set; }
```

**Custom attribute:**

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class AuditableAttribute : Attribute
{
    public string CreatedBy { get; set; }
}

[Auditable(CreatedBy = "system")]
public class Tournament { }

// Read it via reflection
var attr = typeof(Tournament)
    .GetCustomAttribute<AuditableAttribute>();
Console.WriteLine(attr?.CreatedBy); // "system"
```

---

## When you'd actually use it

- Writing a framework or library (not common at junior level)
- Generic serializers / mappers
- Plugin systems where types are loaded dynamically

In day-to-day web dev: you use libraries that use reflection internally. You don't write reflection code directly.

---

### Related

[[Assemblies]] [[Dynamic Type & Object Type