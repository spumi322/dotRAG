using dotRAG.API.Application;
using dotRAG.API.Infrastructure.Embeddings;
using dotRAG.API.Infrastructure.Health;
using dotRAG.API.Infrastructure.LLM;
using dotRAG.API.Infrastructure.RAG;
using dotRAG.API.Middleware;
using dotRAG.API.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting dotRAG.API");

    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, svc, cfg) => cfg
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(svc)
        .WriteTo.Console()
        .Enrich.FromLogContext());  // required for CorrelationId enrichment

    // ── OpenAPI ───────────────────────────────────────────────────────────────
    builder.Services.AddOpenApi();

    // ── Exception handling ────────────────────────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ── HTTP client ───────────────────────────────────────────────────────────
    builder.Services.AddHttpClient();

    // ── HttpContext access for correlation-id propagation into singletons ─────
    builder.Services.AddHttpContextAccessor();

    // ── Health checks ─────────────────────────────────────────────────────────
    builder.Services.AddSingleton<IngestionHealthCheck>();
    builder.Services.AddHealthChecks().AddCheck<IngestionHealthCheck>("ingestion");

    // ── CORS ──────────────────────────────────────────────────────────────────
    builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Middleware ────────────────────────────────────────────────────────────
    builder.Services.AddSingleton<CorrelationIdMiddleware>();

    // ── Infrastructure (all Singleton) ────────────────────────────────────────
    builder.Services.AddSingleton<InMemoryVectorStore>();
    builder.Services.AddSingleton<MarkdownChunker>();
    builder.Services.AddSingleton<EmbeddingCache>();
    builder.Services.AddSingleton<IEmbeddingService, VoyageEmbeddingService>();
    // builder.Services.AddSingleton<ILlmService, AnthropicLlmService>();
    builder.Services.AddSingleton<ILlmService, OpenRouterLlmService>();
    builder.Services.AddSingleton<INotesSearchService, NotesSearchService>();

    // Double-register so IngestionHealthCheck and endpoint get the same IsReady instance.
    builder.Services.AddSingleton<NotesIngestionService>();
    builder.Services.AddHostedService(sp => sp.GetRequiredService<NotesIngestionService>());

    // ── Application ───────────────────────────────────────────────────────────
    builder.Services.AddSingleton<IQueryRewriter, QueryRewriter>();
    builder.Services.AddSingleton<IPromptBuilder, PromptBuilder>();
    builder.Services.AddSingleton<PipelineTraceStore>();
    builder.Services.AddSingleton<IChatService, ChatService>();

    var app = builder.Build();

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseExceptionHandler();
    app.UseCors();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(opts =>
    {
        // Debug endpoints are polled every few hundred ms by the Debug screen —
        // logging each request at Info level drowns the console. Drop them to
        // Verbose so they're filtered out by the default minimum level.
        opts.GetLevel = (ctx, _, ex) =>
        {
            if (ex is not null || ctx.Response.StatusCode >= 500)
                return Serilog.Events.LogEventLevel.Error;
            if (ctx.Request.Path.StartsWithSegments("/api/debug") ||
                ctx.Request.Path.StartsWithSegments("/health"))
                return Serilog.Events.LogEventLevel.Verbose;
            return Serilog.Events.LogEventLevel.Information;
        };
    });
    // SPA static files — production only. In Development the Angular dev
    // server (npm start, :4200) serves the UI and proxies /api here. Serving
    // wwwroot in dev would hand the browser a stale bundle that doesn't
    // reflect ongoing src/ changes.
    if (!app.Environment.IsDevelopment())
    {
        app.UseDefaultFiles();  // maps / → /index.html
        app.UseStaticFiles();
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();            // /openapi/v1.json
        app.MapScalarApiReference(); // /scalar
    }

    // ── Health endpoint ───────────────────────────────────────────────────────
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (ctx, report) =>
        {
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name        = e.Key,
                    status      = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    data        = e.Value.Data
                })
            }));
        }
    });

    // ── Endpoints ─────────────────────────────────────────────────────────────
    app.MapPost("/api/chat", async (
        ChatRequest req,
        IChatService chat,
        NotesIngestionService ingestion,
        CancellationToken ct) =>
    {
        if (!ingestion.IsReady)
            return Results.Problem(
                title: "Service not ready",
                detail: "Notes ingestion is still in progress. Please retry shortly.",
                statusCode: StatusCodes.Status503ServiceUnavailable);

        var result = await chat.AskAsync(req, ct);
        return Results.Ok(new ChatResponse(result.Answer, result.Chunks));
    });

    // ── Debug endpoints (in-memory pipeline traces) ───────────────────────────
    // Frontend polls /recent every ~300ms while the Debug screen is open, and
    // /query/{id} for the currently-selected trace. In-flight traces show up
    // in /recent with running=true; their /query/{id} returns a partial trace.
    app.MapGet("/api/debug/recent", (PipelineTraceStore store) => store.Recent());

    app.MapGet("/api/debug/query/{id}", (string id, PipelineTraceStore store) =>
        store.Get(id) is { } trace ? Results.Ok(trace) : Results.NotFound());

    // ── SPA fallback ──────────────────────────────────────────────────────────
    // Angular handles client-side routing; deep links must serve index.html.
    // Production only — paired with the static-files block above.
    if (!app.Environment.IsDevelopment())
        app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "dotRAG.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Required for integration test host references in Phase 2
public partial class Program { }
