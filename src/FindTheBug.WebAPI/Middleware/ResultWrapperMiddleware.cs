using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace FindTheBug.WebAPI.Middleware;

public class ResultWrapperMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        context.Response.Body = originalBodyStream;
        responseBody.Seek(0, SeekOrigin.Begin);

        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

        // Don't wrap if response is empty
        if (string.IsNullOrEmpty(responseText) || 
            context.Response.StatusCode == StatusCodes.Status204NoContent)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            return;
        }

        // Don't wrap health checks, metrics, swagger endpoints
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/metrics") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            return;
        }

        object wrappedResponse;

        // Handle error responses (4xx, 5xx)
        if (context.Response.StatusCode >= 400)
        {
            try
            {
                // Try to parse as ProblemDetails (from ErrorOrActionFilter)
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseText, JsonOptions);
                if (problemDetails is not null)
                {
                    var errors = new List<string>();
                    
                    if (!string.IsNullOrEmpty(problemDetails.Title))
                        errors.Add(problemDetails.Title);
                    
                    if (!string.IsNullOrEmpty(problemDetails.Detail))
                        errors.Add(problemDetails.Detail);

                    // Extract errors from extensions if available
                    if (problemDetails.Extensions.TryGetValue("errors", out var errorsObj))
                    {
                        try
                        {
                            var errorsList = JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                                JsonSerializer.Serialize(errorsObj), JsonOptions);
                            
                            if (errorsList is not null)
                            {
                                foreach (var error in errorsList)
                                {
                                    foreach(var errorStr in error.Value)
                                    {
                                        if (!errors.Contains(errorStr))
                                            errors.Add(errorStr);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Ignore parsing errors
                        }
                    }

                    wrappedResponse = new
                    {
                        isSuccess = false,
                        data = (object?)null,
                        errorMessage = problemDetails.Title ?? "An error occurred",
                        errors = errors.Count > 0 ? errors : new List<string> { "An error occurred" }
                    };
                }
                else
                {
                    wrappedResponse = new
                    {
                        isSuccess = false,
                        data = (object?)null,
                        errorMessage = "An error occurred",
                        errors = new List<string> { responseText }
                    };
                }
            }
            catch
            {
                wrappedResponse = new
                {
                    isSuccess = false,
                    data = (object?)null,
                    errorMessage = "An error occurred",
                    errors = new List<string> { responseText }
                };
            }
        }
        else
        {
            // Handle success responses (2xx)
            // The ErrorOrActionFilter has already extracted the value from ErrorOr<T>
            try
            {
                // Try to deserialize the response to preserve its structure
                var data = JsonSerializer.Deserialize<object>(responseText, JsonOptions);
                wrappedResponse = new
                {
                    isSuccess = true,
                    data = data,
                    errorMessage = (string?)null,
                    errors = new List<string>()
                };
            }
            catch
            {
                // If deserialization fails, wrap the raw response
                wrappedResponse = new
                {
                    isSuccess = true,
                    data = responseText,
                    errorMessage = (string?)null,
                    errors = new List<string>()
                };
            }
        }

        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, JsonOptions);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = null; // Reset content length
        await context.Response.WriteAsync(wrappedJson);
    }
}
