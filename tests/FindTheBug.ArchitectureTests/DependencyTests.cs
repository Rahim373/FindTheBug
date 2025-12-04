using FindTheBug.WebAPI.Controllers;
using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class DependencyTests
{
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherLayers()
    {
        // Arrange
        var assembly = AssemblyReference.DomainAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("FindTheBug.Application")
            .And()
            .NotHaveDependencyOn("FindTheBug.Infrastructure")
            .And()
            .NotHaveDependencyOn("FindTheBug.WebAPI")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Domain layer should not depend on any other layer");
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnInfrastructure()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("FindTheBug.Infrastructure")
            .And()
            .NotHaveDependencyOn("FindTheBug.WebAPI")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Application layer should not depend on Infrastructure or WebAPI");
    }

    [Fact]
    public void Application_Should_OnlyDependOnDomain()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("FindTheBug.Application")
            .Should()
            .NotHaveDependencyOnAll("FindTheBug.Infrastructure", "FindTheBug.WebAPI")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Application should only depend on Domain");
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnWebAPI()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("FindTheBug.WebAPI")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Infrastructure should not depend on WebAPI");
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        var assembly = AssemblyReference.WebApiAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .DoNotHaveName(nameof(BaseApiController))
            .And()
            .ResideInNamespace("FindTheBug.WebAPI.Controllers")
            .And()
            .AreClasses()
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Controllers should use MediatR for CQRS");
    }
}
