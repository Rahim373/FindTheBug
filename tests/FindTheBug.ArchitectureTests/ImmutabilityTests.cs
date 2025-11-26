using System.Reflection;

namespace FindTheBug.ArchitectureTests;

public class ImmutabilityTests
{
    [Fact]
    public void Commands_Should_BeRecords()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var commandTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Command") && 
                       !t.IsAbstract && 
                       !t.IsInterface &&
                       t.Namespace?.StartsWith("FindTheBug.Application.Features") == true)
            .ToList();

        var nonRecordCommands = commandTypes
            .Where(t => !IsRecord(t))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(nonRecordCommands);
    }

    [Fact]
    public void Queries_Should_BeRecords()
    {
        // Arrange
        var assembly = AssemblyReference.ApplicationAssembly;

        // Act
        var queryTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Query") && 
                       !t.IsAbstract && 
                       !t.IsInterface &&
                       t.Namespace?.StartsWith("FindTheBug.Application.Features") == true)
            .ToList();

        var nonRecordQueries = queryTypes
            .Where(t => !IsRecord(t))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(nonRecordQueries);
    }

    private static bool IsRecord(Type type)
    {
        // Records have a compiler-generated EqualityContract property
        return type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance) != null;
    }
}
