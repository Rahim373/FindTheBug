using System.Net;
using System.Text.Json;
using FindTheBug.Application.Exceptions;
using FindTheBug.Domain.Exceptions;

namespace FindTheBug.WebAPI.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, type, title, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "ValidationError",
                "One or more validation errors occurred",
                validationEx.Errors
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                "NotFound",
                notFoundEx.Message,
                null as IDictionary<string, string[]>
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                "Unauthorized access",
                null as IDictionary<string, string[]>
            ),
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                "DomainError",
                domainEx.Message,
                null as IDictionary<string, string[]>
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "ServerError",
                "An internal server error occurred",
                null as IDictionary<string, string[]>
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            type,
            title,
            status = (int)statusCode,
            traceId = context.TraceIdentifier,
            errors
        };

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(json);
    }
}
