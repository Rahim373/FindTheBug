using FindTheBug.Domain.Common;
using System.Reflection;

namespace FindTheBug.ArchitectureTests;

public static class AssemblyReference
{
    public static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.Data.ApplicationDbContext).Assembly;
    public static readonly Assembly WebApiAssembly = typeof(Program).Assembly;
}
