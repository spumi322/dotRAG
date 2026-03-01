### _Interface Segregation Principle (ISP): No class should be forced to implement methods it doesn't use._

Prefer many small, focused interfaces over one large "god" interface.

### _Violation — fat interface_

```csharp
// ❌ Forces every worker to implement all three methods
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() => Console.WriteLine("Working...");
    public void Eat() => throw new NotSupportedException();  // 💥 Robots don't eat
    public void Sleep() => throw new NotSupportedException(); // 💥 Robots don't sleep
}
```

### _Fixed — segregated interfaces_

```csharp
// ✅ Each interface represents one capability
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }
public interface ISleepable { void Sleep(); }

public class HumanWorker : IWorkable, IFeedable, ISleepable
{
    public void Work() => Console.WriteLine("Working...");
    public void Eat() => Console.WriteLine("Eating...");
    public void Sleep() => Console.WriteLine("Sleeping...");
}

public class Robot : IWorkable
{
    public void Work() => Console.WriteLine("Working..."); // only implements what it needs
}
```

### _Practical ASP.NET Core example_

```csharp
// ❌ Fat service interface
public interface IUserService
{
    User GetById(int id);
    List<User> GetAll();
    void Register(UserDto dto);
    void SendWelcomeEmail(User user);
    void ResetPassword(string email);
    Report GenerateUserReport();
}

// ✅ Split by responsibility
public interface IUserQueryService
{
    User GetById(int id);
    List<User> GetAll();
}

public interface IUserRegistrationService
{
    void Register(UserDto dto);
}

public interface IUserReportService
{
    Report GenerateUserReport();
}
```

Now a controller that only reads users depends on `IUserQueryService` — it doesn't pull in email or report logic.

### _ISP signals_

- Classes implementing an interface with empty methods or `NotImplementedException`.
- Interfaces with 5+ methods that serve different purposes.
- Changing one method forces recompilation/redeployment of unrelated consumers.

[[SOLID]] 
[[Liskov Substitution Principle]]
[[Single Responsibility Principle]]