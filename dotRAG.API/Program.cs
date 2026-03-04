using dotRAG.API.Application;
using dotRAG.API.Infrastructure.Embeddings;
using dotRAG.API.Infrastructure.LLM;
using dotRAG.API.Infrastructure.RAG;
using dotRAG.API.Middleware;
using dotRAG.API.Models;
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
        .WriteTo.Console());

    // ── OpenAPI ───────────────────────────────────────────────────────────────
    builder.Services.AddOpenApi();

    // ── Exception handling ────────────────────────────────────────────────────
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ── HTTP client ───────────────────────────────────────────────────────────
    builder.Services.AddHttpClient();

    // ── Infrastructure (all Singleton — stateless or shared-state) ───────────
    builder.Services.AddSingleton<InMemoryVectorStore>();
    builder.Services.AddSingleton<MarkdownChunker>();
    builder.Services.AddSingleton<IEmbeddingService, VoyageEmbeddingService>();
    builder.Services.AddSingleton<ILlmService, AnthropicLlmService>();
    builder.Services.AddSingleton<INotesSearchService, NotesSearchService>();
    builder.Services.AddHostedService<NotesIngestionService>();

    // ── Application ───────────────────────────────────────────────────────────
    builder.Services.AddSingleton<IPromptBuilder, PromptBuilder>();
    builder.Services.AddSingleton<IChatService, ChatService>();

    var app = builder.Build();

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();            // /openapi/v1.json
        app.MapScalarApiReference(); // /scalar
    }

    // ── Endpoints ─────────────────────────────────────────────────────────────
    app.MapPost("/api/chat", async (ChatRequest req, IChatService chat, CancellationToken ct) =>
        Results.Ok(new ChatResponse(await chat.AskAsync(req.Question, ct))));

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
