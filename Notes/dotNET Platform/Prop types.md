Types of properties (read-write, read-only, write-only, and init).
```csharp
public class Person
{
	// Read and Write
	public int Id { get; set; }
	// Read only, value can be set via constructor or init
	public string Adress { get; }
	// Init property, value to be set at object initialization
	public string Name { get; init; }
}
```
### *`required` modifier (C# 11+)*

Forces callers to set a property during object initialization. Compile-time error if missing.

```csharp
public class CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; } // optional
}

// ✅ Compiles
var req = new CreateUserRequest { Email = "a@b.com", Password = "secret" };

// ❌ CS9035 compile error — Email and Password are required
var bad = new CreateUserRequest { DisplayName = "John" };
```

Use `required` on DTOs and request models to enforce mandatory fields without constructor boilerplate. Commonly paired with `init` properties.