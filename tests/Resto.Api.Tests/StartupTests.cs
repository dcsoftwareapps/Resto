using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Resto.Api.Tests;

public sealed class StartupTests
{
    [Fact]
    public async Task ApiStartsWithoutApplicationEndpointsAsync()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        using var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
