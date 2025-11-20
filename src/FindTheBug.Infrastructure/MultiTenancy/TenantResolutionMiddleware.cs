using FindTheBug.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FindTheBug.Infrastructure.MultiTenancy;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ITenantContext tenantContext)
    {
        var host = context.Request.Host.Host;
        
        // Extract subdomain from host
        var subdomain = ExtractSubdomain(host);

        if (!string.IsNullOrEmpty(subdomain))
        {
            var tenant = await tenantService.GetTenantBySubdomainAsync(subdomain);
            
            if (tenant is null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"Tenant '{subdomain}' not found");
                return;
            }

            if (!tenant.IsActive)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"Tenant '{subdomain}' is not active");
                return;
            }

            tenantContext.SetTenant(tenant);
        }
        else
        {
            // No subdomain - could be accessing tenant management endpoints
            // Allow the request to proceed
        }

        await next(context);
    }

    private static string? ExtractSubdomain(string host)
    {
        // Remove port if present
        var hostWithoutPort = host.Split(':')[0];
        
        // Split by dots
        var parts = hostWithoutPort.Split('.');
        
        // If we have more than 2 parts (e.g., tenant.example.com), first part is subdomain
        // For localhost testing: tenant.localhost
        if (parts.Length >= 2)
        {
            var potentialSubdomain = parts[0];
            
            // Ignore common non-tenant subdomains
            if (potentialSubdomain.Equals("www", StringComparison.OrdinalIgnoreCase) ||
                potentialSubdomain.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            
            // For localhost, if it's just "localhost", no subdomain
            if (parts.Length == 1 && potentialSubdomain.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            
            return potentialSubdomain;
        }

        return null;
    }
}

