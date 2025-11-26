using NetArchTest.Rules;

namespace FindTheBug.ArchitectureTests;

public class InheritanceTests
{
    [Fact]
    public void Commands_Should_ImplementICommand()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act - Use simpler check
        var commandTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Command") && 
                       !t.IsAbstract && 
                       !t.IsInterface &&
                       t.Namespace?.Contains("Commands") == true)
            .ToList();

        var commandsWithoutInterface = commandTypes
            .Where(t => !t.GetInterfaces().Any(i => i.Name.Contains("ICommand")))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(commandsWithoutInterface);
    }

    [Fact]
    public void Queries_Should_ImplementIQuery()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var queryTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Query") && 
                       !t.IsAbstract && 
                       !t.IsInterface &&
                       t.Namespace?.Contains("Queries") == true)
            .ToList();

        var queriesWithoutInterface = queryTypes
            .Where(t => !t.GetInterfaces().Any(i => i.Name.Contains("IQuery")))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(queriesWithoutInterface);
    }

    [Fact]
    public void CommandHandlers_Should_ImplementICommandHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("CommandHandler") && !t.IsAbstract)
            .ToList();

        var handlersWithoutInterface = handlerTypes
            .Where(t => !t.GetInterfaces().Any(i => i.Name.Contains("ICommandHandler")))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(handlersWithoutInterface);
    }

    [Fact]
    public void QueryHandlers_Should_ImplementIQueryHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("QueryHandler") && !t.IsAbstract)
            .ToList();

        var handlersWithoutInterface = handlerTypes
            .Where(t => !t.GetInterfaces().Any(i => i.Name.Contains("IQueryHandler")))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(handlersWithoutInterface);
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
        var entityTypes = assembly.GetTypes()
            .Where(t => t.Namespace == "FindTheBug.Domain.Entities" && 
                       t.IsClass && 
                       !t.IsAbstract)
            .ToList();

        var entitiesWithoutBase = entityTypes
            .Where(t => !IsInheritingFromBaseEntity(t))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(entitiesWithoutBase);
    }

    private static bool IsInheritingFromBaseEntity(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "BaseEntity" || baseType.Name == "BaseAuditableEntity")
                return true;
            baseType = baseType.BaseType;
        }
        return false;
    }
}
