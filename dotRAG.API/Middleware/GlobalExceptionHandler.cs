using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace dotRAG.API.Middleware;

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

        var (status, title) = exception switch
        {
            HttpRequestException { StatusCode: { } code } =>
                ((int)code, $"Upstream API error: {exception.Message}"),
            _ =>
                (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Type   = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

        httpContext.Response.StatusCode  = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
