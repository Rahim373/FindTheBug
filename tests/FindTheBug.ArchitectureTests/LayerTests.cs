using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class LayerTests
{
    [Fact]
    public void DomainEntities_Should_BeInEntitiesNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.DomainAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .DoNotHaveName(nameof(Domain.Common.BaseAuditableEntity))
            .And()
            .Inherit(typeof(Domain.Common.BaseEntity))
            .Or()
            .Inherit(typeof(Domain.Common.BaseAuditableEntity))
            .Should()
            .ResideInNamespace("FindTheBug.Domain.Entities")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All domain entities should be in Entities namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Commands_Should_BeInFeaturesNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .AreNotAbstract()
            .Should()
            .ResideInNamespaceStartingWith("FindTheBug.Application.Features")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All commands should be in Features namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Queries_Should_BeInFeaturesNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .AreNotAbstract()
            .Should()
            .ResideInNamespaceStartingWith("FindTheBug.Application.Features")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All queries should be in Features namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Controllers_Should_BeInControllersNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.WebApiAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .ResideInNamespace("FindTheBug.WebAPI.Controllers")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All controllers should be in Controllers namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Middleware_Should_BeInMiddlewareNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.WebApiAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Middleware")
            .Should()
            .ResideInNamespace("FindTheBug.WebAPI.Middleware")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All middleware should be in Middleware namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
