### _Open/Closed Principle (OCP): Open for extension, closed for modification._

You should be able to add new behavior **without changing existing code**. Achieved through abstractions (interfaces, abstract classes).

### _Violation_

```csharp
// ❌ Every new discount type requires modifying this method
public class DiscountService
{
    public decimal Calculate(Order order, string type) => type switch
    {
        "seasonal" => order.Total * 0.10m,
        "loyalty" => order.Total * 0.15m,
        "employee" => order.Total * 0.25m,
        // Adding "student" means editing this class
        _ => 0m
    };
}
```

### _Fixed with OCP_

```csharp
// ✅ New discount = new class. No existing code changes.
public interface IDiscountStrategy
{
    bool Applies(Order order);
    decimal Calculate(Order order);
}

public class SeasonalDiscount : IDiscountStrategy
{
    public bool Applies(Order order) => order.Date.Month == 12;
    public decimal Calculate(Order order) => order.Total * 0.10m;
}

public class LoyaltyDiscount : IDiscountStrategy
{
    public bool Applies(Order order) => order.Customer.IsLoyal;
    public decimal Calculate(Order order) => order.Total * 0.15m;
}

// Service is closed for modification — just inject new strategies
public class DiscountService(IEnumerable<IDiscountStrategy> strategies)
{
    public decimal Calculate(Order order)
        => strategies.Where(s => s.Applies(order)).Sum(s => s.Calculate(order));
}
```

Adding a `StudentDiscount` only requires creating a new class and registering it in DI — `DiscountService` is never touched.

[[SOLID]]
[[Dependency Inversion Principle]]