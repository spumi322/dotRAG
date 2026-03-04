using Microsoft.AspNetCore.Diagnostics;

namespace dotRAG.API.Middleware;

// Phase 0 placeholder — catches all unhandled exceptions, returns raw HTTP 500.
// Phase 2: replace WriteAsync body with ProblemDetails (RFC 9457) JSON response.
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception caught by GlobalExceptionHandler");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync("An unexpected error occurred.", cancellationToken);

        return true;
    }
}
