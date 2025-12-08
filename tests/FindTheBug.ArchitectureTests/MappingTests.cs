using NetArchTest.Rules;
using FindTheBug.Infrastructure.Common;

namespace FindTheBug.ArchitectureTests;

public class MappingTests
{
    [Fact]
    public void MappingClasses_Should_EndWithMapping()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IMapping<>))
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Mapping")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All mapping classes should end with 'Mapping'. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void MappingClasses_Should_BeInMappingsNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IMapping<>))
            .And()
            .AreClasses()
            .Should()
            .ResideInNamespace("FindTheBug.Infrastructure.Mappings")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All mapping classes should be in Mappings namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void MappingClasses_Should_ImplementIMappingInterface()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var mappingClassesWithoutInterface = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Mapping") && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       !t.GetInterfaces().Any(i => i.IsGenericType && 
                                                  i.GetGenericTypeDefinition() == typeof(IMapping<>)))
            .Select(t => t.FullName)
            .ToList();

        // Assert
        Assert.Empty(mappingClassesWithoutInterface);
    }

    [Fact]
    public void MappingClasses_Should_HaveConfigureMethod()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var mappingClassesWithoutConfigureMethod = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Mapping") && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       t.GetInterfaces().Any(i => i.IsGenericType && 
                                                  i.GetGenericTypeDefinition() == typeof(IMapping<>)))
            .Where(t => t.GetMethod("Configure") == null)
            .Select(t => t.FullName)
            .ToList();

        // Assert
        Assert.Empty(mappingClassesWithoutConfigureMethod);
    }

    [Fact]
    public void DomainEntities_Should_HaveCorrespondingMapping()
    {
        // Arrange
        var domainAssembly = AssemblyReference.DomainAssembly;
        var infrastructureAssembly = AssemblyReference.InfrastructureAssembly;

        // Get all domain entities
        var domainEntities = domainAssembly.GetTypes()
            .Where(t => t.IsClass && 
                       !t.IsAbstract &&
                       (t.BaseType == typeof(Domain.Common.BaseEntity) || 
                        t.BaseType == typeof(Domain.Common.BaseAuditableEntity)) &&
                       t.Namespace == "FindTheBug.Domain.Entities")
            .ToList();

        // Get all mapping types
        var mappingTypes = infrastructureAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && 
                                                   i.GetGenericTypeDefinition() == typeof(IMapping<>)) && 
                       t.IsClass && 
                       !t.IsAbstract)
            .ToList();

        // Find entities without mappings
        var entitiesWithoutMappings = domainEntities
            .Where(entity => !mappingTypes.Any(mapping => 
                mapping.GetInterfaces()
                    .Any(i => i.IsGenericType && 
                             i.GetGenericTypeDefinition() == typeof(IMapping<>) &&
                             i.GetGenericArguments()[0] == entity)))
            .Select(t => t.Name)
            .ToList();

        // Assert
        Assert.Empty(entitiesWithoutMappings);
    }

    [Fact]
    public void MappingInterface_Should_BeInCommonNamespace()
    {
        // Arrange
        var assembly = AssemblyReference.InfrastructureAssembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .AreInterfaces()
            .And()
            .HaveName("IMapping`1")
            .Should()
            .ResideInNamespace("FindTheBug.Infrastructure.Common")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"IMapping interface should be in Common namespace. Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
