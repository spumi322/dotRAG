namespace dotRAG.API.Middleware;

internal sealed class CorrelationIdMiddleware : IMiddleware
{
    private const string Header = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var id = context.Request.Headers[Header].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N");

        context.Response.Headers[Header] = id;

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", id))
        {
            await next(context);
        }
    }
}
