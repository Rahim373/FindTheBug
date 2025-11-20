using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController(IRepository<SampleEntity> repository, ILogger<SampleController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<SampleEntity>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var entities = await repository.GetAllAsync(cancellationToken);
            return Ok(Result<IEnumerable<SampleEntity>>.Success(entities));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all sample entities");
            return StatusCode(500, Result<IEnumerable<SampleEntity>>.Failure("An error occurred while retrieving entities"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<SampleEntity>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                return NotFound(Result<SampleEntity>.Failure($"Entity with ID {id} not found"));
            }
            return Ok(Result<SampleEntity>.Success(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving sample entity with ID {Id}", id);
            return StatusCode(500, Result<SampleEntity>.Failure("An error occurred while retrieving the entity"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<SampleEntity>>> Create([FromBody] SampleEntity entity, CancellationToken cancellationToken)
    {
        try
        {
            entity.Id = Guid.NewGuid();
            var created = await repository.AddAsync(entity, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, Result<SampleEntity>.Success(created));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating sample entity");
            return StatusCode(500, Result<SampleEntity>.Failure("An error occurred while creating the entity"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<SampleEntity>>> Update(Guid id, [FromBody] SampleEntity entity, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await repository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return NotFound(Result<SampleEntity>.Failure($"Entity with ID {id} not found"));
            }

            entity.Id = id;
            await repository.UpdateAsync(entity, cancellationToken);
            return Ok(Result<SampleEntity>.Success(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating sample entity with ID {Id}", id);
            return StatusCode(500, Result<SampleEntity>.Failure("An error occurred while updating the entity"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await repository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return NotFound(Result<bool>.Failure($"Entity with ID {id} not found"));
            }

            await repository.DeleteAsync(id, cancellationToken);
            return Ok(Result<bool>.Success(true));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting sample entity with ID {Id}", id);
            return StatusCode(500, Result<bool>.Failure("An error occurred while deleting the entity"));
        }
    }
}

