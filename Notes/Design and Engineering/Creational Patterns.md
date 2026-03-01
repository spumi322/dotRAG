1. **Singleton**: Ensures only one instance of a class is created and provides a global point of access to it. 
   Modern way: Just pass the DI as AddSingleton<ITEntitiy, TEntity>() it will manage the single instance.
   Use Case: Managing a Database Connection Pool
```csharp
// Thread-safe Singleton (recommended for interviews)
public sealed class Singleton
{
    private static readonly Lazy<Singleton> _instance = 
        new(() => new Singleton());
    private Singleton() { }
    public static Singleton Instance => _instance.Value;
}
```

2. Factory: Creates objects without specifying the exact class of object that will be created. 
   Use Case: Creating Different Types of Responses to an API call.
 ```csharp
// ============================================================
// FACTORY PATTERN
// Problem: you need different notification senders at runtime
//          but the caller shouldn't know which one it gets.
// ============================================================

public interface INotificationSender
{
    void Send(string recipient, string message);
}

public class EmailSender : INotificationSender
{
    public void Send(string recipient, string message)
        => Console.WriteLine($"Email → {recipient}: {message}");
}

public class SmsSender : INotificationSender
{
    public void Send(string recipient, string message)
        => Console.WriteLine($"SMS → {recipient}: {message}");
}

public class WebhookSender : INotificationSender
{
    public void Send(string recipient, string message)
        => Console.WriteLine($"POST {recipient} — {message}");
}

// The factory — caller only knows about the interface
public static class NotificationSenderFactory
{
    public static INotificationSender Create(string type) => type switch
    {
        "email"   => new EmailSender(),
        "sms"     => new SmsSender(),
        "webhook" => new WebhookSender(),
        _ => throw new ArgumentException($"Unknown sender type: {type}")
    };
}

// Usage — TournamentService doesn't know or care which sender it gets
var sender = NotificationSenderFactory.Create("email");
sender.Send("player@example.com", "Your match starts in 10 minutes.");
 ```
3. Builder (Awareness): Constructs a complex object step by step via a fluent chain. Same type every time — solves "too many constructor parameters" not "which type to create" (that's Factory). 
   Use Case: 
   WebApplication.CreateBuilder(args).Services.AddControllers()  ← same fluent chain concept
 ```csharp

public class Tournament
{
    public string Name        { get; init; }
    public int    MaxTeams    { get; init; }
    public string Format      { get; init; }   // "RoundRobin", "SingleElim"
    public bool   IsPublic    { get; init; }
    public int    BestOf      { get; init; }
    public string TenantId    { get; init; }

    // Private — only the builder can construct this
    private Tournament() { }

    public class Builder
    {
        private readonly Tournament _t = new();

        // Each method sets one property and returns the builder (fluent chain)
        public Builder WithName(string name)         { _t.Name = name;          return this; }
        public Builder WithMaxTeams(int max)         { _t.MaxTeams = max;       return this; }
        public Builder WithFormat(string format)     { _t.Format = format;      return this; }
        public Builder IsPublicTournament(bool pub)  { _t.IsPublic = pub;       return this; }
        public Builder WithBestOf(int bestOf)        { _t.BestOf = bestOf;      return this; }
        public Builder ForTenant(string tenantId)    { _t.TenantId = tenantId;  return this; }

        public Tournament Build()
        {
            if (string.IsNullOrEmpty(_t.Name))    throw new InvalidOperationException("Name required.");
            if (_t.MaxTeams < 2)                  throw new InvalidOperationException("Need at least 2 teams.");
            return _t;
        }
    }
}

// Usage — readable, order doesn't matter, only set what you need
var tournament = new Tournament.Builder()
    .WithName("Spring Open 2026")
    .WithMaxTeams(16)
    .WithFormat("SingleElim")
    .WithBestOf(3)
    .ForTenant("tenant-abc")
    .IsPublicTournament(true)
    .Build();
 ```
## Honorable mentions

 Prototype: Creates new objects by copying an existing object, known as the prototype.
   Use Case: Implementing a Templating System

  Abstract Factory: Provides an interface for creating families of related or dependent objects without specifying their concrete classes.
	Use Case: Different Loggers
