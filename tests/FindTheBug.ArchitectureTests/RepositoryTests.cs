using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class RepositoryTests
{
    [Fact]
    public void Repositories_Should_OnlyBeInInfrastructureLayer()
    {
        // Arrange
        var assemblies = new[]
        {
            AssemblyReference.DomainAssembly,
            AssemblyReference.ApplicationAssembly,
            AssemblyReference.WebApiAssembly
        };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var repositoryTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Repository") && t.IsClass && !t.IsAbstract)
                .Select(t => t.FullName)
                .ToList();

            Assert.Empty(repositoryTypes);
        }
    }

    [Fact]
    public void ApplicationLayer_Should_OnlyUseIRepositoryInterface()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("FindTheBug.Infrastructure.Repositories")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            "Application layer should not depend on concrete repository implementations");
    }

    [Fact]
    public void ApplicationLayer_Should_NotUseDbContextDirectly()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore.DbContext")
            .And()
            .NotHaveDependencyOn("FindTheBug.Infrastructure.Data.ApplicationDbContext")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            "Application layer should not use DbContext directly, use IUnitOfWork or IRepository instead");
    }

    [Fact]
    public void Handlers_Should_UseIUnitOfWork()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler") &&
                       !t.IsAbstract &&
                       !t.IsInterface)
            .ToList();

        // Most handlers should use IUnitOfWork (allow some exceptions for read-only queries)
        var handlersWithoutUnitOfWork = handlerTypes
            .Where(t =>
            {
                var constructors = t.GetConstructors();
                return constructors.Any() &&
                       !constructors.Any(c => c.GetParameters()
                           .Any(p => p.ParameterType.Name == "IUnitOfWork"));
            })
            .Select(t => t.Name)
            .ToList();

        // Assert - Allow some handlers without IUnitOfWork (e.g., simple queries)
        // This is a soft check - we expect most handlers to use IUnitOfWork
        var percentageWithUnitOfWork = (handlerTypes.Count - handlersWithoutUnitOfWork.Count) * 100.0 / handlerTypes.Count;

        Assert.True(percentageWithUnitOfWork >= 70,
            $"At least 70% of handlers should use IUnitOfWork. Current: {percentageWithUnitOfWork:F1}%. Handlers without: {string.Join(", ", handlersWithoutUnitOfWork)}");
    }
}
