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
    builder.Services.AddSingleton<IChatService, ChatService>();

    var app = builder.Build();

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseExceptionHandler();
    app.UseCors();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseDefaultFiles();  // maps / → /index.html
    app.UseStaticFiles();

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
                    description = e.Value.Description
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

        return Results.Ok(new ChatResponse(await chat.AskAsync(req, ct)));
    });

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
