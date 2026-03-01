### *Basic concepts and architecture of ASP.NET Core, including the request processing model and request processing pipeline.*

ASP.NET Core is a cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications. It's a complete redesign of the legacy ASP.NET Framework.

1. _Cross-Platform_: Runs on Windows, macOS, and Linux.
2. _Modularity_: Built on NuGet packages, third-party libraries management.
3. _Dependency Injection_: Built-in DI container (`IServiceProvider`), first-class citizen.
4. _Configuration System_: `appsettings.json`, environment variables, `IOptions<T>` pattern.
5. _Middleware_: Request processing pipeline customization.
6. _Hosting_: **Kestrel** is the default cross-platform web server. IIS/Nginx/Apache serve as reverse proxies in production. Docker is a common deployment target.
7. _Razor Pages, MVC, and Minimal APIs_: Three approaches to handling HTTP requests.

**Architecture (.NET 6+ Minimal Hosting Model)**

Since .NET 6, ASP.NET Core uses a single `Program.cs` file with top-level statements. The legacy `Startup.cs` with `ConfigureServices()` / `Configure()` is no longer required.

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register services (replaces ConfigureServices)
builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("..."));
builder.Services.AddProblemDetails(); // standardized error responses

var app = builder.Build();

// 2. Configure middleware pipeline (replaces Configure)
app.UseExceptionHandler();    // global error handling
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 3. Map endpoints
app.MapControllers();          // controller-based APIs
app.MapGet("/ping", () => "pong"); // minimal API endpoint

app.Run();
```

> **Legacy note:** Before .NET 6, the same logic was split across `Startup.cs` (`ConfigureServices` + `Configure`) and `Program.cs` (`CreateHostBuilder`). You may see this in older codebases.

---

### *Minimal APIs vs Controllers*

**.NET 6+** introduced **Minimal APIs** — a lightweight way to define endpoints without controllers. Interviewers expect you to know both approaches.

```csharp
// Minimal API — good for small services, microservices, simple CRUD
app.MapGet("/api/products", async (AppDbContext db) => 
    await db.Products.ToListAsync());

app.MapPost("/api/products", async (Product p, AppDbContext db) => {
    db.Products.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{p.Id}", p);
});
```

| Aspect | Controllers | Minimal APIs |
|---|---|---|
| Best for | Large apps, complex logic | Small services, prototypes |
| Filters | ✅ Full support | ✅ Endpoint filters (.NET 7+) |
| Model binding | ✅ `[FromBody]`, `[FromQuery]` | ✅ Same, auto-inferred |
| Grouping | Controller classes | `MapGroup()` (.NET 7+) |
| Swagger | ✅ Built-in | ✅ Built-in |

---

### *Usage of Middleware for handling requests and responses in ASP.NET Core.*

Middleware components form a pipeline. Each component can process the request, optionally pass it to the next, and process the response on the way back. **Order matters.**

1. _HTTP Request Handling_: Request enters the pipeline from Kestrel.
2. _Middleware Pipeline_: Each component decides to pass to next (`await next(context)`) or short-circuit.
3. _Endpoint Routing_: `UseRouting()` + `UseEndpoints()` (implicit in .NET 6+) identifies the target endpoint.
4. _Response Generation_: The endpoint generates a response, which travels back through the pipeline.

**Typical middleware order:**
```
Exception Handling → HTTPS Redirect → Static Files → Routing 
→ CORS → Authentication → Authorization → Endpoint Execution
```

Custom middleware:
```csharp
app.Use(async (context, next) =>
{
    // Before the next middleware
    var sw = Stopwatch.StartNew();
    await next(context);
    // After — response is on its way back
    sw.Stop();
    context.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
});
```

---
### `app.Use()` vs `app.Run()` vs `app.Map()`

|Method|Passes to next?|Use for|
|---|---|---|
|`app.Use()`|✅ Yes (calls `next`)|Most middleware — inspect/modify and continue|
|`app.Run()`|❌ No (terminal)|End of pipeline — always generates a response|
|`app.Map()`|Branches pipeline|Separate pipeline for specific path prefix|

```csharp
// Use — passes to next middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("Before");
    await next(context);
    Console.WriteLine("After");
});

// Run — terminal, nothing after this runs
app.Run(async context =>
{
    await context.Response.WriteAsync("Final response");
});

// Map — branches for /health path only
app.Map("/health", branch =>
{
    branch.Run(async ctx => await ctx.Response.WriteAsync("Healthy"));
});
```

> **Interview trap:** If you call `app.Run()` before other middleware, everything after it is **dead code** — it never executes.
### *Routing in ASP.NET Core, route parameters, their configuration, and usage.*

Routing maps incoming HTTP requests to endpoints based on URL and HTTP method.
**Attribute routing** (controllers):
```csharp
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet("{id:int}")]           // GET api/products/5
    public IActionResult Get(int id) => Ok(...);
    
    [HttpGet("search/{name:alpha}")] // GET api/products/search/widget
    public IActionResult Search(string name) => Ok(...);
}

// Without [ApiController] — manual validation required
[HttpPost]
public IActionResult Create(CreateTournamentRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // you must do this yourself
    // ...
}
```

**Route constraints:** `:int`, `:guid`, `:alpha`, `:minlength(5)`, `:range(1,100)`.

---

### *Dependency injection principles in ASP.NET Core.*

Register services in `builder.Services`, inject via constructor.

```csharp
public static IConfiguration configuration { get; private set; }

public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Register services
    builder.Services.AddScoped<IGameService, GameService>();
    builder.Services.AddEndpointsApiExplorer();

    // Other service configurations from Infrastructure
    builder.Services.ConfigurePersistenceServices();
    builder.Services.ConfigureIdentityServices(builder.Configuration);

    var app = builder.Build();

    // Middleware pipeline
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    // Begin listening for incoming HTTP calls, sets the app in motion
    app.Run();
}
```

### *Service lifetime management: Scoped, Transient, and Singleton*

1. **Transient:** New instance every time it's requested. Use for: lightweight, stateless services.
2. **Scoped:** One instance per HTTP request. Use for: `DbContext`, unit-of-work.
3. **Singleton:** One instance for the entire app lifetime. Use for: caches, configuration wrappers, `HttpClient` factories.

⚠️ **Captive dependency problem:** Never inject a Scoped service into a Singleton — the scoped instance gets "captured" and lives forever, causing bugs. The DI container throws at runtime in Development if `ValidateScopes` is enabled (default).

---

### *IOptions pattern for configuration binding*

The standard way to bind `appsettings.json` sections to strongly-typed classes:

```csharp
// appsettings.json: { "Smtp": { "Host": "mail.example.com", "Port": 587 } }
public class SmtpSettings { public string Host { get; set; } public int Port { get; set; } }

// Registration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Injection
public class EmailService(IOptions<SmtpSettings> options)
{
    private readonly SmtpSettings _smtp = options.Value;
}
```

| Interface | Behavior |
|---|---|
| `IOptions<T>` | Read once at startup, singleton, no reload |
| `IOptionsSnapshot<T>` | Scoped, re-reads per request |
| `IOptionsMonitor<T>` | Singleton, notifies on changes via `OnChange()` |

---

### *Mechanism of authentication and authorization in ASP.NET Core.*

1. _Authentication_: Verifies **who** the user is. Common schemes: **JWT Bearer** (dominant for APIs), Cookie-based (MVC), OAuth/OpenID Connect (external providers).

```csharp
// JWT setup (common interview question)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = "...",
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });
```

**JWT validation steps (what `UseAuthentication()` does per request):**

1. Extracts token from `Authorization: Bearer <token>` header
2. Validates signature (using `IssuerSigningKey`)
3. Validates issuer (`ValidIssuer`)
4. Validates audience (`ValidAudience`)
5. Validates expiry (`ValidateLifetime`)
6. If all pass → builds `ClaimsPrincipal`, sets it on `HttpContext.User`
7. _Authorization_: Determines **what** the user can do. Managed via roles, policies, or claims.

**Policy-based authorization** (preferred over role-based):
```csharp
builder.Services.AddAuthorization(o =>
    o.AddPolicy("AdminOnly", p => p.RequireClaim("role", "admin")));

[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public IActionResult Delete(int id) => ...;
```

---

### *MVC pattern and its role in web application development.*

Model (data) → View (UI, Razor `.cshtml`) → Controller (intermediary). Server-side rendering.
MVC returns HTML views via Razor, Web API returns data (JSON). Controllers inherit from `Controller` (MVC) or `ControllerBase` (Web API — no view support).

### *Serialization and the ACCEPT header.*

Serialization converts objects to JSON/XML for transmission. The `Accept` header tells the server the preferred format. `System.Text.Json` is the default serializer in ASP.NET Core (replacing `Newtonsoft.Json`).

---

### *Error handling in ASP.NET Core Web API.*

1. _Global exception handler middleware_: `app.UseExceptionHandler()` — catches all unhandled exceptions.
2. _ProblemDetails_: Standardized error response format (RFC 9457). First-class in .NET 7+ via `builder.Services.AddProblemDetails()`.

```csharp
// Automatic ProblemDetails for all error responses
builder.Services.AddProblemDetails();
app.UseExceptionHandler();      // returns ProblemDetails JSON on unhandled exceptions
app.UseStatusCodePages();       // returns ProblemDetails for empty error responses (404, etc.)
```

3. _Exception Filters_: `IExceptionFilter` / `ExceptionFilterAttribute` for controller-scoped handling.

**MVC Filter pipeline execution order:** `Authorization → Resource → Action → Result → Exception`
### Common `IActionResult` return types

|Method|Status Code|When to use|
|---|---|---|
|`Ok(data)`|200|Successful GET / general success|
|`Created(uri, data)`|201|Resource created (POST)|
|`NoContent()`|204|Success, no body (DELETE / PUT)|
|`BadRequest(...)`|400|Validation failure|
|`Unauthorized()`|401|Not authenticated|
|`Forbid()`|403|Authenticated but not authorized|
|`NotFound()`|404|Resource doesn't exist|
|`Conflict()`|409|State conflict|

---
### *Pagination*
Never return unbounded collections. Use query params:
GET /api/tournaments?page=1&pageSize=20

Response should include metadata:
{
  "data": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 84,
  "hasNextPage": true
}

Interview answer: "Offset-based pagination is simplest and sufficient for most junior-level APIs."


[[Exercise/Configuration|Configuration]]
