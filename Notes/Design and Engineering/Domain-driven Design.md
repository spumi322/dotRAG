DDD focuses on aligning software design closely with the business domain. Its main goal is to simplify complex software systems by modeling based on the real-world business. **Entities** in DDD are objects with a unique identity and life cycle. **Value Objects** are simple objects that describe some characteristic but don’t have a unique identity. **Aggregate Roots** used to group together a cluster of associated objects and treat them as a single unit. **Bounded Contexts** define distinct areas of the business domain with specific responsibilities and language. DDD is particularly useful when dealing with complex business logic and large software systems, where understanding the business domain is crucial for successful development.

### *Architectures based on DDD (Hexagonal, Onion, Clean), their features, and differences.*

The main difference is in their focus: Hexagonal deals with external interactions, Onion prioritizes domain-centric layering, and Clean emphasizes strict structure and dependency management.

1. Hexagonal Architecture (Ports and Adapters):

This architecture aims to make the application highly maintainable and adaptable to changes in external systems or databases by decoupling the core logic from external concerns. It's particularly useful when you expect the external interfaces of your application (like the UI, data sources, or third-party services) to change frequently or when you want to support multiple different external interfaces simultaneously. 
e-commerce engine, which handles orders, but the UI, the data storage, the payment providers can be different.

2. Onion Architecture:

The goal here is to maintain the integrity of the domain model and business logic by keeping them at the core of the application, unaffected by external changes such as database or UI framework shifts. It's beneficial in scenarios where the domain logic is complex and needs to evolve independently of the infrastructure or external frameworks.


3. Clean Architecture:

Focuses on creating systems that are independent of UIs, databases, frameworks, and external agencies, making them easy to test, maintain, and adapt to new requirements. This architecture is ideal when building scalable applications that need to withstand significant changes in technology (e.g., changing databases or front-end technologies) or when developing systems with a long lifespan that will require regular updates and modifications.
### _DDD building blocks in C# code_

**Entity** — has a unique identity, equality based on Id:

```csharp
public class Order
{
    public int Id { get; private set; }               // unique identity
    public DateTime CreatedAt { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
        _items.Add(new OrderItem(product.Id, product.Price, qty));
    }

    public decimal Total => _items.Sum(i => i.LineTotal);
}
```

**Value Object** — no identity, equality based on properties. Use `record`:

```csharp
public record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies.");
        return this with { Amount = Amount + other.Amount };
    }
}

public record Address(string Street, string City, string Zip, string Country);

// Value equality:
var a = new Money(10, "USD");
var b = new Money(10, "USD");
a == b // true — records compare by value
```

**Aggregate Root** — entry point to a cluster of related objects. External code only interacts with the root, never with inner entities directly:

```csharp
// Order is the Aggregate Root
// OrderItem is an entity inside the aggregate — no direct access from outside
public class Order // ← aggregate root
{
    // ... (see above)
    
    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("Item not found.");
        _items.Remove(item);
    }
}

// ❌ Wrong — accessing inner entity directly
// orderItem.Quantity = 5;

// ✅ Correct — go through the aggregate root
// order.UpdateItemQuantity(productId, 5);
```

**Bounded Context** — each area of the business has its own model. The same real-world concept may look different in different contexts:

```
// Sales context: Customer has Orders, PaymentMethod, ShippingAddress
// Support context: Customer has Tickets, SatisfactionScore
// These are separate models — not one shared "Customer" class
```

### _When DDD is overkill_

Simple CRUD apps (e.g., admin panels, basic APIs with no business rules) don't benefit from DDD. DDD adds value when business logic is complex and evolves frequently.

[[Design Patterns]] 
[[Records]]
[[OOP]]