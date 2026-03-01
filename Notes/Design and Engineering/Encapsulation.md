### *What Encapsulation is and the access levels existing in the .NET platform: private, protected, public, internal, and their combinations.*

Encapsulation is about keeping data and the methods that work with that **data together** in a class, while controlling **who can access** them. Access levels help with this: '**private**' means only the class itself can access, '**protected**' means the class and its subclasses can access, '**public**' means anyone can access, and '**internal**' means access is limited to the same project or assembly. Combinations like 'protected internal' offer more specific access control by combining these rules.

```csharp
public class Tournament
{
    // State is private — nobody touches it directly
    private int _maxTeams;
    private readonly List<TournamentTeam> _teams = new();

    // Controlled access — validation lives here, not scattered across callers
    public int MaxTeams
    {
        get => _maxTeams;
        private set
        {
            if (value < 2) throw new ArgumentException("Need at least 2 teams.");
            _maxTeams = value;
        }
    }

    // Expose read-only view — callers can iterate, not mutate
    public IReadOnlyList<TournamentTeam> Teams => _teams;

    public void AddTeam(TournamentTeam team)
    {
        if (_teams.Count >= _maxTeams)
            throw new InvalidOperationException("Tournament is full.");
        _teams.Add(team);  // the only valid way to add a team
    }
}
```