# 8 — Testing

## Testing pyramid

|Level|Scope|Speed|Tools|
|---|---|---|---|
|**Unit**|Single class in isolation, all deps mocked|Very fast|xUnit, Moq|
|**Integration**|Multiple real components + real HTTP pipeline|Moderate|WebApplicationFactory|
|**E2E**|Full system through UI/browser|Slow|Playwright, Selenium|

Write many unit tests, fewer integration tests, very few E2E. Expensive tests slow CI and are fragile.

```
Unit test:    [Service] ← mock repo, mock logger
Integration:  HTTP client → [Controller → Service → DbContext → Test DB]
```

---

## AAA Pattern — Arrange / Act / Assert

The universal structure for readable tests. One `Act` per test — if you're asserting two unrelated things, write two tests.

```csharp
[Fact]
public async Task RegisterTeam_ValidRequest_ReturnsCreatedTeam()
{
    // Arrange — set up inputs and dependencies
    var mockRepo = new Mock<ITeamRepository>();
    var service  = new TeamService(mockRepo.Object);
    var request  = new RegisterTeamRequest("Alpha FC", "Budapest");

    mockRepo
        .Setup(r => r.AddAsync(It.IsAny<Team>(), default))
        .ReturnsAsync(new Team { Id = 1, Name = "Alpha FC" });

    // Act — call the thing under test
    var result = await service.RegisterAsync(request);

    // Assert — verify the outcome
    Assert.Equal("Alpha FC", result.Name);
    Assert.Equal(1, result.Id);
}
```

---

## Test naming

**Pattern:** `MethodName_Scenario_ExpectedResult`

```
GetTournament_Exists_ReturnsTournament
GetTournament_NotFound_ThrowsNotFoundException
RegisterTeam_DuplicateName_ReturnsBadRequest
SaveChanges_DbThrows_RollsBackTransaction
```

Self-documenting — tells you what broke without reading the body.

---

## xUnit

```csharp
// [Fact] — single test case, no parameters
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
    var result = new Calculator().Add(2, 3);
    Assert.Equal(5, result);
}

// [Theory] + [InlineData] — same logic, multiple input sets
// each [InlineData] row becomes a separate test case
[Theory]
[InlineData(2,  3,  5)]
[InlineData(0,  0,  0)]
[InlineData(-1, 1,  0)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    Assert.Equal(expected, new Calculator().Add(a, b));
}
```

### Common assertions

```csharp
Assert.Equal(expected, actual);
Assert.NotNull(value);
Assert.True(condition);
Assert.Empty(collection);
Assert.Contains(item, collection);
Assert.IsType<TournamentDto>(result);
await Assert.ThrowsAsync<NotFoundException>(() => sut.GetAsync(999));
```

---

## Moq

Moq creates fake implementations of interfaces to isolate the class under test.

### Setup — define what the mock returns

```csharp
var mockRepo = new Mock<ITournamentRepository>();

// Return a value
mockRepo
    .Setup(r => r.GetByIdAsync(1, default))
    .ReturnsAsync(new Tournament { Id = 1, Name = "Spring Cup" });

// Match any argument
mockRepo
    .Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new Tournament { Id = 99 });

// Throw an exception
mockRepo
    .Setup(r => r.GetByIdAsync(999, default))
    .ThrowsAsync(new NotFoundException("Not found"));
```

### Verify — assert the mock was called as expected

```csharp
// Called exactly once
mockRepo.Verify(r => r.AddAsync(It.IsAny<Tournament>(), default), Times.Once);

// Never called
mockRepo.Verify(r => r.DeleteAsync(It.IsAny<long>(), default), Times.Never);
```

### Full service test

```csharp
public class TournamentServiceTests
{
    private readonly Mock<ITournamentRepository> _mockRepo;
    private readonly TournamentService _sut;

    public TournamentServiceTests()
    {
        _mockRepo = new Mock<ITournamentRepository>();
        _sut      = new TournamentService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAsync_Exists_ReturnsTournament()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(new Tournament { Id = 1, Name = "Spring Cup" });

        // Act
        var result = await _sut.GetAsync(1);

        // Assert
        Assert.Equal("Spring Cup", result.Name);
        _mockRepo.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Tournament?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAsync(999));
    }
}
```

---

## Integration Testing — WebApplicationFactory

`WebApplicationFactory<T>` spins up your full ASP.NET Core app in-process — real DI container, real middleware, real routing. No network port opened; uses an in-memory HTTP transport.

Catches bugs unit tests miss: wrong routing, missing DI registrations, incorrect middleware order, bad EF query translation.

```csharp
// NuGet: Microsoft.AspNetCore.Mvc.Testing
public class TournamentsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TournamentsApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task GetTournament_Exists_Returns200()
    {
        var response = await _client.GetAsync("/api/tournaments/1");

        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<TournamentDto>();
        Assert.Equal("Spring Cup", dto!.Name);
    }
}
```

### Swapping services for tests

```csharp
var factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder =>
    {
        builder.ConfigureServices(services =>
        {
            // Remove real DbContext, replace with test DB
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<AppDbContext>(o =>
                o.UseSqlite("DataSource=:memory:"));

            // Replace external dependency with a fake
            services.AddSingleton<IEmailService, FakeEmailService>();
        });
    });
```

---

## Test database strategy

|Approach|Speed|Fidelity|Use for|
|---|---|---|---|
|**EF In-Memory provider**|✅ Fastest|❌ No SQL, no constraints|Quick smoke tests only|
|**SQLite in-memory**|✅ Fast|✅ Real SQL, FK constraints|Most integration tests|
|**TestContainers (real SQL Server)**|⚠️ Slower|✅ Production-equivalent|Critical query paths|

Avoid EF In-Memory for anything meaningful — it doesn't enforce FK constraints and translates some queries differently from SQL Server. SQLite is the practical default for integration tests.

---

## Interview Traps

- **"Unit vs Integration test — what does each catch?"** — Unit tests catch logic bugs in one class. Integration tests catch wiring bugs: wrong routing, missing DI registration, middleware order, EF query translation errors.
- **"What is AAA?"** — Arrange (set up), Act (invoke), Assert (verify). One Act per test keeps failures clear and tests focused.
- **"Why mock dependencies?"** — To isolate the class under test. Tests shouldn't need a real database, HTTP client, or clock — mocks let you control exact behaviour and assert specific interactions.
- **"`[Fact]` vs `[Theory]`?"** — `[Fact]` is one test case. `[Theory]` is parameterized — runs the same logic with each `[InlineData]` row as a separate test.
- **"Moq `Setup` vs `Verify`?"** — `Setup` defines what the mock returns when called. `Verify` asserts it was actually called in the expected way. You need both: setup to control the scenario, verify to assert the interaction.
- **"What is `WebApplicationFactory`?"** — Starts your full ASP.NET Core app in-process for tests without a real network port. Gives you a real HTTP client backed by in-memory transport.
- **"In-memory DB vs SQLite for tests?"** — EF In-Memory is fast but skips constraints and SQL translation. SQLite is a better default — it runs real SQL and enforces FKs while staying fast.

---

### Related

[[6.1 Architecture & Middleware]] [[6.3 Dependency Injection]] [[7.4 EF Core]]