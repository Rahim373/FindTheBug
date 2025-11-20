using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(ITenantService tenantService, ILogger<TenantsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all active tenants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<Tenant>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var tenants = await tenantService.GetAllTenantsAsync(cancellationToken);
            return Ok(Result<IEnumerable<Tenant>>.Success(tenants));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tenants");
            return StatusCode(500, Result<IEnumerable<Tenant>>.Failure("An error occurred while retrieving tenants"));
        }
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<Tenant>>> GetById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await tenantService.GetTenantByIdAsync(id, cancellationToken);
            if (tenant is null)
            {
                return NotFound(Result<Tenant>.Failure($"Tenant with ID {id} not found"));
            }
            return Ok(Result<Tenant>.Success(tenant));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tenant with ID {Id}", id);
            return StatusCode(500, Result<Tenant>.Failure("An error occurred while retrieving the tenant"));
        }
    }

    /// <summary>
    /// Get tenant by subdomain
    /// </summary>
    [HttpGet("subdomain/{subdomain}")]
    public async Task<ActionResult<Result<Tenant>>> GetBySubdomain(string subdomain, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await tenantService.GetTenantBySubdomainAsync(subdomain, cancellationToken);
            if (tenant is null)
            {
                return NotFound(Result<Tenant>.Failure($"Tenant with subdomain '{subdomain}' not found"));
            }
            return Ok(Result<Tenant>.Success(tenant));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tenant with subdomain {Subdomain}", subdomain);
            return StatusCode(500, Result<Tenant>.Failure("An error occurred while retrieving the tenant"));
        }
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<Tenant>>> Create([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = new Tenant
            {
                Name = request.Name,
                Subdomain = request.Subdomain.ToLowerInvariant(),
                ConnectionString = request.ConnectionString ?? $"FindTheBug_Tenant_{request.Subdomain}",
                IsActive = true,
                SubscriptionTier = request.SubscriptionTier
            };

            var created = await tenantService.CreateTenantAsync(tenant, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id.ToString() }, Result<Tenant>.Success(created));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating tenant");
            return StatusCode(500, Result<Tenant>.Failure("An error occurred while creating the tenant"));
        }
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<Tenant>>> Update(string id, [FromBody] UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await tenantService.GetTenantByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return NotFound(Result<Tenant>.Failure($"Tenant with ID {id} not found"));
            }

            existing.Name = request.Name;
            existing.IsActive = request.IsActive;
            existing.SubscriptionTier = request.SubscriptionTier;

            await tenantService.UpdateTenantAsync(existing, cancellationToken);
            return Ok(Result<Tenant>.Success(existing));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tenant with ID {Id}", id);
            return StatusCode(500, Result<Tenant>.Failure("An error occurred while updating the tenant"));
        }
    }
}

public record CreateTenantRequest(string Name, string Subdomain, string? ConnectionString, string? SubscriptionTier);
public record UpdateTenantRequest(string Name, bool IsActive, string? SubscriptionTier);

