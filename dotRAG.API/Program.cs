using dotRAG.API.Application;
using dotRAG.API.Infrastructure.Embeddings;
using dotRAG.API.Infrastructure.LLM;
using dotRAG.API.Infrastructure.RAG;
using dotRAG.API.Middleware;
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

    // ── Application layer ─────────────────────────────────────────────────────
    builder.Services.AddScoped<IChatService, ChatService>();
    builder.Services.AddScoped<IPromptBuilder, PromptBuilder>();

    // ── Infrastructure layer ──────────────────────────────────────────────────
    builder.Services.AddScoped<ILlmService, AnthropicLlmService>();
    builder.Services.AddScoped<IEmbeddingService, VoyageEmbeddingService>();
    builder.Services.AddScoped<INotesSearchService, NotesSearchService>();
    builder.Services.AddHttpClient();

    var app = builder.Build();

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();            // /openapi/v1.json
        app.MapScalarApiReference(); // /scalar
    }

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
