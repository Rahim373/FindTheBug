using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FindTheBug.WebAPI.Attributes;

/// <summary>
/// Authorization attribute that validates Basic Authentication credentials
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        // Skip authorization if AllowAnonymous attribute is present
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute);

        if (allowAnonymous)
        {
            return;
        }

        // Check for Authorization header
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        // Validate Basic Authentication format
        var authHeaderString = authHeader.ToString();
        if (!authHeaderString.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        // Decode credentials
        try
        {
            var encodedCredentials = authHeaderString.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':');

            if (credentials.Length != 2)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var clientKey = credentials[0];
            var clientSecret = credentials[1];

            // Validate against configuration
            var expectedClientKey = configuration["ApiSettings:SyncClientKey"];
            var expectedClientSecret = configuration["ApiSettings:SyncClientSecret"];

            if (string.IsNullOrEmpty(expectedClientKey) || string.IsNullOrEmpty(expectedClientSecret))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (clientKey != expectedClientKey || clientSecret != expectedClientSecret)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            // Authentication successful
            return;
        }
        catch
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }
    }
}