_Single Responsibility Principle (SRP):_ A class should have one, and only one, reason to change. This means a class should have only one job.
  
To achieve the Single Responsibility Principle:

- Use **classes and methods** that encapsulate a single functionality.
- Define **interfaces** to represent similar behaviors.
- Implement **dependency injection** to manage class dependencies.
- Prefer **composition over inheritance** to assemble behaviors.
- Organize code into **modules or namespaces** for clear separation.
- Apply **design patterns** like Repository and Factory for specific tasks.
- Use **delegates** and **events** to handle behavior extension and communication.

```csharp
// ❌ Violation — 4 reasons to change in one class:
// 1. scoring rules change, 2. standings formula changes,
// 3. notification format changes, 4. DB schema changes
public class MatchService(AppDbContext ctx)
{
    public async Task ProcessResult(int matchId, int homeScore, int awayScore)
    {
        var match = await ctx.Matches.FindAsync(matchId);
        match.HomeScore = homeScore;
        match.AwayScore = awayScore;

        // standings logic — belongs in StandingService
        var standing = await ctx.Standings.FindAsync(match.TournamentId);
        standing.Points += homeScore > awayScore ? 3 : 1;

        // email notification — belongs in NotificationService
        var smtp = new SmtpClient("smtp.example.com");
        await smtp.SendMailAsync("admin@app.com", "Match done", $"{homeScore}-{awayScore}");

        await ctx.SaveChangesAsync();
    }
}

// ✅ SRP — one reason to change per class
public class MatchService(IStandingService standings, INotificationService notify, IMatchRepository repo)
{
    public async Task ProcessResult(int matchId, int homeScore, int awayScore)
    {
        var match = await repo.GetByIdAsync(matchId);
        match.SetScore(homeScore, awayScore);
        await repo.SaveAsync();

        await standings.UpdateAsync(match);   // standing rules can change independently
        await notify.MatchFinishedAsync(match); // notification format can change independently
    }
}
```
