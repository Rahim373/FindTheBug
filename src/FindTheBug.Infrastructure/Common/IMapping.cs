using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Common;

/// <summary>
/// Interface for Entity Framework Core entity mappings
/// </summary>
/// <typeparam name="T">The entity type to configure</typeparam>
public interface IMapping<T> where T : class
{
    /// <summary>
    /// Configures the entity using Fluent API
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure</param>
    void Configure(ModelBuilder modelBuilder);
}
