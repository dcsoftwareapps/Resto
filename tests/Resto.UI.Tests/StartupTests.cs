using Bunit;
using Resto.UI;
using Xunit;

namespace Resto.UI.Tests;

public sealed class StartupTests
{
    [Fact]
    public void SharedRouterStartsAtTheRootRoute()
    {
        using var context = new BunitContext();

        var rendered = context.Render<Routes>();

        Assert.Single(rendered.FindComponents<Pages.Home>());
    }
}
