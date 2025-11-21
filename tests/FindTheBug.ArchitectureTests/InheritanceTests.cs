using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class InheritanceTests
{
    [Fact]
    public void Commands_Should_ImplementICommand()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .AreNotAbstract()
            .And()
            .AreNotInterfaces()
            .Should()
            .ImplementInterface(typeof(Application.Common.Messaging.ICommand<>))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All commands should implement ICommand<T>. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Queries_Should_ImplementIQuery()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .AreNotAbstract()
            .And()
            .AreNotInterfaces()
            .Should()
            .ImplementInterface(typeof(Application.Common.Messaging.IQuery<>))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All queries should implement IQuery<T>. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void CommandHandlers_Should_ImplementICommandHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ImplementInterface(typeof(Application.Common.Messaging.ICommandHandler<,>))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All command handlers should implement ICommandHandler<T, R>. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void QueryHandlers_Should_ImplementIQueryHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .ImplementInterface(typeof(Application.Common.Messaging.IQueryHandler<,>))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All query handlers should implement IQueryHandler<T, R>. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Controllers_Should_InheritFromControllerBase()
    {
        // Arrange
        var assembly = AssemblyReference.WebApiAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .ResideInNamespace("FindTheBug.WebAPI.Controllers")
            .Should()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All controllers should inherit from ControllerBase. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void DomainEntities_Should_InheritFromBaseEntity()
    {
        // Arrange
        var assembly = AssemblyReference.DomainAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace("FindTheBug.Domain.Entities")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(Domain.Common.BaseEntity))
            .Or()
            .Inherit(typeof(Domain.Common.BaseAuditableEntity))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All domain entities should inherit from BaseEntity or BaseAuditableEntity. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
