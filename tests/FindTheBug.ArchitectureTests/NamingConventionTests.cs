using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class NamingConventionTests
{
    [Fact]
    public void Commands_Should_EndWithCommand()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(Application.Common.Messaging.ICommand<>))
            .Or()
            .ResideInNamespace("FindTheBug.Application.Features")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"All commands should end with 'Command'. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Queries_Should_EndWithQuery()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(Application.Common.Messaging.IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All queries should end with 'Query'. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Handlers_Should_EndWithHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(Application.Common.Messaging.ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(Application.Common.Messaging.IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All handlers should end with 'Handler'. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Controllers_Should_EndWithController()
    {
        // Arrange
        var assembly = AssemblyReference.WebApiAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("FindTheBug.WebAPI.Controllers")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All controllers should end with 'Controller'. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Interfaces_Should_StartWithI()
    {
        // Arrange
        var assemblies = new[] 
        { 
            AssemblyReference.DomainAssembly,
            AssemblyReference.ApplicationAssembly,
            AssemblyReference.InfrastructureAssembly
        };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var result = Types.InAssembly(assembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"All interfaces should start with 'I' in {assembly.GetName().Name}. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    [Fact]
    public void Entities_Should_NotHaveEntitySuffix()
    {
        // Arrange
        var assembly = AssemblyReference.DomainAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("FindTheBug.Domain.Entities")
            .Should()
            .NotHaveNameEndingWith("Entity")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain entities should not have 'Entity' suffix. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
