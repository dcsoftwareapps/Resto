using System.Reflection;
using Xunit;

namespace Resto.Domain.Tests;

public sealed class DependencyTests
{
    [Fact]
    public void DomainHasNoDependenciesOnOtherRestoProjects()
    {
        var assembly = Assembly.Load("Resto.Domain");

        Assert.DoesNotContain(assembly.GetReferencedAssemblies(),
            reference => reference.Name?.StartsWith("Resto.", StringComparison.Ordinal) == true);
    }
}
