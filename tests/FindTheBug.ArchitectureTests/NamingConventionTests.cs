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
        var commandTypes = assembly.GetTypes()
            .Where(t => t.Namespace?.Contains("Commands") == true &&
                       t.IsClass &&
                       !t.IsAbstract &&
                       !t.Name.EndsWith("Handler"))
            .Where(t => !t.Name.EndsWith("Command"))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(commandTypes);
    }

    [Fact]
    public void Queries_Should_EndWithQuery()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var queryTypes = assembly.GetTypes()
            .Where(t => t.Namespace?.Contains("Queries") == true &&
                       t.IsClass &&
                       !t.IsAbstract &&
                       !t.Name.EndsWith("Handler"))
            .Where(t => !t.Name.EndsWith("Query"))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(queryTypes);
    }

    [Fact]
    public void Handlers_Should_EndWithHandler()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var handlerTypes = assembly.GetTypes()
            .Where(t => (t.Name.Contains("Command") || t.Name.Contains("Query")) &&
                       t.IsClass &&
                       !t.IsAbstract &&
                       t.Name.Contains("Handler"))
            .Where(t => !t.Name.EndsWith("Handler"))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(handlerTypes);
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
            AssemblyReference.ApplicationAssembly
        };

        // Act & Assert
        foreach (var assembly in assemblies)
        {
            var interfacesWithoutI = assembly.GetTypes()
                .Where(t => t.IsInterface && !t.Name.StartsWith("I"))
                .Select(t => t.FullName)
                .ToList();

            Assert.Empty(interfacesWithoutI);
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
