using System.Diagnostics;

namespace FindTheBug.WebAPI.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private static readonly string[] ExcludedPaths = ["/health", "/metrics", "/swagger"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldLogRequest(context.Request.Path))
        {
            var correlationId = GetOrCreateCorrelationId(context);
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation(
                "HTTP {Method} {Path} started - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms - CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);
            }
        }
        else
        {
            await next(context);
        }
    }

    private static bool ShouldLogRequest(PathString path)
    {
        return !ExcludedPaths.Any(excluded => path.StartsWithSegments(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        const string correlationIdKey = "CorrelationId";

        if (context.Items.TryGetValue(correlationIdKey, out var correlationId) && correlationId is string id)
        {
            return id;
        }

        var newCorrelationId = Guid.NewGuid().ToString();
        context.Items[correlationIdKey] = newCorrelationId;
        return newCorrelationId;
    }
}
